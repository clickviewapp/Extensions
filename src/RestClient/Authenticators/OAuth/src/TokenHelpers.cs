namespace ClickView.Extensions.RestClient.Authenticators.OAuth;

using System;
using IdentityModel.Client;
using Tokens;

public static class TokenHelpers
{
    public static AccessToken CreateAccessToken(TokenResponse tokenResponse)
    {
        return new AccessToken(tokenResponse.AccessToken, GetExpireTime(tokenResponse.ExpiresIn));
    }

    public static RefreshToken CreateRefreshToken(TokenResponse tokenResponse)
    {
        return new RefreshToken(tokenResponse.RefreshToken);
    }

    private static DateTimeOffset? GetExpireTime(int expiresInSeconds)
    {
        // There wasn't an expiry in the token response
        if (expiresInSeconds == 0)
            return null;

        return DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds);
    }
}
