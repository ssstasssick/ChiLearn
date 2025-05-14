using Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Sqlite.JsonModels
{
    public class GrammarBlockConverter : JsonConverter<GrammarBlockJson>
    {
        public override GrammarBlockJson Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            var root = jsonDoc.RootElement;
            var type = root.GetProperty("type").GetString();

            return type switch
            {
                "header" or "text" or "bold" => JsonSerializer.Deserialize<TextBlockJson>(root.GetRawText(), options),
                "example" => JsonSerializer.Deserialize<ExampleBlockJson>(root.GetRawText(), options),
                "table" => JsonSerializer.Deserialize<TableBlockJson>(root.GetRawText(), options),
                _ => throw new JsonException($"Unknown type: {type}")
            };
        }

        public override void Write(Utf8JsonWriter writer, GrammarBlockJson value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }
    }
}
