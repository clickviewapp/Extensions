namespace ClickView.Extensions.RestClient.Authenticators.OAuth.TokenStore
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tokens;

    public interface ITokenStore
    {
        Task<Token> GetTokenAsync(TokenType tokenType);
        Task StoreTokens(IEnumerable<Token> tokens);
    }
}
