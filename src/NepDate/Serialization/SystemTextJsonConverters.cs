using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NepDate.Serialization
{
    /// <summary>
    /// Contains JSON converter implementations for System.Text.Json serialization of <see cref="NepaliDate"/>.
    /// </summary>
    public static class SystemTextJsonConverters
    {
        /// <summary>
        /// Converts a <see cref="NepaliDate"/> to or from JSON using System.Text.Json.
        /// </summary>
        /// <remarks>
        /// This converter serializes NepaliDate as a string in the format "YYYY-MM-DD".
        /// </remarks>
        public class NepaliDateJsonConverter : JsonConverter<NepaliDate>
        {
            /// <summary>
            /// Reads and converts the JSON to a <see cref="NepaliDate"/>.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="typeToConvert">The type to convert.</param>
            /// <param name="options">An object that specifies serialization options to use.</param>
            /// <returns>The converted value.</returns>
            public override NepaliDate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                {
                    return default;
                }

                if (reader.TokenType == JsonTokenType.String)
                {
                    string dateString = reader.GetString();
                    if (NepaliDate.TryParse(dateString, out var result))
                    {
                        return result;
                    }
                    throw new JsonException($"Unable to parse '{dateString}' as a valid Nepali date.");
                }
                else if (reader.TokenType == JsonTokenType.StartObject)
                {
                    int? year = null;
                    int? month = null;
                    int? day = null;

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                    {
                        if (reader.TokenType != JsonTokenType.PropertyName)
                        {
                            throw new JsonException("Expected property name");
                        }

                        string propertyName = reader.GetString();
                        reader.Read();

                        switch (propertyName.ToLower())
                        {
                            case "year":
                                year = reader.GetInt32();
                                break;
                            case "month":
                                month = reader.GetInt32();
                                break;
                            case "day":
                                day = reader.GetInt32();
                                break;
                            default:
                                reader.Skip();
                                break;
                        }
                    }

                    if (year.HasValue && month.HasValue && day.HasValue)
                    {
                        return new NepaliDate(year.Value, month.Value, day.Value);
                    }
                    throw new JsonException("Missing required NepaliDate properties (Year, Month, Day)");
                }

                throw new JsonException($"Unexpected token {reader.TokenType} when parsing NepaliDate");
            }

            /// <summary>
            /// Writes a <see cref="NepaliDate"/> as JSON.
            /// </summary>
            /// <param name="writer">The writer to write to.</param>
            /// <param name="value">The value to convert to JSON.</param>
            /// <param name="options">An object that specifies serialization options to use.</param>
            public override void Write(Utf8JsonWriter writer, NepaliDate value, JsonSerializerOptions options)
            {
                writer.WriteStringValue($"{value.Year:D4}-{value.Month:D2}-{value.Day:D2}");
            }
        }

        /// <summary>
        /// Converts a <see cref="NepaliDate"/> to or from JSON as an object with Year, Month, and Day properties.
        /// </summary>
        public class NepaliDateObjectJsonConverter : JsonConverter<NepaliDate>
        {
            /// <summary>
            /// Reads and converts the JSON to a <see cref="NepaliDate"/>.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="typeToConvert">The type to convert.</param>
            /// <param name="options">An object that specifies serialization options to use.</param>
            /// <returns>The converted value.</returns>
            public override NepaliDate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                {
                    return default;
                }

                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException("Expected start of object");
                }

                int? year = null;
                int? month = null;
                int? day = null;

                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                {
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException("Expected property name");
                    }

                    string propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName.ToLower())
                    {
                        case "year":
                            year = reader.GetInt32();
                            break;
                        case "month":
                            month = reader.GetInt32();
                            break;
                        case "day":
                            day = reader.GetInt32();
                            break;
                        default:
                            reader.Skip();
                            break;
                    }
                }

                if (year.HasValue && month.HasValue && day.HasValue)
                {
                    return new NepaliDate(year.Value, month.Value, day.Value);
                }

                throw new JsonException("Missing required NepaliDate properties (Year, Month, Day)");
            }

            /// <summary>
            /// Writes a <see cref="NepaliDate"/> as JSON.
            /// </summary>
            /// <param name="writer">The writer to write to.</param>
            /// <param name="value">The value to convert to JSON.</param>
            /// <param name="options">An object that specifies serialization options to use.</param>
            public override void Write(Utf8JsonWriter writer, NepaliDate value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteNumber("Year", value.Year);
                writer.WriteNumber("Month", value.Month);
                writer.WriteNumber("Day", value.Day);
                writer.WriteEndObject();
            }
        }
    }
} 