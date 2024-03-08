// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Net.Http.Headers;

namespace Econolite.Ode.Extensions.Service;

public interface IServiceAuthentication
{
    Task<AuthenticationHeaderValue> GetAuthHeaderAsync(bool refresh = false);
    Task<string> GetTokenAsync(bool refresh = false);
}
