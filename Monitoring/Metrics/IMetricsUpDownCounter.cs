// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Monitoring.Metrics
{
    public interface IMetricsUpDownCounter : IMetricsCounter
    {
        void Decrement();
        void Decrement(int count);
    }
}
