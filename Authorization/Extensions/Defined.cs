// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Microsoft.Extensions.DependencyInjection;

namespace Econolite.Ode.Authorization.Extensions;

public static class Defined
{
    /// <summary>
    ///     Add ITokenHandler to services for injection in places that need to convert Authentication:ClientId and
    ///     Authentication:ClientSecret for a jwt token to authenticate an HTTP client.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddTokenHandler(this IServiceCollection services,
        Action<TokenHandlerOptions> options)
    {
        services.AddHttpClient<TokenHandler>("tokenHandler");
        services.Configure(options);
        return services.AddSingleton<ITokenHandler, TokenHandler>();
    }
}
