using System;
using System.Runtime.CompilerServices;

namespace UFunctional
{
    public struct NoneType { }
    public static partial class F
    {
        public static NoneType None => default;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> Some<T>(T value)
        {
            if (typeof(T).IsValueType)
                return new(value);

            if (value is null || value is UnityEngine.Object uo && uo == null)
                return default;

            return new(value);
        }
    }

    public readonly record struct Optional<T>
    {
        internal readonly T _value;


        public bool IsSome { get; init; }
        public bool IsNone => !IsSome;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Optional(T value) => (IsSome, _value) = (true, value);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R Match<R>(Func<R> onNone, Func<T, R> onSome) => IsSome ? onSome(_value) : onNone();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit Match(Action onNone, Action<T> onSome) => Match(onNone.ToFunc(), onSome.ToFunc());


        public override string ToString() => IsSome ? $"Some: {_value}" : "None";
        public static implicit operator Optional<T>(T value) => Some(value);
        public static implicit operator Optional<T>(NoneType _) => default;
    }
}
