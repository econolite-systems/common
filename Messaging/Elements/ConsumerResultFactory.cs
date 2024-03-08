// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Text;

namespace Econolite.Ode.Messaging.Elements;

public interface IConsumeResultFactory<TKey, TValue>
{
    ConsumeResult<TKey, TValue> BuildConsumeResult(Confluent.Kafka.ConsumeResult<byte[], byte[]> consumeResult);
}
public interface IConsumeResultFactory<TValue> : IConsumeResultFactory<Guid, TValue>
{ 
}

public class ConsumeResultFactory<TKey, TValue> : IConsumeResultFactory<TKey, TValue>
{
    private readonly Func<byte[], TKey> _funcBuildKey;
    private readonly IPayloadSpecialist<TValue> _payloadSpecialist;

    public ConsumeResultFactory(Func<byte[], TKey> funcBuildKey, IPayloadSpecialist<TValue> payloadSpecialist)
    {
        _funcBuildKey = funcBuildKey;
        _payloadSpecialist = payloadSpecialist;
    }

    public ConsumeResult<TKey, TValue> BuildConsumeResult(Confluent.Kafka.ConsumeResult<byte[], byte[]> consumeResult)
    {
        var tenantid = Guid.Empty;
        if (consumeResult.Message.Headers.TryGetLastBytes(Consts.TENANT_ID_HEADER, out var buffer))
            Guid.TryParse(Encoding.UTF8.GetString(buffer), out tenantid);

        var type = Consts.TYPE_UNSPECIFIED;
        if (consumeResult.Message.Headers.TryGetLastBytes(Consts.TYPE_HEADER, out buffer))
            type = Encoding.UTF8.GetString(buffer);

        Guid? deviceid = default;
        if (consumeResult.Message.Headers.TryGetLastBytes(Consts.DEVICE_ID_HEADER, out buffer))
            if (Guid.TryParse(Encoding.UTF8.GetString(buffer), out var deviceidguid))
                deviceid = deviceidguid;
        return new ConsumeResult<TKey, TValue>(tenantid, deviceid, type, consumeResult, _funcBuildKey,
            _payloadSpecialist);
    }
}

public class ConsumeResultFactory<TValue> : ConsumeResultFactory<Guid, TValue>, IConsumeResultFactory<TValue>
{
    public ConsumeResultFactory(IPayloadSpecialist<TValue> payloadSpecialist) : base (_ => Guid.Parse(Encoding.UTF8.GetString(_)), payloadSpecialist)
    {

    }
}
