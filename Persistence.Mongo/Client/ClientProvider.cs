// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Econolite.Ode.Persistence.Mongo.Client;

[ExcludeFromCodeCoverage]
public class ClientProvider : IClientProvider
{
    private readonly ILogger _logger;
    private readonly IOptions<MongoOptions> _options;
    private readonly IOptions<MongoConnectionStringOptions> _connectionStringOptions;
    private Lazy<IMongoDbClient>? _client;
    private Lazy<IMongoDatabase>? _database;

    public ClientProvider(
        IOptions<MongoOptions> options,
        IOptions<MongoConnectionStringOptions> connectionStringOptions,
        ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options;
        _connectionStringOptions = connectionStringOptions;
        _logger = loggerFactory.CreateLogger<ClientProvider>();

        ConfigureConnection();
    }

    public IMongoDbClient Client => _client!.Value;

    public IMongoDatabase Database => _database!.Value;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool isDisposing)
    {
        if (isDisposing) Client?.Cluster.Dispose();
    }

    private void ConfigureConnection()
    {
        _client = new Lazy<IMongoDbClient>(InitialiseMongoClient);

        _database = new Lazy<IMongoDatabase>(() =>
        {
            try
            {
                _logger.LogInformation("Getting Mongo database");
                return Client.GetDatabase(_options.Value.DbName);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get mongo database");
                throw;
            }
        });
    }

    private IMongoDbClient InitialiseMongoClient()
    {
        try
        {
            _logger.LogInformation("Initiating Mongo database connection");
            var settings = MongoClientSettings.FromConnectionString(_connectionStringOptions.Value.Mongo);
            settings.AllowInsecureTls = true;
            return new MongoDbClient(settings);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create mongo client and/or database");
            throw;
        }
    }
}
