namespace ClickView.Extensions.RestClient.Exceptions.Http
{
    using System;
    using System.Net;

    public class UnauthorizedException : ClickViewClientHttpException
    {
        public UnauthorizedException(HttpStatusCode httpStatusCode, string message) : base(httpStatusCode, message)
        {
        }

        public UnauthorizedException(HttpStatusCode httpStatusCode, string message, Exception innerException) :
            base(httpStatusCode, message, innerException)
        {
        }
    }
}
