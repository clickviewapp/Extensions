namespace ClickView.Extensions.RestClient.Authenticators.OAuth.TokenStore
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tokens;

    public class InMemoryTokenStore : ITokenStore
    {
        private readonly ConcurrentDictionary<TokenType, Token> _tokens = new ConcurrentDictionary<TokenType, Token>();

        public Task<Token> GetTokenAsync(TokenType tokenType)
        {
            _tokens.TryGetValue(tokenType, out var token);
            return Task.FromResult(token);
        }

        public Task StoreTokensAsync(IEnumerable<Token> tokens)
        {
            foreach (var t in tokens)
            {
                _tokens.AddOrUpdate(t.TokenType, t, (type, _) => t);
            }

            return Task.CompletedTask;
        }
    }
}
