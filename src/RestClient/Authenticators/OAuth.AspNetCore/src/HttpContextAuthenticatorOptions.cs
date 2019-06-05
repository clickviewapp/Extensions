namespace ClickView.Extensions.RestClient.Authenticators.OAuth.AspNetCore
{
    public class HttpContextAuthenticatorOptions : OAuthAuthenticatorOptions
    {
        public string Authority { get; }

        public HttpContextAuthenticatorOptions(string authority)
        {
            Authority = authority;
        }
    }
}