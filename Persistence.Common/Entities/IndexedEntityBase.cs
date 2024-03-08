// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Interfaces;

namespace Econolite.Ode.Persistence.Common.Entities;

public abstract class IndexedEntityBase<T> : IIndexedEntity<T>
{
    public virtual T? Id { get; set; }
}
