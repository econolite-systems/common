// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;

namespace Econolite.Ode.Persistence.Mongo.Client;

[ExcludeFromCodeCoverage]
public class MongoDbClient : MongoClient, IMongoDbClient
{
    public MongoDbClient(string dbConnection)
        : base(dbConnection)
    {
    }
    
    public MongoDbClient(MongoClientSettings setings)
        : base(setings)
    {
    }

    public bool CanSupportTransactions => Cluster.Settings.EndPoints.Count > 1;
}
