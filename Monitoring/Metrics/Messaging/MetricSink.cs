// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Econolite.Ode.Monitoring.Metrics.Messaging
{
    public class MetricSink : IMetricSink
    {
        private readonly ISink<MetricMessage> _sink;
        public MetricSink(ISink<MetricMessage> sink) 
        {
            _sink = sink;
        }

        public async Task SinkAsync(Guid hashKey, MetricMessage metricMessage, CancellationToken cancellationToken) => await _sink.SinkAsync(hashKey, metricMessage, cancellationToken);
    }
}
