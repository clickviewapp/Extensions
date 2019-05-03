namespace ClickView.Extensions.Events.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Bus;
    using Handler;
    using Publisher;
    using Xunit;

    public class EventsIntegrationTests
    {
        [Fact]
        public async Task CanPublishUsingInMemoryBus()
        {
            //arrange
            var eventHandlerFactory = new DefaultEventHandlerFactory();
            var eventBus = new InMemoryEventBus(eventHandlerFactory);
            var eventPublisher = new EventPublisher(eventBus);
            var eventHandler = new CustomEventHandler();

            eventHandlerFactory.RegisterHandler(eventHandler);

            var evt = new CustomEvent("Test message");

            //act
            await eventPublisher.PublishAsync(evt).ConfigureAwait(false);

            //Wait for events to propagate/tasks to finish
            await Task.Delay(100).ConfigureAwait(false);

            //assert
            Assert.Single(eventHandler.Events);
            Assert.Equal(evt.Id, eventHandler.Events[0].Id);
            Assert.Equal(evt.EventId, eventHandler.Events[0].EventId);
            Assert.Equal(evt.Message, eventHandler.Events[0].Message);
        }

        private class CustomEvent : Event
        {
            public string Message { get; }

            public CustomEvent(string message) : base("customevent")
            {
                Message = message;
            }
        }

        private class CustomEventHandler : IEventHandler<CustomEvent>
        {
            public List<CustomEvent> Events { get; } = new List<CustomEvent>();

            public Task HandleEventAsync(CustomEvent evt)
            {
                Events.Add(evt);
                return Task.FromResult(0);
            }
        }
    }
}
