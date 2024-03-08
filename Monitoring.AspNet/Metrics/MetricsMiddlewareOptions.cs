// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Monitoring.AspNet.Metrics;

public class MetricsMiddlewareOptions
{
    public string RequestCounter { get; set; } = null!;
    public string ResponseCounter { get; set; } = null!;
}
