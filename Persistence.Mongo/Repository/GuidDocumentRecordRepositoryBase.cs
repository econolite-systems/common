// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Records;
using Econolite.Ode.Persistence.Mongo.Context;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Persistence.Mongo.Repository;

public class GuidDocumentRecordRepositoryBase<TDocument> : DocumentRepositoryBase<TDocument, Guid>
    where TDocument : IndexedRecordBase<Guid>
{
    protected GuidDocumentRecordRepositoryBase(IMongoContext context, ILogger logger)
        : base(context, logger)
    {
    }
}
