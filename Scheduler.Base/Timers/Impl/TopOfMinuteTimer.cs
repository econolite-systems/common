// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Common.Scheduler.Base.Timers.Impl
{
    public class TopOfMinuteTimer : GeneralTimer
    {
        public TopOfMinuteTimer() : base("0 * * * * *")
        {
        }
    }
}
