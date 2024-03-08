// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Econolite.Ode.Monitoring.Metrics.Messaging
{
    public class MetricSource : IMetricSource
    {
        private readonly ISource<MetricMessage> _source;
        public MetricSource(ISource<MetricMessage> source)
        {
            _source = source;
        }
        public async Task ConsumeOnAsync(Func<ConsumeResult<Guid, MetricMessage>, Task> consumeFunc, CancellationToken stoppingToken) => await _source.ConsumeOnAsync(consumeFunc, stoppingToken);
    }
}
