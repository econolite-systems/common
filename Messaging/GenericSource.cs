// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Confluent.Kafka;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Messaging.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Econolite.Ode.Messaging
{
    public class GenericSource<TKey, TValue> : ISource<TKey, TValue>
    {
        private readonly ILogger _logger;
        private readonly SourceOptions<TKey, TValue> _options;
        private readonly IBuildMessagingConfig _buildMessagingConfig;
        private readonly IConsumeResultFactory<TKey, TValue> _consumeResultFactory;
        private readonly SemaphoreSlim _lock = new(1);

        public GenericSource(IBuildMessagingConfig buildMessagingConfig, IConsumeResultFactory<TKey, TValue> consumeResultFactory, IOptions<SourceOptions<TKey, TValue>> options, ILoggerFactory loggerFactory)
        {
            _buildMessagingConfig = buildMessagingConfig;
            _consumeResultFactory = consumeResultFactory;
            _options = options?.Value ?? new SourceOptions<TKey, TValue>();
            _logger = loggerFactory.CreateLogger($"{GetType().Name}<{typeof(TValue).Name}>");
        }

        public Task ConsumeOnAsync(Func<Elements.ConsumeResult<TKey, TValue>, Task> consumeFunc, CancellationToken stoppingToken) => ConsumeOnAsync(_options.DefaultChannel, _options.DefaultTypeFilter, consumeFunc, stoppingToken);
        public Task ConsumeOnAsync(string channel, Func<Elements.ConsumeResult<TKey, TValue>, Task> consumeFunc, CancellationToken stoppingToken) => ConsumeOnAsync(channel, _options.DefaultTypeFilter, consumeFunc, stoppingToken);

        public async Task ConsumeOnAsync(string channel, Func<string, bool> typeFilter, Func<Elements.ConsumeResult<TKey, TValue>, Task> consumeFunc, CancellationToken stoppingToken)
        {
            if (await _lock.WaitAsync(0, stoppingToken))
            {
                try
                {
                    // If one fails they all fail!
                    await Task.WhenAny(Enumerable.Range(0, _options.MaxConcurrency).Select(_ => Task.Run(async () =>
                    {
                        try
                        {
                            var consumer = new ConsumerBuilder<byte[], byte[]>(_buildMessagingConfig.BuildConsumerClientConfig(_options))
                                .AddLogging(_logger)
                                .Build();
                            consumer.Subscribe(channel);
                            do
                            {
                                var consumed = _consumeResultFactory.BuildConsumeResult(consumer.Consume(stoppingToken));
                                if (typeFilter == null || typeFilter(consumed.Type))
                                {
                                    await consumeFunc(consumed);
                                    consumer.StoreOffset(consumed.InternalConsumeResult);
                                }
                                else
                                {
                                    if (_logger.IsEnabled(LogLevel.Debug))
                                        _logger.LogDebug("Consumed/discarded unwanted type {@}", new { consumed.Type, Tenantid = consumed.TenantId });
                                }

                                // Note this is throwing if the consuming operation has been requested to stop
                                // just to ensure the same code path for stoppingToken being set
                                stoppingToken.ThrowIfCancellationRequested();
                            } while (true);
                        }
                        catch (OperationCanceledException)
                        {
                            // suppress
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "");
                        }
                    })));
                }
                finally
                {
                    _lock.Release();
                }
            }
            else
            {
                throw new InvalidOperationException("ConsumeOn loop is already running");
            }
        }
    }
}
