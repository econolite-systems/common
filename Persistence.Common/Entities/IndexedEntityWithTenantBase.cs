// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Interfaces;

namespace Econolite.Ode.Persistence.Common.Entities;

public abstract class IndexedEntityWithTenantBase<TId, TTenant> : IndexedEntityBase<TId>, IIndexedEntityWithTenant<TId, TTenant>
{
    public virtual TTenant? TenantId { get; set; }
}
