namespace ClickView.Extensions.RestClient.Authenticators.OAuth.TokenSource
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Tokens;

    public interface ITokenSource
    {
        /// <summary>
        ///     Return true if the tokens from this source should be stored
        /// </summary>
        bool StoreTokens { get; }

        Task<IReadOnlyCollection<Token>> GetTokensAsync(CancellationToken cancellationToken = default);
    }
}
