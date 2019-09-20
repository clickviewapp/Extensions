namespace ClickView.Extensions.HealthCheck.Sql
{
    using Builder;

    public static class HealthCheckBuilderExtensions
    {
        public static IHealthCheckBuilder AddSqlCheck(this IHealthCheckBuilder builder, string name,
            string connectionString)
        {
            return builder.AddCheck(new SqlCheck(name, connectionString));
        }
    }
}