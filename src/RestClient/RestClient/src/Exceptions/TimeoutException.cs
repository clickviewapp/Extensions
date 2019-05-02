namespace ClickView.Extensions.RestClient.Exceptions
{
    using System;

    public class TimeoutException : ClickViewClientException
    {
        public TimeoutException(string message) : base(message)
        {
        }

        public TimeoutException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
