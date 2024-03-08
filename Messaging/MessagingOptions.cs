// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;

namespace Econolite.Ode.Messaging
{
    public class MessagingOptions
    {
        public bool IncludeInternalDebug { get; set; } = false;
        public KeyValuePair<string, string>[] ConfigOverrides { get; set; } = Array.Empty<KeyValuePair<string, string>>();
    }
}
