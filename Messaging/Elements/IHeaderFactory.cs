// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Confluent.Kafka;

namespace Econolite.Ode.Messaging.Elements
{
    public interface IHeaderFactory
    {
        Headers CreateHeaders<TKey, TValue>(Message<TKey, TValue> message);
    }
}
