// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;

namespace Econolite.Ode.Messaging.Elements
{
    public class DevicePayload : Payload
    {
        // Included for deserializion
        protected DevicePayload() 
        { }
        public DevicePayload(Guid deviceId) : this(deviceId, string.Empty)
        { }
        public DevicePayload(Guid deviceId, string externalTag)
        {
            ExternalTag = externalTag;
            DeviceId = deviceId;
        }

        public DevicePayload(Guid deviceId, DateTime timeStamp) : base(timeStamp)
        {
            ExternalTag = string.Empty;
            DeviceId = deviceId;
        }

        public DevicePayload(Guid deviceId, DateTime timeStamp, string externalTag) : base(timeStamp)
        {
            ExternalTag = externalTag;
            DeviceId = deviceId;
        }

        // Note without the private set; these may not deserialize depending on the implementation of the deserialization
        // used.
        public string ExternalTag { get; private set; } = string.Empty;
        public Guid DeviceId { get; private set; } = Guid.Empty;
    }
}
