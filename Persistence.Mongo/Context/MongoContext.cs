// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Mongo.Client;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Econolite.Ode.Persistence.Mongo.Context;

public sealed class MongoContext : IMongoContext
{
    private readonly IClientProvider _clientProvider;
    private readonly List<Func<CancellationToken, Task<(bool success, string error)>>> _commands;
    private readonly ILogger _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private IClientSessionHandle? _session;

    public MongoContext(
        IClientProvider clientProvider,
        ILoggerFactory loggerFactory)
    {
        _clientProvider = clientProvider;
        _logger = loggerFactory.CreateLogger(typeof(MongoContext).FullName!);

        _commands = new List<Func<CancellationToken, Task<(bool success, string error)>>>();
    }

    public async Task<(bool success, string? errors)> SaveChangesAsync(CancellationToken? cancellationToken = null)
    {
        (bool success, string errors) formattedResults;
        if (await _semaphore.WaitAsync(-1, cancellationToken ?? CancellationToken.None))
        {
            try
            {
                CheckMongo();

                if (_clientProvider.Client!.CanSupportTransactions)
                {
                    (bool success, string errors) results;

                    using (_session = await _clientProvider.Client!.StartSessionAsync())
                    {
                        _session.StartTransaction();

                        var commandTasks = _commands.Select(c => c(cancellationToken ?? CancellationToken.None));

                        results = FormatResults(await Task.WhenAll(commandTasks));

                        if (results.success)
                            await _session.CommitTransactionAsync();
                        else
                            await _session.AbortTransactionAsync();
                    }

                    return results;
                }

                formattedResults = FormatResults(await Task.WhenAll(_commands.Select(c => c(cancellationToken ?? CancellationToken.None))));

                _commands.Clear();
            }
            finally
            {
                _semaphore.Release();
            }
        }
        else
        {
            formattedResults = (false, "Unable to enter lock.");
        }

        return formattedResults;
    }

    public IMongoCollection<TDocument> GetCollection<TDocument>(string name)
    {
        CheckMongo();

        return _clientProvider.Database!.GetCollection<TDocument>(name);
    }

    public void AddCommand(Func<CancellationToken, Task> func)
    {
        if (_semaphore.Wait(-1))
        {
            try
            {
                _commands.Add(async (token) =>
                {
                    try
                    {
                        await func(token);
                        return (true, string.Empty);
                    }
                    catch (Exception e)
                    {
                        return (false, e.Message);
                    }
                });
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing) _session?.Dispose();
    }

    private void CheckMongo()
    {
        if (_clientProvider.Client == null)
        {
            _logger.LogError("Mongo check failed. Client is null");
            throw new InvalidOperationException("Mongo check failed. Client is null");
        }

        if (_clientProvider.Database == null)
        {
            _logger.LogError("Mongo check failed. Database is null");
            throw new InvalidOperationException("Mongo check failed. Database is null");
        }
    }

    private (bool success, string errors) FormatResults((bool success, string error)[] results)
    {
        var failures = results.Any(r => !r.success);
        var errorsList = results.Where(r => !string.IsNullOrEmpty(r.error)).Select(r => r.error);
        var errors = string.Join(", ", errorsList);

        return (!failures, errors);
    }
}
