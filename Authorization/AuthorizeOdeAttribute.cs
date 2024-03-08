// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Microsoft.AspNetCore.Authorization;

namespace Econolite.Ode.Authorization;

/// <summary>
/// Authorize API requests using pre-canned known roles in the MoundRoadRole enum.
/// </summary>
public class AuthorizeOdeAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Used for high level actions, intended to be smallest group of users with all access. Assumed to provide access for Contributor and RealOnly checks also.
    /// </summary>
    public static readonly string ADMINISTRATOR = "Administrator";

    /// <summary>
    /// Read-write user, intended for technicians who can add to a system, but not remove.
    /// </summary>
    public static readonly string CONTRIBUTOR = "Contributor";

    /// <summary>
    /// Read-only user, intended for viewers, but does not give the ability to modify anything shared with other users.
    /// </summary>
    public static readonly string READ_ONLY = "ReadOnly";

    public AuthorizeOdeAttribute(MoundRoadRole role)
    {
        string result;
        switch (role)
        {
            case MoundRoadRole.Administrator:
                result = ADMINISTRATOR;
                break;
            case MoundRoadRole.Contributor:
                result = CONTRIBUTOR;
                break;
            case MoundRoadRole.ReadOnly:
                result = READ_ONLY;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(role), role, null);
        }

        Roles = result;
    }

    public AuthorizeOdeAttribute(params MoundRoadRole[] roles)
    {
        var result = new List<string>();

        if (roles?.Length > 0)
        {
            foreach (var role in roles)
            {
                switch (role)
                {
                    case MoundRoadRole.Administrator:
                        result.Add(ADMINISTRATOR);
                        break;
                    case MoundRoadRole.Contributor:
                        result.Add(CONTRIBUTOR);
                        break;
                    case MoundRoadRole.ReadOnly:
                        result.Add(READ_ONLY);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(role), role, null);
                }
            }
        }

        Roles = string.Join(",", result);
    }
}
