using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace NepDate.Serialization
{
    /// <summary>
    /// Contains JSON converter implementations for Newtonsoft.Json serialization of <see cref="NepaliDate"/>.
    /// </summary>
    public static class NewtonsoftJsonConverters
    {
        /// <summary>
        /// Provides JSON.NET (Newtonsoft.Json) serialization support for <see cref="NepaliDate"/>.
        /// Serializes the date as a string in ISO format (YYYY-MM-DD).
        /// </summary>
        public class NepaliDateJsonConverter : JsonConverter<NepaliDate>
        {
            /// <summary>
            /// Writes a <see cref="NepaliDate"/> as JSON.
            /// </summary>
            /// <param name="writer">The writer to write to.</param>
            /// <param name="value">The value to convert to JSON.</param>
            /// <param name="serializer">The calling serializer.</param>
            public override void WriteJson(JsonWriter writer, NepaliDate value, JsonSerializer serializer)
            {
                writer.WriteValue($"{value.Year:D4}-{value.Month:D2}-{value.Day:D2}");
            }

            /// <summary>
            /// Reads and converts the JSON to a <see cref="NepaliDate"/>.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="objectType">Type of the object.</param>
            /// <param name="existingValue">The existing value of object being read.</param>
            /// <param name="hasExistingValue">The existing value has a value.</param>
            /// <param name="serializer">The calling serializer.</param>
            /// <returns>The converted NepaliDate value.</returns>
            public override NepaliDate ReadJson(JsonReader reader, Type objectType, NepaliDate existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null)
                    return default;

                if (reader.TokenType == JsonToken.String)
                {
                    string dateString = (string)reader.Value;
                    if (NepaliDate.TryParse(dateString, out var result))
                        return result;

                    throw new JsonSerializationException($"Cannot convert {dateString} to NepaliDate");
                }

                if (reader.TokenType == JsonToken.StartObject)
                {
                    JObject obj = JObject.Load(reader);

                    if (obj.TryGetValue("Year", StringComparison.OrdinalIgnoreCase, out JToken yearToken) &&
                        obj.TryGetValue("Month", StringComparison.OrdinalIgnoreCase, out JToken monthToken) &&
                        obj.TryGetValue("Day", StringComparison.OrdinalIgnoreCase, out JToken dayToken))
                    {
                        int year = yearToken.Value<int>();
                        int month = monthToken.Value<int>();
                        int day = dayToken.Value<int>();

                        return new NepaliDate(year, month, day);
                    }
                }

                throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing NepaliDate");
            }
        }

        /// <summary>
        /// Provides JSON.NET (Newtonsoft.Json) serialization support for <see cref="NepaliDate"/>.
        /// Serializes the date as an object with Year, Month, and Day properties.
        /// </summary>
        public class NepaliDateObjectJsonConverter : JsonConverter<NepaliDate>
        {
            /// <summary>
            /// Writes a <see cref="NepaliDate"/> as JSON.
            /// </summary>
            /// <param name="writer">The writer to write to.</param>
            /// <param name="value">The value to convert to JSON.</param>
            /// <param name="serializer">The calling serializer.</param>
            public override void WriteJson(JsonWriter writer, NepaliDate value, JsonSerializer serializer)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("Year");
                writer.WriteValue(value.Year);

                writer.WritePropertyName("Month");
                writer.WriteValue(value.Month);

                writer.WritePropertyName("Day");
                writer.WriteValue(value.Day);

                writer.WriteEndObject();
            }

            /// <summary>
            /// Reads and converts the JSON to a <see cref="NepaliDate"/>.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="objectType">Type of the object.</param>
            /// <param name="existingValue">The existing value of object being read.</param>
            /// <param name="hasExistingValue">The existing value has a value.</param>
            /// <param name="serializer">The calling serializer.</param>
            /// <returns>The converted NepaliDate value.</returns>
            public override NepaliDate ReadJson(JsonReader reader, Type objectType, NepaliDate existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null)
                    return default;

                if (reader.TokenType == JsonToken.StartObject)
                {
                    JObject obj = JObject.Load(reader);

                    if (obj.TryGetValue("Year", StringComparison.OrdinalIgnoreCase, out JToken yearToken) &&
                        obj.TryGetValue("Month", StringComparison.OrdinalIgnoreCase, out JToken monthToken) &&
                        obj.TryGetValue("Day", StringComparison.OrdinalIgnoreCase, out JToken dayToken))
                    {
                        int year = yearToken.Value<int>();
                        int month = monthToken.Value<int>();
                        int day = dayToken.Value<int>();

                        return new NepaliDate(year, month, day);
                    }

                    throw new JsonSerializationException("Missing required NepaliDate properties (Year, Month, Day)");
                }

                throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing NepaliDate");
            }
        }
    }
}