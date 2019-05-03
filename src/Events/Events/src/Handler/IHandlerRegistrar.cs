namespace ClickView.Extensions.Events.Handler
{
    using System;
    using System.Threading.Tasks;

    public interface IHandlerRegistrar
    {
        void RegisterHandler<T>(IEventHandler<T> handler) where T : Event;
        void RegisterHandler<T>(Func<T, Task> handlerFunc) where T : Event;
    }
}