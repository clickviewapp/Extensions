namespace ClickView.Extensions.RestClient.Authenticators.OAuth.AspNetCore
{
    public class HttpContextAuthenticatorOptions : OAuthAuthenticatorOptions
    {
        public string Authority { get; }

        public bool EnableRefresh { get; set; } = false;

        public HttpContextAuthenticatorOptions(string authority)
        {
            Authority = authority;
        }
    }
}