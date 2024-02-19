namespace ClickView.Extensions.RestClient.Authenticators.OAuth
{
    using Endpoints;
    using IdentityModel;
    using IdentityModel.Client;
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

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

        public async Task<TokenResponse> GetClientCredentialsTokenAsync(string scope, CancellationToken cancellationToken = default)
        {
            var endpoints = await _endpointFactory.GetAsync().ConfigureAwait(false);

            return await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = endpoints.TokenEndpoint,
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                Scope = scope
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TokenResponse> GetResourceOwnerPasswordTokenAsync(string scope, string username, string password,
            CancellationToken cancellationToken = default)
        {
            var endpoints = await _endpointFactory.GetAsync().ConfigureAwait(false);

            return await _httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = endpoints.TokenEndpoint,
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                Scope = scope,
                UserName = username,
                Password = password
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TokenResponse> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var endpoints = await _endpointFactory.GetAsync().ConfigureAwait(false);

            return await _httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = endpoints.TokenEndpoint,
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                RefreshToken = refreshToken
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TokenRevocationResponse> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var endpoints = await _endpointFactory.GetAsync().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(endpoints.RevocationEndpoint))
            {
                throw new InvalidOperationException("Revocation endpoint not configured");
            }

            return await _httpClient.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = endpoints.RevocationEndpoint,
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                Token = refreshToken,
                TokenTypeHint = OidcConstants.TokenTypes.RefreshToken
            }, cancellationToken).ConfigureAwait(false);
        }
    }
}
