namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Authenticators
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Tokens;
    using TokenSource;

    internal class ClientCredentialsTokenSource : AccessTokenSource
    {
        private readonly TokenClient _tokenClient;
        private readonly ILogger<ClientCredentialsTokenSource> _logger;
        private readonly string _scope;

        public ClientCredentialsTokenSource(TokenClient tokenClient, ILoggerFactory loggerFactory,
            IEnumerable<string> scopes)
        {
            _tokenClient = tokenClient;
            _scope = string.Join(" ", scopes);
            _logger = loggerFactory.CreateLogger<ClientCredentialsTokenSource>();
        }

        protected override async Task<AccessToken> GetAccessTokenAsync()
        {
            var response = await _tokenClient.GetClientCredentialsTokenAsync(_scope).ConfigureAwait(false);

            //todo: handle errors
            if (!response.IsError)
                return Helpers.CreateAccessToken(response);

            _logger.LogError(response.Exception,
                "Error fetching client credentials token. {Error} ({ErrorDescription})", response.Error,
                response.ErrorDescription);

            return null;
        }
    }
}
