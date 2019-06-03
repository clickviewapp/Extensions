namespace ClickView.Extensions.RestClient.Authenticators.OAuth.AspNetCore
{
    public class HttpContextAuthenticator : OAuthAuthenticator<HttpContextAuthenticatorOptions>
    {
        public HttpContextAuthenticator(HttpContextAuthenticatorOptions options) : base(options)
        {
        }
    }
}