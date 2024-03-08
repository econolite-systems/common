// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Monitoring.Events.Messaging;

namespace Econolite.Ode.Monitoring.Events
{
    internal class SinkContext : ISinkContext
    {
        public SinkContext(IEventSink eventSink)
        {
            EventSink = eventSink;
        }
        public IEventSink EventSink { get; set; }
    }
}
