namespace ClickView.Extensions.HealthCheck.MySql
{
    using Builder;

    public static class HealthCheckBuilderExtensions
    {
        public static IHealthCheckBuilder AddMySqlCheck(this IHealthCheckBuilder builder, string name,
            string connectionString)
        {
            return builder.AddCheck(new MySqlCheck(name, connectionString));
        }
    }
}