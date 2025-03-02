namespace ClickView.Extensions.RestClient;

public static class StatusCodePhrases
{
    // 400
    public const string BadRequest = "Bad Request";

    // 401
    public const string Unauthorized = "Unauthorized";

    // 403
    public const string Forbidden = "Forbidden";

    // 404
    public const string NotFound = "Not Found";

    // 429
    public const string TooManyRequests = "Too Many Requests";

    // 500
    public const string InternalServerError = "Internal Server Error";

    // 501
    public const string NotImplemented = "Not Implemented";

    // 502
    public const string BadGateway = "Bad Gateway";

    // 503
    public const string ServiceUnavailable = "Service Unavailable";

    // 504
    public const string GatewayTimeout = "Gateway Timeout";
}
