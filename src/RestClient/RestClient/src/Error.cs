namespace ClickView.Extensions.RestClient
{
    using System.Net;

    public class Error
    {
        public Error(HttpStatusCode httpStatusCode, ErrorBody body)
        {
            HttpStatusCode = httpStatusCode;
            Body = body;
        }

        public HttpStatusCode HttpStatusCode { get; }
        public string Content { get; set; } = null!;
        public ErrorBody Body { get; }
    }
}
