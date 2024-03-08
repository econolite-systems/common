// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;

namespace Econolite.Ode.Monitoring.Events
{
    public class UserEventFactoryOptions
    {
        public Guid DefaultTenantId { get; set; } = Guid.Empty;
        public LogName DefaultLogName { get; set; } = LogName.SystemEvent;
        public string DefaultSource { get; set; } = "Unspecified";
        /// <summary>
        /// Note the UserEventFactory will us the Environment.MachineName if this is not set
        /// </summary>
        public string Computer { get; set; } = "";
        public Category DefaultCategory { get; set; } = Category.User;
    }
}
