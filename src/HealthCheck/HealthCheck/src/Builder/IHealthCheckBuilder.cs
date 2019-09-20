namespace ClickView.Extensions.HealthCheck.Builder
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IHealthCheckBuilder
    {
        IHealthCheckBuilder AddCheck(IHealthCheck healthCheck);
        IHealthCheckBuilder AddCheck(string name, Func<CancellationToken, Task<HealthCheckResult>> checkFunc);
        IHealthCheckBuilder Configure(Action<HealthCheckerOptions> configureOptions);
        IHealthChecker Build();
    }
}