namespace ClickView.Extensions.RestClient.Authenticators.OAuth.AspNetCore
{
    using TokenSource;

    public class HttpContextAuthenticator : OAuthAuthenticator<HttpContextAuthenticatorOptions>
    {
        public HttpContextAuthenticator(HttpContextAuthenticatorOptions options) : base(options)
        {
            if (!options.EnableRefresh) return;

            var tokenClient = new TokenClient(HttpClient, options.ClientId, options.ClientSecret, CreateEndpointFactory(options.Authority));

            AddTokenSource(new RefreshTokenSource(TokenStore, tokenClient, options.LoggerFactory));
        }
    }
}