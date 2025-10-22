namespace ClickView.Extensions.RestClient;

using System.Net.Http.Headers;

internal abstract class ContentTypes
{
    internal static readonly MediaTypeHeaderValue OctetStream = new("application/octet-stream");
}
