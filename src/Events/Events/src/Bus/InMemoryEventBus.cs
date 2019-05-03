namespace ClickView.Extensions.Events.Bus
{
    using System.Threading.Tasks;
    using Handler;

    internal class InMemoryEventBus : IEventBus
    {
        private readonly IHandlerRunner _handlerRunner;

        public InMemoryEventBus(IHandlerRunner handlerRunner)
        {
            _handlerRunner = handlerRunner;
        }

        public Task PublishAsync(Event evt)
        {
            //Run in a new task because we dont want it to be blocking
            //We do this in the bus because if we swap this out to be something like redis
            //we might not need to do this
            return Task.Run(() => _handlerRunner.HandleAsync(evt));
        }
    }
}