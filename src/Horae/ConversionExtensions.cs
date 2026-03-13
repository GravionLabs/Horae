namespace Horae;

/// <summary>
///     Extension methods for converting between <see cref="DateTimeOffset" />, <see cref="DateTime" />,
///     <see cref="DateOnly" />, and related types.
///     Methods without "Utc" in their name accept an optional <see cref="TimeZoneInfo" /> and fall back
///     to <see cref="HoareSettings.CurrentTimeZone" />. Methods with "Utc" always use UTC explicitly.
/// </summary>
public static class ConversionExtensions
{
    /// <summary>
    ///     Returns the calendar date of the <see cref="DateTimeOffset" /> value
    ///     in the specified (or default) time zone.
    /// </summary>
    /// <param name="dto">The value to convert.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar date.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>The calendar date in the given time zone.</returns>
    /// <example>
    ///     <code>
    ///         // 2026-01-01 23:00 UTC = 2026-01-02 08:00 JST
    ///         var dto = new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero);
    ///         var date = dto.AsDateOnly(TimeZones.Japan); // 2026-01-02
    ///     </code>
    /// </example>
    public static DateOnly AsDateOnly(this DateTimeOffset dto, TimeZoneInfo? timeZone = null)
    {
        var tz = timeZone ?? HoareSettings.CurrentTimeZone;
        return DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(dto, tz).DateTime);
    }

    /// <summary>
    ///     Returns the calendar date of the <see cref="DateTimeOffset" /> value expressed in UTC.
    /// </summary>
    /// <param name="dto">The value to convert.</param>
    /// <returns>The calendar date in UTC.</returns>
    public static DateOnly AsUtcDateOnly(this DateTimeOffset dto) =>
        DateOnly.FromDateTime(dto.UtcDateTime);

    /// <summary>
    ///     Returns a <see cref="DateTime" /> representing the same instant in the specified
    ///     (or default) time zone. The returned value has <see cref="DateTimeKind.Unspecified" />.
    /// </summary>
    /// <param name="dto">The value to convert.</param>
    /// <param name="timeZone">
    ///     The target time zone.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>
    ///     The local date and time in the given time zone (<see cref="DateTimeKind.Unspecified" />).
    /// </returns>
    public static DateTime AsDateTime(this DateTimeOffset dto, TimeZoneInfo? timeZone = null)
    {
        var tz = timeZone ?? HoareSettings.CurrentTimeZone;
        return TimeZoneInfo.ConvertTime(dto, tz).DateTime;
    }

    /// <summary>
    ///     Returns a <see cref="DateTime" /> representing the same instant in UTC.
    ///     The returned value has <see cref="DateTimeKind.Utc" />.
    /// </summary>
    /// <param name="dto">The value to convert.</param>
    /// <returns><see cref="DateTimeOffset.UtcDateTime" /> of the source value.</returns>
    public static DateTime AsUtcDateTime(this DateTimeOffset dto) =>
        dto.UtcDateTime;

    /// <summary>
    ///     Returns a <see cref="DateTimeOffset" /> representing midnight of the given
    ///     <see cref="DateOnly" /> date in the specified (or default) time zone.
    /// </summary>
    /// <param name="date">The date to convert.</param>
    /// <param name="timeZone">
    ///     The time zone that defines the midnight offset.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of <paramref name="date" /> with the UTC offset of the given time zone.</returns>
    /// <remarks>
    ///     During a DST fall-back transition (e.g. the last Sunday in October in CET) midnight may
    ///     be ambiguous. In that case <see cref="TimeZoneInfo.GetUtcOffset(DateTime)" /> returns
    ///     the <em>standard</em> time offset (e.g. <c>+01:00</c> for CET), which is the second
    ///     occurrence. There is no parameter to choose the first occurrence.
    ///     During a DST spring-forward transition midnight may be invalid; the framework adjusts
    ///     the offset silently to the DST offset.
    /// </remarks>
    /// <example>
    ///     <code>
    ///         var date = new DateOnly(2026, 6, 15);
    ///         var dto = date.AsDateTimeOffset(TimeZones.Japan); // 2026-06-15 00:00:00 +09:00
    ///     </code>
    /// </example>
    public static DateTimeOffset AsDateTimeOffset(this DateOnly date, TimeZoneInfo? timeZone = null)
    {
        var tz = timeZone ?? HoareSettings.CurrentTimeZone;
        var midnight = date.ToDateTime(TimeOnly.MinValue);
        var offset = tz.GetUtcOffset(midnight);
        return new DateTimeOffset(midnight, offset);
    }

    /// <summary>
    ///     Returns a <see cref="DateTimeOffset" /> representing midnight of the given
    ///     <see cref="DateOnly" /> date in UTC (<see cref="TimeSpan.Zero" /> offset).
    /// </summary>
    /// <param name="date">The date to convert.</param>
    /// <returns>Midnight of <paramref name="date" /> with a zero UTC offset.</returns>
    public static DateTimeOffset AsUtcDateTimeOffset(this DateOnly date) =>
        new(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
}
