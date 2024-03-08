// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Common.Scheduler.Base.Timers;
using Microsoft.Extensions.DependencyInjection;

namespace Econolite.Ode.Common.Scheduler.Base.Extensions
{
    static public class Defined
    {
        static public IServiceCollection AddTimerFactory(this IServiceCollection services) =>
            services.AddTransient<IPeriodicTimerFactory, PeriodicTimerFactory>();
    }
}
