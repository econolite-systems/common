// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Messaging.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Econolite.Ode.Messaging;

public class Consumer<TKey, TValue> : IConsumer<TKey, TValue>
{
    private readonly Confluent.Kafka.IConsumer<byte[], byte[]> _consumer;
    private readonly IConsumeResultFactory<TKey, TValue> _consumeResultFactory;
    private readonly ILogger<Consumer<TKey, TValue>> _logger;

    public Consumer(IBuildMessagingConfig buildMessagingConfig, IConsumeResultFactory<TKey, TValue> consumeResultFactory,
        ILogger<Consumer<TKey, TValue>> logger, IOptions<ConsumerOptions<TKey, TValue>> options)
    {
        _consumeResultFactory = consumeResultFactory;
        _logger = logger;

        _consumer = new Confluent.Kafka.ConsumerBuilder<byte[], byte[]>(buildMessagingConfig.BuildConsumerClientConfig(options.Value))
            .AddLogging(_logger)
            .Build();
    }

    public void Subscribe(string topic)
    {
        _consumer.Subscribe(topic);
    }
    
    public void Subscribe(IEnumerable<string> topics)
    {
        _consumer.Subscribe(topics);
    }

    public void Complete(ConsumeResult consumeResult)
    {
        _consumer.StoreOffset(consumeResult.InternalConsumeResult);
    }

    public ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken)
    {
        return Consume(_ => true, cancellationToken);
    }

    public ConsumeResult<TKey, TValue> Consume(Func<string, bool> typeFilter, CancellationToken cancellationToken)
    {
        var done = false;
        ConsumeResult<TKey, TValue>? result = default;
        do
        {
            var consumeresult = _consumeResultFactory.BuildConsumeResult(_consumer.Consume(cancellationToken));
            if (typeFilter == null || typeFilter(consumeresult.Type))
            {
                result = consumeresult;
                done = true;
            }
            else
            {
                if(_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("Consumed/discarded unwanted type {@}", new { consumeresult.Type, Tenantid = consumeresult.TenantId });
            }
        } while (!done);
        return result ?? throw new NullReferenceException("Unable to retrieve the message");
    }
}
