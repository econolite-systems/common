// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;

namespace Econolite.Ode.Messaging.Elements;

public class ConsumeResult
{
    public ConsumeResult(Confluent.Kafka.ConsumeResult<byte[], byte[]> consumeResult)
    {
        InternalConsumeResult = consumeResult;
    }

    internal Confluent.Kafka.ConsumeResult<byte[], byte[]> InternalConsumeResult { get; }
}

public class ConsumeResult<TKey, TValue>: ConsumeResult
{
    private readonly Func<byte[], TKey> _funcBuildKey;
    private readonly IPayloadSpecialist<TValue> _payloadSpecialist;

    public ConsumeResult(Guid tenantId, string type, Confluent.Kafka.ConsumeResult<byte[], byte[]> consumeResult,
        Func<byte[], TKey> funcBuildKey, IPayloadSpecialist<TValue> payloadSpecialist)
        : this(tenantId, null, type, consumeResult, funcBuildKey, payloadSpecialist)
    {
    }

    public ConsumeResult(Guid tenantId, Guid? deviceId, string type,
        Confluent.Kafka.ConsumeResult<byte[], byte[]> consumeResult, Func<byte[], TKey> funcBuildKey,
        IPayloadSpecialist<TValue> payloadSpecialist) : base(consumeResult)
    {
        TenantId = tenantId;
        Type = type;
        _funcBuildKey = funcBuildKey;
        _payloadSpecialist = payloadSpecialist;
        DeviceId = deviceId;
    }

    public Guid? DeviceId { get; }
    public Guid TenantId { get; }
    public string Type { get; }

    public TKey Key => _funcBuildKey(InternalConsumeResult.Message.Key);
    public TValue Value =>_payloadSpecialist.To<TValue>(InternalConsumeResult);

    public Derived ToObject<Derived>() => _payloadSpecialist.To<Derived>(InternalConsumeResult);
}
