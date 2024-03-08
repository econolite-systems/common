// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Confluent.Kafka;
using System.Text;

namespace Econolite.Ode.Messaging.Elements
{
    public class HeaderFactory : IHeaderFactory
    {
        public Headers CreateHeaders<TKey, TValue>(Message<TKey, TValue> message)
        {
            var result = new Headers
            {
                {Consts.TENANT_ID_HEADER, Encoding.UTF8.GetBytes(message.TenantId.ToString())},
                {Consts.TYPE_HEADER, Encoding.UTF8.GetBytes(message.Payload.MessageType ?? Consts.TYPE_UNSPECIFIED)}
            };
            if (message.DeviceId.HasValue)
                result.Add(Consts.DEVICE_ID_HEADER, Encoding.UTF8.GetBytes(message.DeviceId.Value.ToString()));
            return result;
        }
    }
}
