namespace ClickView.Extensions.Utilities.Tests.Security.Cryptography
{
    using System.IO;
    using Utilities.Security.Cryptography;
    using Xunit;

    public class X509CertificateHelperTests
    {
        [Fact]
        public static void GetBytes()
        {
            var fileContents = File.ReadAllText("./Security/Cryptography/4096b-rsa-example-cert.pem");

            var certificate = X509CertificateHelper.GetCertificate(fileContents);

            Assert.NotNull(certificate);

            // openssl x509 -noout -fingerprint -sha1 -inform pem -in 4096b-rsa-example-cert.pem
            Assert.Equal("2013BADFCD6BDD058E39B98D6B1177E870603B93", certificate.Thumbprint);
        }
    }
}
