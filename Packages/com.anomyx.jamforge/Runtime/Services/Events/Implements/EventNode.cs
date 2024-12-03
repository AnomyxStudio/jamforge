using System;
using System.Collections.Generic;
using System.Linq;

namespace JamForge.Events
{
    public class EventNode : NodeData<string>
    {
        public HashSet<Subscription> Subscriptions { get; private set; } = new();

        public EventNode(string path) : base(path, path) { }

        public void AddSubscription(Subscription subscription)
        {
            if (string.IsNullOrEmpty(subscription.Path) || string.IsNullOrEmpty(subscription.MethodName))
            {
                throw new NullReferenceException();
            }

            Subscriptions.Add(subscription);
        }

        public void RemoveSubscription(Subscription subscription)
        {
            if (string.IsNullOrEmpty(subscription.Path) || string.IsNullOrEmpty(subscription.MethodName))
            {
                throw new NullReferenceException();
            }

            Subscriptions = new HashSet<Subscription>(Subscriptions.Except(new[] { subscription }));
        }

        public static void AddSubscription(EventNode rootNode, Subscription subscription)
        {
            var targetNodes = rootNode.GetNodes(subscription.Path);
            foreach (var targetNode in targetNodes)
            {
                var eventNode = (EventNode)targetNode;
                eventNode.AddSubscription(subscription);
            }
        }

        public static void RemoveSubscription(EventNode rootNode, Subscription subscription)
        {
            var targetNodes = rootNode.GetNodes(subscription.Path);
            foreach (var targetNode in targetNodes)
            {
                var eventNode = (EventNode)targetNode;
                eventNode.RemoveSubscription(subscription);
            }
        }

        private static EventNode EventNodeCreation(string path)
        {
            //TODO: fetch instances from pool
            return new EventNode(path);
        }

        public void EnsureNodePath(string path)
        {
            this.EnsureNodePath(path, EventNodeCreation);
        }

        public void GetSubscriptions(string path, List<Subscription> subscriptions)
        {
            var targetNodes = this.GetNodes(path);
            foreach (var targetNode in targetNodes)
            {
                var eventNode = (EventNode)targetNode;
                subscriptions.AddRange(eventNode.Subscriptions);
            }
        }
    }
}
