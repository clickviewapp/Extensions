namespace ClickView.Extensions.RestClient.Exceptions.Http;

using System;

public class TooManyRequestsException : ClickViewClientHttpException
{
    public TooManyRequestsException(string? message)
        : base(Shims.TooManyRequest, message ?? StatusCodePhrases.TooManyRequests)
    {
    }

    public TooManyRequestsException(string? message, Exception innerException)
        : base(Shims.TooManyRequest, message ?? StatusCodePhrases.TooManyRequests, innerException)
    {
    }
}
