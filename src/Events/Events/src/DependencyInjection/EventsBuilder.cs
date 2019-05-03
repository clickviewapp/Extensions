namespace ClickView.Extensions.Events.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection;

    public class EventsBuilder : IEventsBuilder
    {
        public IServiceCollection Services { get; }

        public EventsBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}