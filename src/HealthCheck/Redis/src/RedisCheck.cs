namespace ClickView.Extensions.HealthCheck.Redis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using StackExchange.Redis;

    public class RedisCheck : IHealthCheck
    {
        private readonly RedisCheckOptions _options;
        private readonly Lazy<IConnectionMultiplexer> _connectionMultiplexerLazy;

        public RedisCheck(string name, string connectionString) : this(name, connectionString, new RedisCheckOptions())
        {
        }

        public RedisCheck(string name, string connectionString, RedisCheckOptions options)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            Name = name;
            _options = options ?? throw new ArgumentNullException();
            _connectionMultiplexerLazy =
                new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString));
        }

        public RedisCheck(string name, IConnectionMultiplexer connectionMultiplexer) : this(name, connectionMultiplexer, new RedisCheckOptions())
        {
        }

        public RedisCheck(string name, IConnectionMultiplexer connectionMultiplexer, RedisCheckOptions options)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (connectionMultiplexer == null) throw new ArgumentNullException(nameof(connectionMultiplexer));

            Name = name;
            _options = options ?? throw new ArgumentNullException();
            _connectionMultiplexerLazy = new Lazy<IConnectionMultiplexer>(() => connectionMultiplexer);
        }

        public string Name { get; }

        public async Task<HealthCheckResult> CheckAsync(CancellationToken cancellationToken)
        {
            var timer = CheckTimer.Start();

            try
            {
                var servers = GetServers().ToList();
                var pingTasks = new List<Task<TimeSpan>>();

                foreach (var s in servers)
                {
                    if (!s.IsConnected)
                        continue;

                    pingTasks.Add(s.PingAsync());
                }

                var connectedMsg = $"{pingTasks.Count}/{servers.Count} redis servers connected";

                //Nothing connected
                if (pingTasks.Count == 0)
                    return HealthCheckResult.Unhealthy(connectedMsg);

                //Get the average ping times
                var pingResults = await Task.WhenAll(pingTasks).ConfigureAwait(false);
                var avg = TimeSpan.FromTicks(Convert.ToInt64(pingResults.Average(p => p.Ticks)));

                //Some connected
                if (pingTasks.Count != servers.Count)
                    return TimedHealthCheckResult.Degraded(avg, connectedMsg);

                //All connected
                //Check timing
                if(avg > _options.UnhealthyThreshold)
                    return TimedHealthCheckResult.Degraded(avg);

                //All good
                return TimedHealthCheckResult.Healthy(avg);
            }
            catch (Exception ex)
            {
                return TimedHealthCheckResult.Unhealthy(timer.Stop(), ex);
            }
        }

        private IEnumerable<IServer> GetServers()
        {
            var mux = _connectionMultiplexerLazy.Value;

            var endpoints = mux.GetEndPoints();

            return endpoints.Select(e => mux.GetServer(e));
        }
    }
}
