namespace ClickView.Extensions.Events.Bus
{
    using System.Threading.Tasks;

    public interface IEventBus
    {
        Task PublishAsync(Event evt);
    }
}