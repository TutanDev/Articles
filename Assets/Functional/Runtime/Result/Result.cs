using System;
using System.Runtime.CompilerServices;
using UnityEditor.PackageManager;

namespace UFunctional
{
    public static partial class F
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Success<T>(T value)
        {
            if (typeof(T).IsValueType)
                return new(value);

            if (value is null || value is UnityEngine.Object uo && uo == null)
                return new("Value is null");

            return new(value);
        }
    }

    public readonly record struct Result<T>
    {
        internal readonly T _value;
        internal readonly Error _error;

        public T ValueUnsafe => _value;
        public Error ErrorUnsafe => _error;

        public bool IsSuccess { get; init; }
        public bool IsError => !IsSuccess;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Result(T data) 
            => (IsSuccess, _value, _error) = (true, data, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Result(Error error) 
            => (IsSuccess, _value, _error) = (false, default, error);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R Match<R>(Func<Error, R> onError, Func<T, R> onSuccess) => IsSuccess ? onSuccess(_value) : onError(_error);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit Match(Action<Error> onError, Action<T> onSuccess) => Match(onError.ToFunc(), onSuccess.ToFunc());


        public override string ToString() => IsSuccess ? $"Success: {_value}" : $"Error: {_error}";
        public static implicit operator Result<T>(T value) => Success(value);
        public static implicit operator Result<T>(Error error) => new(error);
    }
}
