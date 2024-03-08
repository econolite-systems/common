// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Econolite.Ode.Monitoring.Metrics.Messaging.Extensions
{
    public static class Defined
    {
        // Add the sources
        public static IServiceCollection AddMetricMonitoringSources(this IServiceCollection services, Action<SourceOptions<MetricMessage>> action) => services
            .AddMessaging()
            .AddMessagingProtobufSource(action)
            .AddTransient<IMetricSource, MetricSource>();
        public static IServiceCollection AddMetricMonitoringSources(this IServiceCollection services, IConfiguration configuration, string configPath = Consts.DEFAULT_CHANNEL_CONFIG_PATH) => services
            .AddMetricMonitoringSources(_ => _.DefaultChannel = configuration[configPath] ?? Consts.DEFAULT_CHANNEL);
        public static IServiceCollection AddMetricMonitoringSources(this IServiceCollection services, string defaultChannel = Consts.DEFAULT_CHANNEL) => services
            .AddMetricMonitoringSources(_ => _.DefaultChannel = defaultChannel);

        // Add the sinks
        public static IServiceCollection AddMetricMonitoringSinks(this IServiceCollection services, Action<SinkOptions<MetricMessage>> action) => services
            .AddMessaging()
            .AddMessagingProtobufSink(action)
            .AddTransient<IMetricSink, MetricSink>();

        public static IServiceCollection AddMetricMonitoringSinks(this IServiceCollection services, IConfiguration configuration, string configPath = Consts.DEFAULT_CHANNEL_CONFIG_PATH) => services
            .AddMetricMonitoringSinks(_ => _.DefaultChannel = configuration[configPath] ?? Consts.DEFAULT_CHANNEL);
       public static IServiceCollection AddMetricMonitoringSinks(this IServiceCollection services, string defaultChannel = Consts.DEFAULT_CHANNEL) => services
            .AddMetricMonitoringSinks(_ => _.DefaultChannel = defaultChannel);
    }
}
