namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Exceptions
{
    using System;
    using ClickView.Extensions.RestClient.Exceptions;

    public class OAuthAuthenticatorException : ClickViewClientException
    {
        public OAuthAuthenticatorException(string message) : base(message)
        {
        }

        public OAuthAuthenticatorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
