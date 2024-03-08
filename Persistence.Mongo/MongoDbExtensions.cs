// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common;
using Econolite.Ode.Persistence.Mongo.Client;
using Econolite.Ode.Persistence.Mongo.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Econolite.Ode.Persistence.Mongo;

public static class MongoDbExtensions
{
    public static IMongoDbBuilder AddMongo(
        this IServiceCollection services,
        bool register = true)
    {
        services.RegisterCommonServices();
        services.ConfigureOptions<BuildMongoDbConfig>();
        services.ConfigureOptions<BuildMongoDbConnectionConfig>();
        services.AddSingleton<IClientProvider, ClientProvider>();
        services.AddTransient<IMongoContext, MongoContext>();

        var builder = services.BuildServiceProvider();

        return new MongoDbBuilder(builder.GetService<IOptions<MongoOptions>>()!, register);
    }
    
    public static IMongoDbBuilder AddMongoTest(this IServiceCollection services)
    {
        services.RegisterCommonServices();
        services.AddSingleton<IClientProvider, ClientProvider>();
        services.AddTransient<IMongoContext, MongoContext>();
        
        var builder = services.BuildServiceProvider();

        return new MongoDbBuilder(new MongoOptionsWrapper(), false);
    }
}
