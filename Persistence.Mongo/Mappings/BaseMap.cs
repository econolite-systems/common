// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Entities;
using Econolite.Ode.Persistence.Common.Records;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Econolite.Ode.Persistence.Mongo.Mappings;

public static class BaseMap
{
    public static void MapGuidIndexedEntity()
    {
        BsonClassMap.RegisterClassMap<IndexedEntityBase<Guid>>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);
            map.MapIdMember(x => x.Id);
        });
    }

    public static void MapObjectIdIndexedEntity()
    {
        BsonClassMap.RegisterClassMap<IndexedEntityBase<ObjectId>>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);
            map.MapIdMember(x => x.Id);
        });
    }

    public static void MapGuidIndexedRecord()
    {
        BsonClassMap.RegisterClassMap<IndexedRecordBase<Guid>>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);
            map.MapIdMember(x => x.Id);
        });
    }

    public static void MapObjectIdIndexedRecord()
    {
        BsonClassMap.RegisterClassMap<IndexedRecordBase<ObjectId>>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);
            map.MapIdMember(x => x.Id);
        });
    }
}
