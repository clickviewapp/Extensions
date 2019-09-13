namespace ClickView.Extensions.HealthCheck
{
    public class HealthCheckerOptions
    {
        /// <summary>
        /// The number of checks to run concurrently
        /// </summary>
        public int CheckConcurrency { get; set; } = 6;
    }
}