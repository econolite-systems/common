// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Monitoring.Metrics
{
    public interface IMetricsCounter
    {
        public string BaseName { get; }
        void Increment();
        void Increment(int count);
    }
}
