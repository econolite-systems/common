// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Confluent.Kafka;

namespace Econolite.Ode.Messaging
{
    public interface IBuildMessagingConfig
    {
        ConsumerConfig BuildConsumerClientConfig(ConsumerOptions options);
        ProducerConfig BuildProducerClientConfig(ProducerOptions options);
    }

    public interface IBuildMessagingConfig<TKey, TValue> : IBuildMessagingConfig
    {
    }
}
