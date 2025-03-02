namespace ClickView.Extensions.RestClient;

using System.Net;

internal static class Shims
{
    // HttpStatusCode.TooManyRequests enum value was added in later version of .net
    // such as net standard 2.1 and .net core 2.1 and above. So it is not available
    // in full fat .net framework 4.6.2 
    public const HttpStatusCode TooManyRequest = (HttpStatusCode) 429;
}
