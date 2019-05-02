# OAuth authenticator for ClickView.Extensions.RestClient #


This package contains some oauth authenticators to use with the `ClickView.Extensions.RestClient` package.


## Authenticators ##

### ClientCredentialsAuthenticator ###

Used to authenticate against an API with client credentials

#### Options ####
| Option |  Default | Description |
| ---- |  ---- | ---- |
| Scopes | [] | List of scopes to authenticate with |
| ClientId | null | The client id |
| ClientSecret | null | The client secret |
| EnableDiscovery | true | If `true`, use OpenID Connect discovery to find the correct auth endpoints |
| TokenStore | InMemoryTokenStore | The `TokenStore` used to store the cached tokens |
| Endpoints | null | _(Not implemented)_ |


#### Usage ####
```cs
var options = new RestClientOptions
{
    Authenticator = new ClientCredentialsAuthenticator("https://oauth.example.com",
        new ClientCredentialsAuthenticatorOptions
        {
            ClientId = "test_client",
            ClientSecret = "test_sec_id",
            Scopes = new[] {"read"}
        })
};
```


## Extending ##

You can create your own OAuth authenticators by extending `OAuthAuthenticator<TOptions>`

## Token Stores ##

You can create your own token stores by implementing `ITokenStore`