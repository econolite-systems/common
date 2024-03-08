// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using ProtoBuf;
using System.IO;

namespace Econolite.Ode.Messaging.Elements
{
    public class BaseProtobufPayload<TValue> : BaseMessagePayload<TValue>
    {
        private readonly TValue _element;

        public BaseProtobufPayload(TValue element)
        {
            _element = element;
        }

        public override byte[] ToSerialized()
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, _element);
            return stream.ToArray();
        }

        protected override string GetMessageType() => typeof(TValue).Name;
    }
}
