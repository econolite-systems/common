// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Monitoring.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Monitoring.AspNet.Metrics;

public class MetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMetricsCounter _requestCounter;
    private readonly IMetricsCounter _responseCounter;

    public MetricsMiddleware(RequestDelegate next, IMetricsFactory metricsFactory, IOptions<MetricsMiddlewareOptions> metricsOptions)
    {
        _next = next;
        _requestCounter = metricsFactory.GetMetricsCounter(metricsOptions.Value.RequestCounter ?? throw new NullReferenceException("RequestCounter not set in services.ConfigureRequestMetrics"));
        _responseCounter = metricsFactory.GetMetricsCounter(metricsOptions.Value.ResponseCounter ?? throw new NullReferenceException("ResponseCounter not set in services.ConfigureRequestMetrics"));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // If need to filter counter(s) to specific endpoints can check the paths off of context.Request
        // If a scoped dependency is needed, inject via the InvokeAsync arguments instead of the constructor
        _requestCounter.Increment();

        await _next(context);
        
        _responseCounter.Increment();
    }
}

public static class RequestCultureMiddlewareExtensions
{
    /// <summary>
    /// Configuration required for MetricsMiddleware, CounterName needs to be set.
    /// </summary>
    public static IServiceCollection ConfigureRequestMetrics(this IServiceCollection services, Action<MetricsMiddlewareOptions> options) => services.Configure(options);

    /// <summary>
    /// Adds a metrics counter to count all API calls made and flag them under the CounterName configured in services.ConfigureRequestMetrics
    /// </summary>
    public static IApplicationBuilder AddRequestMetrics(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MetricsMiddleware>();
    }
}
