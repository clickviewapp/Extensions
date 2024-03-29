﻿namespace ClickView.Extensions.RestClient.Authenticators.OAuth.TokenSource
{
    using System;
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
                return Array.Empty<Token>();

            return new[]
            {
                token
            };
        }

        public Task RevokeTokenAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public bool StoreTokens => false;
    }
}
