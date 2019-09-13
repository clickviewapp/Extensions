namespace ClickView.Extensions.HealthCheck.Redis
{
    using System;
    using Builder;

    public static class HealthCheckBuilderExtensions
    {
        public static IHealthCheckBuilder AddRedisCheck(this IHealthCheckBuilder builder, string name,
            string connectionString)
        {
            return builder.AddCheck(new RedisCheck(name, connectionString));
        }

        public static IHealthCheckBuilder AddRedisCheck(this IHealthCheckBuilder builder, string name,
            string connectionString, Action<RedisCheckOptions> configureOptions)
        {
            var options = new RedisCheckOptions();

            configureOptions(options);

            return builder.AddCheck(new RedisCheck(name, connectionString, options));
        }
    }
}