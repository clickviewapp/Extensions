namespace ClickView.Extensions.RestClient.Extensions;

using System.Net.Http.Headers;

public static class HttpHeadersExtensions
{
    public static bool TryGetFirstHeaderValue(this HttpHeaders headers, string name, out string? value)
    {
        if (!headers.TryGetValues(name, out var values))
        {
            value = null;
            return false;
        }

        if (!values.TryGetFirstValue(out var first))
        {
            value = null;
            return false;
        }

        value = first;
        return true;
    }
}
