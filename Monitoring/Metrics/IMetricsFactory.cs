// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Monitoring.Metrics
{
    public interface IMetricsFactory
    {
        IMetricsCounter GetMetricsCounter(string baseName, string units = "#");
        IMetricsUpDownCounter GetMetricsUpDownCounter(string baseName, string units = "#");
    }
}
