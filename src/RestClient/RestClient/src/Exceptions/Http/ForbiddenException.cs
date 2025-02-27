namespace ClickView.Extensions.RestClient.Exceptions.Http;

using System;
using System.Net;

public class ForbiddenException : ClickViewClientHttpException
{
    public ForbiddenException(string? message)
        : base(HttpStatusCode.Forbidden, message ?? StatusCodePhrases.Forbidden)
    {
    }

    public ForbiddenException(string? message, Exception innerException)
        : base(HttpStatusCode.Forbidden, message ?? StatusCodePhrases.Forbidden, innerException)
    {
    }
}
