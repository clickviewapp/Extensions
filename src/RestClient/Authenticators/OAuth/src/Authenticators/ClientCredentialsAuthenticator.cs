namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Authenticators
{
    public class ClientCredentialsAuthenticator : OAuthAuthenticator<ClientCredentialsAuthenticatorOptions>
    {
        public ClientCredentialsAuthenticator(string endpoint, ClientCredentialsAuthenticatorOptions options) :
            base(options)
        {
            var tokenClient = new TokenClient(HttpClient, options.ClientId, options.ClientSecret, CreateEndpointFactory(endpoint));

            AddTokenSource(new ClientCredentialsTokenSource(tokenClient, options.LoggerFactory, options.Scopes));
        }
    }
}
