namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Internal.ClientFactory
{
    public static class ClientFactoryHelper
    {
        public static IClientFactory CreateClientFactory(string authority, OAuthAuthenticatorOptions options)
        {
            if (options.EnableDiscovery)
                return new DiscoveryClientFactory(authority, options);

            return new ClientFactory(options.Endpoints);
        }
    }
}