using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Horae.Converters;

/// <summary>
///     <see cref="System.Text.Json" /> converter that serialises <see cref="DateTimeOffset" />
///     as an ISO 8601 string in UTC, e.g. <c>"2026-01-01T23:00:00Z"</c>.
///     Sub-second precision is included only when non-zero (e.g. <c>"2026-01-01T23:00:00.5Z"</c>).
/// </summary>
/// <example>
///     <code>
///         [JsonConverter(typeof(IsoDateTimeOffsetConverter))]
///         public DateTimeOffset Timestamp { get; init; }
///     </code>
/// </example>
public sealed class IsoDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    /// <inheritdoc />
    public override DateTimeOffset Read(ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var s = reader.GetString() ?? throw new JsonException("Expected a non-null ISO 8601 date-time string.");
        return DateTimeOffset.Parse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer,
        DateTimeOffset value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToIsoString());
}
