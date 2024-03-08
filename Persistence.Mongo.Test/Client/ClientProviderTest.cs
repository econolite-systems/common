// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using Econolite.Ode.Persistence.Mongo.Client;
using Econolite.Ode.Persistence.Mongo.Test.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Econolite.Ode.Persistence.Mongo.Test.Client;

[Collection(nameof(MongoCollection))]
public class ClientProviderTest
{
    private readonly ILogger<ClientProvider> _logger = Mock.Of<ILogger<ClientProvider>>();
    private readonly ILoggerFactory _loggerFactory = Mock.Of<ILoggerFactory>();
    private readonly IOptions<MongoConnectionStringOptions> _connectionStringOptions = Mock.Of<IOptions<MongoConnectionStringOptions>>();

    public ClientProviderTest()
    {
        Mock.Get(_loggerFactory)
            .Setup(x => x.CreateLogger(typeof(ClientProvider).FullName!))
            .Returns(_logger);
    }

    [Fact]
    public void CreateClientProvider_OptionsLogger_ClientDatabase()
    {
        Assert.Throws<ArgumentNullException>(
#pragma warning disable CS8625
            () => new ClientProvider(null, Mock.Get(_connectionStringOptions).Object, Mock.Get(_loggerFactory).Object)
#pragma warning restore CS8625
        );
    }
}
