// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;

namespace Econolite.Ode.Messaging.Elements
{
    public class NullMessagePayload<TValue> : BaseMessagePayload<TValue>
    {
        public override byte[] ToSerialized() => Array.Empty<byte>();
        protected override string GetMessageType() => typeof(NullMessage).GetType().Name;
    }
}
