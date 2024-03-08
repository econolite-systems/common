// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace Econolite.Ode.Serialization.Kafka.Json
{
    public class Serializer<T> : ISerializer<T>
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        public byte[] Serialize(T data, SerializationContext context)
        {
            var str = JsonConvert.SerializeObject(data, _jsonSerializerSettings);
            return Encoding.UTF8.GetBytes(str);
        }
    }
}
