namespace ClickView.Extensions.RestClient.Exceptions;

using System;

/// <inheritdoc />
/// <summary>
///     The base exception thrown by the ClickView Clients
/// </summary>
public class ClickViewClientException : Exception
{
    public ClickViewClientException(string message) : base(message)
    {
    }

    public ClickViewClientException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
