namespace ClickView.Extensions.RestClient.Extensions;

using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;

public static class HttpHeadersExtensions
{
    public static bool TryGetFirstHeaderValue(this HttpHeaders headers, string name,
        [MaybeNullWhen(false)] out string value)
    {
        if (!headers.TryGetValues(name, out var values))
        {
            value = null;
            return false;
        }

        if (!values.TryGetFirstValue(out var first) || first is null)
        {
            value = null;
            return false;
        }

        value = first;
        return true;
    }
}
