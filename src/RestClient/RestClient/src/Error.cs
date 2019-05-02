namespace ClickView.Extensions.RestClient
{
    using System.Net;

    public class Error
    {
        public HttpStatusCode HttpStatusCode { get; }
        public ErrorBody Body { get; }

        public Error(HttpStatusCode httpStatusCode, ErrorBody body)
        {
            HttpStatusCode = httpStatusCode;
            Body = body;
        }
    }
}