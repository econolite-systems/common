// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Common.Scheduler.Base.Timers
{
    public interface IPeriodicTimerFactory
    {
        /// <summary>
        /// The expression for the determining when the timer will execute
        /// is a fairly standard cron expression much like the cron tab
        /// facilities of *nix OSs.
        /// Note that is assumed the second will be included in the expression
        /// and will be of the form:
        ///                                                Allowed values    Allowed special characters   Comment
        ///
        ///         +------------- second                  0-59              * , - /                      
        ///         ¦ +------------- minute                0-59              * , - /                      
        ///         ¦ ¦ +------------- hour                0-23              * , - /                      
        ///         ¦ ¦ ¦ +------------- day of month      1-31              * , - / L W ?                
        ///         ¦ ¦ ¦ ¦ +------------- month           1-12 or JAN-DEC   * , - /                      
        ///         ¦ ¦ ¦ ¦ ¦ +------------- day of week   0-6  or SUN-SAT   * , - / # L ?                Both 0 and 7 means SUN
        ///         ¦ ¦ ¦ ¦ ¦ ¦
        ///         * * * * * *
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IPeriodicTimer CreatePeriodicTimer(string expression);

        /// <summary>
        /// Creates a specialized timer that fires at the top of the minute
        /// </summary>
        /// <returns></returns>
        IPeriodicTimer CreateTopOfMinuteTimer();
    }
}
