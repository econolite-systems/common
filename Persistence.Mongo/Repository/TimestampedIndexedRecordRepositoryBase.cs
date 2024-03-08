// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Mongo.Context;
using Econolite.Ode.Persistence.Mongo.Records;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Econolite.Ode.Persistence.Mongo.Repository
{
    public class TimestampedIndexedRecordRepositoryBase<TDocument> : DocumentRepositoryBase<TDocument, ObjectId>
        where TDocument : TimestampedIndexedRecordBase
    {
        protected TimestampedIndexedRecordRepositoryBase(IMongoContext context, ILogger logger) : base(context, logger)
        {
        }
    }
}
