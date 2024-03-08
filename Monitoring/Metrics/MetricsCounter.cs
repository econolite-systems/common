// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Diagnostics.Metrics;
using System.Threading;

namespace Econolite.Ode.Monitoring.Metrics
{
    public class MetricsCounter : IMetricsCounter
    {
        private long _count;
#pragma warning disable IDE0052 // Remove unread private members
        private readonly ObservableCounter<long> _counter;
        private readonly ObservableGauge<long> _gauge;
#pragma warning restore IDE0052 // Remove unread private members

        public MetricsCounter(Meter meter, string baseName, string units)
        {
            _counter = meter.CreateObservableCounter($"{baseName}-rate", () => Interlocked.Read(ref _count), unit: units, description: $"{baseName} rate");
            _gauge = meter.CreateObservableGauge($"{baseName}-total", () => Interlocked.Read(ref _count), unit: units, description: $"{baseName} total");
            BaseName = baseName;
        }

        public string BaseName { get; }
        public void Increment() => Interlocked.Increment(ref _count);
        public void Increment(int count) => Interlocked.Add(ref _count, count);
    }
}
