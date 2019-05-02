namespace ClickView.Extensions.Events.Handler
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class DefaultEventHandlerFactory : IHandlerRunner, IHandlerRegistrar
    {
        private readonly ConcurrentDictionary<Type, List<Func<Event, Task>>> _handlers =
            new ConcurrentDictionary<Type, List<Func<Event, Task>>>();

        public void RegisterHandler<T>(IEventHandler<T> handler) where T : Event
        {
            RegisterHandler<T>(handler.HandleEventAsync);
        }

        public void RegisterHandler<T>(Func<T, Task> handlerFunc) where T : Event
        {
            var eventType = typeof(T);

            Task Func(Event e) => handlerFunc((T)e);

            _handlers.AddOrUpdate(eventType,
                new List<Func<Event, Task>> { Func },
                (type, list) =>
                {
                    list.Add(Func);
                    return list;
                });
        }

        public Task HandleAsync(Event evt)
        {
            var eventType = evt.GetType();

            if (!_handlers.TryGetValue(eventType, out var handlers))
            {
                //TODO: Maybe throw an exception saying no handlers?
                return Task.FromResult(0);
            }

            return Task.WhenAll(handlers.Select(h => h(evt)));
        }
    }
}