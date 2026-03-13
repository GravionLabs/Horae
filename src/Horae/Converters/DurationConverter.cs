using System.Text.Json;
using System.Text.Json.Serialization;

namespace Horae.Converters;

/// <summary>
///     <see cref="System.Text.Json" /> converter that serializes <see cref="Duration" />
///     as an ISO 8601 duration string (e.g. <c>"PT15M"</c>).
/// </summary>
public sealed class DurationConverter : JsonConverter<Duration>
{
    /// <inheritdoc />
    public override Duration Read(ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        Duration.Parse(reader.GetString() ?? throw new JsonException("Expected a non-null string."));

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer,
        Duration value,
        JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteStringValue(value.ToString());
    }
}
