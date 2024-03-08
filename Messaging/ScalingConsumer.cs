// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging.Elements;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Econolite.Ode.Messaging
{
    public class ScalingConsumerOptions : ConsumerOptions
    {
        public int MaxConcurrency = 1;
        public IEnumerable<KeyValuePair<string, string>> KafkaConfig = Array.Empty<KeyValuePair<string, string>>();
    }
    public class ScalingConsumerOptions<TKey, TValue> : ScalingConsumerOptions
    {
    }

    public class ScalingConsumer<TKey, TValue> : IScalingConsumer<TKey, TValue>
    {
        private readonly ScalingConsumerOptions<TKey, TValue> _options;
        private readonly IBuildMessagingConfig _buildMessagingConfig;
        private readonly IConsumeResultFactory<TKey, TValue> _consumeResultFactory;
        private readonly ILogger<Consumer<TKey, TValue>> _logger;

        public ScalingConsumer(IBuildMessagingConfig buildMessagingConfig, IOptions<ScalingConsumerOptions<TKey, TValue>> options, IConsumeResultFactory<TKey, TValue> consumeResultFactory, ILogger<Consumer<TKey, TValue>> logger)
        {
            _options = options.Value;
            _buildMessagingConfig = buildMessagingConfig;
            _consumeResultFactory = consumeResultFactory;
            _logger = logger;
        }

        public async Task ConsumeOn(string topic, Func<ConsumeResult<TKey, TValue>, Task> consumeFunc, CancellationToken cancellationToken)
        {
            var consumeroptions = new OptionsWrapper<ConsumerOptions<TKey, TValue>>(new ConsumerOptions<TKey, TValue>
            {
                ConfigOverrides = _options.ConfigOverrides,
                ConsumerGroupOverride = _options.ConsumerGroupOverride,
                ConsumerGroupSuffix = _options.ConsumerGroupSuffix
            });
            await Task.WhenAll(Enumerable.Range(0, _options.MaxConcurrency).Select(_ => Task.Run(async () =>
            {
                var consumer = new Consumer<TKey, TValue>(_buildMessagingConfig, _consumeResultFactory, _logger, consumeroptions);
                try
                {
                    _logger.LogInformation($"Starting consumer {_}");
                    var initialscribe = false;
                    while (true)
                    {
                        try
                        {
                            if (! initialscribe)
                            {
                                consumer.Subscribe(topic);
                                initialscribe = true;
                            }
                            var consumeresult = consumer.Consume(cancellationToken);
                            await consumeFunc(consumeresult);
                            consumer.Complete(consumeresult);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Processing Error {_}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation($"Consumer stopping {_}");
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, $"Consumer processing loop terminated! {_}");
                }
            })));
        }
    }
}
