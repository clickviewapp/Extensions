namespace ClickView.Extensions.Events.Tests
{
    using System.Threading.Tasks;
    using Bus;
    using DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using Publisher;
    using Xunit;

    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddEvents_UseBus_Resolves()
        {
            var services = new ServiceCollection();
            services.AddEvents().UseBus<TestBus>();

            var provider = services.BuildServiceProvider();

            var eventPublisher = provider.GetService<IEventPublisher>();

            Assert.NotNull(eventPublisher);
        }

        public class TestBus : IEventBus
        {
            public Task PublishAsync(Event evt)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
