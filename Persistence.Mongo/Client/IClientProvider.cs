// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using MongoDB.Driver;

namespace Econolite.Ode.Persistence.Mongo.Client;

public interface IClientProvider
{
    /// <summary>
    ///     Return the Mongo Client.
    /// </summary>
    IMongoDbClient? Client { get; }

    /// <summary>
    ///     Returns the Database as per the options.
    /// </summary>
    IMongoDatabase? Database { get; }
}
