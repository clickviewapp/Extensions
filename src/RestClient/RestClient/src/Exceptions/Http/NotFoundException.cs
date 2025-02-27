namespace ClickView.Extensions.RestClient.Exceptions.Http;

using System;
using System.Net;

public class NotFoundException : ClickViewClientHttpException
{
    public NotFoundException(string? message)
        : base(HttpStatusCode.NotFound, message ?? StatusCodePhrases.NotFound)
    {
    }

    public NotFoundException(string? message, Exception innerException)
        : base(HttpStatusCode.NotFound, message ?? StatusCodePhrases.NotFound, innerException)
    {
    }
}
