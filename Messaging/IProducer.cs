// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Threading.Tasks;
using Econolite.Ode.Messaging.Elements;

namespace Econolite.Ode.Messaging;

public interface IProducer<TKey, TValue>
{
    Task ProduceAsync(string topic, Message<TKey, TValue> message);
}
