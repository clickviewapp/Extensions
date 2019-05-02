namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Tests
{
    using System;
    using System.Threading.Tasks;
    using Exceptions;
    using Internal.ClientFactory;
    using OAuth;
    using Xunit;

    public class DiscoveryClientFactoryTests
    {
        [Fact]
        public async Task GetTokenClientAsync()
        {
            var factory = new DiscoveryClientFactory("https://samples.auth0.com",
                new OAuthAuthenticatorOptions
                {
                    EnableDiscovery = true,
                    ClientId = "test",
                    ClientSecret = "test-secret"
                });

            var client = await factory.GetTokenClientAsync();

            Assert.NotNull(client);
        }

        [Fact]
        public async Task GetTokenClientAsync_SameInstance()
        {
            var factory = new DiscoveryClientFactory("https://samples.auth0.com",
                    new OAuthAuthenticatorOptions
                    {
                        EnableDiscovery = true,
                        ClientId = "test",
                        ClientSecret = "test-secret"
                    })
                { CacheDuration = TimeSpan.FromSeconds(1) };


            var client = await factory.GetTokenClientAsync();
            var client2 = await factory.GetTokenClientAsync();

            Assert.Equal(client, client2);
        }

        [Fact]
        public async Task GetTokenClientAsync_Expired_Refreshes()
        {
            var factory = new DiscoveryClientFactory("https://samples.auth0.com",
                new OAuthAuthenticatorOptions
                {
                    EnableDiscovery = true,
                    ClientId = "test",
                    ClientSecret = "test-secret"
                }) {CacheDuration = TimeSpan.FromSeconds(1)};

            var client = await factory.GetTokenClientAsync();

            await Task.Delay(1500);

            var client2 = await factory.GetTokenClientAsync();

            Assert.NotEqual(client, client2);
        }

        [Fact]
        public async Task GetTokenClientAsync_InaccessibleAuthority_ThrowsClientDiscoveryException()
        {
            var factory = new DiscoveryClientFactory("https://oauth.nohost.local",
                new OAuthAuthenticatorOptions
                {
                    EnableDiscovery = true,
                    ClientId = "test",
                    ClientSecret = "test-secret"
                });

            await Assert.ThrowsAsync<ClientDiscoveryException>(() => factory.GetTokenClientAsync());
        }
    }
}
