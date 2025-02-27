namespace ClickView.Extensions.RestClient;

using System.Net;

public class Error(HttpStatusCode httpStatusCode, ErrorBody? body, string content)
{
    public HttpStatusCode HttpStatusCode { get; } = httpStatusCode;
    public string Content { get; } = content;
    public ErrorBody? Body { get; } = body;
}
