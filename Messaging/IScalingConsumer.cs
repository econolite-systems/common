// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Econolite.Ode.Messaging
{
    public interface IScalingConsumer<TKey, TValue>
    {
        Task ConsumeOn(string topic, Func<Elements.ConsumeResult<TKey, TValue>, Task> consumeFunc, CancellationToken stoppingToken);
    }
}
