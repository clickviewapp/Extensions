namespace ClickView.Extensions.HealthCheck
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class HealthCheckResults : IEnumerable<HealthCheckResultItem>
    {
        internal HealthCheckResults(IEnumerable<HealthCheckResultItem> results)
        {
            Results = results.ToList();
            IsHealthy = GetHealthy(Results);
        }

        private static bool GetHealthy(IEnumerable<HealthCheckResultItem> results)
        {
            if (results.All(r => r.Result.Status == HealthCheckStatus.Healthy))
                return true;

            return false;
        }

        /// <summary>
        /// The overall status
        /// </summary>
        public bool IsHealthy { get; }

        private List<HealthCheckResultItem> Results { get; }

        public IEnumerator<HealthCheckResultItem> GetEnumerator()
        {
            return Results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}