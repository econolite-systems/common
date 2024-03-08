// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Mongo.Enums;
using Econolite.Ode.Persistence.Mongo.Mappings;
using Econolite.Ode.Persistence.Mongo.Serializers;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace Econolite.Ode.Persistence.Mongo;

public interface IMongoDbBuilder
{
}

public class MongoDbBuilder : IMongoDbBuilder
{
    public MongoDbBuilder(IOptions<MongoOptions> options, bool register = true)
    {
        RegisterMongo(options, register);
    }

    private void RegisterMongo(IOptions<MongoOptions> options, bool register)
    {
        var pack = new ConventionPack
        {
            new IgnoreExtraElementsConvention(options.Value.IgnoreExtraElementsConvention),
            new IgnoreIfDefaultConvention(options.Value.IgnoreIfDefaultConvention),
            new EnumRepresentationConvention(options.Value.EnumRepresentation == EnumRepresentation.String
                ? BsonType.String
                : BsonType.Int32)
        };

        if (options.Value.UseCamelCaseConvention) pack.Add(new CamelCaseElementNameConvention());

        ConventionRegistry.Register("MongoDb Conventions", pack, t => true);
        if (register)
        {
            // This is currently needed to set the GuidRepresentation to Standard.
#pragma warning disable CS0618
            BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
#pragma warning restore CS0618
            BsonSerializer.RegisterSerializer(new GuidSerializer(options.Value.GuidRepresentation));
            BsonSerializer.RegisterSerializer(new JsonDocumentBsonSerializer());

            BaseMap.MapGuidIndexedEntity();
            BaseMap.MapGuidIndexedRecord();
            BaseMap.MapObjectIdIndexedEntity();
            BaseMap.MapObjectIdIndexedRecord();
        }
    }
}

public static class MongoDbBuilderExtensions
{
    public static IMongoDbBuilder AddMappings<T>(this IMongoDbBuilder builder,
        params Action<BsonClassMap<T>>[] mappings)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);

        BsonClassMap.RegisterClassMap<T>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);
            foreach (var mapping in mappings) mapping.Invoke(map);
        });

        return builder;
    }
}
