namespace ClickView.Extensions.Events.Handler
{
    using System.Threading.Tasks;

    internal interface IHandlerRunner
    {
        Task HandleAsync(Event evt);
    }
}