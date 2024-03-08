// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using MongoDB.Driver;

namespace Econolite.Ode.Persistence.Mongo.Client;

public interface IMongoDbClient : IMongoClient
{
    bool CanSupportTransactions { get; }
}
