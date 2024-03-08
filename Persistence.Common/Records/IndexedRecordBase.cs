// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Interfaces;

namespace Econolite.Ode.Persistence.Common.Records;

public abstract record IndexedRecordBase<T>(T Id) : IIndexedEntity<T>;
