namespace ClickView.Extensions.RestClient.Exceptions.Http
{
    using System;
    using System.Net;

    public class BadRequestException : ClickViewClientHttpException
    {
        public BadRequestException(HttpStatusCode httpStatusCode, string message) : base(httpStatusCode, message)
        {
        }

        public BadRequestException(HttpStatusCode httpStatusCode, string message, Exception innerException) :
            base(httpStatusCode, message, innerException)
        {
        }
    }
}