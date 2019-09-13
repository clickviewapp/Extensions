namespace ClickView.Extensions.HealthCheck.Builder
{
    public static class HealthCheck
    {
        public static HealthCheckBuilder CreateDefaultBuilder()
        {
            return new HealthCheckBuilder(new HealthCheckerOptions());
        }
    }
}
