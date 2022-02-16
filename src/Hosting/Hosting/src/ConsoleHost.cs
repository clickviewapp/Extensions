namespace ClickView.Extensions.Hosting
{
    using System;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    [Obsolete("Use the built in .NET Generic Host instead")]
    public static class ConsoleHost
    {
        public static IHostBuilder CreateDefaultBuilder(string[] args) =>
            CreateDefaultBuilder(args, new ConsoleHostOptions());

        public static IHostBuilder CreateDefaultBuilder(string[] args, ConsoleHostOptions options)
        {
            return new HostBuilder()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true)
                        .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                            optional: true)
                        .AddEnvironmentVariables(prefix: options.EnvironmentVariablesPrefix + "_")
                        .AddCommandLine(args);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                })
                .UseServiceProviderFactory(new DefaultServiceProviderFactory(new ServiceProviderOptions
                {
                    ValidateScopes = true
                }));
        }
    }
}
