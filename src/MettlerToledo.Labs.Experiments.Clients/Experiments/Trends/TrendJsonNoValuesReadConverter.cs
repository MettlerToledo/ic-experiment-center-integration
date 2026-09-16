// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Trends;

/// <summary>
/// A converter for <see cref="TrendJson"/> that skip reading the trend values.
/// </summary>
public class TrendJsonNoValuesReadConverter : JsonConverter<TrendJson>
{
    private const string NameProperty = "name";
    private const string UnitProperty = "unit";
    private const string StartTimeUtcProperty = "startTimeUtc";
    private const string ValuesProperty = "values";

    /// <inheritdoc/>
    public override TrendJson? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        string? name = null;
        string? unit = null;
        DateTime? startTimeUTC = null;

        while (reader.TokenType != JsonTokenType.EndObject)
        {
            reader.Read();
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }
            var readBasicProperty = ReadBasicProperty(ref reader, ref name, ref unit, ref startTimeUTC);

            if (!readBasicProperty && reader.ValueTextEquals(ValuesProperty))
            {
                reader.Read();
                if (reader.TokenType != JsonTokenType.StartArray)
                {
                    throw new JsonException();
                }
                while (reader.TokenType != JsonTokenType.EndArray)
                {
                    reader.Read();
                }
            }
            else if (!readBasicProperty)
            {
                throw new JsonException($"Unexpected property {reader.GetString()}");
            }
        }

        if (name is null || unit is null || startTimeUTC is null)
        {
            throw new JsonException("Property missing for TrendJson");
        }

        return new TrendJson { Name = name, Unit = unit, StartTimeUtc = startTimeUTC.Value };
    }

    internal static bool ReadBasicProperty(ref Utf8JsonReader reader, ref string? name, ref string? unit, ref DateTime? startTimeUTC)
    {
        if (reader.ValueTextEquals(NameProperty))
        {
            reader.Read();
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }
            name = reader.GetString();
            return true;
        }
        else if (reader.ValueTextEquals(UnitProperty))
        {
            reader.Read();
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }
            unit = reader.GetString();
            return true;
        }
        else if (reader.ValueTextEquals(StartTimeUtcProperty))
        {
            reader.Read();
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }
            startTimeUTC = reader.GetDateTime();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TrendJson value, JsonSerializerOptions options)
    {
        Write(writer, value, value.Values, options);
    }

    internal static void Write(Utf8JsonWriter writer, TrendJson trend, IEnumerable<TrendValueJson> values, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString(NameProperty, trend.Name);
        writer.WriteString(UnitProperty, trend.Unit);
        writer.WriteString(StartTimeUtcProperty, trend.StartTimeUtc);

        writer.WritePropertyName(ValuesProperty);
        writer.WriteStartArray();
        foreach (var item in values)
        {
            if (!double.IsNaN(item.Value) && !double.IsInfinity(item.Value)) // Skip values that are not a number.
            {
                // Note while functionally this could use JsonSerializer.Serialize(_jsonWriter, item, typeof(TrendValueJson), options)
                // The performance is too slow.
                writer.WriteStartObject();
                writer.WriteString("t", item.RelativeTime.ToString());
                writer.WriteNumber("v", item.Value);
                writer.WriteEndObject();
            }
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }
}
