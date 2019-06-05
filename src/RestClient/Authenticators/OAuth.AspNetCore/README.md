# OAuth authenticator for ClickView.Extensions.RestClient

An authenticator to use with with the `ClickView.Extensions.RestClient` package which uses access tokens stored by AspNetCore's `AddOpenIdConnect` and `AddCookie`.

## HttpContextAuthenticator

Used to authenticate against an API with client credentials

### Options

| Option |  Default | Description |
| ---- |  ---- | ---- |
| Authority | null | The url of the authentication authority (ie url) |
| ClientId | null | The client id |
| ClientSecret | null | The client secret |
| EnableDiscovery | true | If `true`, use OpenID Connect discovery to find the correct auth endpoints |
| TokenStore | HttpContextTokenStore | The `TokenStore` used to store the cached tokens. This should not be changed. |

### Usage

```cs
services.AddHttpContextAuthenticator("https://auth.example.com");
```

Both `HttpContextAuthenticatorOptions` and `HttpContextAuthenticator` can/should be registered as singleton so you don't have to new up new clients on each request.

### Full Example

```cs
var clientId = "client";
var clientSecret = "sec";
var authority = "https://auth.example.com";

services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "oidc";
    })
    .AddCookie()
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = authority;
        options.ClientId = clientId;
        options.ClientSecret = clientSecret;

        //Required to use HttpContextTokenStore
        options.SaveTokens = true;

        options.Scope.Clear();
        options.Scope.Add("read");

        options.ResponseType = "code id_token";
    });

services.AddHttpContextAuthenticator(authority);
```
