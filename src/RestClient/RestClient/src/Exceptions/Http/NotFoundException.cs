namespace ClickView.Extensions.RestClient.Exceptions.Http
{
    using System;
    using System.Net;

    public class NotFoundException : ClickViewClientHttpException
    {
        public NotFoundException(HttpStatusCode httpStatusCode, string message) : base(httpStatusCode, message)
        {
        }

        public NotFoundException(HttpStatusCode httpStatusCode, string message, Exception innerException) :
            base(httpStatusCode, message, innerException)
        {
        }
    }
}