using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Horae.Converters;

/// <summary>
///     <see cref="System.Text.Json" /> converter that serialises <see cref="DateOnly" />
///     as an ISO 8601 date string, e.g. <c>"2026-01-01"</c>.
/// </summary>
/// <example>
///     <code>
///         [JsonConverter(typeof(IsoDateConverter))]
///         public DateOnly Date { get; init; }
///     </code>
/// </example>
public sealed class IsoDateConverter : JsonConverter<DateOnly>
{
    /// <inheritdoc />
    public override DateOnly Read(ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var s = reader.GetString() ?? throw new JsonException("Expected a non-null ISO 8601 date string.");
        return DateOnly.ParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer,
        DateOnly value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToIsoString());
}
