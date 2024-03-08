// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Confluent.Kafka;
using Econolite.Ode.Messaging.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Econolite.Ode.Messaging;

public class Producer<TKey, TValue> : IProducer<TKey, TValue>
{
    private readonly ILogger<Producer<TKey, TValue>> _logger;
    private readonly Confluent.Kafka.IProducer<byte[], byte[]> _producer;
    private readonly Elements.IHeaderFactory _headerFactory;

    public Producer(IBuildMessagingConfig buildMessagingConfig, Elements.IHeaderFactory headerFactory, ILogger<Producer<TKey, TValue>> logger, IOptions<ProducerOptions<TKey, TValue>> options)
    {
        _headerFactory = headerFactory;
        _logger = logger;

        _producer = new ProducerBuilder<byte[], byte[]>(buildMessagingConfig.BuildProducerClientConfig(options.Value))
            .AddLogging(_logger)
            .Build();
    }

    public async Task ProduceAsync(string topic, Elements.Message<TKey, TValue> message)
    {
        await _producer.ProduceAsync(topic, new Message<byte[], byte[]>
        {
            Headers = _headerFactory.CreateHeaders(message),
            Key = message.Key.ToSerialized(),
            Value = message.Payload.ToSerialized()
        });
    }
}
