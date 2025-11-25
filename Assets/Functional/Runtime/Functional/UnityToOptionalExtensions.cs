using System.Runtime.CompilerServices;
using UnityEngine;

namespace TutanDev.Core
{
    public static class UnityToOptionalExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> LookupComponent<T>(this GameObject go) where T : Component
            => go && go.TryGetComponent<T>(out var c) ? Optional<T>.Some(c) : F.None<T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<Transform> LookupParent(this Transform t)
            => t && t.parent ? Optional<Transform>.Some(t.parent) : F.None<Transform>();
    }
}
