namespace ClickView.Extensions.Events.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection;

    public interface IEventsBuilder
    {
        IServiceCollection Services { get; }
    }
}