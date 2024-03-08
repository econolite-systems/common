// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Persistence.Common.Interfaces;

public interface IIndexedEntityWithTenant<out TId, out TTenant> : IIndexedEntity<TId>
{
    TTenant? TenantId { get; }
}
