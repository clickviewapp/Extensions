namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Authenticators
{
    using Internal.ClientFactory;

    public class ClientCredentialsAuthenticator : OAuthAuthenticator<ClientCredentialsAuthenticatorOptions>
    {
        public ClientCredentialsAuthenticator(string endpoint, ClientCredentialsAuthenticatorOptions options) :
            base(options)
        {
            AddTokenSource(new ClientCredentialsTokenSource(ClientFactoryHelper.CreateClientFactory(endpoint, options),
                options.LoggerFactory, options.Scopes));
        }
    }
}
