// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Linq.Expressions;
using Econolite.Ode.Persistence.Common.Interfaces;
using Econolite.Ode.Persistence.Common.Repository;
using Econolite.Ode.Persistence.Mongo.Context;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Econolite.Ode.Persistence.Mongo.Repository;

public abstract class DocumentRepositoryWithTenantBase<TDocument, TId, TTenant> : RepositoryWithTenantBase<TDocument, IMongoContext, TId, TTenant>
    where TDocument : IIndexedEntityWithTenant<TId, TTenant>
{
    private readonly IMongoContext _context;
    protected readonly ILogger _logger;

    protected DocumentRepositoryWithTenantBase(IMongoContext context, ILogger logger)
        : base(context)
    {
        _context = context;
        _logger = logger;
    }

    public override void Add(TDocument document)
    {
        ExecuteDbSetAction((ctx, collection) => ctx.AddCommand((cancellationToken) => collection.InsertOneAsync(document, null, cancellationToken)));
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

    public override async Task<IEnumerable<TDocument>> GetAllAsync(TTenant tenant)
    {
        var result = await ExecuteDbSetFuncAsync(collection => collection
                .FindAsync(Builders<TDocument>.Filter.Eq(x => x.TenantId, tenant)))
            .ConfigureAwait(false);
        return result.ToList();
    }

    public override IEnumerable<TDocument> GetAll(TTenant tenant)
    {
        var result = ExecuteDbSetFunc(collection => collection.Find(Builders<TDocument>.Filter.Eq(x => x.TenantId, tenant)));
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

    protected void AddCommandFunc(Func<IMongoCollection<TDocument>, CancellationToken, Task> func)
    {
        ExecuteDbSetAction((ctx, collection) =>
            ctx.AddCommand((cancellationToken) => func(collection, cancellationToken)));
    }
    
    protected void AddCommandFunc(Func<IMongoCollection<TDocument>, Func<CancellationToken,Task>> func)
    {
        ExecuteDbSetAction((ctx, collection) => ctx.AddCommand(func(collection)));
    }

    protected TResult Query<TResult>(Func<IMongoCollection<TDocument>, TResult> func)
    {
        var dbSet = GetDbSet();

        return func(dbSet);
    }
    
    protected async Task<TDocument> QueryAsync(Func<TDocument, bool> filter, CancellationToken cancellationToken = default)
    {
        var dbSet = GetDbSet();
        var query = CreateQuery(filter, cancellationToken);
        return await query(dbSet).ConfigureAwait(false);
    }
    
    protected Func<IMongoCollection<TDocument>, Task<TDocument>> CreateQuery(Func<TDocument, bool> filter, CancellationToken cancellationToken = default)
    {
        Expression<Func<TDocument,bool>> filterExpression = x => filter(x);
        return collection => collection.Find(filterExpression).FirstOrDefaultAsync(cancellationToken);
    }
    
    protected async Task<List<TDocument>> QueryListAsync(Func<TDocument, bool> filter, CancellationToken cancellationToken = default)
    {
        var dbSet = GetDbSet();
        var query = CreateListQuery(filter, cancellationToken);
        return await query(dbSet).ConfigureAwait(false);
    }
    
    protected Func<IMongoCollection<TDocument>, Task<List<TDocument>>> CreateListQuery(Func<TDocument, bool> filter, CancellationToken cancellationToken = default)
    {
        Expression<Func<TDocument,bool>> filterExpression = x => filter(x);
        return collection => collection.Find(filterExpression).ToListAsync(cancellationToken);
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
