// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;

namespace Econolite.Ode.Cache.Extensions
{
    public static class Defined
    {
        public static IServiceCollection AddCaching(this IServiceCollection services, Action<CacheOptions> options)
        {
            CacheOptions cacheOptions = new CacheOptions();
            options(cacheOptions);

            services
                // The following allows for the IConnectionMultiplexer to be injected
                .AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(cacheOptions.ConnectionString))
                // The following allows for IDistributedCache to be injected
                .AddStackExchangeRedisCache(_ => _.Configuration = cacheOptions.ConnectionString);
            return services;
        }
    }
}
