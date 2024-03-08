// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging.Elements;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Econolite.Ode.Monitoring.Metrics.Messaging
{
    public interface IMetricSource
    {
        Task ConsumeOnAsync(Func<ConsumeResult<Guid, MetricMessage>, Task> consumeFunc, CancellationToken stoppingToken);
    }
}
