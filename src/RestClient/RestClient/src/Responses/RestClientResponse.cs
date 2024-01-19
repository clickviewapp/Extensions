namespace ClickView.Extensions.RestClient.Responses
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class RestClientResponse
    {
        public RestClientResponse(HttpResponseMessage message)
        {
            HttpStatusCode = message.StatusCode;
            Headers = message.Headers;
        }

        public HttpStatusCode HttpStatusCode { get; }
        public Error? Error { get; internal set; }
        public HttpResponseHeaders Headers { get; }
    }

    public class RestClientResponse<T> : RestClientResponse
    {
        public RestClientResponse(HttpResponseMessage message, T? data) : base(message)
        {
            Data = data;
        }

        public T? Data { get; internal set; }
    }
}
