namespace ClickView.Extensions.HealthCheck.Http
{
    using System;
    using Builder;

    public static class HealthCheckBuilderExtensions
    {
        public static IHealthCheckBuilder AddHttpCheck(this IHealthCheckBuilder builder, string name, Uri uri)
        {
            return builder.AddCheck(new HttpCheck(name, uri));
        }

        public static IHealthCheckBuilder AddHttpCheck(this IHealthCheckBuilder builder, string name, Uri uri,
            Action<HttpCheckOptions> configureOptions)
        {
            var options = new HttpCheckOptions();

            configureOptions(options);

            return builder.AddCheck(new HttpCheck(name, uri, options));
        }

        public static IHealthCheckBuilder AddHttpPingCheck(this IHealthCheckBuilder builder, string name, Uri baseAddress)
        {
            var options = new HttpCheckOptions {ExpectedContent = "PONG"};

            var uriBuilder = new UriBuilder(baseAddress.GetLeftPart(UriPartial.Authority))
            {
                Path = "_diagnostics/ping"
            };

            return builder.AddCheck(new HttpCheck(name, uriBuilder.Uri, options));
        }
    }
}
