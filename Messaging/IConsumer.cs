// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging.Elements;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Econolite.Ode.Messaging;

public interface IConsumer<TKey, TValue>
{
    void Subscribe(string topic);
    void Subscribe(IEnumerable<string> topics);
    ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken);
    ConsumeResult<TKey, TValue> Consume(Func<string, bool> typeFilter, CancellationToken cancellationToken);
    void Complete(ConsumeResult consumeResult);
}
