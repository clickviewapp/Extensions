namespace ClickView.Extensions.HealthCheck.Checks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class CustomCheck : IHealthCheck
    {
        private readonly Func<CancellationToken, Task<HealthCheckResult>> _checkFunc;

        public CustomCheck(string name, Func<CancellationToken, Task<HealthCheckResult>> checkFunc)
        {
            _checkFunc = checkFunc;
            Name = name;
        }

        public string Name { get; }

        public Task<HealthCheckResult> CheckAsync(CancellationToken cancellationToken = default)
        {
            return _checkFunc(cancellationToken);
        }
    }
}
