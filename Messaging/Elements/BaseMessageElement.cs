// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text;

namespace Econolite.Ode.Messaging.Elements;

public abstract class BaseMessageElement<TElement>
{
    public abstract byte[] ToSerialized();
}

public class ToStringElement<TElement> : BaseMessageElement<TElement> where TElement : notnull
{
    protected readonly TElement _element;

    public ToStringElement(TElement element)
    {
        _element = element;
    }

    public override byte[] ToSerialized()
    {
        return Encoding.UTF8.GetBytes(_element.ToString() ?? "");
    }
}
