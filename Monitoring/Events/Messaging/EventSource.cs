// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using Econolite.Ode.Monitoring.Events.Messaging.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Econolite.Ode.Monitoring.Events.Messaging
{
    public class EventSource : IEventSource
    {
        private readonly ISource<UserEventMessage> _source;
        private readonly ISinkContext _sinkContext;
        private readonly ILogger _logger;
        public EventSource(ISource<UserEventMessage> source, ISinkContext sinkContext, ILoggerFactory loggerFactory)
        {
            _source = source;
            _sinkContext = sinkContext;
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public Task ConsumeOnAsync(Func<UserEvent, Task> consumeFunc, CancellationToken stoppingToken) => 
            _source.ConsumeOnAsync(async (consumed) =>
            {
                if (consumed.Type == typeof(UserEventMessage).Name)
                {
                    var data = consumed.ToObject<UserEventMessage>().FromProtobuf(_sinkContext);
                    await consumeFunc(data);
                }
                else
                {
                    _logger.LogDebug("Consumed non ConfigRequest type {@}", consumed);
                }
            }, stoppingToken);
    }
}
