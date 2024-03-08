// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text;

namespace Econolite.Ode.Messaging.Elements;

public class BaseJsonPayload<TValue> : BaseMessagePayload<TValue> where TValue : notnull
{
    private readonly TValue _element;

    public BaseJsonPayload(TValue element)
    {
        _element = element;
    }

    public override byte[] ToSerialized()
    {
        return Encoding.UTF8.GetBytes(JSonPayload.AsJson(_element));
    }

    protected override string GetMessageType() => _element.GetType().Name;
}
