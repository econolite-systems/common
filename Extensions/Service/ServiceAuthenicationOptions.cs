// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Extensions.Service;

public class ServiceAuthenticationOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Tenant { get; set; } = string.Empty;
}
