// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using Econolite.Ode.Monitoring.Metrics.Messaging.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Econolite.Ode.Monitoring.Metrics.Extensions
{
    public static class Defined
    {
        static public IServiceCollection AddMetrics(this IServiceCollection service, IConfiguration configuration, string source) => service
            .AddMetrics<LocalMonitor>(configuration, source);
        static public IServiceCollection AddMetrics(this IServiceCollection service, IConfiguration configuration, string source, Guid tenantId) =>service
            .AddMetrics<LocalMonitor>(configuration, source, tenantId);
        static public IServiceCollection AddMetrics(this IServiceCollection service, IConfiguration configuration, Action<MetricsFactoryOptions> factoryOptions, string source) => service
            .AddMetrics<LocalMonitor>(configuration, factoryOptions, source);
        static public IServiceCollection AddMetrics(this IServiceCollection services, IConfiguration configuration, Action<MetricsFactoryOptions> factoryOptions, Action<MetricsMonitorOptions> monitorOptions) => services
            .AddMetrics<LocalMonitor>(configuration, factoryOptions, monitorOptions);

        static public IServiceCollection AddMetrics<TLocalMonitor>(this IServiceCollection service, IConfiguration configuration, string source)
            where TLocalMonitor : class, ILocalMonitor => service
                .AddMetrics<TLocalMonitor>(configuration, _ => { }, _ => _.Source = source);
        static public IServiceCollection AddMetrics<TLocalMonitor>(this IServiceCollection service, IConfiguration configuration, string source, Guid tenantId)
            where TLocalMonitor : class, ILocalMonitor => service
                .AddMetrics<TLocalMonitor>(configuration, _ => { }, _ => { _.Source = source; _.TenantId = tenantId; });
        static public IServiceCollection AddMetrics<TLocalMonitor>(this IServiceCollection service, IConfiguration configuration, Action<MetricsFactoryOptions> factoryOptions, string source)
            where TLocalMonitor : class, ILocalMonitor => service
                .AddMetrics<TLocalMonitor>(configuration, factoryOptions, _ => _.Source = source);

        static public IServiceCollection AddMetrics<TLocalMonitor>(this IServiceCollection services, IConfiguration configuration, Action<MetricsFactoryOptions> factoryOptions, Action<MetricsMonitorOptions> monitorOptions)
            where TLocalMonitor : class, ILocalMonitor => services
                .Configure(factoryOptions)
                .Configure(monitorOptions)
                .AddSingleton<IMetricsFactory, MetricsFactory>()
                .AddMetricMonitoringSinks(configuration)
                .AddSingleton<ILocalMonitor, TLocalMonitor>()
                .AddHostedService<MetricsMonitor>();
    }
}
