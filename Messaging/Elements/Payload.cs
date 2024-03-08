// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Econolite.Ode.Messaging.Elements
{
    public class Payload
    {
        // Included for deserializion
        protected Payload() : this(DateTime.UtcNow)
        {
        }

        public Payload(DateTime timeStamp)
        {
            TimeStamp = timeStamp;
        }

        // Note without the private set; these may not deserialize depending on the implementation of the deserialization
        // used.
        public DateTime TimeStamp { get; }
    }
}
