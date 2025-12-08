namespace ClickView.Extensions.Utilities.Security.Cryptography
{
    using System;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text.RegularExpressions;

    public static class X509CertificateHelper
    {
        private static readonly Regex Regex = new(
            @"-----BEGIN CERTIFICATE-----\r?\n?([A-Za-z0-9+\/=\s]+?)-----END CERTIFICATE-----",
            RegexOptions.Compiled);

        public static byte[] GetBytes(string base64String)
        {
            if (base64String == null)
                throw new ArgumentNullException(nameof(base64String));

            var match = Regex.Match(base64String);

            if (!match.Success || match.Groups.Count != 2)
                throw new CryptographicException("Input is not in the correct format");

            var str = match.Groups[1].Value;

            return Convert.FromBase64String(str);
        }

        public static X509Certificate2 GetCertificate(string base64String)
        {
#if NET10_0_OR_GREATER
            return X509CertificateLoader.LoadCertificate(GetBytes(base64String));
#else
            return new X509Certificate2(GetBytes(base64String));
#endif
        }
    }
}
