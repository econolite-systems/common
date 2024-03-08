// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Interfaces;
using Econolite.Ode.Persistence.Mongo.Context;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Persistence.Mongo.Repository;

public class GuidDocumentRepositoryBase<TDocument> : DocumentRepositoryBase<TDocument, Guid>
    where TDocument : IIndexedEntity<Guid>
{
    protected GuidDocumentRepositoryBase(IMongoContext context, ILogger logger)
        : base(context, logger)
    {
    }
}
