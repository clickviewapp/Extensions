namespace ClickView.Extensions.RestClient.Helpers
{
    using System;
    using System.Net;
    using Exceptions;
    using Exceptions.Http;

    public static class ErrorHelper
    {
        public static string? GetDefaultErrorMessage(HttpStatusCode httpStatusCode)
        {
            switch (httpStatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return "Authorization has been denied for this request";
                case HttpStatusCode.Forbidden:
                    return "You are not allowed to access this resource";

                default:
                    return null;
            }
        }

        public static ClickViewClientException GetErrorException(Error error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            if (error.Body == null)
                throw new ArgumentNullException(nameof(error.Body));

            var message = error.Body.Message;
            var statusCode = error.HttpStatusCode;

            // HttpStatusCode.TooManyRequests enum value was added in later version of .net
            // such as net standard 2.1 and .net core 2.1 and above. So it is not available
            // in full fat .net framework 4.6.2 
            if ((int)statusCode == 429)
                return new TooManyRequestException(statusCode, message);

            switch (error.HttpStatusCode)
            {
                //errors to do with bad user input
                case HttpStatusCode.BadRequest:
                    return new BadRequestException(statusCode, message);
                case HttpStatusCode.NotFound:
                    return new NotFoundException(statusCode, message);

                //authentication errors
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.Unauthorized:
                    return new UnauthorizedException(statusCode, message);

                //errors because of a bad client
                //todo: Maybe these should their own exceptions?
                case HttpStatusCode.Gone:
                case HttpStatusCode.MethodNotAllowed:
                case HttpStatusCode.NotAcceptable:
                case HttpStatusCode.RequestEntityTooLarge:
                case HttpStatusCode.RequestUriTooLong:
                case HttpStatusCode.UnsupportedMediaType:
                    return new ClickViewClientHttpException(statusCode, message);

                //networking errors
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.GatewayTimeout:
                    return new NetworkException(statusCode, message);

                //server errors
                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.NotImplemented:
                case HttpStatusCode.ServiceUnavailable:
                    return new RemoteServerException(statusCode, message);

                //not errors
                //100
                case HttpStatusCode.Continue:
                case HttpStatusCode.SwitchingProtocols:
                //200
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                case HttpStatusCode.NonAuthoritativeInformation:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.ResetContent:
                case HttpStatusCode.PartialContent:
                //300
                case HttpStatusCode.Ambiguous:
                case HttpStatusCode.Moved:
                case HttpStatusCode.Found:
                case HttpStatusCode.RedirectMethod:
                case HttpStatusCode.NotModified:
                case HttpStatusCode.UseProxy:
                case HttpStatusCode.Unused:
                case HttpStatusCode.RedirectKeepVerb:
                    break;

                //not implemented - return a generic error
                case HttpStatusCode.Conflict:
                case HttpStatusCode.ExpectationFailed:
                case HttpStatusCode.HttpVersionNotSupported:
                case HttpStatusCode.LengthRequired:
                case HttpStatusCode.PaymentRequired:
                case HttpStatusCode.PreconditionFailed:
                case HttpStatusCode.ProxyAuthenticationRequired:
                case HttpStatusCode.RequestedRangeNotSatisfiable:
                case HttpStatusCode.RequestTimeout:
                case HttpStatusCode.UpgradeRequired:
                    return new ClickViewClientHttpException(statusCode, message);

                default:
                    throw new ArgumentOutOfRangeException($"Unknown Http Status Code '{statusCode}'");
            }

            throw new ClickViewClientException("Cannot get error for success response");
        }
    }
}
