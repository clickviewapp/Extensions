namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Endpoints
{
    using System.Threading.Tasks;

    public class DefaultEndpointFactory : IAuthenticatorEndpointFactory
    {
        private readonly AuthenticatorEndpoints _endpoints;

        public DefaultEndpointFactory(AuthenticatorEndpoints endpoints)
        {
            _endpoints = endpoints;
        }

        public Task<AuthenticatorEndpoints> GetAsync()
        {
            return Task.FromResult(_endpoints);
        }
    }
}
