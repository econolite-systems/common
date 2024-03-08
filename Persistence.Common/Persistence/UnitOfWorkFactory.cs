// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Repository;

namespace Econolite.Ode.Persistence.Common.Persistence;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    public IUnitOfWork CreateUnitOfWork(IRepository repository)
    {
        return new UnitOfWork(repository);
    }
}
