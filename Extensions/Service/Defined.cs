// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Microsoft.Extensions.DependencyInjection;

namespace Econolite.Ode.Extensions.Service;

public static class Defined
{
    public static IServiceCollection AddServiceAuthentication(this IServiceCollection services, Action<ServiceAuthenticationOptions> options)
    {
        services.AddHttpClient();
        services.Configure<ServiceAuthenticationOptions>(options);
        services.AddTransient<IServiceAuthentication, ServiceAuthentication>();
        return services;
    }
}
