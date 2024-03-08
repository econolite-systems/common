// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Econolite.Ode.Monitoring.Events.Messaging
{
    public interface IEventSink
    {
        Task SinkAsync(UserEvent userEvent, CancellationToken cancellationToken);
        Task SinkAsync(Guid tenantId, UserEvent userEvent, CancellationToken cancellationToken);
    }
}
