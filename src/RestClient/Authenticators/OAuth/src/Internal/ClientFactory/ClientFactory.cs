namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Internal.ClientFactory
{
    using System;
    using System.Threading.Tasks;
    using IdentityModel.Client;

    internal class ClientFactory : IClientFactory
    {
        private readonly Lazy<TokenClient> _tokenClientLazy;

        public ClientFactory(AuthenticatorEndpoints endpoints)
        {
            _tokenClientLazy = new Lazy<TokenClient>(() => new TokenClient(endpoints.TokenEndpoint));
        }

        public Task<TokenClient> GetTokenClientAsync()
        {
            return Task.FromResult(_tokenClientLazy.Value);
        }
    }
}