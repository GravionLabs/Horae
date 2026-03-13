using System.Globalization;

namespace Horae;

/// <summary>
///     Extension methods for formatting <see cref="DateTimeOffset" /> and <see cref="DateOnly" />
///     values as ISO 8601 strings, and for parsing ISO 8601 strings back to those types.
/// </summary>
public static class IsoStringExtensions
{
    // Format used for DateTimeOffset serialisation (UTC, no sub-second component).
    private const string IsoDateTimeFormat = "yyyy-MM-ddTHH:mm:ss'Z'";

    // Format used when sub-second precision is present (trailing zeros suppressed via FFFFFFF).
    private const string IsoDateTimeFormatFractional = "yyyy-MM-ddTHH:mm:ss.FFFFFFF'Z'";

    // Format used when sub-second digits must always appear (7 fixed digits, no trimming).
    private const string IsoDateTimeFormatFractionalAlways = "yyyy-MM-ddTHH:mm:ss.fffffff'Z'";

    // Format used for DateOnly serialisation.
    private const string IsoDateFormat = "yyyy-MM-dd";

    /// <summary>
    ///     Formats a <see cref="DateTimeOffset" /> as an ISO 8601 UTC string.
    /// </summary>
    /// <param name="dto">The value to format. Converted to UTC before formatting.</param>
    /// <param name="precision">
    ///     Controls sub-second digits in the output.
    ///     <list type="bullet">
    ///         <item>
    ///             <see cref="IsoDateTimePrecision.Seconds" /> (default) — always omit sub-second digits,
    ///             e.g. <c>"2026-01-01T23:00:00Z"</c>.
    ///         </item>
    ///         <item>
    ///             <see cref="IsoDateTimePrecision.Auto" /> — sub-second digits only when non-zero,
    ///             e.g. <c>"2026-01-01T23:00:00Z"</c> or <c>"2026-01-01T23:00:00.5Z"</c>.
    ///         </item>
    ///         <item>
    ///             <see cref="IsoDateTimePrecision.Fractional" /> — always include 7 sub-second digits,
    ///             e.g. <c>"2026-01-01T23:00:00.0000000Z"</c> or <c>"2026-01-01T23:00:00.5000000Z"</c>.
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>An ISO 8601 UTC string.</returns>
    /// <example>
    ///     <code>
    ///         var dto = new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero);
    ///         dto.ToIsoString();                                    // "2026-01-01T23:00:00Z"
    ///         dto.ToIsoString(IsoDateTimePrecision.Fractional);     // "2026-01-01T23:00:00.0000000Z"
    ///         dto.ToIsoString(IsoDateTimePrecision.Seconds);        // "2026-01-01T23:00:00Z"
    ///     </code>
    /// </example>
    public static string ToIsoString(this DateTimeOffset dto,
        IsoDateTimePrecision precision = IsoDateTimePrecision.Seconds)
    {
        var utc = dto.ToUniversalTime();
        var fmt = precision switch
        {
            IsoDateTimePrecision.Seconds    => IsoDateTimeFormat,
            IsoDateTimePrecision.Fractional => IsoDateTimeFormatFractionalAlways,
            _                               => utc.Ticks % TimeSpan.TicksPerSecond == 0
                                                   ? IsoDateTimeFormat
                                                   : IsoDateTimeFormatFractional,
        };
        return utc.ToString(fmt, CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///     Formats a nullable <see cref="DateTimeOffset" /> as an ISO 8601 UTC string,
    ///     or returns <see langword="null" /> when the value is absent.
    /// </summary>
    /// <param name="dto">The value to format.</param>
    /// <returns>An ISO 8601 UTC string, or <see langword="null" />.</returns>
    public static string? ToIsoString(this DateTimeOffset? dto) =>
        dto is null ? null : dto.Value.ToIsoString();

    /// <summary>
    ///     Formats a <see cref="DateOnly" /> as an ISO 8601 date string,
    ///     e.g. <c>"2026-01-01"</c>.
    /// </summary>
    /// <param name="date">The date to format.</param>
    /// <returns>An ISO 8601 date string in the form <c>yyyy-MM-dd</c>.</returns>
    /// <example>
    ///     <code>
    ///         new DateOnly(2026, 1, 1).ToIsoString(); // "2026-01-01"
    ///     </code>
    /// </example>
    public static string ToIsoString(this DateOnly date) =>
        date.ToString(IsoDateFormat, CultureInfo.InvariantCulture);

    /// <summary>
    ///     Formats a nullable <see cref="DateOnly" /> as an ISO 8601 date string,
    ///     or returns <see langword="null" /> when the value is absent.
    /// </summary>
    /// <param name="date">The date to format.</param>
    /// <returns>An ISO 8601 date string, or <see langword="null" />.</returns>
    public static string? ToIsoString(this DateOnly? date) =>
        date is null ? null : date.Value.ToIsoString();

    /// <summary>
    ///     Parses an ISO 8601 date-time string (e.g. <c>"2026-01-01T23:00:00Z"</c> or
    ///     <c>"2026-01-01T23:00:00+01:00"</c>) into a <see cref="DateTimeOffset" />.
    ///     The UTC offset is preserved as given in the string.
    /// </summary>
    /// <param name="s">The ISO 8601 string to parse.</param>
    /// <returns>The parsed <see cref="DateTimeOffset" />.</returns>
    /// <exception cref="FormatException">
    ///     Thrown when <paramref name="s" /> is not a valid ISO 8601 date-time string.
    /// </exception>
    public static DateTimeOffset ParseIsoDateTime(this string s) =>
        DateTimeOffset.Parse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

    /// <summary>
    ///     Tries to parse an ISO 8601 date-time string into a <see cref="DateTimeOffset" />.
    /// </summary>
    /// <param name="s">The ISO 8601 string to parse.</param>
    /// <param name="result">
    ///     When this method returns <see langword="true" />, contains the parsed value;
    ///     otherwise <see cref="DateTimeOffset.MinValue" />.
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="s" /> was successfully parsed;
    ///     otherwise <see langword="false" />.
    /// </returns>
    public static bool TryParseIsoDateTime(this string s, out DateTimeOffset result) =>
        DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out result);

    /// <summary>
    ///     Parses an ISO 8601 date string (e.g. <c>"2026-01-01"</c>) into a <see cref="DateOnly" />.
    /// </summary>
    /// <param name="s">The ISO 8601 date string to parse.</param>
    /// <returns>The parsed <see cref="DateOnly" />.</returns>
    /// <exception cref="FormatException">
    ///     Thrown when <paramref name="s" /> is not a valid ISO 8601 date string.
    /// </exception>
    public static DateOnly ParseIsoDate(this string s) =>
        DateOnly.ParseExact(s, IsoDateFormat, CultureInfo.InvariantCulture);

    /// <summary>
    ///     Tries to parse an ISO 8601 date string (e.g. <c>"2026-01-01"</c>) into a
    ///     <see cref="DateOnly" />.
    /// </summary>
    /// <param name="s">The ISO 8601 date string to parse.</param>
    /// <param name="result">
    ///     When this method returns <see langword="true" />, contains the parsed value;
    ///     otherwise <see cref="DateOnly.MinValue" />.
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="s" /> was successfully parsed;
    ///     otherwise <see langword="false" />.
    /// </returns>
    public static bool TryParseIsoDate(this string s, out DateOnly result) =>
        DateOnly.TryParseExact(s, IsoDateFormat, CultureInfo.InvariantCulture,
            DateTimeStyles.None, out result);
}
