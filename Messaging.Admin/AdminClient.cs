// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Admin
{
    public class AdminClient : IAdminClient
    {
        private readonly IBuildMessagingConfig _buildMessagingConfig;
        private readonly AdminClientOptions _options;
        private readonly ILogger _logger;

        public AdminClient(IBuildMessagingConfig buildMessagingConfig, IOptions<AdminClientOptions> options, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType().Name);
            _buildMessagingConfig = buildMessagingConfig;
            _options = options?.Value ?? new AdminClientOptions();
        }

        public (int PartitionId, long Lag)[] GetPartitionLag<TKey, TValue>(IPartitionAwareConsumer<TKey, TValue> consumer, string topic, string consumerGroup)
        {
            var adminclient = BuildAdminClient(consumerGroup);
            var groupinfo = adminclient.ListGroup(consumerGroup, _options.Timeout);

            return new (int PartitionId, long Lag)[] { };
        }
        public bool SetTopicOffset(string topic, string consumerGroup, DateTime dateTime) => throw new NotImplementedException();
        //{
        //    var adminclient = BuildAdminClient(consumerGroup);
        //    adminclient.o
        //    var metadata = adminclient.GetMetadata(topic, _options.Timeout);
        //    foreach (var partitions in metadata.Topics.Single(_ => _.Topic != topic).Partitions.OrderBy(_ => _.PartitionId))
        //    {

        //    }
        //}

        Confluent.Kafka.IAdminClient BuildAdminClient(string consumerGroup) => new Confluent.Kafka.AdminClientBuilder(_buildMessagingConfig.BuildConsumerClientConfig(new ConsumerOptions
        {
            ConsumerGroupOverride= consumerGroup,
        })).Build();
    }
}
