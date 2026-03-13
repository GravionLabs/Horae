namespace Horae;

/// <summary>
///     Controls the sub-second precision of ISO 8601 date-time strings produced by
///     <see cref="IsoStringExtensions.ToIsoString(System.DateTimeOffset, IsoDateTimePrecision)" />
///     and the <see cref="Horae.Converters.IsoDateTimeFormatAttribute" />.
/// </summary>
public enum IsoDateTimePrecision
{
    /// <summary>
    ///     Include sub-second digits only when the value has a non-zero sub-second component.
    ///     <c>2026-01-01T23:00:00Z</c> or <c>2026-01-01T23:00:00.5Z</c>.
    ///     This is the default behaviour.
    /// </summary>
    Auto,

    /// <summary>
    ///     Always omit sub-second digits, even when the value has a non-zero sub-second component.
    ///     Result is always in the form <c>2026-01-01T23:00:00Z</c>.
    /// </summary>
    Seconds,

    /// <summary>
    ///     Always include sub-second digits with 7 fixed decimal places (no trailing-zero trimming).
    ///     Result is always in the form <c>2026-01-01T23:00:00.0000000Z</c>.
    /// </summary>
    Fractional,
}
