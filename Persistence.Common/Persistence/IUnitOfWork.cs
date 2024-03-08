// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Persistence.Common.Persistence;

public interface IUnitOfWork : IDisposable
{
    Task<RepositoryResult> SaveChangesAsync(CancellationToken cancellationToken = default);
}
