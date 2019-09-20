namespace ClickView.Extensions.HealthCheck
{
    using System;

    public class HealthCheckResult
    {
        public HealthCheckStatus Status { get; }
        public string Message { get; }
        public Exception Exception { get; }

        protected HealthCheckResult(HealthCheckStatus status, string message = null, Exception exception = null)
        {
            Status = status;
            Message = message;
            Exception = exception;
        }

        public static HealthCheckResult Healthy()
        {
            return new HealthCheckResult(HealthCheckStatus.Healthy);
        }

        public static HealthCheckResult Unhealthy()
        {
            return new HealthCheckResult(HealthCheckStatus.Unhealthy);
        }

        public static HealthCheckResult Unhealthy(string message)
        {
            return new HealthCheckResult(HealthCheckStatus.Unhealthy, message);
        }

        public static HealthCheckResult Degraded()
        {
            return new HealthCheckResult(HealthCheckStatus.Degraded);
        }

        public static HealthCheckResult Degraded(string message)
        {
            return new HealthCheckResult(HealthCheckStatus.Degraded, message);
        }

        public static HealthCheckResult Degraded(Exception exception)
        {
            return new HealthCheckResult(HealthCheckStatus.Degraded, exception.Message, exception);
        }

        internal virtual string GetFormattedString()
        {
            var str = $"{Status}";

            if (string.IsNullOrWhiteSpace(Message))
                return str;

            return $"{str} - {Message}";
        }

        public override string ToString()
        {
            return GetFormattedString();
        }
    }
}