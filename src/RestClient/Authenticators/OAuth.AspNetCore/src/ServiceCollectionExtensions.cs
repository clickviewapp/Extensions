namespace ClickView.Extensions.RestClient.Authenticators.OAuth.AspNetCore
{
    using System;
    using Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class ServiceCollectionExtensions
    {
        public static void AddHttpContextAuthenticator(this IServiceCollection serviceCollection, string authority)
        {
            serviceCollection.AddHttpContextAuthenticator(authority, _ => { });
        }

        public static void AddHttpContextAuthenticator(this IServiceCollection services, string authority,
            Action<HttpContextAuthenticatorOptions> configureOptions)
        {
            //register options
            services.AddSingleton(p =>
            {
                var loggerFactory = p.GetRequiredService<ILoggerFactory>();
                var httpContextAccessor = p.GetRequiredService<IHttpContextAccessor>();

                var options = new HttpContextAuthenticatorOptions(authority)
                {
                    LoggerFactory = loggerFactory,

                    // NOTE: this does mean that if the logger factory is changed in configureOptions, it wont be changed here
                    TokenStore = new HttpContextTokenStore(httpContextAccessor, loggerFactory)
                };

                configureOptions(options);

                return options;
            });

            // ensure we have the HttpContextAccessor
            services.AddHttpContextAccessor();

            //register authenticator
            services.AddSingleton<IAuthenticator, HttpContextAuthenticator>();
        }
    }
}
