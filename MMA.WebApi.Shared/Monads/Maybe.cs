namespace MMA.WebApi.Shared.Monads
{
    /// <summary>
    /// Maybe monad implementation
    /// </summary>
    /// <example>
    /// Maybe<string> str = "hello";
    /// 
    /// if (result is Maybe<string>.None)
    /// {
    ///     Console.WriteLine("String is null")
    /// }
    /// 
    /// if (result is Maybe<string>.Some)
    /// {
    ///     Console.WriteLine("String is not null")
    /// }
    /// </example>
    /// <remarks>
    /// For explanation see https://www.dotnetcurry.com/patterns-practices/1510/maybe-monad-csharp
    /// </remarks>
    /// <typeparam name="T">Type of object, implicit conversion is possible</typeparam>
    public abstract class Maybe<T> where T : class
    {
        private Maybe()
        {
        }

        public sealed class Some : Maybe<T>
        {
            public Some(T value) => Value = value;

            public override T Value { get; }
        }

        public sealed class None : Maybe<T>
        {
        }

        public virtual T Value
        {
            get
            {
                if (this is Some some)
                {
                    return some.Value;
                }

                return default(T);
            }
        }

        public bool TryGetValue(out T value)
        {
            if (this is Some some)
            {
                value = some.Value;
                return true;
            }

            value = default(T);

            return false;
        }

        /// <summary>
        /// Provide implicit conversion
        /// </summary>
        /// <example>
        /// Maybe<UserModel> model = new UserModel();
        /// 
        /// if (result is Maybe<string>.None)
        /// {
        ///     Console.WriteLine("String is null")
        /// }
        /// 
        /// if (result is Maybe<string>.Some)
        /// {
        ///     Console.WriteLine("String is not null")
        /// }
        /// </example>
        /// <remarks>
        /// For explanation see https://www.dotnetcurry.com/patterns-practices/1510/maybe-monad-csharp
        /// </remarks>
        /// <param name="value">Object instance, can be null</param>
        public static implicit operator Maybe<T>(T value)
        {
            if (value == null)
                return new None();

            return new Some(value);
        }

        public static implicit operator T(Maybe<T> value)
        {
            if (value is Maybe<T>.None)
                return default;

            return value;
        }
    }
}
