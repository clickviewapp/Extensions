﻿namespace ClickView.Extensions.HealthCheck
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// This service is used to perform various health checks
    /// </summary>
    public sealed class HealthChecker : IHealthChecker
    {
        private readonly IEnumerable<IHealthCheck> _checks;
        private readonly SemaphoreSlim _checkSlim;

        public HealthChecker(IEnumerable<IHealthCheck> checks, IOptions<HealthCheckerOptions> options)
        {
            _checks = checks;

            var o = options.Value;

            _checkSlim = new SemaphoreSlim(o.CheckConcurrency, o.CheckConcurrency);
        }

        public async Task<HealthCheckResults> CheckAllAsync(CancellationToken cancellationToken)
        {
            var tasks = _checks.Select(c => RunCheck(c, cancellationToken));
            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            return new HealthCheckResults(results);
        }

        public async Task<HealthCheckResultItem> RunCheck(IHealthCheck check, CancellationToken cancellationToken)
        {
            await _checkSlim.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                var result = await check.CheckAsync(cancellationToken).ConfigureAwait(false);

                return new HealthCheckResultItem(check.Name, result);
            }
            finally
            {
                _checkSlim.Release();
            }
        }
    }
}
