// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Econolite.Ode.Monitoring.Metrics.Messaging
{
    public interface IMetricSink
    {
        Task SinkAsync(Guid hashKey, MetricMessage metricMessage, CancellationToken cancellationToken);
    }
}
