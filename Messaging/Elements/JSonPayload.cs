// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Econolite.Ode.Messaging.Elements
{
    public static class JSonPayload
    {
        public static TValue ToObject<TValue>(string payload)
        {
            return JsonSerializer.Deserialize<TValue>(payload, JsonPayloadSerializerOptions.Options) ?? throw new NullReferenceException("Unable to deserialize to object");
        }

        public static JsonNode ToJsonNode(string payload)
        {
            return JsonSerializer.Deserialize<JsonNode>(payload, JsonPayloadSerializerOptions.Options) ?? throw new NullReferenceException("payload cannot be processed");
        }

        public static string AsJson(object element)
        {
            return JsonSerializer.Serialize(element, JsonPayloadSerializerOptions.Options) ?? throw new NullReferenceException("payload cannot be processed");
        }
    }
}
