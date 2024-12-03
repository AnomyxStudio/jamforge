using System;

namespace JamForge.Monad
{
    /// <summary>
    /// Represents a value of one of two possible types (a disjoint union).
    /// An Either is a Left value or a Right value, commonly used for error handling
    /// where Left represents failure and Right represents success.
    /// </summary>
    /// <typeparam name="TLeft">The type of the Left value</typeparam>
    /// <typeparam name="TRight">The type of the Right value</typeparam>
    public readonly struct Either<TLeft, TRight>
    {
        private readonly TLeft _left;
        private readonly TRight _right;

        /// <summary>
        /// Gets whether this Either is a Left value.
        /// </summary>
        public bool IsLeft { get; }

        /// <summary>
        /// Gets whether this Either is a Right value.
        /// </summary>
        public bool IsRight => !IsLeft;

        /// <summary>
        /// Creates a Left Either.
        /// </summary>
        /// <param name="left">The Left value</param>
        private Either(TLeft left)
        {
            _left = left;
            _right = default;
            IsLeft = true;
        }

        /// <summary>
        /// Creates a Right Either.
        /// </summary>
        /// <param name="right">The Right value</param>
        private Either(TRight right)
        {
            _left = default;
            _right = right;
            IsLeft = false;
        }

        /// <summary>
        /// Creates a Left Either.
        /// </summary>
        /// <param name="value">The Left value</param>
        public static Either<TLeft, TRight> Left(TLeft value) => new(value);

        /// <summary>
        /// Creates a Right Either.
        /// </summary>
        /// <param name="value">The Right value</param>
        public static Either<TLeft, TRight> Right(TRight value) => new(value);

        /// <summary>
        /// Gets the Left value if present, otherwise throws InvalidOperationException.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when accessing LeftValue on a Right Either</exception>
        public TLeft LeftValue
        {
            get
            {
                if (!IsLeft) { throw new InvalidOperationException("Cannot access LeftValue on Right Either"); }
                return _left;
            }
        }

        /// <summary>
        /// Gets the Right value if present, otherwise throws InvalidOperationException.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when accessing RightValue on a Left Either</exception>
        public TRight RightValue
        {
            get
            {
                if (IsLeft) { throw new InvalidOperationException("Cannot access RightValue on Left Either"); }
                return _right;
            }
        }

        /// <summary>
        /// Maps the Left value of this Either to a new type.
        /// </summary>
        /// <typeparam name="TNewLeft">The new Left type</typeparam>
        /// <param name="mapper">The mapping function</param>
        public Either<TNewLeft, TRight> MapLeft<TNewLeft>(Func<TLeft, TNewLeft> mapper) =>
            IsLeft
                ? Either<TNewLeft, TRight>.Left(mapper(_left))
                : Either<TNewLeft, TRight>.Right(_right);

        /// <summary>
        /// Maps the Right value of this Either to a new type.
        /// </summary>
        /// <typeparam name="TNewRight">The new Right type</typeparam>
        /// <param name="mapper">The mapping function</param>
        public Either<TLeft, TNewRight> MapRight<TNewRight>(Func<TRight, TNewRight> mapper) =>
            IsLeft
                ? Either<TLeft, TNewRight>.Left(_left)
                : Either<TLeft, TNewRight>.Right(mapper(_right));

        /// <summary>
        /// Maps the Right value of this Either to a new type.
        /// </summary>
        /// <typeparam name="TNewRight">The new Right type</typeparam>
        /// <param name="mapper">The mapping function</param>
        public Either<TLeft, TNewRight> Map<TNewRight>(Func<TRight, TNewRight> mapper)
        {
            if (IsLeft) { return Either<TLeft, TNewRight>.Left(_left); }
            return Either<TLeft, TNewRight>.Right(mapper.Invoke(_right));
        }

        /// <summary>
        /// Executes one of two actions depending on whether this Either is Left or Right.
        /// </summary>
        /// <param name="leftAction">Action to execute for Left value</param>
        /// <param name="rightAction">Action to execute for Right value</param>
        public void Match(Action<TLeft> leftAction, Action<TRight> rightAction)
        {
            if (IsLeft) { leftAction?.Invoke(_left); }
            else { rightAction?.Invoke(_right); }
        }

        /// <summary>
        /// Pattern matches on the Either, returning a value based on whether it's Left or Right.
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="left">Function to execute if Either is Left</param>
        /// <param name="right">Function to execute if Either is Right</param>
        public TResult Match<TResult>(Func<TLeft, TResult> left, Func<TRight, TResult> right)
        {
            return IsLeft ? left.Invoke(_left) : right.Invoke(_right);
        }

        /// <summary>
        /// Filters the Right value using a predicate. If the predicate returns false, returns a Left with the provided value.
        /// </summary>
        /// <param name="predicate">The predicate to test the Right value against</param>
        /// <param name="leftSupplier">Function to generate the Left value if predicate fails</param>
        public Either<TLeft, TRight> Filter(Func<TRight, bool> predicate, Func<TLeft> leftSupplier)
        {
            if (IsLeft) return this;
            return predicate.Invoke(_right) ? this : Left(leftSupplier.Invoke());
        }

        /// <summary>
        /// Gets the Right value if present, otherwise returns the provided default value.
        /// </summary>
        /// <param name="defaultValue">The default value to return if this is a Left Either</param>
        public TRight GetRightOrDefault(TRight defaultValue = default) => IsLeft ? defaultValue : _right;

        /// <summary>
        /// Gets the Left value if present, otherwise returns the provided default value.
        /// </summary>
        /// <param name="defaultValue">The default value to return if this is a Right Either</param>
        public TLeft GetLeftOrDefault(TLeft defaultValue = default) => IsLeft ? _left : defaultValue;

        public override bool Equals(object obj)
        {
            if (obj is not Either<TLeft, TRight> other) { return false; }
            if (IsLeft != other.IsLeft) { return false; }
            return IsLeft
                ? Equals(_left, other._left)
                : Equals(_right, other._right);
        }

        public override int GetHashCode() => IsLeft ? _left?.GetHashCode() ?? 0 : _right?.GetHashCode() ?? 0;

        public override string ToString() => IsLeft ? $"Left({_left})" : $"Right({_right})";
    }
}
