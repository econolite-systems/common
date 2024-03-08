// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Econolite.Ode.Persistence.Mongo.Test.Helpers;

public static class FilterDefinitionExtensions
{
    public static string RenderToJson<TDocument>(this FilterDefinition<TDocument> filter)
    {
        var serializerRegistry = BsonSerializer.SerializerRegistry;
        var documentSerializer = serializerRegistry.GetSerializer<TDocument>();
        return filter.Render(documentSerializer, serializerRegistry).ToJson();
    }

    public static string RenderToJson<TDocument>(this UpdateDefinition<TDocument> update)
    {
        var serializerRegistry = BsonSerializer.SerializerRegistry;
        var documentSerializer = serializerRegistry.GetSerializer<TDocument>();
        return update.Render(documentSerializer, serializerRegistry).ToJson();
    }
}
