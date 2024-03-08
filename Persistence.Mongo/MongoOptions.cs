// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Mongo.Enums;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace Econolite.Ode.Persistence.Mongo;

public class MongoOptions
{
    public string DbName { get; set; } = default!;

    public GuidRepresentation GuidRepresentation { get; set; } = GuidRepresentation.Standard;

    public bool IgnoreExtraElementsConvention { get; set; } = true;

    public bool IgnoreIfDefaultConvention { get; set; } = false;

    public EnumRepresentation EnumRepresentation { get; set; } = EnumRepresentation.String;

    public bool UseCamelCaseConvention { get; set; } = true;
}

public class MongoConnectionStringOptions
{
    public string Mongo { get; set; } = default!;
}

public class MongoOptionsWrapper : IOptions<MongoOptions>
{
    public MongoOptions Value { get; } = new ();
}
