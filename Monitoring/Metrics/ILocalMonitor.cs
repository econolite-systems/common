// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Econolite.Ode.Monitoring.Metrics
{
    public interface ILocalMonitor
    {
        Task OnMonitoringAsync(KeyValuePair<Instrument, long>[] observations, CancellationToken cancellationToken);
    }

    public class LocalMonitor : ILocalMonitor
    {
        private readonly ILogger _logger;

        public LocalMonitor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType().Name);
        }

        public Task OnMonitoringAsync(KeyValuePair<Instrument, long>[] observations, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Monitoring data: {@}", new { Metrics = observations.Select(_ => new { _.Key.Name, _.Value }) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to log monitoring data");
            }

            return Task.CompletedTask;
        }
    }
}
