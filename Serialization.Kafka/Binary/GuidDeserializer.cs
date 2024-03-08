// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using Confluent.Kafka;

namespace Econolite.Ode.Serialization.Kafka.Binary
{
    public class GuidDeserializer : IDeserializer<Guid>
    {
        public Guid Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return Deserializers.Guid(isNull ? null : data);
        }
    }
}
