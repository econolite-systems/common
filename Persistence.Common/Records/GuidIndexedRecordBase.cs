// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Persistence.Common.Records;

public abstract record GuidIndexedRecordBase(Guid Id) : IndexedRecordBase<Guid>(Id);
