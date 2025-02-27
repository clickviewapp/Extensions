namespace ClickView.Extensions.RestClient.Exceptions.Http;

using System;
using System.Net;

public class UnauthorizedException : ClickViewClientHttpException
{
    public UnauthorizedException(string? message)
        : base(HttpStatusCode.Unauthorized, message ?? StatusCodePhrases.Unauthorized)
    {
    }

    public UnauthorizedException(string? message, Exception innerException)
        : base(HttpStatusCode.Unauthorized, message ?? StatusCodePhrases.Unauthorized, innerException)
    {
    }
}
