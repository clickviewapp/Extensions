namespace ClickView.Extensions.RestClient.Authenticators.OAuth.AspNetCore
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Tokens;
    using TokenStore;

    public class HttpContextTokenStore : ITokenStore
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HttpContextTokenStore> _logger;

        private const string ExpiresAtKey = "expires_at";

        public HttpContextTokenStore(IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger<HttpContextTokenStore>();
        }

        public async Task<Token> GetTokenAsync(TokenType tokenType)
        {
            var httpContext = GetHttpContext();

            var expireTime = GetTokenExpireTimeAsync(httpContext);
            var tokenValue = httpContext.GetTokenAsync(GetHttpTokenName(tokenType));

            return CreateToken(tokenType, await tokenValue, await expireTime);
        }

        //Token reference https://github.com/aspnet/AspNetCore/blob/master/src/Security/Authentication/OAuth/src/OAuthHandler.cs#L116
        public async Task StoreTokensAsync(IEnumerable<Token> tokens)
        {
            var httpContext = GetHttpContext();
            var authResult = await httpContext.AuthenticateAsync();

            foreach (var token in tokens)
            {
                var tokenName = GetHttpTokenName(token.TokenType);

                UpdateTokenValue(authResult.Properties, tokenName, token.Value);

                // update expires_at token for access tokens
                if (token.TokenType == TokenType.AccessToken && token.ExpireTime.HasValue)
                {
                    var expireValue = token.ExpireTime.Value.ToString("o", CultureInfo.InvariantCulture);

                    UpdateTokenValue(authResult.Properties, ExpiresAtKey, expireValue);
                }
            }

            await httpContext.SignInAsync(authResult.Principal, authResult.Properties);
        }

        private void UpdateTokenValue(AuthenticationProperties properties, string tokenName, string tokenValue)
        {
            if (properties.UpdateTokenValue(tokenName, tokenValue))
            {
                _logger.LogDebug("Updated token {TokenName}", tokenName);
                return;
            }

            _logger.LogWarning("Failed to update token {TokenName}", tokenName);
        }

        private HttpContext GetHttpContext()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
                throw new Exception("Failed to get HTTP context");

            return context;
        }

        private Token CreateToken(TokenType tokenType, string value, DateTimeOffset? expireTime)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _logger.LogDebug("Failed to fetch token {TokenType} from HttpContext", tokenType);
                return null;
            }

            _logger.LogDebug("Fetched {TokenType} from HttpContext with expiry {ExpireTime}", tokenType, expireTime);

            return new Token(tokenType, value)
            {
                ExpireTime = expireTime
            };
        }

        private static async Task<DateTimeOffset?> GetTokenExpireTimeAsync(HttpContext httpContext)
        {
            var token = await httpContext.GetTokenAsync(ExpiresAtKey).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(token))
                return null;

            if (!DateTimeOffset.TryParse(token, out var expireTime))
                return null;

            return expireTime;
        }

        private static string GetHttpTokenName(TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.AccessToken:
                    return "access_token";
                case TokenType.RefreshToken:
                    return "refresh_token";
                default:
                    throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null);
            }
        }
    }
}
