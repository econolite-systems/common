// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Messaging
{
    public class ConsumerOptions : MessagingOptions
    {
        public string ConsumerGroupSuffix { get; set; } = string.Empty;
        public string ConsumerGroupOverride { get; set; } = string.Empty;
    }

    public class ConsumerOptions<TKey, TValue> : ConsumerOptions
    { }
}
