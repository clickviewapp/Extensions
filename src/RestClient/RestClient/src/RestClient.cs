namespace ClickView.Extensions.RestClient
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Exceptions;
    using Helpers;
    using Http;
    using Requests;
    using Responses;

    public class RestClient
    {
        private readonly HttpClient _httpClient;
        private readonly CoreRestClientOptions _options;

        public RestClient(Uri baseAddress) : this(baseAddress, RestClientOptions.Default)
        {
        }

        public RestClient(Uri baseAddress, RestClientOptions options) : this(baseAddress, CreateHandler(options),
            options)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RestClient" /> class with a specified
        ///     <param name="httpClient">
        ///         <see cref="HttpClient" />
        ///     </param>
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="options"></param>
        public RestClient(HttpClient httpClient, CoreRestClientOptions options)
        {
            _options = options;
            _httpClient = httpClient;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RestClient" /> class with a specified
        ///     <param name="httpClient">
        ///         <see cref="HttpClient" />
        ///     </param>
        /// </summary>
        /// <param name="httpClient"></param>
        public RestClient(HttpClient httpClient) : this(httpClient, new CoreRestClientOptions())
        {
        }

        internal RestClient(Uri baseAddress, HttpMessageHandler handler, RestClientOptions options) : this(
            CreateClient(baseAddress, handler, options), options)
        {
        }

        public async Task<TResponse> ExecuteAsync<TResponse>(BaseRestClientRequest<TResponse> request, CancellationToken token = default)
            where TResponse : RestClientResponse
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            token.ThrowIfCancellationRequested();

            if (_options.Authenticator != null)
                await _options.Authenticator.AuthenticateAsync(request, token).ConfigureAwait(false);

            token.ThrowIfCancellationRequested();

            var resource = QueryHelpers.AddQueryString(request.Resource, request.Parameters);

            using (var httpRequest = new HttpRequestMessage(request.Method, resource))
            {
                foreach (var h in request.Headers)
                {
                    httpRequest.Headers.TryAddWithoutValidation(h.Key, h.Value);
                }

                SetContent(httpRequest, request);

                try
                {
                    using (var response = await _httpClient.SendAsync(httpRequest, token).ConfigureAwait(false))
                    {
                        return await request.GetResponseAsync(response).ConfigureAwait(false);
                    }
                }
                catch (HttpRequestException e)
                {
                    throw new ClickViewClientException(e.Message, e);
                }
            }
        }

        internal void SetContent<TResponse>(HttpRequestMessage httpRequest, BaseRestClientRequest<TResponse> request)
            where TResponse : RestClientResponse
        {
            if (request.Content == null)
                return;

            if (_options.CompressionMethod == CompressionMethod.None)
            {
                httpRequest.Content = request.Content;
                return;
            }

            //we need to compress
            request.Content = new CompressedContent(request.Content, _options.CompressionMethod);
        }

        private static HttpMessageHandler CreateHandler(RestClientOptions options)
        {
            var handler = new HttpClientHandler();

            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = options.DecompressionMethods;

            return handler;
        }

        private static HttpClient CreateClient(Uri baseAddress, HttpMessageHandler handler, RestClientOptions options)
        {
            return new HttpClient(handler)
            {
                BaseAddress = baseAddress,
                Timeout = options.Timeout
            };
        }
    }
}
