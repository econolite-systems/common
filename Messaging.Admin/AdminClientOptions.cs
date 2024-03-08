// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Messaging.Admin
{
    public class AdminClientOptions
    {
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);
    }
}
