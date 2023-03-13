namespace ClickView.Extensions.Events.Tests
{
    using System.Threading.Tasks;
    using Xunit;

    public class SimpleEventServiceTests
    {
        [Fact]
        public async Task PublishAndHandle()
        {
            var eventService = new SimpleEventService();

            var called = false;

            eventService.RegisterHandler<ExampleEvent>(_ =>
            {
                called = true;
                return Task.CompletedTask;
            });

            await eventService.PublishAsync(new ExampleEvent());

            Assert.True(called);
        }

        public class ExampleEvent : Event
        {
            public ExampleEvent() : base("hello")
            {
            }
        }
    }
}
