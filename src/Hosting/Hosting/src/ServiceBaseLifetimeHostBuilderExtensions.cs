namespace ClickView.Extensions.Hosting
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class ServiceBaseLifetimeHostBuilderExtensions
    {
        /// <summary>
        /// Sets the host lifetime to ServiceBaseLifetime and sets the Content Root.
        /// </summary>
        /// <remarks>
        /// This is context aware and will only activate if it detects the process is running
        /// as a Windows Service.
        /// </remarks>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IHostBuilder UseServiceBaseLifetime(this IHostBuilder hostBuilder)
        {
            if (IsWindowsService())
            {
                // CurrentDirectory for services is c:\Windows\System32, but that's what Host.CreateDefaultBuilder uses for VS scenarios.
                hostBuilder.UseContentRoot(AppContext.BaseDirectory);
                return hostBuilder.ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IHostLifetime, ServiceBaseLifetime>();
                });
            }

            return hostBuilder;
        }

        public static Task RunAsServiceAsync(this IHostBuilder hostBuilder, CancellationToken cancellationToken = default)
        {
            return hostBuilder.UseServiceBaseLifetime().Build().RunAsync(cancellationToken);
        }

        private static bool IsWindowsService()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return false;
            }

            var parent = Win32.GetParentProcess();
            if (parent == null)
            {
                return false;
            }

            return parent.SessionId == 0 && string.Equals("services", parent.ProcessName, StringComparison.OrdinalIgnoreCase);
        }
    }
}