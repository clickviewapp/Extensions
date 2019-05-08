namespace ClickView.Extensions.RestClient.Exceptions
{
    using System;

    /// <inheritdoc />
    /// <summary>
    ///     The exception thrown when a serialization error occurs
    /// </summary>
    public class SerializationException : ClickViewClientException
    {
        public SerializationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
