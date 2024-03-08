// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Econolite.Ode.Persistence.Common;

public static class ServiceCollectionExtensions
{
    public static void RegisterCommonServices(this IServiceCollection services)
    {
        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
    }
}
