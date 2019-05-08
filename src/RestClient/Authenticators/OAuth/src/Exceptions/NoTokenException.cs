namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Exceptions
{
    public class NoTokenException : OAuthAuthenticatorException
    {
        public NoTokenException(string message) : base(message)
        {
        }
    }
}
