namespace ClickView.Extensions.Hosting.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

public class InvalidCronWorkerOptionException : Exception
{
    public InvalidCronWorkerOptionException(string optionPropertyName, string message)
        : base($"Invalid option value for {optionPropertyName}. {message}")
    {
    }
}
