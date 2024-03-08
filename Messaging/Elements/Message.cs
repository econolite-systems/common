// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;

namespace Econolite.Ode.Messaging.Elements;

public class Message<TKey, TValue>
{
    public Message(BaseMessageElement<TKey> key, BaseMessagePayload<TValue> payload)
    {
        Key= key;
        Payload = payload;
    }
    public Guid TenantId { get; set; }
    public Guid? DeviceId { get; set; }

    public BaseMessageElement<TKey> Key { get; set; }
    public BaseMessagePayload<TValue> Payload { get; set; }
}
