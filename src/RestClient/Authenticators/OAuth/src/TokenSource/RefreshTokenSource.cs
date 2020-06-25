namespace ClickView.Extensions.RestClient.Authenticators.OAuth.TokenSource
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Tokens;
    using TokenStore;

    public class RefreshTokenSource : ITokenSource
    {
        private readonly ILogger<RefreshTokenSource> _logger;
        private readonly ITokenStore _tokenStore;
        private readonly TokenClient _tokenClient;

        public RefreshTokenSource(ITokenStore tokenStore, TokenClient tokenClient, ILoggerFactory loggerFactory)
        {
            _tokenStore = tokenStore;
            _tokenClient = tokenClient;
            _logger = loggerFactory.CreateLogger<RefreshTokenSource>();
        }

        public async Task<IReadOnlyCollection<Token>> GetTokensAsync(CancellationToken cancellationToken = default)
        {
            var refreshToken = await _tokenStore.GetTokenAsync(TokenType.RefreshToken).ConfigureAwait(false);
            if (refreshToken == null)
                return new List<Token>();

            _logger.LogDebug("Refreshing token");

            var refreshTokenResponse = await _tokenClient.GetRefreshTokenAsync(refreshToken.Value, cancellationToken)
                .ConfigureAwait(false);

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
