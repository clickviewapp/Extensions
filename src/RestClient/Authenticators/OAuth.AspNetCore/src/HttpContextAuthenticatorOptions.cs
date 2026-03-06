namespace ClickView.Extensions.RestClient.Authenticators.OAuth.AspNetCore;

public class HttpContextAuthenticatorOptions(string authority) : OAuthAuthenticatorOptions
{
    public string Authority { get; } = authority;

    public bool EnableRefresh { get; set; }
}
