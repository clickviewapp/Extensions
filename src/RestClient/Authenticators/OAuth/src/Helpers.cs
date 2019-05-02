namespace ClickView.Extensions.RestClient.Authenticators.OAuth
{
    using System;
    using IdentityModel.Client;
    using Tokens;

    internal static class Helpers
    {
        public static DateTimeOffset GetExpireTime(int expiresInSeconds)
        {
            return DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds - 10); //allow 10sec of clock drift
        }

        public static AccessToken CreateAccessToken(TokenResponse tokenResponse)
        {
            return new AccessToken(tokenResponse.AccessToken, GetExpireTime(tokenResponse.ExpiresIn));
        }
    }
}