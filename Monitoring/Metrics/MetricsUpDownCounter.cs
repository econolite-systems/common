// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Diagnostics.Metrics;
using System.Threading;

namespace Econolite.Ode.Monitoring.Metrics
{
    public class MetricsUpDownCounter : IMetricsUpDownCounter
    {
        private long _count;
#pragma warning disable IDE0052 // Remove unread private members
        private readonly ObservableUpDownCounter<long> _counter;
#pragma warning restore IDE0052 // Remove unread private members

        public MetricsUpDownCounter(Meter meter, string baseName, string units)
        {
            _counter = meter.CreateObservableUpDownCounter($"{baseName}", () => Interlocked.Read(ref _count), unit: units, description: $"{baseName} counter");
            BaseName = baseName;
        }
        public string BaseName { get; }

        public void Decrement() => Interlocked.Decrement(ref _count);
        public void Decrement(int count) => Interlocked.Add(ref _count, -count);
        public void Increment() => Interlocked.Increment(ref _count);
        public void Increment(int count) => Interlocked.Add(ref _count, count);
    }
}
