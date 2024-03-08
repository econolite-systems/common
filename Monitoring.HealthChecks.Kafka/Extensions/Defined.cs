// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Econolite.Ode.Monitoring.HealthChecks.Kafka.Extensions;

public static class Defined
{
    private const string Name = "Kafka";
    private const string Topic = "health-checks";
    private static readonly string[] Tags = new[] {"messaging", "kafka"};

    public static IHealthChecksBuilder AddKafkaHealthCheck(
        this IHealthChecksBuilder builder,
        string? topic = default,
        string? name = default,
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default,
        TimeSpan? timeout = default)
    {
        var serviceBuilder = builder.Services.BuildServiceProvider();
        
        var factory = new KafkaHealthCheckWithOptions(
            serviceBuilder.GetService<IBuildMessagingConfig>() ?? throw new ArgumentNullException(nameof(BuildMessagingConfig)), topic ?? Topic);
        
        builder.Add((new HealthCheckRegistration(
            name ?? Name,
            sp => factory,
            failureStatus,
            tags ?? Tags,
            timeout)));
        return builder;
    }
}
