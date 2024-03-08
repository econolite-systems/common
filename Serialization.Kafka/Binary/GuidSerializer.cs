// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using Confluent.Kafka;

namespace Econolite.Ode.Serialization.Kafka.Binary
{
    public class GuidSerializer : ISerializer<Guid>, IDeserializer<Guid>
    {
        public Guid Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return Deserializers.Guid(data);
        }

        public byte[] Serialize(Guid data, SerializationContext context)
        {
            return Serializers.Guid(data);
        }
    }
}
