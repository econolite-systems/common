// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace Econolite.Ode.Extensions.AspNet;

public static class Logging
{
    public record LoggingOptions
    {
        public string LoggingConfigurationSectionName = "NLog";
    }
        
    public static WebApplicationBuilder AddOdeLogging(this WebApplicationBuilder hostBuilder, Action<LoggingOptions> options)
    {
        // This not being handled with DI as the with the ConfigureServices
        // in part as it is needed below and I didn't want to play games with
        // having to wait for the services to be configured.
        var loggingOptions = new LoggingOptions();
        options(loggingOptions);
        _ = hostBuilder.Logging.ClearProviders().SetMinimumLevel(LogLevel.Trace).AddNLogWeb(new NLogAspNetCoreOptions
        {
            LoggingConfigurationSectionName = loggingOptions.LoggingConfigurationSectionName,
        });
        return hostBuilder;
    }

    public static WebApplicationBuilder AddOdeLogging(this WebApplicationBuilder hostBuilder)
    {
        return hostBuilder.AddOdeLogging(options => { });
    }
}
