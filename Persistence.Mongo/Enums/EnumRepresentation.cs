// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Persistence.Mongo.Enums;

public enum EnumRepresentation
{
    /// <summary>
    ///     Represents enums in a mongo database as the integer equivalent
    /// </summary>
    Numeric,

    /// <summary>
    ///     Represents enums as their names
    /// </summary>
    String
}
