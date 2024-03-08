// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Records;
using Econolite.Ode.Persistence.Mongo.Context;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Persistence.Mongo.Repository;

public class StringDocumentRecordRepositoryBase<TDocument> : DocumentRepositoryBase<TDocument, string>
    where TDocument : IndexedRecordBase<string>
{
    protected StringDocumentRecordRepositoryBase(IMongoContext context, ILogger logger)
        : base(context, logger)
    {
    }
}
