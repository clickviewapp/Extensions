namespace ClickView.Extensions.RestClient.Authenticators.OAuth.TokenSource
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tokens;

    public abstract class AccessTokenSource : ITokenSource
    {
        public async Task<IReadOnlyCollection<Token>> GetTokensAsync()
        {
            var token = await GetAccessTokenAsync().ConfigureAwait(false);

            if (token == null)
                return new List<Token>();

            return new[] {token};
        }

        public virtual bool StoreTokens => true;

        protected abstract Task<AccessToken> GetAccessTokenAsync();
    }
}