namespace ClickView.Extensions.Primitives.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static partial class EnumerableExtensions
    {
        public static bool TryGetFirstValue<T>(this IEnumerable<T> source, out T value)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                //is a list?
                case IList<T> col when col.Count == 0:
                    value = default;
                    return false;
                case IList<T> col:
                    value = col[0];
                    return true;
            }

            //enumerate
            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                {
                    value = default;
                    return false;
                }

                value = e.Current;
                return true;
            }
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

            return enumerable.ToList();
        }
    }
}
