// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Mongo.Enums;
using Econolite.Ode.Persistence.Mongo.Test.Repository;
using FluentAssertions;
using MongoDB.Bson;
using Xunit;

namespace Econolite.Ode.Persistence.Mongo.Test;

[Collection(nameof(MongoCollection))]
public class MongoOptionsTest
{
    [Fact]
    public void CreateMongoOptions_SetEveryThing_GetEveryThing()
    {
        var target = new MongoOptions
        {
            DbName = "",
            GuidRepresentation = GuidRepresentation.Unspecified,
            IgnoreExtraElementsConvention = false,
            IgnoreIfDefaultConvention = false,
            EnumRepresentation = EnumRepresentation.Numeric,
            UseCamelCaseConvention = false
        };

        target.Should().BeEquivalentTo(target);
    }
    
    [Fact]
    public void CreateMongoOptions_SetDbName_GetDbName()
    {
        var target = new MongoOptions
        {
            DbName = "Test"
        };

        var result = target.DbName;

        result.Should().Match("Test");
    }
}
