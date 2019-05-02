namespace ClickView.Extensions.RestClient.Authenticators.OAuth.TokenSource
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tokens;

    public interface ITokenSource
    {
        Task<IReadOnlyCollection<Token>> GetTokensAsync();

        /// <summary>
        /// Return true if the tokens from this source should be stored
        /// </summary>
        bool StoreTokens { get; }
    }
}
