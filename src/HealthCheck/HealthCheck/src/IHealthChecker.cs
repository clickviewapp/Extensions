namespace ClickView.Extensions.HealthCheck
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IHealthChecker
    {
        Task<HealthCheckResults> CheckAllAsync(CancellationToken cancellationToken = default);
        Task<HealthCheckResultItem> RunCheckAsync(IHealthCheck check, CancellationToken cancellationToken = default);
    }
}