// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using MongoDB.Bson;

namespace Econolite.Ode.Persistence.Mongo.Records;

public record TimestampedIndexedRecordBase(ObjectId Id) : ObjectIdIndexedRecordBase(Id)
{
    public DateTime Timestamp = DateTime.UtcNow;
}
