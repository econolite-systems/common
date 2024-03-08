// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Persistence.Common.Interfaces;

public interface IIndexedEntity<out TId>
{
    TId? Id { get; }
}
