// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace Econolite.Ode.Serialization.Kafka.Json
{
    public class Deserializer<T> : IDeserializer<T>
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data.ToArray()), _jsonSerializerSettings);
        }
    }
}
