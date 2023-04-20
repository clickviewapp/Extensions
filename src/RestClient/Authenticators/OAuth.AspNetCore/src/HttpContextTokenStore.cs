namespace ClickView.Extensions.RestClient.Authenticators.OAuth.AspNetCore
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
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

        public async Task<Token?> GetTokenAsync(TokenType tokenType)
        {
            var httpContext = GetHttpContext();

            var authResult = await httpContext.AuthenticateAsync();
            var properties = authResult.Properties;

            if (properties is null)
                return null;

            var tokenValue = properties.GetTokenValue(GetHttpTokenName(tokenType));

            DateTimeOffset? expireTime = null;

            // Only access tokens have expire time
            if (tokenType == TokenType.AccessToken)
                expireTime = GetTokenExpireTime(properties);

            return CreateToken(tokenType, tokenValue, expireTime);
        }

        //Token reference https://github.com/aspnet/AspNetCore/blob/master/src/Security/Authentication/OAuth/src/OAuthHandler.cs#L116
        public async Task StoreTokensAsync(IEnumerable<Token> tokens)
        {
            var httpContext = GetHttpContext();
            var authResult = await httpContext.AuthenticateAsync();

            var principal = authResult.Principal;
            if (principal is null)
            {
                _logger.LogWarning("Failed to store tokens. No principal set");
                return;
            }

            var properties = authResult.Properties;

            // todo: Should this null check or should this create a new properties?
            if (properties is null)
            {
                _logger.LogWarning("Failed to store tokens. No properties");
                return;
            }

            foreach (var token in tokens)
            {
                var tokenName = GetHttpTokenName(token.TokenType);

                UpdateTokenValue(properties, tokenName, token.Value);

                // update expires_at token for access tokens
                if (token is { TokenType: TokenType.AccessToken, ExpireTime: not null })
                {
                    var expireValue = token.ExpireTime.Value.ToString("o", CultureInfo.InvariantCulture);

                    UpdateTokenValue(properties, ExpiresAtKey, expireValue);
                }
            }

            var options = httpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            var schemeProvider = httpContext.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
            var scheme = (await schemeProvider.GetDefaultSignInSchemeAsync())?.Name;
            var cookieOptions = options.Get(scheme);

            if (properties.AllowRefresh == true || properties.AllowRefresh == null && cookieOptions.SlidingExpiration)
            {
                // this will allow the cookie to be issued with a new issuedUtc (and thus a new expiration)
                properties.IssuedUtc = null;
                properties.ExpiresUtc = null;
            }

            await httpContext.SignInAsync(principal, properties);
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

        private Token? CreateToken(TokenType tokenType, string? value, DateTimeOffset? expireTime)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _logger.LogDebug("Failed to fetch token {TokenType} from HttpContext. Missing value", tokenType);
                return null;
            }

            _logger.LogDebug("Fetched {TokenType} from HttpContext with expiry {ExpireTime}", tokenType, expireTime);

            return new Token(tokenType, value)
            {
                ExpireTime = expireTime
            };
        }

        private static DateTimeOffset? GetTokenExpireTime(AuthenticationProperties properties)
        {
            var token = properties.GetTokenValue(ExpiresAtKey);

            if (string.IsNullOrWhiteSpace(token))
                return null;

            if (!DateTimeOffset.TryParse(token, out var expireTime))
                return null;

            return expireTime;
        }

        private static string GetHttpTokenName(TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.AccessToken => "access_token",
                TokenType.RefreshToken => "refresh_token",
                _ => throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null)
            };
        }
    }
}
