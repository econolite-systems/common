// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using Econolite.Ode.Monitoring.Events.Messaging;
using Econolite.Ode.Monitoring.Events.Messaging.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;
using System;
using System.Threading;
using Consts = Econolite.Ode.Monitoring.Events.Messaging.Consts;

namespace Econolite.Ode.Monitoring.Events.Extensions
{
    public static class Defined
    {
        public static IServiceCollection AddUserEventSupport(this IServiceCollection services, IConfiguration configuration, Guid defaultTenantId, string configPath = Consts.DEFAULT_CHANNEL_CONFIG_PATH) => services
            .AddUserEventSupport(configuration, defaultTenantId, configPath);
        public static IServiceCollection AddUserEventSupport(this IServiceCollection services, Guid defaultTenantId, string defaultChannel = Consts.DEFAULT_CHANNEL) => services
            .AddEventSourceServices(defaultChannel)
            .AddUserEventSupport(_ => _.DefaultTenantId = defaultTenantId);
        public static IServiceCollection AddUserEventSupport(this IServiceCollection services, Guid defaultTenantId, Action<SourceOptions<UserEventMessage>> configure) => services
            .AddEventSourceServices(configure)
            .AddUserEventSupport(_ => _.DefaultTenantId = defaultTenantId);

        /// <summary>
        /// This is a helper that Setups the UserEventFactory with defaults to make using it easier
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="action">Allows for setting values in the UserEventFactoryOptions</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddUserEventSupport(this IServiceCollection services, IConfiguration configuration, Action<UserEventFactoryOptions> action, string configPath = Consts.DEFAULT_CHANNEL_CONFIG_PATH) => services
            .AddEventSourceServices(configuration, configPath)
            .AddUserEventSupport(action);
        public static IServiceCollection AddUserEventSupport(this IServiceCollection services, Action<UserEventFactoryOptions> action, string defaultChannel = Consts.DEFAULT_CHANNEL) => services
            .AddEventSourceServices(defaultChannel)
            .AddUserEventSupport(action);
        public static IServiceCollection AddUserEventSupport(this IServiceCollection services, Action<UserEventFactoryOptions> action, Action<SourceOptions<UserEventMessage>> configure) => services
            .AddEventSourceServices(configure)
            .AddUserEventSupport(action);
        private static IServiceCollection AddUserEventSupport(this IServiceCollection services, Action<UserEventFactoryOptions> action) => services
            .Configure(action)
            .AddSingleton<UserEventFactory>();

        public static LogLevel ToLogLevel(this EventLevel eventLevel) => eventLevel switch 
        {
            EventLevel.Critical => LogLevel.Critical,
            EventLevel.Error => LogLevel.Error,
            EventLevel.Warning => LogLevel.Warning,
            EventLevel.Information => LogLevel.Information,
            EventLevel.Debug => LogLevel.Debug,
            _ => LogLevel.Critical,
        };

        public static void ExposeUserEvent(this ILogger logger, UserEvent userEvent) => logger
            .SinkUserEvent(userEvent)
            .Log(userEvent.Level.ToLogLevel(), "{@ExposedUserEvent}", userEvent);

        public static void ExposeUserEvent(this ILogger logger, Exception ex, UserEvent userEvent) => logger
            .SinkUserEvent(userEvent)
            .Log(userEvent.Level.ToLogLevel(), ex, "{@ExposedUserEvent}", userEvent);

        internal static ILogger SinkUserEvent(this ILogger logger, UserEvent userEvent)
        {
            var jtf = new JoinableTaskFactory(new JoinableTaskContext());
            jtf.Run(async () => await userEvent.SinkContext.EventSink.SinkAsync(userEvent, CancellationToken.None));
            return logger;
        }
    }
}
