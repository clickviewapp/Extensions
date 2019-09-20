namespace ClickView.Extensions.HealthCheck.Http
{
    using System;

    public class HttpCheckOptions
    {
        public TimeSpan UnhealthyThreshold { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// If not null, will match the response of the HttpCheck with the value defined
        /// </summary>
        public string ExpectedContent { get; set; }
    }
}