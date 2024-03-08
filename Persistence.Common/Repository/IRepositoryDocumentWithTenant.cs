// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Interfaces;

namespace Econolite.Ode.Persistence.Common.Repository;

public interface IRepository<TDocument, in TId, in TTenant> : IRepository, IDisposable
    where TDocument : IIndexedEntityWithTenant<TId, TTenant>
{
    public void Add(TDocument document);
    public Task<TDocument?> GetByIdAsync(TId id);
    public TDocument? GetById(TId id);
    public Task<IEnumerable<TDocument>> GetAllAsync(TTenant tenant);
    public IEnumerable<TDocument> GetAll(TTenant tenant);
    public void Update(TDocument document);
    public void Remove(TId id);
}
