// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

namespace Econolite.Ode.Monitoring.Metrics
{
    public class MetricsMonitorOptions
    {
        public TimeSpan MeasurementPeriod { get; set; } = TimeSpan.FromMinutes(1);
        public string Source { get; set; } = "Unspecified";
        public Guid TenantId { get; set; } = Guid.Empty;
    }
}
