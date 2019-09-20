namespace ClickView.Extensions.HealthCheck.Http
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class HttpCheck : IHealthCheck
    {
        private readonly Uri _uri;
        private readonly HttpCheckOptions _options;

        private static readonly HttpClient HttpClient = new HttpClient();

        public HttpCheck(string name, Uri uri) : this(name, uri, new HttpCheckOptions())
        {
        }

        public HttpCheck(string name, Uri uri, HttpCheckOptions options)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            _uri = uri ?? throw new ArgumentNullException(nameof(uri));
            _options = options ?? throw new ArgumentNullException(nameof(options));

            Name = name;
        }

        public string Name { get; }

        public async Task<HealthCheckResult> CheckAsync(CancellationToken cancellationToken = default)
        {
            var timer = CheckTimer.Start();

            try
            {
                var matchContent = _options.ExpectedContent != null;

                var completionOption = matchContent
                    ? HttpCompletionOption.ResponseContentRead
                    : HttpCompletionOption.ResponseHeadersRead;

                using (var response = await HttpClient
                    .GetAsync(_uri, completionOption, cancellationToken)
                    .ConfigureAwait(false))
                {
                    string contentString = null;

                    // if we need to match the content, read it while still timing
                    if(matchContent)
                        contentString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    // all work has been finished, so stop the timer
                    var timing = timer.Stop();

                    if (!response.IsSuccessStatusCode)
                        return TimedHealthCheckResult.Unhealthy(timing, response.ReasonPhrase);

                    if (timing > _options.UnhealthyThreshold)
                        return TimedHealthCheckResult.Degraded(timing);

                    if (!matchContent)
                        return TimedHealthCheckResult.Healthy(timing);

                    // need to match the response content
                    if(_options.ExpectedContent.Equals(contentString))
                        return TimedHealthCheckResult.Healthy(timing);

                    return HealthCheckResult.Degraded("Response content does not match expected content");
                }
            }
            catch (Exception ex)
            {
                return TimedHealthCheckResult.Unhealthy(timer.Stop(), ex);
            }
        }
    }
}
