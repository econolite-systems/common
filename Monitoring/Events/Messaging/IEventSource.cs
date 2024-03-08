// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Econolite.Ode.Monitoring.Events.Messaging
{
    public interface IEventSource
    {
        Task ConsumeOnAsync(Func<UserEvent, Task> consumeFunc, CancellationToken stoppingToken);
    }
}
