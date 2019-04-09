namespace ClickView.Extensions.Primitives.Extensions
{
    using System;
    using System.Text;

    public static class StringExtensions
    {
        public static string ToBase64String(this string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64String(this string base64)
        {
            if (base64 == null)
                throw new ArgumentNullException(nameof(base64));

            var bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
