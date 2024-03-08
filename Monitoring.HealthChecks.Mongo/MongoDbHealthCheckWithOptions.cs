// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using HealthChecks.MongoDb;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Econolite.Ode.Persistence.Mongo.Health;

public class MongoDbHealthCheckWithOptions : MongoDbHealthCheck
{
    public MongoDbHealthCheckWithOptions(IOptions<MongoConnectionStringOptions> connectionStringOptions, IOptions<MongoOptions> options) : base(connectionStringOptions.Value.Mongo, options.Value.DbName)
    {
    }
}
