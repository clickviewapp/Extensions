namespace ClickView.Extensions.HealthCheck
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IHealthChecker
    {
        Task<HealthCheckResults> CheckAllAsync(CancellationToken cancellationToken);
        Task<HealthCheckResultItem> RunCheck(IHealthCheck check, CancellationToken cancellationToken);
    }
}