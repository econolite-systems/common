// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Econolite.Ode.Messaging
{
    public interface ISink<TKey, TValue>
    {
        Task SinkAsync(TKey key, TValue value, CancellationToken cancellationToken);
        Task SinkAsync((string Channel, Guid TenantId) options, TKey key, TValue value, CancellationToken cancellationToken);
    }

    public interface ISink<TValue> : ISink<Guid, TValue>
    {
    }
}
