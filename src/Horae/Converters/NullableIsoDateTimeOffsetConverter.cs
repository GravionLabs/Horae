using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Horae.Converters;

/// <summary>
///     <see cref="System.Text.Json" /> converter that serialises <see cref="DateTimeOffset" />?
///     as an ISO 8601 string in UTC (e.g. <c>"2026-01-01T23:00:00Z"</c>), or as
///     <c>null</c> when the value is absent.
/// </summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
public sealed class NullableIsoDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
    /// <inheritdoc />
    public override DateTimeOffset? Read(ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var s = reader.GetString() ?? throw new JsonException("Expected a non-null ISO 8601 date-time string.");
        return DateTimeOffset.Parse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer,
        DateTimeOffset? value,
        JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Value.ToIsoString());
    }
}
