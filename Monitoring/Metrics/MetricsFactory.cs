// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.Metrics;

namespace Econolite.Ode.Monitoring.Metrics
{
    public class MetricsFactory : IMetricsFactory
    {
        private readonly ConcurrentDictionary<string, Lazy<IMetricsCounter>> _counters;
        private readonly MetricsFactoryOptions _options;
        private readonly Meter _meter;

        public MetricsFactory(IOptions<MetricsFactoryOptions> options)
        {
            _counters = new ConcurrentDictionary<string, Lazy<IMetricsCounter>>();
            _options = options.Value;
            _meter = new Meter(_options.MeterName, _options.MeterVersion);
        }

        public IMetricsCounter GetMetricsCounter(string baseName, string units = "#") => _counters.GetOrAdd(baseName, new Lazy<IMetricsCounter>(() => new MetricsCounter(_meter, baseName, units))).Value;
        public IMetricsUpDownCounter GetMetricsUpDownCounter(string baseName, string units) => (IMetricsUpDownCounter)_counters.GetOrAdd(baseName, new Lazy<IMetricsCounter>(() => new MetricsUpDownCounter(_meter, baseName, units))).Value;
    }
}
