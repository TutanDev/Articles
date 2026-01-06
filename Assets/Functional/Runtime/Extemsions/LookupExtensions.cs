using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UFunctional
{
    public static class LookupExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> LookupComponent<T>(this GameObject go) where T : Component
            => go && go.TryGetComponent<T>(out var c) ? Some(c) : default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<Transform> LookupParent(this Transform t)
            => t && t.parent ? Some(t.parent) : default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> Lookup<K, T>(this IDictionary<K, T> dict, K key)
           => dict.TryGetValue(key, out T value) ? Some(value) : default;
    }
}
