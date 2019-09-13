namespace ClickView.Extensions.HealthCheck
{
    using System;
    using System.Threading.Tasks;

    public class TimedHealthCheckResult : HealthCheckResult
    {
        /// <summary>
        /// The time it took the check to run
        /// </summary>
        public TimeSpan Timing { get; }

        public TimedHealthCheckResult(HealthCheckStatus status, TimeSpan timing, string message = null, Exception exception = null) : base(status, message, exception)
        {
            Timing = timing;
        }

        public static HealthCheckResult Ok(TimeSpan timing)
        {
            return new TimedHealthCheckResult(HealthCheckStatus.Ok, timing);
        }

        public static HealthCheckResult Unhealthy(TimeSpan timing)
        {
            return new TimedHealthCheckResult(HealthCheckStatus.Unhealthy, timing);
        }

        public static HealthCheckResult Unhealthy(TimeSpan timing, string message)
        {
            return new TimedHealthCheckResult(HealthCheckStatus.Unhealthy, timing, message);
        }

        public static HealthCheckResult Degraded(TimeSpan timing)
        {
            return new TimedHealthCheckResult(HealthCheckStatus.Degraded, timing);
        }

        public static HealthCheckResult Degraded(TimeSpan timing, string message)
        {
            return new TimedHealthCheckResult(HealthCheckStatus.Degraded, timing, message);
        }

        public static HealthCheckResult Degraded(TimeSpan timing, Exception exception)
        {
            return new TimedHealthCheckResult(HealthCheckStatus.Degraded, timing, exception.Message, exception);
        }

        public static async Task<HealthCheckResult> TimeAsync(Func<Task<HealthCheckResult>> checkFunc)
        {
            var timer = CheckTimer.Start();

            try
            {
                var result = await checkFunc().ConfigureAwait(false);

                return new TimedHealthCheckResult(result.Status, timer.Stop(), result.Message, result.Exception);
            }
            catch (Exception ex)
            {
                return new TimedHealthCheckResult(HealthCheckStatus.Degraded, timer.Stop(), exception: ex);
            }
        }

        internal override string GetFormattedString()
        {
            var str = $"{Status} [{Timing.TotalMilliseconds}ms]";

            if (string.IsNullOrWhiteSpace(Message))
                return str;

            return $"{str} - {Message}";
        }
    }
}