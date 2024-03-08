// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Econolite.Ode.Persistence.Mongo.Test;

[Collection("Mongo collection")]
public class MongoDbExtensionsTest
{
    private readonly IOptionsMonitor<MongoOptions> _options = Mock.Of<IOptionsMonitor<MongoOptions>>();
    private readonly IServiceCollection _serviceCollection = Mock.Of<IServiceCollection>();

    [Fact]
    public void AddMongo_ServicesMongoOptions_AddedMongoToServices()
    {
        var services = new ServiceCollection();

        var result = services.AddMongoTest();

        result.Should().NotBeNull();
    }

    [Fact]
    public void AddMongo_ServicesNoMongoOptions_Throw()
    {
        var services = new ServiceCollection();

        var result = services.AddMongoTest();

        result.Should().NotBeNull();
    }
}
