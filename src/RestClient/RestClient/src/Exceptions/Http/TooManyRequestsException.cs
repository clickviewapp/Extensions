namespace ClickView.Extensions.RestClient.Exceptions.Http;

using System;

public class TooManyRequestsException : ClickViewClientHttpException
{
    public TooManyRequestsException(string? message)
        : base(Shims.TooManyRequest, message ?? DefaultMessage)
    {
    }

    public TooManyRequestsException(string? message, Exception innerException)
        : base(Shims.TooManyRequest, message ?? DefaultMessage, innerException)
    {
    }

    private const string DefaultMessage = "Too Many Requests.";
}
