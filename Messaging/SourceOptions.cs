// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;

namespace Econolite.Ode.Messaging
{
    public class SourceOptions : ConsumerOptions
    {
        public int MaxConcurrency = 1;
        public string DefaultChannel { get; set; } = "Default";
        public Func<string, bool> DefaultTypeFilter { get; set; } = _=> true;
    }

    public class SourceOptions<TKey, TValue> : SourceOptions
    { }

    public class SourceOptions<TValue> : SourceOptions
    { }
}
