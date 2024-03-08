// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;

namespace Messaging.Admin
{
    public interface IAdminClient
    {
        bool SetTopicOffset(string topic, string consumerGroup, DateTime dateTime);
        (int PartitionId, long Lag)[] GetPartitionLag<TKey, TValue>(IPartitionAwareConsumer<TKey, TValue> consumer, string topic, string consumerGroup);
    }
}
