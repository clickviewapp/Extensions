namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Internal.ClientFactory
{
    using System;
    using System.Threading.Tasks;
    using Exceptions;
    using IdentityModel.Client;

    internal class DiscoveryClientFactory : IClientFactory
    {
        private readonly OAuthAuthenticatorOptions _options;
        private readonly DiscoveryClient _discoveryClient;
        private AsyncLazy<Clients> _clientsCache;

        private DateTime _nextReload = DateTime.MinValue;

        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromHours(8);

        public DiscoveryClientFactory(string authority, OAuthAuthenticatorOptions options)
        {
            _options = options;

            _discoveryClient = new DiscoveryClient(authority)
            {
                Policy =
                {
                    RequireHttps = authority.StartsWith("https://")
                }
            };
        }

        public async Task<TokenClient> GetTokenClientAsync()
        {
            var clients = await GetClientsAsync().ConfigureAwait(false);
            return clients.TokenClient;
        }

        public void Refresh()
        {
            _clientsCache?.Value.Dispose();
            _clientsCache = new AsyncLazy<Clients>(GetClientsInternalAsync);
        }

        private Task<Clients> GetClientsAsync()
        {
            if (_nextReload <= DateTime.UtcNow)
            {
                Refresh();
            }

            return _clientsCache.Value;
        }

        private async Task<Clients> GetClientsInternalAsync()
        {
            var endpoints = await _discoveryClient.GetAsync().ConfigureAwait(false);

            if(endpoints.IsError)
                throw new ClientDiscoveryException("Failed to retrieve endpoints from discovery document", endpoints.Exception);

            var clients = new Clients
            {
                TokenClient = new TokenClient(endpoints.TokenEndpoint, _options.ClientId, _options.ClientSecret)
            };

            _nextReload = DateTime.UtcNow.Add(CacheDuration);

            return clients;
        }

        private class Clients : IDisposable
        {
            internal TokenClient TokenClient { get; set; }

            public void Dispose()
            {
                TokenClient?.Dispose();
            }
        }
    }
}