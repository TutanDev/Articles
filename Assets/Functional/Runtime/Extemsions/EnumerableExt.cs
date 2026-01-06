using System;
using System.Collections.Generic;
using System.Linq;

namespace UFunctional
{
    public static class EnumerableExt
    {
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, params T[] ts)
            => source.Concat(ts);

        static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T val)
        {
            yield return val;
            foreach (T t in source) yield return t;
        }

        public static Optional<T> Head<T>(this IEnumerable<T> list)
        {
            if (list == null) return default;
            var enumerator = list.GetEnumerator();
            return enumerator.MoveNext() ? Some(enumerator.Current) : default;
        }

        public static Optional<T> FindFirst<T>(this IEnumerable<T> source, Func<T, bool> predicate)
            => source.Where(predicate).Head();

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> list)
            => list.SelectMany(x => x);

        public static R Match<T, R>(this IEnumerable<T> list,
            Func<R> Empty,
            Func<T, IEnumerable<T>, R> Otherwise)
            => list.Head()
                .Match(Empty, head => Otherwise(head, list.Skip(1)));

        static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> @this, Func<T, bool> pred)
        {
            foreach (var item in @this)
            {
                if (pred(item)) yield return item;
                else yield break;
            }
        }

        static IEnumerable<T> DropWhile<T>(this IEnumerable<T> @this, Func<T, bool> pred)
        {
            bool clean = true;
            foreach (var item in @this)
            {
                if (!clean || !pred(item))
                {
                    yield return item;
                    clean = false;
                }
            }
        }
    }

    public static class Enumerable_Monad
    {
        // RETURN
        public static Func<T, IEnumerable<T>> Return<T>() => t => List(t);

        // MAP
        public static IEnumerable<R> Map_InTermsOfFold<T, R>
            (this IEnumerable<T> ts, Func<T, R> f)
            => ts.Aggregate(List<R>(), (rs, t) => rs.Append(f(t)));

        public static IEnumerable<R> Map<T, R>
            (this IEnumerable<T> list, Func<T, R> func)
            => list.Select(func);

        public static IEnumerable<Func<T2, R>> Map<T1, T2, R>
            (this IEnumerable<T1> list, Func<T1, T2, R> func)
            => list.Map(func.Curry());

        public static IEnumerable<Func<T2, Func<T3, R>>> Map<T1, T2, T3, R>
            (this IEnumerable<T1> opt, Func<T1, T2, T3, R> func)
            => opt.Map(func.Curry());

        // FOREACH
        public static IEnumerable<Unit> ForEach<T>(this IEnumerable<T> ts, Action<T> action)
            => ts.Map(action.ToFunc());

        // BIND
        static IEnumerable<R> Bind_InTermsOfFold<T, R>
            (this IEnumerable<T> ts, Func<T, IEnumerable<R>> f)
            => ts.Aggregate(List<R>(), (rs, t) => rs.Concat(f(t)));

        public static IEnumerable<R> Bind<T, R>
            (this IEnumerable<T> list, Func<T, IEnumerable<R>> func)
            => list.SelectMany(func);

        public static IEnumerable<R> Bind<T, R>
            (this IEnumerable<T> list, Func<T, Optional<R>> func)
          => list.Bind(t => func(t).AsEnumerable());
    }
}
