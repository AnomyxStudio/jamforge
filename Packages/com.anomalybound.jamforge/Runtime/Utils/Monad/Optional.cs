using System;
using System.Collections.Generic;

namespace JamForge.Monad
{
    /// <summary>
    /// Represents an optional value that may or may not be present.
    /// This is a more robust alternative to nullable types, providing additional functional programming capabilities.
    /// </summary>
    /// <typeparam name="T">The type of the wrapped value</typeparam>
    public readonly struct Optional<T>
    {
        private readonly T _value;
        private readonly bool _hasValue;

        /// <summary>
        /// Gets whether this Optional has a value.
        /// </summary>
        public bool HasValue => _hasValue;

        /// <summary>
        /// Creates an Optional with a value.
        /// </summary>
        /// <param name="value">The value to wrap</param>
        private Optional(T value)
        {
            _value = value;
            _hasValue = true;
        }

        /// <summary>
        /// Creates an Optional from a value, handling null case automatically.
        /// </summary>
        /// <param name="value">The value to create an Optional from</param>
        public static Optional<T> From(T value) => value == null ? Empty : new Optional<T>(value);

        /// <summary>
        /// Gets an empty Optional instance.
        /// </summary>
        public static Optional<T> Empty => new();

        /// <summary>
        /// Gets the value if present, otherwise throws InvalidOperationException.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when accessing Value on an empty Optional</exception>
        public T Value
        {
            get
            {
                if (!_hasValue) { throw new InvalidOperationException("Cannot access Value on empty Optional"); }
                return _value;
            }
        }

        /// <summary>
        /// Gets the value if present, otherwise returns the default value.
        /// </summary>
        /// <param name="defaultValue">The default value to return if no value is present</param>
        public T GetOrDefault(T defaultValue = default) => _hasValue ? _value : defaultValue;

        /// <summary>
        /// Executes an action if a value is present.
        /// </summary>
        /// <param name="action">The action to execute with the value</param>
        public void IfPresent(Action<T> action)
        {
            if (_hasValue && action != null) { action(_value); }
        }

        /// <summary>
        /// Maps the Optional to a new Optional with a different type using the provided mapping function.
        /// </summary>
        /// <typeparam name="TResult">The type to map to</typeparam>
        /// <param name="mapper">The mapping function</param>
        public Optional<TResult> Map<TResult>(Func<T, TResult> mapper) =>
            !_hasValue
                ? Optional<TResult>.Empty
                : Optional<TResult>.From(mapper(_value));

        /// <summary>
        /// Returns the value if present, otherwise returns the result of the supplier function.
        /// </summary>
        /// <param name="supplier">Function to generate alternative value</param>
        public T OrElseGet(Func<T> supplier) => _hasValue ? _value : supplier();

        /// <summary>
        /// Returns the value if present, otherwise throws the supplied exception.
        /// </summary>
        /// <param name="exceptionSupplier">Function to generate exception</param>
        public T OrElseThrow<TException>(Func<TException> exceptionSupplier) where TException : Exception => _hasValue ? _value : throw exceptionSupplier();

        /// <summary>
        /// Filters the Optional using a predicate. If the predicate returns false, returns Empty.
        /// </summary>
        /// <param name="predicate">The predicate to test the value against</param>
        public Optional<T> Filter(Func<T, bool> predicate) => !_hasValue || !predicate(_value) ? Empty : this;

        /// <summary>
        /// Creates an Optional containing the specified value.
        /// </summary>
        /// <param name="value">The value to wrap</param>
        public static Optional<T> Some(T value) => From(value);

        /// <summary>
        /// Creates an empty Optional.
        /// </summary>
        public static Optional<T> None() => Empty;

        /// <summary>
        /// Pattern matches on the Optional, executing different actions based on whether a value is present.
        /// </summary>
        /// <param name="some">Action to execute if value is present</param>
        /// <param name="none">Action to execute if value is not present</param>
        public void Match(Action<T> some, Action none)
        {
            if (_hasValue && some != null)
            {
                some(_value);
            }
            else if (!_hasValue && none != null)
            {
                none();
            }
        }

        /// <summary>
        /// Pattern matches on the Optional, returning a value based on whether a value is present.
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="some">Function to execute if value is present</param>
        /// <param name="none">Function to execute if value is not present</param>
        public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
        {
            return _hasValue ? some(_value) : none();
        }

        public override bool Equals(object obj)
        {
            if (obj is Optional<T> other)
            {
                if (!_hasValue && !other._hasValue) { return true; }
                if (_hasValue != other._hasValue) { return false; }
                return EqualityComparer<T>.Default.Equals(_value, other._value);
            }
            return false;
        }

        public override int GetHashCode() => _hasValue ? _value?.GetHashCode() ?? 0 : 0;

        public override string ToString() => _hasValue ? $"Optional[{_value}]" : "Optional.Empty";

        // Implicit conversion from T to Optional<T>
        public static implicit operator Optional<T>(T value) => From(value);
    }
}
