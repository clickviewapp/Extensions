namespace ClickView.Extensions.Hosting
{
    using System;

    [Obsolete("Use the built in .NET Generic Host instead")]
    public class ConsoleHostOptions
    {
        public string EnvironmentVariablesPrefix { get; set; } = "NETCORE";
    }
}
