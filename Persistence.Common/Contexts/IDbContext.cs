// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Persistence.Common.Contexts;

public interface IDbContext : IDisposable
{
    void AddCommand(Func<CancellationToken, Task> func);
    Task<(bool success, string? errors)> SaveChangesAsync(CancellationToken? cancellationToken = null);
}
