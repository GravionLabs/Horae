using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Horae.Converters;

/// <summary>
///     <see cref="System.Text.Json" /> converter that serializes <see cref="DateTimeOffset" />
///     as an ISO 8601 string in UTC, e.g. <c>"2026-01-01T23:00:00Z"</c>.
///     Sub-second precision is controlled by <see cref="IsoDateTimePrecision" />.
/// </summary>
/// <example>
///     <code>
///         [JsonConverter(typeof(IsoDateTimeOffsetConverter))]
///         public DateTimeOffset Timestamp { get; init; }
///
///         // Or via the attribute for fine-grained precision control:
///         [IsoDateTimeFormat(IsoDateTimePrecision.Fractional)]
///         public DateTimeOffset Timestamp { get; init; }
///     </code>
/// </example>
public sealed class IsoDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    private readonly IsoDateTimePrecision _precision;

    /// <summary>
    ///     Initializes a new <see cref="IsoDateTimeOffsetConverter" /> with <see cref="IsoDateTimePrecision.Seconds" />.
    /// </summary>
    public IsoDateTimeOffsetConverter() : this(IsoDateTimePrecision.Seconds) { }

    /// <summary>
    ///     Initializes a new <see cref="IsoDateTimeOffsetConverter" />.
    /// </summary>
    /// <param name="precision">Sub-second precision to use when writing.</param>
    public IsoDateTimeOffsetConverter(IsoDateTimePrecision precision) =>
        _precision = precision;

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
        JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteStringValue(value.ToIsoString(_precision));
    }
}
