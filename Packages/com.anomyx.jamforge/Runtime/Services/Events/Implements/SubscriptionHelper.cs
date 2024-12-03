using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace JamForge.Events
{
    public static class SubscriptionHelper
    {
        private static readonly Dictionary<object, Subscription[]> ObjectCaches = new();
        private static readonly Dictionary<Type, MethodInfo[]> MethodCaches = new();

        private static MethodInfo[] GetMethodInfo(Type type)
        {
            if (MethodCaches.TryGetValue(type, out var methodInfos)) { return methodInfos; }

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                       BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            methodInfos = type.GetMethods(flags);
            MethodCaches.Add(type, methodInfos);
            return methodInfos;
        }

        public static IEnumerable<Subscription> FindSubscriber(object obj)
        {
            if (obj == null) { throw new ArgumentNullException(); }

            if (ObjectCaches.TryGetValue(obj, out var subscriptions)) { return subscriptions; }

            var type = obj.GetType();
            var methodInfos = GetMethodInfo(type);

            var list = new List<Subscription>();
            for (var i = 0; i < methodInfos.Length; i++)
            {
                var methodInfo = methodInfos[i];
                var subscribeAttribute = methodInfo.GetCustomAttribute<SubscribeAttribute>();
                if (subscribeAttribute == null) { continue; }

                var path = subscribeAttribute.Path;
                var mode = subscribeAttribute.Mode;
                var priority = subscribeAttribute.Priority;

                var sub = new Subscription(path, priority, mode, methodInfo, obj);
                list.Add(sub);
            }

            var array = list.ToArray();
            ObjectCaches.Add(obj, array);
            return array;
        }
    }
}
