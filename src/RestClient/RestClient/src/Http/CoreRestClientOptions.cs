namespace ClickView.Extensions.RestClient.Http
{
    using System.Net.Http.Headers;
    using Authentication;

    public class CoreRestClientOptions
    {
        public CompressionMethod CompressionMethod { get; set; } = CompressionMethod.None;
        public IAuthenticator? Authenticator { get; set; } = null;
        public ProductInfoHeaderValue? DefaultUserAgent { get; set; }
    }
}
