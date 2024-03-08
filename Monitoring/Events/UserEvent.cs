// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;

namespace Econolite.Ode.Monitoring.Events
{
    public class UserEvent
    {
        internal UserEvent(ISinkContext sinkContext)
        {
            SinkContext = sinkContext;
        }

        public LogName LogName { get; set; }

        public string Source { get; set; } = "";

        public Guid TenantId { get; set; } = Guid.Empty;

        public EventLevel Level { get; set; }

        public DateTime? Logged { get; set; }

        public Category Category { get; set; }

        public string Computer { get; set; } = "";

        public string Details { get; set; } = "";

        internal ISinkContext SinkContext { get; set; }
    }

    public enum LogName
    {
        LogNameUnknown = 0,
        SystemEvent = 1,
        Audit = 2,
    }

    public enum EventLevel
    {
        EventLevelUnknown = 0,
        Debug = 1,
        Information = 2,
        Warning = 3,
        Error = 4,
        Critical = 5,
    }

    public enum Category
    {
        CategoryUnknown = 0,
        Server = 1,
        Operational = 2,
        User = 3,
    }
}
