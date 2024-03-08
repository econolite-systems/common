// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Contexts;

namespace Econolite.Ode.Persistence.Common.Repository;

public interface IRepository
{
    public IDbContext DbContext { get; }
}
