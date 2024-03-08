// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Microsoft.Extensions.Options;
using System;

namespace Econolite.Ode.Messaging.Elements;

public interface IMessageFactory<TKey, TValue> where TKey : notnull
{
    Message<TKey, TValue> Build(TKey key, TValue payload);
    Message<TKey, TValue> Build(Func<TKey, TValue, Guid> funcDevice, TKey key, TValue payload);
    Message<TKey, TValue> Build(Guid tenantId, TKey key, TValue payload);
    Message<TKey, TValue> Build(Guid tenantId, Guid deviceId, TKey key, TValue payload);
}

public class MessageFactory<TKey, TValue> : IMessageFactory<TKey, TValue> where TKey : notnull
{
    private readonly MessageFactoryOptions<TKey, TValue> _options;

    public MessageFactory(IOptions<MessageFactoryOptions<TKey, TValue>> options)
    {
        _options = options?.Value ?? new MessageFactoryOptions<TKey, TValue>();
    }

    public Message<TKey, TValue> Build(TKey key, TValue payload)
    {
        return Build(_options.TenantId, key, payload);
    }

    public Message<TKey, TValue> Build(Guid tenantId, TKey key, TValue payload)
    {
        return BuildInternal(tenantId, _options.FuncDeviceId, key, payload);
    }

    public Message<TKey, TValue> Build(Func<TKey, TValue, Guid> funcDevice, TKey key, TValue payload)
    {
        return BuildInternal(_options.TenantId, funcDevice, key, payload);
    }

    public Message<TKey, TValue> Build(Guid tenantId, Guid deviceId, TKey key, TValue payload)
    {
        return BuildInternal(tenantId, (_, _) => deviceId, key, payload);
    }

    private Message<TKey, TValue> BuildInternal(Guid tenantId, Func<TKey, TValue, Guid> funcDevice, TKey key, TValue payload)
    {
        var id = funcDevice(key, payload);
        return id != Guid.Empty
            ? new Message<TKey, TValue>(_options.FuncBuildKeyElement(key), _options.FuncBuildPayloadElement(payload))
            {
                DeviceId = id,
                TenantId = tenantId
            }
            : new Message<TKey, TValue>(_options.FuncBuildKeyElement(key), _options.FuncBuildPayloadElement(payload))
            {
                TenantId = tenantId
            };
    }
}

public interface IMessageFactory<TValue> : IMessageFactory<Guid, TValue>
{
}

public class MessageFactory<TValue> : MessageFactory<Guid, TValue>, IMessageFactory<TValue>
{
    public MessageFactory(IOptions<MessageFactoryOptions<TValue>> options) : base(options)
    {

    }
}
