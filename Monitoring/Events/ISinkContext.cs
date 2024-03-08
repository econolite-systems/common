// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Monitoring.Events.Messaging;

namespace Econolite.Ode.Monitoring.Events
{
    public interface ISinkContext
    {
        IEventSink EventSink { get; set; }
    }
}
