namespace ClickView.Extensions.Primitives.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public static partial class EnumerableExtensions
    {
        public static bool TryGetFirstValue<T>(this IEnumerable<T> source, [MaybeNullWhen(false)] out T value)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            // Check to see if the list has 0 items in it. If it does, return false
#if NET
            if (source.TryGetNonEnumeratedCount(out var count) && count == 0)
            {
                value = default;
                return false;
            }
#else
            if (source is ICollection<T> {Count: 0} or ICollection {Count: 0})
            {
                value = default;
                return false;
            }
#endif

            if (source is IList<T> list)
            {
                value = list[0];
                return true;
            }

            //enumerate
            using var e = source.GetEnumerator();

            if (!e.MoveNext())
            {
                value = default;
                return false;
            }

            value = e.Current;
            return true;
        }

        public static ICollection<T> AsCollection<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable is ICollection<T> collection)
                return collection;

            return enumerable.ToList();
        }

        public static IReadOnlyList<T> AsReadOnlyList<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable is IReadOnlyList<T> collection)
                return collection;

            return new ReadOnlyCollection<T>(enumerable.ToList());
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable) where T : class
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));

            foreach (var t in enumerable)
            {
                if (t is not null)
                    yield return t;
            }
        }

        public static IEnumerable<TResult> WhereNotNull<T, TResult>(
            this IEnumerable<T> enumerable,
            Func<T, TResult?> selector)
            where TResult : class
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            foreach (var t in enumerable.Select(selector))
            {
                if (t is not null)
                    yield return t;
            }
        }
    }
}
