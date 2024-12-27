using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JamForge.Messages
{
     public class MessageCenter : IMessageCenter
    {
        private readonly ConcurrentDictionary<Type, List<Delegate>> _subscribers = new();

        public void Subscribe<T>(Action<T> handler) where T : IMessage
        {
            var type = typeof(T);
            _subscribers.AddOrUpdate(
                type,
                new List<Delegate> { handler },
                (_, list) =>
                {
                    lock (list)
                    {
                        list.Add(handler);
                        return list;
                    }
                });
        }

        public void Unsubscribe<T>(Action<T> handler) where T : IMessage
        {
            if (!_subscribers.TryGetValue(typeof(T), out var handlers))
            {
                return;
            }
            
            lock (handlers)
            {
                handlers.Remove(handler);
                if (handlers.Count == 0)
                {
                    _subscribers.TryRemove(typeof(T), out _);
                }
            }
        }

        public void Publish<T>(T message, bool async = false) where T : IMessage
        {
            if (!_subscribers.TryGetValue(typeof(T), out var handlers))
            {
                return;
            }
            
            List<Delegate> snapshot;
            lock (handlers)
            {
                snapshot = new List<Delegate>(handlers);
            }

            if (async)
            {
                Parallel.ForEach(snapshot, handler =>
                {
                    ((Action<T>)handler)(message);
                });
            }
            else
            {
                foreach (var handler in snapshot)
                {
                    ((Action<T>)handler)(message);
                }
            }
        }

        public void Clear() => _subscribers.Clear();
    }
}