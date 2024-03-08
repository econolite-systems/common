// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Common.Scheduler.Base.Timers
{
    public interface IPeriodicTimer
    {
        void Start(Func<Task> task);
        Task StopAsync();
    }
}
