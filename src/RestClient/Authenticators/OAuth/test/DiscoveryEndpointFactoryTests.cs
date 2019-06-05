namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Tests
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Exceptions;
    using Internal.Endpoints;
    using Xunit;

    public class DiscoveryEndpointFactoryTests
    {
        [Fact]
        public async Task GetTokenClientAsync()
        {
            var factory = new DiscoveryEndpointFactory("https://samples.auth0.com", new HttpClient());

            var endpoints = await factory.GetAsync();

            Assert.NotNull(endpoints);
            Assert.NotNull(endpoints.TokenEndpoint);
        }

        [Fact]
        public async Task GetTokenClientAsync_SameInstance()
        {
            var factory = new DiscoveryEndpointFactory("https://samples.auth0.com", new HttpClient());

            var endpoints = await factory.GetAsync();
            var endpoints2 = await factory.GetAsync();

            Assert.Equal(endpoints, endpoints2);
        }

        [Fact]
        public async Task GetTokenClientAsync_Expired_Refreshes()
        {
            var factory = new DiscoveryEndpointFactory("https://samples.auth0.com", new HttpClient())
                {CacheDuration = TimeSpan.FromSeconds(1)};

            var endpoints = await factory.GetAsync();

            await Task.Delay(1500);

            var endpoints2 = await factory.GetAsync();

            Assert.NotEqual(endpoints, endpoints2);
        }

        [Fact]
        public async Task GetTokenClientAsync_InaccessibleAuthority_ThrowsClientDiscoveryException()
        {
            var factory = new DiscoveryEndpointFactory("https://oauth.nohost.local", new HttpClient());

            await Assert.ThrowsAsync<ClientDiscoveryException>(() => factory.GetAsync());
        }

        [Fact]
        public async Task GetTokenClientAsync_ThreadSafe()
        {
            var factory = new DiscoveryEndpointFactory("https://samples.auth0.com", new HttpClient());

            var endpoints1Task = factory.GetAsync();
            var endpoints2Task = factory.GetAsync();

            await endpoints1Task;
            await endpoints2Task;
        }

        [Fact]
        public async Task GetTokenClientAsync_ThreadSafe_SameInstance()
        {
            var factory = new DiscoveryEndpointFactory("https://samples.auth0.com", new HttpClient());

            var endpoints1Task = factory.GetAsync();
            var endpoints2Task = factory.GetAsync();

            Assert.Equal(await endpoints1Task, await endpoints2Task);
        }
    }
}
