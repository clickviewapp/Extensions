namespace ClickView.Extensions.Hosting.Exceptions;
using System;

public class InvalidCronWorkerOptionException : Exception
{
    public InvalidCronWorkerOptionException(string optionPropertyName, string message)
        : base($"Invalid option value for {optionPropertyName}. {message}")
    {
    }
}
