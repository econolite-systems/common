// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Econolite.Ode.Messaging
{
    public interface ISource<TKey, TValue>
    {
        Task ConsumeOnAsync(Func<Elements.ConsumeResult<TKey, TValue>, Task> consumeFunc, CancellationToken stoppingToken);
        Task ConsumeOnAsync(string channel, Func<Elements.ConsumeResult<TKey, TValue>, Task> consumeFunc, CancellationToken stoppingToken);
        Task ConsumeOnAsync(string channel, Func<string, bool> typeFilter, Func<Elements.ConsumeResult<TKey, TValue>, Task> consumeFunc, CancellationToken stoppingToken);
    }

    public interface ISource<TValue> : ISource<Guid, TValue>
    {
    }
}
