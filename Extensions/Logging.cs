// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Hosting;

namespace Common.Extensions
{
    public static class Logging
    {
        public record LoggingOptions
        {
            public string LoggingConfigurationSectionName = "NLog";
        }

        public static IHostBuilder AddODELogging(this IHostBuilder hostBuilder, Action<LoggingOptions> options)
        {
            // This not being handled with DI as the with the ConfigureServices
            // in part as it is needed below and I didn't want to play games with
            // having to wait for the services to be configured.
            var loggingOptions = new LoggingOptions();
            options(loggingOptions);
            _ = hostBuilder.ConfigureLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog(new NLog.Extensions.Logging.NLogProviderOptions
                {
                    LoggingConfigurationSectionName = loggingOptions.LoggingConfigurationSectionName,
                });
            return hostBuilder;
        }

        public static IHostBuilder AddODELogging(this IHostBuilder hostBuilder)
        {
            return hostBuilder.AddODELogging(options => { });
        }

        public static IHost AddUnhandledExceptionLogging(this IHost host)
        {
            TaskScheduler.UnobservedTaskException += (_, e) =>
            {
                var logger = (host.Services.GetService<ILoggerFactory>() ?? throw new ArgumentNullException(nameof(ILoggerFactory))).CreateLogger("ErrorHandling");
                logger.LogCritical(e.Exception, "Unhandled Exception");
                Environment.FailFast("Faulted", e.Exception);
            };
            return host;
        }

        public static IHost LogStartup(this IHost host)
        {
            return host.LogStartup(additionalInfo => { });
        }

        public static IHost LogStartup(this IHost host, Action<ILogger> additionalInfo)
        {
            var logger = (host.Services.GetService<ILoggerFactory>() ?? throw new ArgumentNullException(nameof(ILoggerFactory))).CreateLogger("Startup");
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            logger.LogInformation("------------------------------------------------------");
            logger.LogInformation("---------------------- Starting ----------------------");
            logger.LogInformation("------------------------------------------------------");
            logger.LogInformation("Assembly {@assembly} ...", assembly);
            var hostingEnvironment = host.Services.GetService<IHostEnvironment>() ?? throw new ArgumentNullException(nameof(IHostEnvironment));
            logger.LogInformation("Hosting Environment Name {@HostingEnvironment}", hostingEnvironment.EnvironmentName);
            logger.LogInformation("Platform {@Version}", Environment.OSVersion.VersionString);
            logger.LogInformation("Timezone {@}", TimeZoneInfo.Local);

            // Allow the caller to log additional information
            additionalInfo(logger);

            return host;
        }
    }
}

