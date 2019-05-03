namespace ClickView.Extensions.RestClient.Exceptions.Http
{
    using System;
    using System.Net;

    public class NetworkException : ClickViewClientHttpException
    {
        public NetworkException(HttpStatusCode httpStatusCode, string message) : base(httpStatusCode, message)
        {
        }

        public NetworkException(HttpStatusCode httpStatusCode, string message, Exception innerException) :
            base(httpStatusCode, message, innerException)
        {
        }
    }
}