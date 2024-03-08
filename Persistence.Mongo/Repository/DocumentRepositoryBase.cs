// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Interfaces;
using Econolite.Ode.Persistence.Common.Repository;
using Econolite.Ode.Persistence.Mongo.Context;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Econolite.Ode.Persistence.Mongo.Repository;

public abstract class DocumentRepositoryBase<TDocument, TId> : RepositoryBase<TDocument, IMongoContext, TId>
    where TDocument : IIndexedEntity<TId>
{
    private readonly IMongoContext _context;
    protected readonly ILogger _logger;

    protected DocumentRepositoryBase(IMongoContext context, ILogger logger)
        : base(context)
    {
        _context = context;
        _logger = logger;
    }

    public override void Add(TDocument document)
    {
        ExecuteDbSetAction((ctx, collection) => ctx.AddCommand((cancellationToken) => collection.InsertOneAsync(document, null, cancellationToken)));
    }

    public override async Task AddAsync(TDocument document)
    {
        await GetDbSet().InsertOneAsync(document);
    }

    public override async Task<TDocument?> GetByIdAsync(TId id)
    {
        var result = await ExecuteDbSetFuncAsync(collection => collection.FindAsync(
                Builders<TDocument>.Filter.Eq("_id", id)))
            .ConfigureAwait(false);
        return result.SingleOrDefault();
    }

    public override async Task<IEnumerable<TDocument>> GetByIdsAsync(IEnumerable<TId> ids)
    {
        var result = await ExecuteDbSetFuncAsync(collection => collection.FindAsync(
                Builders<TDocument>.Filter.Where(x => ids.Contains(x.Id))))
            .ConfigureAwait(false);
        return result.ToList();
    }

    public override TDocument? GetById(TId id)
    {
        var result = ExecuteDbSetFunc(collection => collection.Find(x => x.Id!.Equals(id)));
        return result.SingleOrDefault();
    }

    public override async Task<IEnumerable<TDocument>> GetAllAsync()
    {
        var result = await ExecuteDbSetFuncAsync(collection => collection
                .FindAsync(Builders<TDocument>.Filter.Empty))
            .ConfigureAwait(false);
        return result.ToList();
    }

    public override IEnumerable<TDocument> GetAll()
    {
        var result = ExecuteDbSetFunc(collection => collection.Find(Builders<TDocument>.Filter.Empty));
        return result.ToList();
    }

    public override void Update(TDocument document)
    {
        ExecuteDbSetAction((ctx, collection) => ctx.AddCommand(
            (cancellationToken) => collection.ReplaceOneAsync(Builders<TDocument>.Filter.Eq("_id", document.Id), document, new ReplaceOptions(), cancellationToken))
        );
    }

    public override void Remove(TId id)
    {
        ExecuteDbSetAction((ctx, collection) => ctx.AddCommand(
            (cancellationToken) => collection.DeleteOneAsync(Builders<TDocument>.Filter.Eq("_id", id), cancellationToken)));
    }

    protected void AddCommandFunc(Func<IMongoCollection<TDocument>, Func<CancellationToken,Task>> func)
    {
        ExecuteDbSetAction((ctx, collection) => ctx.AddCommand(func(collection)));
    }

    protected void ExecuteDbSetAction(Action<IMongoContext, IMongoCollection<TDocument>> action)
    {
        var dbSet = GetDbSet();

        action.Invoke(_context, dbSet);
    }

    protected async Task<TResult> ExecuteDbSetFuncAsync<TResult>(Func<IMongoCollection<TDocument>, Task<TResult>> func)
    {
        var dbSet = GetDbSet();

        return await func(dbSet).ConfigureAwait(false);
    }

    protected TResult ExecuteDbSetFunc<TResult>(Func<IMongoCollection<TDocument>, TResult> func)
    {
        var dbSet = GetDbSet();

        return func(dbSet);
    }

    private IMongoCollection<TDocument> GetDbSet()
    {
        try
        {
            return _context.GetCollection<TDocument>(CollectionName);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get collection");

            throw;
        }
    }
}
