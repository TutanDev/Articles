using UnityEngine;

namespace UFunctional
{
    public static partial class F
    {
        public static Error Error(string message) => new Error(message);
    }

    public record Error(string Message)
    {
        public static implicit operator Error(string message) => new(message);
    }
}
