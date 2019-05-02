namespace ClickView.Extensions.RestClient.Exceptions.Http
{
    using System;
    using System.Net;

    /// <inheritdoc />
    /// <summary>
    /// The exception thrown when the exception is caused by the remote server
    /// </summary>
    public class RemoteServerException : ClickViewClientHttpException
    {
        public RemoteServerException(HttpStatusCode httpStatusCode, string message) : base(httpStatusCode, message)
        {
        }

        public RemoteServerException(HttpStatusCode httpStatusCode, string message, Exception innerException) : base(httpStatusCode, message, innerException)
        {
        }
    }
}
