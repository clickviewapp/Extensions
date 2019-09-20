namespace ClickView.Extensions.HealthCheck
{
    public enum HealthCheckStatus
    {
        /// <summary>
        /// The component has failed
        /// </summary>
        Unhealthy = 0,

        /// <summary>
        /// The component is running but in a slow or unstable state
        /// </summary>
        Degraded = 1,

        /// <summary>
        /// Everything is ok
        /// </summary>
        Healthy = 2
    }
}