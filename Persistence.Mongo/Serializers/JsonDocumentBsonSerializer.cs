// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Econolite.Ode.Persistence.Mongo.Serializers;

public class JsonDocumentBsonSerializer : SerializerBase<JsonDocument>
{
    public override JsonDocument Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var doc = BsonDocumentSerializer.Instance.Deserialize(context);
        return JsonDocument.Parse(doc.ToString());
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JsonDocument value)
    {
        var doc = BsonDocument.Parse(ToJsonString(value));
        BsonDocumentSerializer.Instance.Serialize(context, doc);
    }

    private string ToJsonString(JsonDocument jdoc)
    {
        using (var stream = new MemoryStream())
        {
            Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
            jdoc.WriteTo(writer);
            writer.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
