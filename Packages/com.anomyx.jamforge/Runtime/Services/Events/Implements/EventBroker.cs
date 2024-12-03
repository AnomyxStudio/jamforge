using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;

namespace JamForge.Events
{
    [Preserve]
    public sealed class EventBroker : IEventBrokerFacade
    {
        public const string DefaultPath = "Default";

        private const int MaxCallStack = 16;

        private readonly IDispatcher _dispatcher;

        public EventBroker(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        #region Runtime variables

        private int _callStack;

        private readonly object _locker = new();

        private readonly EventNode _rootNode = new("Root");

        private readonly ConcurrentDictionary<string, Payloads> _stickyEvents = new();

        private readonly List<object> _registeredObjects = new();

        #endregion

        #region Registration

        public void Register(object obj)
        {
            if (obj == null) { return; }

            lock (_locker)
            {
                if (_registeredObjects.Contains(obj)) { return; }

                _registeredObjects.Add(obj);
            }

            var subscriptions = SubscriptionHelper.FindSubscriber(obj);
            foreach (var subscription in subscriptions)
            {
                var path = subscription.Path;
                if (_stickyEvents.TryGetValue(path, out var eventData))
                {
                    FireEvent(subscription, eventData);
                }

                EnsurePathExists(path);
                EventNode.AddSubscription(_rootNode, subscription);
            }
        }

        public void Unregister(object obj)
        {
            if (obj == null) { return; }

            lock (_locker)
            {
                if (!_registeredObjects.Contains(obj)) { return; }

                _registeredObjects.Remove(obj);
            }

            var subscriptions = SubscriptionHelper.FindSubscriber(obj);
            foreach (var subscription in subscriptions)
            {
                EventNode.RemoveSubscription(_rootNode, subscription);
            }
        }

        public void Register<TEvent>(Action<TEvent> action, short priority = 0, ThreadMode threadMode = ThreadMode.Current)
            where TEvent : Payloads
        {
            Register(DefaultPath, action, priority, threadMode);
        }

        public void Unregister<TEvent>(Action<TEvent> action) where TEvent : Payloads
        {
            Unregister(DefaultPath, action);
        }

        public void Register<TEvent>(string path, Action<TEvent> action, short priority = 0, ThreadMode threadMode = ThreadMode.Current)
            where TEvent : Payloads
        {
            EnsurePathExists(path);
            var subscription = Subscription.CreateFromAction(path, action, priority, threadMode);
            EventNode.AddSubscription(_rootNode, subscription);
        }

        public void Unregister<TEvent>(string path, Action<TEvent> action) where TEvent : Payloads
        {
            var subscription = Subscription.CreateFromAction(path, action);
            EventNode.RemoveSubscription(_rootNode, subscription);
        }

        #endregion

        #region Trigger

        private void EnsurePathExists(string path)
        {
            _rootNode.EnsureNodePath(path);
        }

        public void Fire(string path)
        {
            Fire(path, Payloads.Empty);
        }

        public void Fire<TEventData>(TEventData payloads) where TEventData : Payloads
        {
            Fire(DefaultPath, payloads);
        }

        public void Fire<TEventData>(string path, TEventData payloads) where TEventData : Payloads
        {
            var subscriptions = new List<Subscription>();

            EnsurePathExists(path);
            _rootNode.GetSubscriptions(path, subscriptions);

            foreach (var subscription in subscriptions) { FireEvent(subscription, payloads); }
        }

        public void FireSticky(string path)
        {
            FireSticky(path, Payloads.Empty);
        }

        public void FireSticky<TEventData>(TEventData payloads) where TEventData : Payloads
        {
            FireSticky(DefaultPath, payloads);
        }

        public void FireSticky<TEventData>(string path, TEventData payloads) where TEventData : Payloads
        {
            Fire(path, payloads);
            CacheStickyEvents(path, payloads);
        }

        public async UniTask FireAsync(string path)
        {
            await FireAsync(path, Payloads.Empty);
        }

        public async UniTask FireAsync<TEventData>(TEventData payloads)
            where TEventData : Payloads
        {
            await FireAsync(DefaultPath, payloads);
        }

        public async UniTask FireAsync<TEventData>(string path, TEventData payloads)
            where TEventData : Payloads
        {
            var subscriptions = new List<Subscription>();

            EnsurePathExists(path);
            _rootNode.GetSubscriptions(path, subscriptions);
            subscriptions.Sort();

            foreach (var subscription in subscriptions) { await TriggerEventAsync(subscription, payloads); }
        }

        #endregion

        #region Helpers

        private void CacheStickyEvents(string path, Payloads payload)
        {
            var nodes = path.Split('.');
            var lastElement = nodes[^1];
            switch (lastElement)
            {
                case "*":
                    throw new Exception("Trigger sticky with wildcard path is not supported.");
                default:
                    _stickyEvents.TryAdd(path, payload);
                    break;
            }
        }

        private bool IsMainThread()
        {
            var currentThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            var mainThreadId = MainThreadDispatcher.Instance.MainThreadId;
            return currentThreadId == mainThreadId;
        }

        private void FireEvent<TEventData>(Subscription subscription, TEventData data)
            where TEventData : Payloads
        {
            var eventDataType = typeof(TEventData);
            var methodName = subscription.MethodName;
            var subEventType = subscription.EventDataType;
            var noParameters = subscription.EventDataType == null;

            if (!noParameters && !subscription.EventDataType.IsAssignableFrom(eventDataType))
            {
                InternalLog.Error($"[{methodName}]: [{subEventType}] is not match [{eventDataType}].");
                return;
            }

            if (subscription.ThreadMode == ThreadMode.Main && !IsMainThread())
            {
                _dispatcher.Dispatch(() =>
                {
                    if (noParameters) { ((Action)subscription.MethodInvoker).Invoke(); }
                    else { ((Action<TEventData>)subscription.MethodInvoker).Invoke(data); }
                });
            }

            try
            {
                _callStack++;

                if (_callStack >= MaxCallStack)
                {
                    _callStack = 0;
                    throw new Exception($"[{methodName}]: Call stack overflow [Depth: {MaxCallStack}].");
                }

                if (noParameters)
                {
                    ((Action)subscription.MethodInvoker).Invoke();
                }
                else
                {
                    ((Action<TEventData>)subscription.MethodInvoker).Invoke(data);
                }
                _callStack--;
            } catch (Exception ex)
            {
                throw new Exception($"[{methodName}]: {ex.Message}", ex);
            }
        }

        private static async UniTask TriggerEventAsync<TEventData>(Subscription subscription, TEventData data)
            where TEventData : Payloads
        {
            var eventDataType = typeof(TEventData);
            var methodName = subscription.MethodName;
            var subEventType = subscription.EventDataType;
            var noParameters = subscription.EventDataType == null;

            if (!noParameters && !subscription.EventDataType.IsAssignableFrom(eventDataType))
            {
                InternalLog.Error($"[{methodName}]: [{subEventType}] is not match [{eventDataType}].");
                return;
            }

            // if (subscription.ThreadMode == ThreadMode.Main)
            // {
            //     Debug.LogWarning($"ThreadMode must be ThreadMode.Current while using async calls.");
            // }

            if (noParameters) { await ((Func<UniTask>)subscription.MethodInvoker).Invoke(); }
            else { await ((Func<TEventData, UniTask>)subscription.MethodInvoker).Invoke(data); }
        }

        #endregion
    }
}
