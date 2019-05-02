namespace ClickView.Extensions.Events
{
    using System;
    using System.Threading.Tasks;
    using Bus;
    using Handler;
    using Publisher;

    public class SimpleEventService : IEventPublisher, IHandlerRegistrar
    {
        private readonly IHandlerRegistrar _registrar;
        private readonly IEventPublisher _publisher;

        public SimpleEventService()
        {
            var factory = new DefaultEventHandlerFactory();
            var eventBus = new InMemoryEventBus(factory);

            _registrar = factory;
            _publisher = new EventPublisher(eventBus);
        }

        public Task PublishAsync(Event evt)
        {
            return _publisher.PublishAsync(evt);
        }

        public void RegisterHandler<T>(IEventHandler<T> handler) where T : Event
        {
            _registrar.RegisterHandler(handler);
        }

        public void RegisterHandler<T>(Func<T, Task> handlerFunc) where T : Event
        {
            _registrar.RegisterHandler(handlerFunc);
        }
    }
}
