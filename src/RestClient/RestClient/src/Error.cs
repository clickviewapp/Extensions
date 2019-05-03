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
        public ErrorBody Body { get; }
    }
}