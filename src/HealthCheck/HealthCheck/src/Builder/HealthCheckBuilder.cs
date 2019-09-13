namespace ClickView.Extensions.HealthCheck.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Checks;
    using Microsoft.Extensions.Options;

    public sealed class HealthCheckBuilder : IHealthCheckBuilder
    {
        private readonly List<IHealthCheck> _checks = new List<IHealthCheck>();
        private readonly HealthCheckerOptions _options;

        public HealthCheckBuilder(HealthCheckerOptions options)
        {
            _options = options;
        }

        public IHealthCheckBuilder AddCheck(IHealthCheck healthCheck)
        {
            _checks.Add(healthCheck);

            return this;
        }

        public IHealthCheckBuilder AddCheck(string name, Func<CancellationToken, Task<HealthCheckResult>> checkFunc)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (checkFunc == null) throw new ArgumentNullException(nameof(checkFunc));

            _checks.Add(new CustomCheck(name, checkFunc));

            return this;
        }

        public IHealthCheckBuilder Configure(Action<HealthCheckerOptions> configureOptions)
        {
            configureOptions(_options);

            return this;
        }

        public IHealthChecker Build()
        {
            var checks = new List<IHealthCheck>(_checks); // copy list

            return new HealthChecker(checks, new OptionsWrapper<HealthCheckerOptions>(_options));
        }
    }
}