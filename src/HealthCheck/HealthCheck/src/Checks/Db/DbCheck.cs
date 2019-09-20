namespace ClickView.Extensions.HealthCheck.Checks.Db
{
    using System;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class DbCheck : IHealthCheck
    {
        private readonly string _connectionString;

        protected DbCheck(string name, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;

            Name = name;
        }

        public string Name { get; }

        public async Task<HealthCheckResult> CheckAsync(CancellationToken cancellationToken)
        {
            var timer = CheckTimer.Start();

            try
            {
                using (var conn = GetConnection(_connectionString))
                {
                    await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

                    return TimedHealthCheckResult.Healthy(timer.Stop());
                }
            }
            catch (Exception ex)
            {
                return TimedHealthCheckResult.Unhealthy(timer.Stop(), ex);
            }
        }

        protected abstract DbConnection GetConnection(string connectionString);
    }
}
