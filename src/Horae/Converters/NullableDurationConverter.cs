using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Horae.Converters;

/// <summary>
///     <see cref="System.Text.Json" /> converter that serialises <see cref="Duration" />
///     as an ISO 8601 duration string (e.g. <c>"PT15M"</c>).
/// </summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
public sealed class NullableDurationConverter : JsonConverter<Duration?>
{
    /// <inheritdoc />
    public override Duration? Read(ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        return Duration.Parse(reader.GetString() ?? throw new JsonException("Expected a non-null string."));
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer,
        Duration? value,
        JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.ToString());
    }
}
