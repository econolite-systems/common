// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using ProtoBuf;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace Econolite.Ode.Messaging.Elements
{
    public interface IPayloadSpecialist<T>
    {
        Derived To<Derived>(Confluent.Kafka.ConsumeResult<byte[], byte[]> consumeResult);
    }

    public class JsonPayloadSpecialist<T> : IPayloadSpecialist<T>
    {
        public Derived To<Derived>(Confluent.Kafka.ConsumeResult<byte[], byte[]> consumeResult) => JSonPayload.ToObject<Derived>(Encoding.UTF8.GetString(consumeResult.Message.Value));
    }

    public class ProtobufPayloadSpecialist<T> : IPayloadSpecialist<T>
    {
        public Derived To<Derived>(Confluent.Kafka.ConsumeResult<byte[], byte[]> consumeResult)
        {
            using var stream = new MemoryStream(consumeResult.Message.Value?? Array.Empty<byte>());
            return Serializer.Deserialize<Derived>(stream);
        }
    }

    public class CombinedPayloadSpecialist<T> : IPayloadSpecialist<T>
    {
        ConcurrentDictionary<Type, IPayloadSpecialist<T>> _payloadSpecialists = new();
        private readonly JsonPayloadSpecialist<T> _jsonPayloadSpecialist;
        private readonly ProtobufPayloadSpecialist<T> _protobufPayloadSpecialist;

        public CombinedPayloadSpecialist(JsonPayloadSpecialist<T> jsonPayloadSpecialist, ProtobufPayloadSpecialist<T> protobufPayloadSpecialist)
        {
            _protobufPayloadSpecialist = protobufPayloadSpecialist;
            _jsonPayloadSpecialist = jsonPayloadSpecialist;
        }

        public Derived To<Derived>(Confluent.Kafka.ConsumeResult<byte[], byte[]> consumeResult)
        {
            if (_payloadSpecialists.TryGetValue(typeof(Derived), out var specialist))
            {
                return specialist.To<Derived>(consumeResult);
            }
            else
            {
                try
                {
                    Derived result = _jsonPayloadSpecialist.To<Derived>(consumeResult);
                    _payloadSpecialists.AddOrUpdate(typeof(Derived), (_) => _jsonPayloadSpecialist, (_, _) => _jsonPayloadSpecialist);
                    return result;
                }
                catch (Exception)
                {
                    // Suppress
                }
                _payloadSpecialists.AddOrUpdate(typeof(Derived), (_) => _protobufPayloadSpecialist, (_, _) => _jsonPayloadSpecialist);
                return _protobufPayloadSpecialist.To<Derived>(consumeResult);
            }
        }
    }
}
