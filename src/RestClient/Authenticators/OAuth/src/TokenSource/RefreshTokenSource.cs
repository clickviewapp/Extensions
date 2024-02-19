namespace ClickView.Extensions.RestClient.Authenticators.OAuth.TokenSource
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Tokens;
    using TokenStore;
    using Utilities.Threading;

    public class RefreshTokenSource : ITokenSource
    {
        private readonly ILogger<RefreshTokenSource> _logger;
        private readonly ITokenStore _tokenStore;
        private readonly TokenClient _tokenClient;
        private readonly TaskSingle<string> _singleTask;

        public RefreshTokenSource(ITokenStore tokenStore, TokenClient tokenClient, ILoggerFactory loggerFactory)
        {
            _tokenStore = tokenStore;
            _tokenClient = tokenClient;
            _logger = loggerFactory.CreateLogger<RefreshTokenSource>();
            _singleTask = new TaskSingle<string>();
        }

        public async Task<IReadOnlyCollection<Token>> GetTokensAsync(CancellationToken cancellationToken = default)
        {
            var refreshToken = await _tokenStore.GetTokenAsync(TokenType.RefreshToken).ConfigureAwait(false);
            if (refreshToken == null)
                return Array.Empty<Token>();

            var accessToken = await _tokenStore.GetTokenAsync(TokenType.AccessToken).ConfigureAwait(false);
            if (accessToken is not null && !accessToken.HasExpired())
            {
                return new List<Token>
                {
                    accessToken,
                    refreshToken
                };
            }

            _logger.LogDebug("Refreshing token");

            var refreshTokenResponse = await _singleTask.RunAsync(refreshToken.Value, () =>
                _tokenClient.GetRefreshTokenAsync(refreshToken.Value, cancellationToken)).ConfigureAwait(false);

            //todo: handle errors
            if (refreshTokenResponse.IsError)
            {
                _logger.LogError(refreshTokenResponse.Exception, "Error refreshing token. {Message}",
                    refreshTokenResponse.ErrorDescription);

                return Array.Empty<Token>();
            }

            var newAccessToken = TokenHelpers.CreateAccessToken(refreshTokenResponse);
            var newRefreshToken = TokenHelpers.CreateRefreshToken(refreshTokenResponse);

            _logger.LogDebug("Tokens refreshed successfully");

            return new List<Token>
            {
                newAccessToken,
                newRefreshToken
            };
        }

        public async Task RevokeTokenAsync(CancellationToken cancellationToken = default)
        {
            var refreshToken = await _tokenStore.GetTokenAsync(TokenType.RefreshToken).ConfigureAwait(false);
            if (refreshToken == null)
            {
                _logger.LogDebug("Refresh token was not found in the store");
                return;
            }

            try
            {
                var response = await _tokenClient.RevokeRefreshTokenAsync(refreshToken.Value, cancellationToken)
                    .ConfigureAwait(false);

                if (response.IsError)
                    _logger.LogError("Failed to revoke refresh token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while revoking the refresh token");
            }
        }

        public bool StoreTokens => true;
    }
}
