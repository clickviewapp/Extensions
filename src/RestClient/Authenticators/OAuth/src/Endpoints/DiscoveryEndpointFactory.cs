namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Endpoints;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Exceptions;
using IdentityModel.Client;

public class DiscoveryEndpointFactory : IAuthenticatorEndpointFactory
{
    private readonly string _authority;
    private readonly HttpClient _httpClient;
    private readonly DiscoveryPolicy _discoveryPolicy;

    private volatile Task<AuthenticatorEndpoints> _discoveryCache;
    private DateTime _nextReload = DateTime.MinValue;
    private readonly object _reloadLock = new object();

    public DiscoveryEndpointFactory(string authority, HttpClient httpClient)
    {
        _authority = authority ?? throw new ArgumentNullException(nameof(authority));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _discoveryPolicy = new DiscoveryPolicy
        {
            RequireHttps = authority.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
        };
    }

    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromHours(8);

    public Task<AuthenticatorEndpoints> GetAsync()
    {
        var now = DateTime.UtcNow;

        if (_nextReload <= now)
            Refresh();

        return _discoveryCache;
    }

    public void Refresh()
    {
        lock (_reloadLock)
        {
            var now = DateTime.UtcNow;

            if (_nextReload > now)
                return;

            _nextReload = now.Add(CacheDuration);
            _discoveryCache = LoadEndpointsAsync();
        }
    }

    private async Task<AuthenticatorEndpoints> LoadEndpointsAsync()
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
                TokenEndpoint = result.TokenEndpoint,
                RevocationEndpoint = result.RevocationEndpoint
            };
        }

        // Force next request to retry immediately
        lock (_reloadLock)
        {
            _nextReload = DateTime.MinValue;
        }

        throw new ClientDiscoveryException(
            "Failed to retrieve endpoints from discovery document",
            result.Exception);
    }
}
