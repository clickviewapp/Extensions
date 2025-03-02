namespace ClickView.Extensions.RestClient.Exceptions.Http;

using System;
using System.Net;

public class BadRequestException : ClickViewClientHttpException
{
    public BadRequestException(string? message)
        : base(HttpStatusCode.BadRequest, message ?? StatusCodePhrases.BadRequest)
    {
    }

    public BadRequestException(string? message, Exception innerException)
        : base(HttpStatusCode.BadRequest, message ?? StatusCodePhrases.BadRequest, innerException)
    {
    }
}
