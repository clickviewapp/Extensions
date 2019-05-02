namespace ClickView.Extensions.Events.Handler
{
    using System.Threading.Tasks;

    public interface IEventHandler<in T> where T : Event
    {
        Task HandleEventAsync(T evt);
    }
}