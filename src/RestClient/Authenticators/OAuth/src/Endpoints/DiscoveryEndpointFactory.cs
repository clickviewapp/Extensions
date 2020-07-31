namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Endpoints
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Internal;
    using Exceptions;
    using IdentityModel.Client;

    public class DiscoveryEndpointFactory : IAuthenticatorEndpointFactory
    {
        private readonly string _authority;
        private readonly HttpClient _httpClient;
        private readonly DiscoveryPolicy _discoveryPolicy;

        private AsyncLazy<AuthenticatorEndpoints> _discoveryCache;

        private DateTime _nextReload = DateTime.MinValue;
        private readonly object _reloadLock = new object();

        public DiscoveryEndpointFactory(string authority, HttpClient httpClient)
        {
            _authority = authority;
            _httpClient = httpClient;
            _discoveryPolicy = new DiscoveryPolicy
            {
                RequireHttps = authority.StartsWith("https://")
            };
        }

        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromHours(8);

        public Task<AuthenticatorEndpoints> GetAsync()
        {
            if (_nextReload <= DateTime.UtcNow)
                Refresh();

            return _discoveryCache.Value;
        }

        public void Refresh()
        {
            lock (_reloadLock)
            {
                if (_nextReload > DateTime.UtcNow)
                    return;

                _nextReload = DateTime.UtcNow.Add(CacheDuration);
                _discoveryCache = new AsyncLazy<AuthenticatorEndpoints>(GetEndpointsAsync);
            }
        }

        private async Task<AuthenticatorEndpoints> GetEndpointsAsync()
        {
            var result = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _authority,
                Policy = _discoveryPolicy
            }).ConfigureAwait(false);

            if (!result.IsError)
            {
                return new AuthenticatorEndpoints
                {
                    TokenEndpoint = result.TokenEndpoint
                };
            }

            lock (_reloadLock)
            {
                // Force reload next request
                _nextReload = DateTime.MinValue;
            }

            throw new ClientDiscoveryException("Failed to retrieve endpoints from discovery document",
                result.Exception);
        }
    }
}
