// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Diagnostics.CodeAnalysis;
using Econolite.Ode.Persistence.Common.Records;
using MongoDB.Bson;

namespace Econolite.Ode.Persistence.Mongo.Records;

[ExcludeFromCodeCoverage]
public abstract record ObjectIdIndexedRecordBase(ObjectId Id) : IndexedRecordBase<ObjectId>(Id);
