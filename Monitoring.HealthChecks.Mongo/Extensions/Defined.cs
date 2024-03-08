// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Mongo;
using Econolite.Ode.Persistence.Mongo.Health;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Econolite.Ode.Monitoring.HealthChecks.Mongo.Extensions;

public static class Defined
{
    private const string Name = "Mongo Db";
    private static readonly string[] Tags = new[] {"storage", "db", "mongo"};
    
    public static IHealthChecksBuilder AddMongoDbHealthCheck(
        this IHealthChecksBuilder builder,
        string? name = default,
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default,
        TimeSpan? timeout = default)
    {
        var serviceBuilder = builder.Services.BuildServiceProvider();
        
        var factory = new MongoDbHealthCheckWithOptions(
            serviceBuilder.GetService<IOptions<MongoConnectionStringOptions>>() ??
            new OptionsWrapper<MongoConnectionStringOptions>(new()),
            serviceBuilder.GetService<IOptions<MongoOptions>>() ??
            new OptionsWrapper<MongoOptions>(new()));
        
        builder.Add((new HealthCheckRegistration(
            name ?? Name,
            sp => factory,
            failureStatus,
            tags ?? Tags,
            timeout)));
        return builder;
    }
}
