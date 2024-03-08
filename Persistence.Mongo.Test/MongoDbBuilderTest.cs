// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using Econolite.Ode.Persistence.Common.Entities;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Econolite.Ode.Persistence.Mongo.Test;

[Collection("Mongo collection")]
public class MongoDbBuilderTest
{
    private readonly IOptions<MongoOptions> _options = Mock.Of<IOptions<MongoOptions>>();

    [Fact]
    public void CreateBuilder_WithMongoOptions_InstanceOfBuilder()
    {
        Mock.Get(_options)
            .Setup(x => x.Value)
            .Returns(new MongoOptions())
            .Verifiable();

        var result = new MongoDbBuilder(Mock.Get(_options).Object, false);

        result.Should().NotBeNull();

        Mock.Verify(Mock.Get(_options));
    }

    [Fact]
    public void AddMapping_WithMongoOptions_InstanceOfBuilder()
    {
        Mock.Get(_options)
            .Setup(x => x.Value)
            .Returns(new MongoOptions())
            .Verifiable();

        var result = new MongoDbBuilder(Mock.Get(_options).Object, false);

        result.AddMappings<TestGuidDocument>(map => map.AutoMap());

        Mock.Verify(Mock.Get(_options));
    }
}

public class TestGuidDocument : IndexedEntityBase<Guid>
{
}
