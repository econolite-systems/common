// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Repository;

namespace Econolite.Ode.Persistence.Common.Persistence;

public interface IUnitOfWorkFactory
{
    IUnitOfWork CreateUnitOfWork(IRepository repository);
}
