namespace ClickView.Extensions.RestClient.Authenticators.OAuth
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Authentication;
    using Exceptions;
    using Internal.Endpoints;
    using Microsoft.Extensions.Logging;
    using Requests;
    using Tokens;
    using TokenSource;
    using TokenStore;

    public abstract class OAuthAuthenticator<TOptions> : IAuthenticator where TOptions : OAuthAuthenticatorOptions
    {
        private readonly ILogger<OAuthAuthenticator<TOptions>> _logger;

        private readonly List<ITokenSource> _tokenSources = new List<ITokenSource>();

        protected readonly TOptions Options;
        protected readonly ITokenStore TokenStore;
        protected readonly HttpClient HttpClient;

        protected OAuthAuthenticator(TOptions options)
        {
            HttpClient = options.HttpClient ?? new HttpClient();

            Options = options;

            if (options.LoggerFactory == null)
                throw new ArgumentException("No LoggerFactory configured");

            _logger = options.LoggerFactory.CreateLogger<OAuthAuthenticator<TOptions>>();

            TokenStore = options.TokenStore;

            // our token store should be the first place we look, so add that to our sources first
            if (TokenStore != null)
                AddTokenSource(new TokenStoreTokenSource(TokenStore));
        }

        public virtual async Task AuthenticateAsync(IClientRequest request, CancellationToken token = default)
        {
            var oAuthToken = await GetTokenAsync(token).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(oAuthToken))
            {
                if (Options.ThrowIfNoToken)
                    throw new NoTokenException("No token could be found in any stores");

                return;
            }

            request.AddHeader("Authorization", "Bearer " + oAuthToken);
        }

        protected void AddTokenSource(ITokenSource tokenSource)
        {
            _tokenSources.Add(tokenSource);
        }

        protected async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            foreach (var ts in _tokenSources)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // todo: find a better way to get the name
                var storeName = ts.GetType().Name;

                var tokens = await ts.GetTokensAsync(cancellationToken);
                var accessToken = tokens.FirstOrDefault(t => t.TokenType == TokenType.AccessToken);

                if (accessToken == null)
                {
                    _logger.LogInformation("No token found in store: {StoreType}", storeName);

                    continue;
                }

                // check expired
                if (accessToken.ExpireTime.HasValue && accessToken.ExpireTime < DateTimeOffset.UtcNow)
                {
                    _logger.LogInformation("Token expired ({ExpireTime}) in store: {StoreType}",
                        accessToken.ExpireTime.Value.ToString("O"), storeName);

                    continue;
                }

                // should we store these tokens?
                if (ts.StoreTokens)
                {
                    if (TokenStore == null)
                        throw new OAuthAuthenticatorException("No TokenStore configured");

                    await TokenStore.StoreTokens(tokens).ConfigureAwait(false);
                }

                return accessToken.Value;
            }

            _logger.LogWarning("No token could be found in any store");

            return null;
        }

        protected IAuthenticatorEndpointFactory CreateEndpointFactory(string endpoint)
        {
            if (Options.EnableDiscovery)
                return new DiscoveryEndpointFactory(endpoint, HttpClient);

            return new DefaultEndpointFactory(Options.Endpoints);
        }
    }
}
