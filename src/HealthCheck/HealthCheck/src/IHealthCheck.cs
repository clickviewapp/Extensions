namespace ClickView.Extensions.HealthCheck
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IHealthCheck
    {
        string Name { get; }

        Task<HealthCheckResult> CheckAsync(CancellationToken cancellationToken = default);
    }
}