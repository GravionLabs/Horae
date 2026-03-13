using System.Text.Json.Serialization;

namespace Horae.Converters;

/// <summary>
///     Marks a <see cref="System.DateTimeOffset" /> or nullable <see cref="System.DateTimeOffset" />
///     property for ISO 8601 UTC serialisation with configurable sub-second precision.
///     Implements <see cref="JsonConverterAttribute" /> so it acts directly as a JSON converter attribute.
/// </summary>
/// <remarks>
///     This attribute is a convenient alternative to
///     <c>[JsonConverter(typeof(IsoDateTimeOffsetConverter))]</c> that also lets you control
///     sub-second precision without creating separate converter classes.
/// </remarks>
/// <example>
///     <code>
///         // Always emit 7 fractional digits:
///         [IsoDateTimeFormat(IsoDateTimePrecision.Fractional)]
///         public DateTimeOffset Timestamp { get; init; }
///
///         // Always omit fractional digits:
///         [IsoDateTimeFormat(IsoDateTimePrecision.Seconds)]
///         public DateTimeOffset? StartTime { get; init; }
///
///         // Default: include fractional digits only when non-zero
///         [IsoDateTimeFormat]
///         public DateTimeOffset EndTime { get; init; }
///     </code>
/// </example>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class IsoDateTimeFormatAttribute(IsoDateTimePrecision precision = IsoDateTimePrecision.Auto)
    : JsonConverterAttribute
{
    /// <summary>Gets the sub-second precision used when serialising.</summary>
    public IsoDateTimePrecision Precision { get; } = precision;

    /// <inheritdoc />
    public override JsonConverter? CreateConverter(Type typeToConvert) => typeToConvert switch
    {
        _ when typeToConvert == typeof(DateTimeOffset)  => new IsoDateTimeOffsetConverter(Precision),
        _ when typeToConvert == typeof(DateTimeOffset?) => new NullableIsoDateTimeOffsetConverter(Precision),
        _ => null,
    };
}
