namespace ClickView.Extensions.RestClient.Responses
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class RestClientResponse
    {
        public HttpStatusCode HttpStatusCode { get; }
        public Error Error { get; internal set; }
        public HttpResponseHeaders Headers { get; }

        public RestClientResponse(HttpResponseMessage message)
        {
            HttpStatusCode = message.StatusCode;
            Headers = message.Headers;
        }
    }

    public class RestClientResponse<T> : RestClientResponse
    {
        public T Data { get; internal set; }

        public RestClientResponse(HttpResponseMessage message, T data) : base(message)
        {
            Data = data;
        }
    }
}
