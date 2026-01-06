using System;
using System.Runtime.CompilerServices;

namespace UFunctional
{
    public static class ActionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<Unit> ToFunc(this Action action)
            => () =>
            {
                action();
                return default;
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<T, Unit> ToFunc<T>(this Action<T> action)
            => t => 
            { 
                action(t); 
                return default; 
            };
    }
}
