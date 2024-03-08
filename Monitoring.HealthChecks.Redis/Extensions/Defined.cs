// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using HealthChecks.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Econolite.Ode.Monitoring.HealthChecks.Redis.Extensions;

public static class Defined
{
    private const string Name = "Redis";
    private static readonly string[] Tags = { "storage", "cache", "redis" };

    public static IHealthChecksBuilder AddRedisHealthCheck(
        this IHealthChecksBuilder builder,
        string connectionString,
        string? name = default,
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default,
        TimeSpan? timeout = default)
    {
        var factory = new RedisHealthCheck(connectionString);

        builder.Add((new HealthCheckRegistration(
            name ?? Name,
            sp => factory,
            failureStatus,
            tags ?? Tags,
            timeout)));
        return builder;
    }
}
