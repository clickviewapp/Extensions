namespace ClickView.Extensions.RestClient.Requests
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Responses;

    public class RestClientRequest<TData> : BaseRestClientRequest<RestClientResponse<TData>>
    {
        public RestClientRequest(HttpMethod method, string resource) : base(method, resource)
        {
        }

        protected override async ValueTask<RestClientResponse<TData>> ParseResponseAsync(HttpResponseMessage message)
        {
            if (!message.IsSuccessStatusCode)
                return new RestClientResponse<TData>(message, default);

            TData? data;
            if (message.Content.Headers.ContentLength == 0)
            {
                // If we explicitly have a content length of 0, then there is no content to read
                data = default;
            }
            else
            {
#if NET
                await
#endif
                using var stream = await message.Content.ReadAsStreamAsync().ConfigureAwait(false);

                data = await DeserializeAsync<TData>(stream).ConfigureAwait(false);
            }

            return new RestClientResponse<TData>(message, data);
        }

        protected TData? Deserialize(string input)
        {
            return Deserialize<TData>(input);
        }
    }

    public class RestClientRequest : BaseRestClientRequest<RestClientResponse>
    {
        public RestClientRequest(HttpMethod method, string resource) : base(method, resource)
        {
        }

        protected override ValueTask<RestClientResponse> ParseResponseAsync(HttpResponseMessage message)
        {
            return new ValueTask<RestClientResponse>(new RestClientResponse(message));
        }
    }
}
