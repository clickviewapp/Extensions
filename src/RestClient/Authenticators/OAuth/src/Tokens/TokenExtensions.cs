namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Tokens
{
    using System;

    public static class TokenExtensions
    {
        public static bool HasExpired(this Token token)
        {
            if (token is null)
                throw new ArgumentNullException(nameof(token));

            // No expire time means its not expired
            if (!token.ExpireTime.HasValue)
                return false;

            // Remove 10 seconds from the expire time to allow for clock drift
            return token.ExpireTime.Value.AddSeconds(-10) <= DateTimeOffset.UtcNow;
        }
    }
}
