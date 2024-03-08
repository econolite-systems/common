// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;

namespace Messaging.Admin
{
    public interface IPartitionAwareConsumer<TKey, TValue> : IConsumer<TKey, TValue>
    {
        void Subscribe(string topic, int partitionId);
        void Commit(List<Confluent.Kafka.TopicPartitionOffset> partitionOffsets);
        Confluent.Kafka.IConsumer<string, string> Consumer { get; }
    }
}
