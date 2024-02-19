﻿namespace ClickView.Extensions.RestClient.Authenticators.OAuth;

using System;
using IdentityModel.Client;
using Tokens;

public static class TokenHelpers
{
    public static DateTimeOffset GetExpireTime(int expiresInSeconds)
    {
        return DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds);
    }

    public static AccessToken CreateAccessToken(TokenResponse tokenResponse)
    {
        return new AccessToken(tokenResponse.AccessToken, GetExpireTime(tokenResponse.ExpiresIn));
    }

    public static RefreshToken CreateRefreshToken(TokenResponse tokenResponse)
    {
        return new RefreshToken(tokenResponse.RefreshToken);
    }
}
