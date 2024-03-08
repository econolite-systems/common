// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Confluent.Kafka;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Admin
{
    public class PartitionAwareConsumer<TKey, TValue> : Consumer<TKey, TValue>, IPartitionAwareConsumer<TKey, TValue>
    {
        public PartitionAwareConsumer(IBuildMessagingConfig buildMessagingConfig, ConsumeResultFactory<TKey, TValue> consumeResultFactory, ILogger<Consumer<TKey, TValue>> logger, IOptions<ConsumerOptions<TKey, TValue>> options = null) : base(buildMessagingConfig, consumeResultFactory, logger, options)
        {
        }

        public Confluent.Kafka.IConsumer<string, string> Consumer => _consumer;
        public void Commit(List<TopicPartitionOffset> partitionOffsets) => _consumer.Commit(partitionOffsets);
        public void Subscribe(string topic, int partitionId) => _consumer.Assign(new TopicPartition(topic, new Partition(partitionId)));
    }
}
