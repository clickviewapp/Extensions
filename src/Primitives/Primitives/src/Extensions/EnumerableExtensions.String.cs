namespace ClickView.Extensions.Primitives.Extensions
{
    using System.Collections.Generic;

    public static partial class EnumerableExtensions
    {
        public static string Join(this IEnumerable<string> enumerable, string separator)
        {
            return string.Join(separator, enumerable);
        }
    }
}