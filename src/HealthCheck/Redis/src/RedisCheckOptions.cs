namespace ClickView.Extensions.HealthCheck.Redis
{
    using System;

    public class RedisCheckOptions
    {
        public TimeSpan UnhealthyThreshold { get; set; } = TimeSpan.FromSeconds(1);
    }
}