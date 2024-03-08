// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Contexts;
using MongoDB.Driver;

namespace Econolite.Ode.Persistence.Mongo.Context;

public interface IMongoContext : IDbContext
{
    IMongoCollection<TDocument> GetCollection<TDocument>(string name);
}
