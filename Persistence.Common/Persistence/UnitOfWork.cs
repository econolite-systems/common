// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Repository;

namespace Econolite.Ode.Persistence.Common.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly IRepository _repository;

    public UnitOfWork(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<RepositoryResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var (success, errors) = await _repository.DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return success ? RepositoryResult.SuccessResult() : RepositoryResult.FailureResult(errors);
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (disposing) _repository.DbContext?.Dispose();
    }
}
