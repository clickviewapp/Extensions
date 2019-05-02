namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Authenticators
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using Internal.ClientFactory;
    using Microsoft.Extensions.Logging;
    using Tokens;
    using TokenSource;

    internal class ClientCredentialsTokenSource : AccessTokenSource
    {
        private readonly IClientFactory _clientFactory;
        private readonly string _scope;
        private readonly ILogger<ClientCredentialsTokenSource> _logger;

        public ClientCredentialsTokenSource(IClientFactory clientFactory, ILoggerFactory loggerFactory, IEnumerable<string> scopes)
        {
            _clientFactory = clientFactory;
            _scope = string.Join(" ", scopes);
            _logger = loggerFactory.CreateLogger<ClientCredentialsTokenSource>();
        }

        protected override async Task<AccessToken> GetAccessTokenAsync()
        {
            var tokenClient = await _clientFactory.GetTokenClientAsync().ConfigureAwait(false);

            var response = await tokenClient.RequestClientCredentialsAsync(_scope).ConfigureAwait(false);

            //todo: handle errors
            if (!response.IsError)
                return Helpers.CreateAccessToken(response);

            _logger.LogError(response.Exception, "Error fetching client credentials token. {Error} ({ErrorDescription})", response.Error, response.ErrorDescription);

            return null;
        }
    }
}