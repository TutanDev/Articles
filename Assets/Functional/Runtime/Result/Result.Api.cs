using System;
using System.Runtime.CompilerServices;


namespace UFunctional
{
    public static partial class ResultExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<R> Then<T, R>(this Result<T> result, Func<T, R> func) => result.Map(func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Then<T>(this Result<T> result, Action<T> action) => result.Map(F.Tee(action));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<R> Then<T, R>(this Result<T> result, Func<T, Result<R>> func) => result.Bind(func);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Or<T>(this Result<T> result, T fallback) => result.Match(() => fallback, t => t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T OrElse<T>(this Result<T> result, Func<T> fallback) => result.Match(() => fallback(), t => t);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Filter<T>(this Result<T> result, Func<T, bool> predicate) => result.IsSuccess && predicate(result._value) ? result : default;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSuccess<T>(this Result<T> result, out T value)
        {
            if (result.IsSuccess)
            {
                value = result._value;
                return true;
            }

            value = default;
            return false;
        }

        public static T ValueUnsafe<T>(this Result<T> @this)
            => @this.Match(
                (fail) => { throw new InvalidOperationException($"ValueUnsafe<{typeof(T).FullName}> was called on an Error ({fail}). Ensure the Result is Success before using ValueUnsafe"); },
                (t) => t);

        public static Error ErrorUnsafe<T>(this Result<T> @this)
            => @this.Match(
                (fail) => fail,
                (t) => { throw new InvalidOperationException("ErrorUnsafe was called on a Success"); });
    }
}
