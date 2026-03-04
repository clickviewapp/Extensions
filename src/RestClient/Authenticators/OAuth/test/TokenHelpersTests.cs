using System.Net;
using System.Net.Http;
using System.Text.Json;
using ClickView.Extensions.RestClient.Authenticators.OAuth.Tokens;
using IdentityModel.Client;
using Xunit;

namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Tests;

public class TokenHelpersTests
{
    [Fact]
    public async Task CreateAccessToken_WithExpiresIn_SetsExpireTime()
    {
        var tokenResponse = await CreateTokenResponse(new { access_token = "access_token", expires_in = 3600 });
        var token = TokenHelpers.CreateAccessToken(tokenResponse);

        Assert.Equal("access_token", token.Value);
        Assert.Equal(TokenType.AccessToken, token.TokenType);
        Assert.NotNull(token.ExpireTime);
    }

    [Fact]
    public async Task CreateAccessToken_WithoutExpiresIn_SetsNullExpireTime()
    {
        var tokenResponse = await CreateTokenResponse(new { access_token = "access_token" });
        var token = TokenHelpers.CreateAccessToken(tokenResponse);

        Assert.Equal("access_token", token.Value);
        Assert.Equal(TokenType.AccessToken, token.TokenType);
        Assert.Null(token.ExpireTime);
    }

    private static Task<TokenResponse> CreateTokenResponse(object token)
    {
        return ProtocolResponse.FromHttpResponseAsync<TokenResponse>(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(token))
            });
    }
}
