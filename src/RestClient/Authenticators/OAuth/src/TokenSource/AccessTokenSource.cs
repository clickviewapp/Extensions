﻿namespace ClickView.Extensions.RestClient.Authenticators.OAuth.TokenSource
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Tokens;

    public abstract class AccessTokenSource : ITokenSource
    {
        public async Task<IReadOnlyCollection<Token>> GetTokensAsync(CancellationToken cancellationToken = default)
        {
            var token = await GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);

            if (token == null)
                return Array.Empty<Token>();

            return new[] { token };
        }

        public virtual bool StoreTokens => true;

        protected abstract Task<AccessToken> GetAccessTokenAsync(CancellationToken cancellationToken = default);

        public virtual Task RevokeTokenAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
