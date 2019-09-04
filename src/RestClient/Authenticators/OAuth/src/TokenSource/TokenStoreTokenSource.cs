namespace ClickView.Extensions.RestClient.Authenticators.OAuth.TokenSource
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Tokens;
    using TokenStore;

    public class TokenStoreTokenSource : ITokenSource
    {
        private readonly ITokenStore _tokenStore;

        public TokenStoreTokenSource(ITokenStore tokenStore)
        {
            _tokenStore = tokenStore;
        }

        public async Task<IReadOnlyCollection<Token>> GetTokensAsync(CancellationToken cancellationToken = default)
        {
            var token = await _tokenStore.GetTokenAsync(TokenType.AccessToken).ConfigureAwait(false);

            if (token == null)
                return new List<Token>();

            return new[]
            {
                token
            };
        }

        public bool StoreTokens => false;
    }
}
