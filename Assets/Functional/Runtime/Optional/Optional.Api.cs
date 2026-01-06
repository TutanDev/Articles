using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UFunctional
{
    public static partial class OptionalExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasValue<T>(this Optional<T> opt, out T value)
        {
            if (opt.IsSome)
            {
                value = opt._value;
                return true;
            }

            value = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ValueUnsafe<T>(this Optional<T> opt)
            => opt.Match(
                () => { throw new InvalidOperationException($"ValueUnsafe<{typeof(T).FullName}> was called on None. Ensure the Option is Some before using ValueUnsafe"); },
                (t) => t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<R> Then<T, R>(this Optional<T> opt, Func<T, R> func) => opt.Map(func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> Then<T>(this Optional<T> opt, Action<T> action) => opt.Map(F.Tee(action));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<R> Then<T, R>(this Optional<T> opt, Func<T, Optional<R>> func) => opt.Bind(func);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Or<T>(this Optional<T> opt, T fallback) => opt.Match(() => fallback, t => t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T OrElse<T>(this Optional<T> opt, Func<T> fallback) => opt.Match(() => fallback(), t => t);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> Filter<T>(this Optional<T> opt, Func<T, bool> predicate) => opt.IsSome && predicate(opt._value) ? opt : default;

        
        public static IEnumerable<Optional<R>> Traverse<T, R>(this Optional<T> @this, Func<T, IEnumerable<R>> func)
         => @this.Match(
            () => List((Optional<R>)None),
            (t) => func(t).Map(r => Some(r)));
    }
}
