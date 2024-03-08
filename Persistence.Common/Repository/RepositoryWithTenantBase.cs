// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Contexts;
using Econolite.Ode.Persistence.Common.Interfaces;

namespace Econolite.Ode.Persistence.Common.Repository;

public abstract class RepositoryWithTenantBase<TDocument, TContext, TId, TTenant> : IRepository<TDocument, TId, TTenant>
    where TContext : IDbContext
    where TDocument : IIndexedEntityWithTenant<TId, TTenant>
{
    private readonly TContext _context;
    private string _collectionName;

    protected RepositoryWithTenantBase(TContext context)
    {
        _context = context;
        _collectionName = typeof(TDocument).Name;
    }

    public virtual string CollectionName
    {
        get => _collectionName;
        protected set => _collectionName = value;
    }

    public IDbContext DbContext => _context;

    public abstract void Add(TDocument document);

    public abstract Task<TDocument?> GetByIdAsync(TId id);

    public abstract Task<IEnumerable<TDocument>> GetByIdsAsync(IEnumerable<TId> ids);

    public abstract TDocument? GetById(TId id);

    public abstract Task<IEnumerable<TDocument>> GetAllAsync(TTenant tenant);

    public abstract IEnumerable<TDocument> GetAll(TTenant tenant);

    public abstract void Update(TDocument document);

    public abstract void Remove(TId id);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing) _context?.Dispose();
    }
}
