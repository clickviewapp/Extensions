namespace ClickView.Extensions.RestClient.Authenticators.OAuth
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using Internal.Endpoints;

    public class TokenClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly IAuthenticatorEndpointFactory _endpointFactory;

        public TokenClient(HttpClient httpClient, string clientId, string clientSecret, IAuthenticatorEndpointFactory endpointFactory)
        {
            _httpClient = httpClient;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _endpointFactory = endpointFactory;
        }

        public async Task<TokenResponse> GetClientCredentialsTokenAsync(string scope)
        {
            var endpoints = await _endpointFactory.GetAsync().ConfigureAwait(false);

            return await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                Scope = scope,
                Address = endpoints.TokenEndpoint
            }).ConfigureAwait(false);
        }

        public async Task<TokenResponse> GetRefreshTokenAsync(string refreshToken)
        {
            var endpoints = await _endpointFactory.GetAsync().ConfigureAwait(false);

            return await _httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = endpoints.TokenEndpoint,
                RefreshToken = refreshToken
            }).ConfigureAwait(false);
        }
    }
}
