namespace ClickView.Extensions.RestClient.Helpers;

using System;
using System.Net;
using Exceptions;
using Exceptions.Http;
using static StatusCodePhrases;

public static class ErrorHelper
{
    public static ClickViewClientException? GetErrorException(Error error)
    {
        if (error == null)
            throw new ArgumentNullException(nameof(error));

        var message = error.Body?.Message;
        var statusCode = error.HttpStatusCode;

        return statusCode switch
        {
            // Informational - not errors
            _ when (int) statusCode is >= 100 and <= 199 => null,

            // Success - not errors
            _ when (int) statusCode is >= 200 and <= 299 => null,

            // Redirection - not errors
            _ when (int) statusCode is >= 300 and <= 399 => null,

            // Client errors
            HttpStatusCode.BadRequest => new BadRequestException(message),
            HttpStatusCode.Unauthorized => new UnauthorizedException(message),
            HttpStatusCode.Forbidden => new ForbiddenException(message),
            HttpStatusCode.NotFound => new NotFoundException(message),
            Shims.TooManyRequest => new TooManyRequestsException(message),

            // Server errors
            HttpStatusCode.InternalServerError => new RemoteServerException(statusCode, InternalServerError),
            HttpStatusCode.NotImplemented => new RemoteServerException(statusCode, NotImplemented),
            HttpStatusCode.BadGateway => new NetworkException(statusCode, BadGateway),
            HttpStatusCode.ServiceUnavailable => new RemoteServerException(statusCode, ServiceUnavailable),
            HttpStatusCode.GatewayTimeout => new NetworkException(statusCode, GatewayTimeout),

            // Catch all
            _ => new ClickViewClientHttpException(statusCode)
        };
    }
}
