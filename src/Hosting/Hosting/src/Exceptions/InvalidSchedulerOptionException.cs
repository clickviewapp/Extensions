namespace ClickView.Extensions.Hosting.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

public class InvalidSchedulerOptionException : Exception
{
    public InvalidSchedulerOptionException(string optionPropertyName, string message)
        : base($"Invalid option value for {optionPropertyName}. {message}")
    {
    }
}
