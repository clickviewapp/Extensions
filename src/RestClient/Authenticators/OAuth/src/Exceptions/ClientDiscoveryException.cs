namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Exceptions
{
    using System;

    public class ClientDiscoveryException : OAuthAuthenticatorException
    {
        public ClientDiscoveryException(string message) : base(message)
        {
        }

        public ClientDiscoveryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
