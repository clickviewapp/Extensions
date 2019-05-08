namespace ClickView.Extensions.RestClient.Authenticators.OAuth.TokenSource
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using Internal.ClientFactory;
    using Microsoft.Extensions.Logging;
    using Tokens;
    using TokenStore;

    public class RefreshTokenSource : ITokenSource
    {
        private readonly IClientFactory _clientFactory;
        private readonly ILogger<RefreshTokenSource> _logger;
        private readonly ITokenStore _tokenStore;

        public RefreshTokenSource(ITokenStore tokenStore, IClientFactory clientFactory, ILoggerFactory loggerFactory)
        {
            _tokenStore = tokenStore;
            _clientFactory = clientFactory;
            _logger = loggerFactory.CreateLogger<RefreshTokenSource>();
        }

        public async Task<IReadOnlyCollection<Token>> GetTokensAsync()
        {
            var refreshToken = await _tokenStore.GetTokenAsync(TokenType.RefreshToken).ConfigureAwait(false);
            if (refreshToken == null)
                return new List<Token>();

            var tokenClient = await _clientFactory.GetTokenClientAsync().ConfigureAwait(false);

            _logger.LogDebug("Refreshing token");

            var refreshTokenResponse =
                await tokenClient.RequestRefreshTokenAsync(refreshToken.Value).ConfigureAwait(false);

            //todo: handle errors
            if (refreshTokenResponse.IsError)
            {
                _logger.LogError(refreshTokenResponse.Exception, "Error refreshing token. {Message}",
                    refreshTokenResponse.ErrorDescription);
                return new List<Token>();
            }

            var accessToken = Helpers.CreateAccessToken(refreshTokenResponse);

            _logger.LogDebug("Tokens refreshed successfully.");

            return new List<Token>
            {
                accessToken,
                refreshToken
            };
        }

        public bool StoreTokens => true;
    }
}
