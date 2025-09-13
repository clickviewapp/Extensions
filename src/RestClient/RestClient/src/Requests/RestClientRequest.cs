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

        protected override async Task<RestClientResponse<TData>> ParseResponseAsync(HttpResponseMessage message)
        {
            if (!message.IsSuccessStatusCode)
                return new RestClientResponse<TData>(message, default);

            var stream = await message.Content.ReadAsStreamAsync().ConfigureAwait(false);
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

        protected override Task<RestClientResponse> ParseResponseAsync(HttpResponseMessage message)
        {
            return Task.FromResult(new RestClientResponse(message));
        }
    }
}
