using System.Text.Json.Serialization;
using System.Text.Json;

namespace GameServer.Resources
{
    public class HashNameConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString()!;
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                return doc.RootElement.GetRawText();
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                using JsonDocument document = JsonDocument.ParseValue(ref reader);
                if (document.RootElement.TryGetProperty("Hash", out JsonElement hashElement))
                {
                    return hashElement.ValueKind switch
                    {
                        JsonValueKind.String => hashElement.GetString()!,
                        JsonValueKind.Number => hashElement.GetRawText(),
                        _ => throw new JsonException("Hash 属性必须是字符串或数字")
                    };
                }
                throw new JsonException("在对象中找不到 Hash 属性");
            }

            throw new JsonException($"无效的 JSON 哈希结构类型：{reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }

    public class CustomJsonFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(string);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return new HashNameConverter();
        }
    }
}
