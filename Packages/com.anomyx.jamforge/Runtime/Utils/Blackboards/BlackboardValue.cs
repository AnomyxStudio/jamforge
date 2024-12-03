using System;
using UnityEngine;

namespace JamForge.Blackboards
{
    [Serializable]
    public abstract class BlackboardValue : ISerializationCallbackReceiver
    {
        public abstract Type ValueType { get; }
        public abstract object GetValue();
        public abstract void SetValue(object objValue);

        public virtual void OnBeforeSerialize() { }
        public virtual void OnAfterDeserialize() { }
    }

    [Serializable]
    public class BlackboardValue<T> : BlackboardValue
    {
        [SerializeField] private T value;

        public BlackboardValue(T initialValue = default)
        {
            value = initialValue;
        }

        public override Type ValueType => typeof(T);

        public override object GetValue() => value;

        public override void SetValue(object objValue)
        {
            if (objValue is T typedValue) { value = typedValue; }
            else { throw new ArgumentException($"Cannot set value of type {objValue?.GetType()} to BlackboardValue<{typeof(T)}>"); }
        }

        public T Value
        {
            get => value;
            set => this.value = value;
        }
    }
}
