// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Interfaces;

namespace Econolite.Ode.Persistence.Common.Repository;

public interface IRepository<TDocument, in TId> : IRepository, IDisposable
    where TDocument : IIndexedEntity<TId>
{
    public void Add(TDocument document);
    public Task<TDocument?> GetByIdAsync(TId id);
    public TDocument? GetById(TId id);
    public Task<IEnumerable<TDocument>> GetAllAsync();
    public IEnumerable<TDocument> GetAll();
    public void Update(TDocument document);
    public void Remove(TId id);
}
