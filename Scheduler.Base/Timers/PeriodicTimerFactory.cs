// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Common.Scheduler.Base.Timers.Impl;

namespace Econolite.Ode.Common.Scheduler.Base.Timers
{
    public class PeriodicTimerFactory : IPeriodicTimerFactory
    {
        public IPeriodicTimer CreatePeriodicTimer(string expression) => new GeneralTimer(expression);
        public IPeriodicTimer CreateTopOfMinuteTimer() => new TopOfMinuteTimer();
    }
}
