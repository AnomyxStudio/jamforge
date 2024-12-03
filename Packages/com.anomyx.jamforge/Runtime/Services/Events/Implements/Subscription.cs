using System;
using System.Reflection;

namespace JamForge.Events
{
    public readonly struct Subscription : IComparable<Subscription>, IEquatable<Subscription>
    {
        public string Path { get; }

        public short Priority { get; }

        public ThreadMode ThreadMode { get; }

        public Type EventDataType { get; }

        public string MethodName { get; }

        public Delegate MethodInvoker { get; }

        public static Subscription CreateFromAction<TEventData>(string path, Action<TEventData> action)
        {
            return new Subscription(path, 0, ThreadMode.Current, action.Method, action.Target);
        }

        public static Subscription CreateFromAction<TEventData>(string path, Action<TEventData> action,
            short priority, ThreadMode threadMode)
        {
            return new Subscription(path, priority, threadMode, action.Method, action.Target);
        }

        public Subscription(string path, short priority, ThreadMode threadMode,
            MethodInfo methodInfo, object target)
        {
            Path = path;
            Priority = priority;
            ThreadMode = threadMode;

            MethodName = methodInfo.Name;

            var parameters = methodInfo.GetParameters();
            if (parameters.Length <= 0)
            {
                EventDataType = null;
                MethodInvoker = methodInfo.CreateDelegate(target);
            }
            else
            {
                EventDataType = methodInfo.GetParameters()[0].ParameterType;
                MethodInvoker = methodInfo.CreateDelegate(target);
            }
        }

        public int CompareTo(Subscription other)
        {
            return other.Priority.CompareTo(Priority);
        }

        public bool Equals(Subscription other)
        {
            return Path == other.Path && EventDataType == other.EventDataType && MethodName == other.MethodName;
        }

        public override bool Equals(object obj)
        {
            return obj is Subscription other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Path, Priority, (int)ThreadMode, EventDataType, MethodName, MethodInvoker);
        }
    }
}
