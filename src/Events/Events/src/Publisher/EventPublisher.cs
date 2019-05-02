namespace ClickView.Extensions.Events.Publisher
{
    using System;
    using System.Threading.Tasks;
    using Bus;

    internal class EventPublisher : IEventPublisher
    {
        private readonly IEventBus _eventBus;

        public EventPublisher(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public Task PublishAsync(Event evt)
        {
            if(evt == null)
                throw new ArgumentNullException(nameof(evt));

            return _eventBus.PublishAsync(evt);
        }
    }
}
