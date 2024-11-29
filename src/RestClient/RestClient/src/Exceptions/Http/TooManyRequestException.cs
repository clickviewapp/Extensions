namespace ClickView.Extensions.RestClient.Exceptions.Http;

using System;
using System.Net;

public class TooManyRequestException : ClickViewClientHttpException
{
    public TooManyRequestException(HttpStatusCode httpStatusCode, string message) : base(httpStatusCode, message)
    {
    }

    public TooManyRequestException(HttpStatusCode httpStatusCode, string message, Exception innerException) :
        base(httpStatusCode, message, innerException)
    {
    }
}
