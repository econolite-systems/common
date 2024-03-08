// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Xml.Linq;

namespace Econolite.Ode.Messaging.Elements;

public abstract class BaseMessagePayload<TValue> : BaseMessageElement<TValue>
{
    public string MessageType => GetMessageType();

    abstract protected string GetMessageType();
}
