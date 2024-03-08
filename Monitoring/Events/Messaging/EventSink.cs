// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using Econolite.Ode.Monitoring.Events.Messaging.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Econolite.Ode.Monitoring.Events.Messaging
{
    public class EventSink : IEventSink
    {
        private readonly ISink<UserEventMessage> _sink;
        private readonly UserEventSinkOptions _options;
        public EventSink(ISink<UserEventMessage> sink, IOptions<UserEventSinkOptions> options)
        {
            _options = options?.Value ?? new();
            _sink = sink;
        }

        public Task SinkAsync(UserEvent userEvent, CancellationToken cancellationToken) => _sink.SinkAsync(_options.DefaultTenantId, userEvent.ToProtobuf(), cancellationToken);
        public Task SinkAsync(Guid tenantId, UserEvent userEvent, CancellationToken cancellationToken) => _sink.SinkAsync(tenantId, userEvent.ToProtobuf(), cancellationToken);
    }
}
