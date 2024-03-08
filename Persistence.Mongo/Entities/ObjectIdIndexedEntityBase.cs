// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Diagnostics.CodeAnalysis;
using Econolite.Ode.Persistence.Common.Entities;
using MongoDB.Bson;

namespace Econolite.Ode.Persistence.Mongo.Entities;

[ExcludeFromCodeCoverage]
public abstract class ObjectIdIndexedEntityBase : IndexedEntityBase<ObjectId>
{
}
