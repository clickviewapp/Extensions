namespace ClickView.Extensions.RestClient.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class EnumerableExtensions
    {
        public static bool TryGetFirstValue<T>(this IEnumerable<T> source, out T value)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            //is a list?
            if (source is IList<T> col)
            {
                if (col.Count == 0)
                {
                    value = default;
                    return false;
                }

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
    }
}