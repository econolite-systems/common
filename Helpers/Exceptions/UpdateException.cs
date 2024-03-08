// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Helpers.Exceptions;

public sealed class UpdateException : Exception
{
    public UpdateException(string message) : base(message)
    {
    }
}
