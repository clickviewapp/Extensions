namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Tests.TokenSource
{
    using System.Threading.Tasks;
    using Moq;
    using OAuth.TokenSource;
    using Tokens;
    using TokenStore;
    using Xunit;

    public class TokenStoreTokenSourceTests
    {
        [Fact]
        public async Task GetTokensAsync_TokenStoreReturnsNull_DoesNotReturnListWithNull()
        {
            var tokenStore = new Mock<ITokenStore>();
            tokenStore.Setup(p => p.GetTokenAsync(TokenType.AccessToken)).ReturnsAsync(() => null);

            var source = new TokenStoreTokenSource(tokenStore.Object);

            var tokens = await source.GetTokensAsync().ConfigureAwait(false);

            Assert.Empty(tokens);
        }
    }
}
