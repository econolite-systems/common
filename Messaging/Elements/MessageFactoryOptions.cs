// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;

namespace Econolite.Ode.Messaging.Elements
{
    public class MessageFactoryOptions<TKey, TValue> where TKey : notnull
    {
        public Guid TenantId { get; set; } = Guid.Empty;
        public Func<TKey, BaseMessageElement<TKey>> FuncBuildKeyElement { get; set; } = _ => new ToStringElement<TKey>(_);
        public Func<TValue, BaseMessagePayload<TValue>> FuncBuildPayloadElement { get; set; } = _ => new NullMessagePayload<TValue>();
        public Func<TKey, TValue, Guid> FuncDeviceId  { get; set; } = (key, value) => Guid.Empty;
    }

    public class MessageFactoryOptions<TValue> : MessageFactoryOptions<Guid, TValue>
    { }
}
