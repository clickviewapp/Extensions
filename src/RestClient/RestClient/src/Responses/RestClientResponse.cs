namespace ClickView.Extensions.RestClient.Responses
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class RestClientResponse(HttpResponseMessage message)
    {
        public HttpStatusCode HttpStatusCode { get; } = message.StatusCode;
        public Error? Error { get; internal set; }
        public HttpResponseHeaders Headers { get; } = message.Headers;
    }

    public class RestClientResponse<T>(HttpResponseMessage message, T? data) : RestClientResponse(message)
    {
        public T? Data { get; internal set; } = data;
    }
}
