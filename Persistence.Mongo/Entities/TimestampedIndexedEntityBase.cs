// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Persistence.Mongo.Entities;

public class TimestampedIndexedEntityBase : ObjectIdIndexedEntityBase
{
    public DateTime Timestamp = DateTime.UtcNow;
}
