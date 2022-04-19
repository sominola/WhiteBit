using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WhiteBit.Api.Utilities.Converters;

public class DecimalConverter: JsonConverter<decimal>
{
   
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                {
                    var stringValue = reader.GetString();
                    if (decimal.TryParse(stringValue, NumberStyles.Any , CultureInfo.InvariantCulture, out var value))
                    {
                        return value;
                    }
                    
                    break;
                }
                case JsonTokenType.Number:
                    return reader.GetInt64();
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
}