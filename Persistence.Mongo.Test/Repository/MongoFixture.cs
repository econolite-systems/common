// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Mongo.Client;
using Econolite.Ode.Persistence.Mongo.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Econolite.Ode.Persistence.Mongo.Test.Repository;

public class MongoFixture
{
    private readonly ILogger<ClientProvider> _clientProviderlogger = Mock.Of<ILogger<ClientProvider>>();
    private readonly ILogger<MongoContext> _mongoContextlogger = Mock.Of<ILogger<MongoContext>>();

    public MongoFixture()
    {
        var services = new ServiceCollection();
        services.AddMongoTest();

        Mock.Get(LoggerFactory)
            .Setup(x => x.CreateLogger(typeof(ClientProvider).FullName!))
            .Returns(_clientProviderlogger);

        Mock.Get(LoggerFactory)
            .Setup(x => x.CreateLogger(typeof(MongoContext).FullName!))
            .Returns(_mongoContextlogger);
    }

    public IMongoContext Context { get; } = Mock.Of<IMongoContext>();

    public ILoggerFactory LoggerFactory { get; } = Mock.Of<ILoggerFactory>();

    public IOptionsMonitor<MongoOptions> MongoOptions { get; } = Mock.Of<IOptionsMonitor<MongoOptions>>();
}

[CollectionDefinition("MongoCollection")]
public class MongoCollection : ICollectionFixture<MongoFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
