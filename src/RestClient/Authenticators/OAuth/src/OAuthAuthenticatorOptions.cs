namespace ClickView.Extensions.RestClient.Authenticators.OAuth
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using TokenStore;

    public class OAuthAuthenticatorOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public ITokenStore TokenStore { get; set; } = new InMemoryTokenStore();
        public ILoggerFactory LoggerFactory { get; set; } = new NullLoggerFactory();

        public bool EnableDiscovery { get; set; } = true;

        /// <summary>
        /// If set to true, a <see cref="Exceptions.NoTokenException"/> will be thrown if no token could be found in any store
        /// </summary>
        public bool ThrowIfNoToken { get; set; } = false;

        public AuthenticatorEndpoints Endpoints { get; set; }
    }
}