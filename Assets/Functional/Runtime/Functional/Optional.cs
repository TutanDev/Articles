using System;
using System.Runtime.CompilerServices;

namespace TutanDev.Core
{
    public static partial class F
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> Some<T>(T value) => Optional<T>.Some(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> None<T>() => default;
    }

    public readonly record struct Optional<T>
    {
        private readonly T _value;

        public bool IsSome { get; }
        public bool IsNone => !IsSome;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Optional(T value)
        {
            _value = value!;
            IsSome = true;
        }

        internal static Optional<T> None => default;
        internal static Optional<T> Some(T value)
        {
            if (typeof(T).IsValueType) 
                return new(value);

            if (value is null || value is UnityEngine.Object uo && uo == null) 
                    return None;

            return new(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Optional<R> Then<R>(Func<T, R> func) => Map(func);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Optional<T> Then<R>(Action<T> action) => Map(action.ToFunc());
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Optional<R> Then<R>(Func<T, Optional<R>> func) => Bind(func);

        // -------- Core ops --------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R Match<R>(Func<R> none, Func<T, R> some) => IsSome ? some(_value) : none();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Optional<R> Map<R>(Func<T, R> func) => Match(() => default, t => F.Some(func(t)));


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Optional<R> Bind<R>(Func<T, Optional<R>> func) => Match(() => default, t =>func(t));


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Optional<T> Where(Func<T, bool> predicate) => IsSome && predicate(_value) ? this : default;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetOrElse(T fallback) => Match(() => fallback, t => t);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetOrElse(Func<T> fallback) => Match(() => fallback(), t => t);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet(out T value)
        {
            value = _value!;
            return IsSome;
        }


        public override string ToString() => IsSome ? $"Some({_value})" : "None";
    }
}
