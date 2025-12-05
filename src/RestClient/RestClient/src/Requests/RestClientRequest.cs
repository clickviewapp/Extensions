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

        protected override ValueTask<RestClientResponse<TData>> ParseResponseAsync(HttpResponseMessage message)
        {
            // If the response is not a success, return early or if we explicitly have a content length of 0, then there is no content to read
            // If the response is NoContent, Content.Headers is empty, i.e. Content.Headers.ContentLength is null, then return early
            if (!message.IsSuccessStatusCode || message.StatusCode == HttpStatusCode.NoContent || message.Content.Headers.ContentLength == 0)
                return new ValueTask<RestClientResponse<TData>>(new RestClientResponse<TData>(message, default));

            return ReadContentAsync(message);
        }

        private async ValueTask<RestClientResponse<TData>> ReadContentAsync(HttpResponseMessage message)
        {
#if NET
            await
#endif
                using var stream = await message.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var data = await DeserializeAsync<TData>(stream).ConfigureAwait(false);

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
