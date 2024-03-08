// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Confluent.Kafka;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Messaging.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Econolite.Ode.Messaging
{
    public class GenericSink<TKey, TValue> : ISink<TKey, TValue> where TKey : notnull
    {
        private readonly ILogger _logger;
        private readonly IMessageFactory<TKey, TValue> _messageFactory;
        private readonly IHeaderFactory _headerFactory;
        private readonly Confluent.Kafka.IProducer<byte[], byte[]> _producer;
        private readonly SinkOptions<TKey, TValue> _options;

        public GenericSink(IBuildMessagingConfig buildMessagingConfig, IHeaderFactory headerFactory, IMessageFactory<TKey, TValue> messageFactory, IOptions<SinkOptions<TKey, TValue>> options, ILoggerFactory loggerFactory)
        {
            _options = options?.Value ?? new SinkOptions<TKey, TValue>();
            _headerFactory = headerFactory;
            _messageFactory = messageFactory;
            _logger = loggerFactory.CreateLogger($"{GetType().Name}<{typeof(TValue).Name}>");

            _producer = new ProducerBuilder<byte[], byte[]>(buildMessagingConfig.BuildProducerClientConfig(_options))
                .AddLogging(_logger)
                .Build();
        }

        public async Task SinkAsync(TKey key, TValue value, CancellationToken cancellationToken) =>
            await SinkInternalAsync(_options.DefaultChannel, _messageFactory.Build(key, value), cancellationToken);

        public async Task SinkAsync((string Channel, Guid TenantId) options, TKey key, TValue value, CancellationToken cancellationToken) =>
            await SinkInternalAsync(!string.IsNullOrEmpty(options.Channel) ? options.Channel : _options.DefaultChannel, _messageFactory.Build(options.TenantId, key, value), cancellationToken);

        private async Task SinkInternalAsync(string channel, Elements.Message<TKey, TValue> message, CancellationToken cancellationToken)
        {
            await _producer.ProduceAsync(channel, new Confluent.Kafka.Message<byte[], byte[]>
            {
                Headers = _headerFactory.CreateHeaders(message),
                Key = message.Key.ToSerialized(),
                Value = message.Payload.ToSerialized()
            }, cancellationToken);
        }
    }
}
