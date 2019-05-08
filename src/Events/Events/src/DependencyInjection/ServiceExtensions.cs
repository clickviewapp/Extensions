namespace ClickView.Extensions.Events.DependencyInjection
{
    using System;
    using Bus;
    using Handler;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Publisher;

    public static class ServiceCollectionExtensions
    {
        public static IEventsBuilder AddEvents(this IServiceCollection services)
        {
            var factory = new DefaultEventHandlerFactory();

            //Core
            services.AddSingleton<IHandlerRegistrar>(factory);
            services.AddSingleton<IHandlerRunner>(factory);

            services.AddTransient<IEventPublisher, EventPublisher>();
            services.AddTransient<IEventBus, InMemoryEventBus>();

            return new EventsBuilder(services);
        }

        public static void UseBus(this IEventsBuilder builder, Func<IServiceProvider, IEventBus> implementationFactory)
        {
            builder.Services.Replace(ServiceDescriptor.Singleton(implementationFactory));
        }

        public static void UseBus<TBus>(this IEventsBuilder builder) where TBus : IEventBus
        {
            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IEventBus), typeof(TBus)));
        }
    }
}