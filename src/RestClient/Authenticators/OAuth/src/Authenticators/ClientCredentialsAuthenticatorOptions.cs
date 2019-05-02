namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Authenticators
{
    using System.Collections.Generic;

    public class ClientCredentialsAuthenticatorOptions : OAuthAuthenticatorOptions
    {
        public IEnumerable<string> Scopes { get; set; }
    }
}