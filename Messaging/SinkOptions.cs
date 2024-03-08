// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Messaging
{
    public class SinkOptions : ProducerOptions
    {
        public string DefaultChannel { get; set; } = "Default";
    }

    public class SinkOptions<TKey, TValue> : SinkOptions
    { }

    public class SinkOptions<TValue> : SinkOptions
    { }
}
