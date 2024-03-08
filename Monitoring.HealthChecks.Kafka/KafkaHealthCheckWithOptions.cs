// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using HealthChecks.Kafka;
using Microsoft.Extensions.Options;

namespace Econolite.Ode.Monitoring.HealthChecks.Kafka;

public class KafkaHealthCheckWithOptions : KafkaHealthCheck
{
    public KafkaHealthCheckWithOptions(IBuildMessagingConfig options, string topic) : base(options.BuildProducerClientConfig(new ProducerOptions()), topic)
    {
    }
}
