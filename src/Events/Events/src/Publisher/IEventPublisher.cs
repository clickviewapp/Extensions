namespace ClickView.Extensions.Events.Publisher
{
    using System.Threading.Tasks;

    public interface IEventPublisher
    {
        Task PublishAsync(Event evt);
    }
}
