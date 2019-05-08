namespace ClickView.Extensions.RestClient.Exceptions.Http
{
    using System;
    using System.Net;

    /// <inheritdoc />
    /// <summary>
    ///     The exception thrown when the exception is caused by a http error
    /// </summary>
    public class ClickViewClientHttpException : ClickViewClientException
    {
        public ClickViewClientHttpException(HttpStatusCode httpStatusCode, string message) : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }

        public ClickViewClientHttpException(HttpStatusCode httpStatusCode, string message, Exception innerException) :
            base(message, innerException)
        {
            HttpStatusCode = httpStatusCode;
        }

        public HttpStatusCode HttpStatusCode { get; }
    }
}
