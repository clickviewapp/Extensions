namespace ClickView.Extensions.RestClient.Http
{
    using Authentication;

    public class CoreRestClientOptions
    {
        public CompressionMethod CompressionMethod { get; set; } = CompressionMethod.None;
        public IAuthenticator Authenticator { get; set; } = null;

        public bool ThrowOnErrorStatusCode { get; set; } = true;
    }
}
