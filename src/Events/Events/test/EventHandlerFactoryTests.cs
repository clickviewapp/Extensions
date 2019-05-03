namespace ClickView.Extensions.Events.Tests
{
    using System.Threading.Tasks;
    using Handler;
    using Moq;
    using Xunit;

    public class EventHandlerFactoryTests
    {
        [Fact]
        public async Task HandleAsync_MultipleHandlers_RunsCorrectHandler()
        {
            //arrange
            var handlerFactory = new DefaultEventHandlerFactory();

            var handled1 = false;
            var handled2 = false;

            var mockHandler1 = new Mock<IEventHandler<CustomEvent1>>();
            mockHandler1
                .Setup(o => o.HandleEventAsync(It.IsAny<CustomEvent1>()))
                .Returns(() => Task.FromResult(0))
                .Callback(() => handled1 = true);

            var mockHandler2 = new Mock<IEventHandler<CustomEvent2>>();
            mockHandler2
                .Setup(o => o.HandleEventAsync(It.IsAny<CustomEvent2>()))
                .Returns(() => Task.FromResult(0))
                .Callback(() => handled2 = true);

            //register
            handlerFactory.RegisterHandler(mockHandler1.Object);
            handlerFactory.RegisterHandler(mockHandler2.Object);

            //act
            await handlerFactory.HandleAsync(new CustomEvent1()).ConfigureAwait(false);

            //assert
            Assert.True(handled1);
            Assert.False(handled2);
        }


        public class CustomEvent1 : Event
        {
            public CustomEvent1() : base("customevent.1")
            {
            }
        }

        public class CustomEvent2 : Event
        {
            public CustomEvent2() : base("customevent.1")
            {
            }
        }
    }
}
