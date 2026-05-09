using System.Text.Json;
using System.Text.Json.Serialization;

namespace Horae.Converters;

/// <summary>
///     <see cref="System.Text.Json" /> converter for <see cref="DayOffset" />.
/// </summary>
/// <remarks>
///     <para>
///         Serialization format depends on whether a time zone is embedded:
///         <list type="bullet">
///             <item>
///                 <description>
///                     No time zone → plain integer: <c>1</c>
///                 </description>
///             </item>
///             <item>
///                 <description>
///                     With time zone → JSON object: <c>{ "days": 1, "timezone": "Europe/Berlin" }</c>
///                 </description>
///             </item>
///         </list>
///     </para>
///     <para>
///         Both formats are accepted when reading, so an API can switch from the compact integer
///         representation to the richer object form without a breaking change.
///     </para>
/// </remarks>
public sealed class DayOffsetConverter : JsonConverter<DayOffset>
{
    /// <inheritdoc />
    public override DayOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
            return new DayOffset(reader.GetInt32());

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            var days = 0;
            TimeZoneInfo? tz = null;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Expected a property name inside DayOffset object.");

                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "days":
                        days = reader.GetInt32();
                        break;
                    case "timezone":
                        var tzId = reader.GetString();
                        if (tzId is not null)
                            tz = TimeZoneInfo.FindSystemTimeZoneById(tzId);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            return new DayOffset(days, tz);
        }

        throw new JsonException(
            $"Cannot deserialize DayOffset from token type '{reader.TokenType}'. Expected a number or an object.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DayOffset value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);

        if (value.TimeZone is null)
        {
            writer.WriteNumberValue(value.Days);
            return;
        }

        writer.WriteStartObject();
        writer.WriteNumber("days", value.Days);
        writer.WriteString("timezone", value.TimeZone.Id);
        writer.WriteEndObject();
    }
}
