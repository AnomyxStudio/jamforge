using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace JamForge.Blackboards
{
    internal readonly struct BlackboardKey : IEquatable<BlackboardKey>
    {
        private readonly string _name;
        private readonly int _hashedKey;

        public BlackboardKey(string name)
        {
            _name = name;
            _hashedKey = name.GetHashCode();
        }

        public bool Equals(BlackboardKey other) => _hashedKey == other._hashedKey;
        public override bool Equals(object obj) => obj is BlackboardKey other && Equals(other);
        public override int GetHashCode() => _hashedKey;
        public override string ToString() => _name;
        public static bool operator ==(BlackboardKey left, BlackboardKey right) => left.Equals(right);
        public static bool operator !=(BlackboardKey left, BlackboardKey right) => !left.Equals(right);
    }

    [Serializable]
    public class Blackboard : ISerializationCallbackReceiver
    {
        // Events for monitoring blackboard changes
        public delegate void BlackboardValueChanged<T>(string key, T oldValue, T newValue);

        public delegate void BlackboardKeyEvent(string key);

        private readonly Dictionary<Type, Delegate> _valueChangedEvents = new();
        public event BlackboardKeyEvent OnKeyCreated;
        public event BlackboardKeyEvent OnKeyRemoved;

        [SerializeField] private List<string> keys = new();
        [SerializeReference] private List<BlackboardValue> values = new();
        private Dictionary<BlackboardKey, int> _keyToIndexMap = new();

        private static BlackboardKey GetKey(string name)
        {
            Assert.IsNotNull(name, "Blackboard key name cannot be null");
            return new BlackboardKey(name);
        }

        public bool HasKey(string key) => _keyToIndexMap.ContainsKey(GetKey(key));

        public void SetValue<T>(string key, T value)
        {
            Assert.IsFalse(string.IsNullOrEmpty(key), "Blackboard key cannot be null or empty");
            var blackboardKey = GetKey(key);

            // Check if this is a new key that would collide with an existing one at a different index
            if (_keyToIndexMap.TryGetValue(blackboardKey, out var existingIndex))
            {
                if (keys[existingIndex] != key)
                {
                    throw new ArgumentException($"Key collision detected. '{key}' collides with existing key '{keys[existingIndex]}'");
                }

                // Get old value for event
                T oldValue = default;
                if (values[existingIndex] is BlackboardValue<T> typedOldValue) { oldValue = typedOldValue.Value; }

                values[existingIndex] = new BlackboardValue<T>(value);

                // Trigger value changed event
                if (_valueChangedEvents.TryGetValue(typeof(T), out var eventDelegate))
                {
                    var typedEvent = (BlackboardValueChanged<T>)eventDelegate;
                    typedEvent?.Invoke(key, oldValue, value);
                }
            }
            else
            {
                var index = values.Count;
                _keyToIndexMap[blackboardKey] = index;
                keys.Add(key);
                values.Add(new BlackboardValue<T>(value));

                // Trigger key created event
                OnKeyCreated?.Invoke(key);
            }
        }

        public T GetValue<T>(string key)
        {
            var blackboardKey = GetKey(key);
            if (!_keyToIndexMap.TryGetValue(blackboardKey, out var index)) { throw new KeyNotFoundException($"Key '{key}' not found in blackboard"); }
            if (values[index] is BlackboardValue<T> typedValue) { return typedValue.Value; }
            throw new InvalidOperationException(
                $"Type mismatch: Trying to get value of type {typeof(T)} from BlackboardValue of type {values[index].ValueType}"
            );
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            value = default;
            var blackboardKey = GetKey(key);
            if (!_keyToIndexMap.TryGetValue(blackboardKey, out var index)) { return false; }
            if (values[index] is not BlackboardValue<T> typedValue) { return false; }

            value = typedValue.Value;
            return true;
        }

        public void RemoveKey(string key)
        {
            var blackboardKey = GetKey(key);
            if (!_keyToIndexMap.TryGetValue(blackboardKey, out var index)) { return; }

            keys.RemoveAt(index);
            values.RemoveAt(index);
            _keyToIndexMap.Remove(blackboardKey);

            // Trigger key removed event
            OnKeyRemoved?.Invoke(key);

            // Update indices for remaining keys
            for (var i = index; i < keys.Count; i++)
            {
                var remainingKey = GetKey(keys[i]);
                _keyToIndexMap[remainingKey] = i;
            }
        }

        public void Clear()
        {
            var keysToRemove = new List<string>(keys);
            foreach (var key in keysToRemove) { OnKeyRemoved?.Invoke(key); }

            keys.Clear();
            values.Clear();
            _keyToIndexMap.Clear();
        }

        // Methods to subscribe to value changes for specific types
        public void SubscribeToValueChanged<T>(BlackboardValueChanged<T> handler)
        {
            var type = typeof(T);
            if (_valueChangedEvents.TryGetValue(type, out var existingDelegate)) { _valueChangedEvents[type] = Delegate.Combine(existingDelegate, handler); }
            else { _valueChangedEvents[type] = handler; }
        }

        public void UnsubscribeFromValueChanged<T>(BlackboardValueChanged<T> handler)
        {
            var type = typeof(T);
            if (_valueChangedEvents.TryGetValue(type, out var existingDelegate))
            {
                var newDelegate = Delegate.Remove(existingDelegate, handler);
                if (newDelegate != null) { _valueChangedEvents[type] = newDelegate; }
                else { _valueChangedEvents.Remove(type); }
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Nothing needed here
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Rebuild the key-to-index map after deserialization
            _keyToIndexMap = new Dictionary<BlackboardKey, int>(keys.Count);

            for (var i = 0; i < keys.Count; i++)
            {
                var key = GetKey(keys[i]);
                _keyToIndexMap[key] = i;
            }
        }
    }
}
