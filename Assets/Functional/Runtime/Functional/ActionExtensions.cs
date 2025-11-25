using System;

using Unit = System.ValueTuple;


namespace TutanDev.Core
{
    public static class ActionExtensions
    {
        public static Func<Unit> ToFunc(this Action action)=> () => 
        { 
            action(); 
            return default; 
        };

        public static Func<T, T> ToFunc<T>(this Action<T> action) => (t) =>
        {
            action(t);
            return t;
        };
    }
}
