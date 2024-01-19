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
        private readonly Uri? _baseAddress;
        private readonly HttpClient _httpClient;
        private readonly CoreRestClientOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClient" /> class
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="options"></param>
        public RestClient(Uri baseAddress, RestClientOptions? options = null)
        {
            _baseAddress = baseAddress ?? throw new ArgumentNullException(nameof(baseAddress));

            var o = options ?? RestClientOptions.Default;
            _options = o;

            _httpClient = CreateClient(o);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RestClient" /> class with a specified <paramref name="httpClient"/>
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="options"></param>
        public RestClient(HttpClient httpClient, CoreRestClientOptions? options = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options ?? RestClientOptions.Default;
            _baseAddress = null;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RestClient" /> class with a specified <paramref name="httpClient"/>
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="httpClient"></param>
        /// <param name="options"></param>
        public RestClient(Uri baseAddress, HttpClient httpClient, CoreRestClientOptions? options = null)
            : this(httpClient, options)
        {
            _baseAddress = baseAddress ?? throw new ArgumentNullException(nameof(baseAddress));
        }

        /// <summary>
        /// Executes a <see cref="RestClientRequest{TData}"/>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ClickViewClientException"></exception>
        public async Task<TResponse> ExecuteAsync<TResponse>(BaseRestClientRequest<TResponse> request, CancellationToken token = default)
            where TResponse : RestClientResponse
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            token.ThrowIfCancellationRequested();

            if (_options.Authenticator != null)
                await _options.Authenticator.AuthenticateAsync(request, token).ConfigureAwait(false);

            token.ThrowIfCancellationRequested();

            var requestUri = new Uri(
                QueryHelpers.AddQueryString(request.Resource, request.Parameters),
                UriKind.RelativeOrAbsolute);

            // if the request uri is not absolute, use the base uri
            if (!requestUri.IsAbsoluteUri)
            {
                if (_baseAddress != null)
                {
                    requestUri = new Uri(_baseAddress, requestUri);
                }
                else
                {
                    // only do the following logic if the _httpClient doesn't have a base address already
                    // we should remove this if we update the ctor to always have a baseAddress

                    if (_httpClient.BaseAddress == null)
                    {
                        throw new InvalidOperationException(
                            "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.");
                    }

                    requestUri = new Uri(_httpClient.BaseAddress, requestUri);
                }
            }

            using var httpRequest = new HttpRequestMessage(request.Method, requestUri);
            foreach (var h in request.Headers)
            {
                httpRequest.Headers.TryAddWithoutValidation(h.Key, h.Value);
            }

            SetContent(httpRequest, request);

            try
            {
                using var response = await _httpClient.SendAsync(httpRequest, token).ConfigureAwait(false);
                return await request.GetResponseAsync(response).ConfigureAwait(false);
            }
            catch (HttpRequestException e)
            {
                throw new ClickViewClientException(e.Message, e);
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

        private static HttpClient CreateClient(RestClientOptions options)
        {
            var handler = new HttpClientHandler();

            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = options.DecompressionMethods;

            return new HttpClient(handler)
            {
                Timeout = options.Timeout
            };
        }
    }
}
