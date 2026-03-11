namespace Horae;

/// <summary>
///     Provides extension methods on <see cref="DateTimeOffset" /> for timezone-aware
///     day-boundary calculations, rounding, and time-zone conversions.
///     Methods that accept an optional <see cref="TimeZoneInfo" /> parameter fall back to
///     <see cref="HoareSettings.CurrentTimeZone" /> when <see langword="null" /> is passed.
/// </summary>
public static class DateTimeOffsetExtensions
{
    /// <summary>
    ///     Converts a <see cref="DateTimeOffset" /> value to the specified time zone.
    /// </summary>
    /// <param name="dateTimeOffset">The value to convert.</param>
    /// <param name="timeZone">The target time zone.</param>
    /// <returns>The same instant expressed in <paramref name="timeZone" />.</returns>
    public static DateTimeOffset ToTimeZone(this DateTimeOffset dateTimeOffset, TimeZoneInfo timeZone) =>
        TimeZoneInfo.ConvertTime(dateTimeOffset, timeZone);

    /// <summary>
    ///     Converts a <see cref="DateTimeOffset" /> value to Central European Time (CET/CEST).
    /// </summary>
    /// <param name="dateTimeOffset">The value to convert.</param>
    /// <returns>The same instant expressed in Central European Time.</returns>
    public static DateTimeOffset ToCentralEuropeanTime(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToTimeZone(TimeZones.CentralEuropean);

    /// <summary>
    ///     Converts a <see cref="DateTimeOffset" /> value to UTC.
    ///     This is a named convenience alias kept for backward compatibility.
    /// </summary>
    /// <param name="dateTimeOffset">The value to convert.</param>
    /// <returns>The same instant expressed as UTC.</returns>
    public static DateTimeOffset CentralEuropeanToUtcTime(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.CentralEuropeanToUtc();


    /// <summary>
    ///     Gets the start of the day (midnight) for a <see cref="DateTimeOffset" /> value
    ///     in the specified time zone.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar day.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of the same calendar day in the given time zone.</returns>
    public static DateTimeOffset StartOfDay(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null)
    {
        var tz = timeZone ?? HoareSettings.CurrentTimeZone;
        var inTz = TimeZoneInfo.ConvertTime(dateTimeOffset, tz);
        return new DateTimeOffset(inTz.Year, inTz.Month, inTz.Day, 0, 0, 0, inTz.Offset);
    }

    /// <summary>
    ///     Gets the start of the day (midnight) expressed as UTC.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar day.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of the same calendar day expressed in UTC.</returns>
    public static DateTimeOffset StartOfDayUtc(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null) =>
        TimeZoneInfo.ConvertTime(dateTimeOffset.StartOfDay(timeZone), TimeZoneInfo.Utc);


    /// <summary>
    ///     Gets the end of the day for a <see cref="DateTimeOffset" /> value in the specified
    ///     time zone. The returned value is the exclusive upper boundary (i.e. the first instant
    ///     of the following day), which is the conventional half-open interval pattern.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar day.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>The first instant of the next calendar day in the given time zone.</returns>
    public static DateTimeOffset EndOfDay(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null) =>
        dateTimeOffset.StartOfDay(timeZone).AddDays(1);

    /// <summary>
    ///     Gets the end of the day expressed as UTC.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar day.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>The first instant of the next calendar day expressed in UTC.</returns>
    public static DateTimeOffset EndOfDayUtc(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null) =>
        TimeZoneInfo.ConvertTime(dateTimeOffset.EndOfDay(timeZone), TimeZoneInfo.Utc);


    /// <summary>
    ///     Gets the start of the intraday session (midnight) in the specified time zone.
    ///     Semantically equivalent to <see cref="StartOfDay" /> but using the trading-system
    ///     concept of an "intraday" boundary.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar day.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of the same calendar day in the given time zone.</returns>
    public static DateTimeOffset StartOfIntraday(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null)
    {
        var tz = timeZone ?? HoareSettings.CurrentTimeZone;
        var inTz = TimeZoneInfo.ConvertTime(dateTimeOffset, tz);
        return new DateTimeOffset(inTz.Year, inTz.Month, inTz.Day, 0, 0, 0, inTz.Offset);
    }

    /// <summary>
    ///     Gets the start of the intraday session expressed as UTC.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar day.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of the same calendar day expressed in UTC.</returns>
    public static DateTimeOffset
        StartOfIntradayUtc(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null) =>
        TimeZoneInfo.ConvertTime(dateTimeOffset.StartOfIntraday(timeZone), TimeZoneInfo.Utc);


    /// <summary>
    ///     Gets the end of the intraday session in the specified time zone (exclusive upper boundary,
    ///     i.e. the first instant of the following day).
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar day.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>The first instant of the next calendar day in the given time zone.</returns>
    public static DateTimeOffset EndOfIntraday(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null) =>
        dateTimeOffset.StartOfIntraday(timeZone).AddDays(1);

    /// <summary>
    ///     Gets the end of the intraday session expressed as UTC.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar day.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>The first instant of the next calendar day expressed in UTC.</returns>
    public static DateTimeOffset EndOfIntradayUtc(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null) =>
        TimeZoneInfo.ConvertTime(dateTimeOffset.EndOfIntraday(timeZone), TimeZoneInfo.Utc);


    /// <summary>
    ///     Gets the start of the day-ahead session in the specified time zone
    ///     (i.e. the end of the current intraday, which is midnight of the next day).
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar day.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of the next calendar day in the given time zone.</returns>
    public static DateTimeOffset StartOfDayAhead(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null) =>
        dateTimeOffset.EndOfIntraday(timeZone);

    /// <summary>
    ///     Gets the start of the day-ahead session expressed as UTC.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar day.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of the next calendar day expressed in UTC.</returns>
    public static DateTimeOffset
        StartOfDayAheadUtc(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null) =>
        TimeZoneInfo.ConvertTime(dateTimeOffset.StartOfDayAhead(timeZone), TimeZoneInfo.Utc);

    /// <summary>
    ///     Gets the end of the day-ahead session in the specified time zone
    ///     (i.e. the start of the day after next — the exclusive upper boundary of the day-ahead period).
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar day.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight two calendar days ahead in the given time zone.</returns>
    public static DateTimeOffset EndOfDayAhead(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null) =>
        dateTimeOffset.EndOfIntraday(timeZone).AddDays(1);

    /// <summary>
    ///     Gets the end of the day-ahead session expressed as UTC.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar day.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight two calendar days ahead expressed in UTC.</returns>
    public static DateTimeOffset EndOfDayAheadUtc(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null) =>
        TimeZoneInfo.ConvertTime(dateTimeOffset.EndOfDayAhead(timeZone), TimeZoneInfo.Utc);


    /// <summary>
    ///     Rounds down a <see cref="DateTimeOffset" /> value to the nearest multiple of
    ///     <paramref name="timeSpan" /> in the specified time zone and returns the result as UTC.
    /// </summary>
    /// <param name="dateTimeOffset">The value to round.</param>
    /// <param name="timeSpan">The granularity to round down to.</param>
    /// <param name="timeZone">
    ///     The time zone in which rounding is performed.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>The rounded value expressed in UTC.</returns>
    public static DateTimeOffset RoundDown(this DateTimeOffset dateTimeOffset,
        TimeSpan timeSpan,
        TimeZoneInfo? timeZone = null)
    {
        var tz = dateTimeOffset.ToTimeZone(timeZone ?? HoareSettings.CurrentTimeZone);
        var delta = tz.Ticks % timeSpan.Ticks;
        var rounded = new DateTimeOffset(tz.Ticks - delta, tz.Offset);
        return TimeZoneInfo.ConvertTime(rounded, TimeZoneInfo.Utc);
    }

    /// <summary>
    ///     Rounds a <see cref="DateTimeOffset" /> value to the nearest multiple of
    ///     <paramref name="timeSpan" /> in the specified time zone and returns the result as UTC.
    /// </summary>
    /// <param name="dateTimeOffset">The value to round.</param>
    /// <param name="timeSpan">The granularity to round to.</param>
    /// <param name="timeZone">
    ///     The time zone in which rounding is performed.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>The rounded value expressed in UTC.</returns>
    public static DateTimeOffset RoundToNearest(this DateTimeOffset dateTimeOffset,
        TimeSpan timeSpan,
        TimeZoneInfo? timeZone = null)
    {
        var tz = dateTimeOffset.ToTimeZone(timeZone ?? HoareSettings.CurrentTimeZone);
        var delta = tz.Ticks % timeSpan.Ticks;
        var roundUp = delta > timeSpan.Ticks / 2;
        var offset = roundUp ? timeSpan.Ticks : 0;
        var rounded = new DateTimeOffset(tz.Ticks + offset - delta, tz.Offset);
        return TimeZoneInfo.ConvertTime(rounded, TimeZoneInfo.Utc);
    }

    /// <summary>
    ///     Rounds up a <see cref="DateTimeOffset" /> value to the next multiple of
    ///     <paramref name="timeSpan" /> in the specified time zone and returns the result as UTC.
    /// </summary>
    /// <param name="dateTimeOffset">The value to round up.</param>
    /// <param name="timeSpan">The granularity to round up to.</param>
    /// <param name="timeZone">
    ///     The time zone in which rounding is performed.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>The rounded value expressed in UTC.</returns>
    public static DateTimeOffset RoundUp(this DateTimeOffset dateTimeOffset,
        TimeSpan timeSpan,
        TimeZoneInfo? timeZone = null)
    {
        var tz = dateTimeOffset.ToTimeZone(timeZone ?? HoareSettings.CurrentTimeZone);
        var modTicks = tz.Ticks % timeSpan.Ticks;
        var delta = modTicks != 0 ? timeSpan.Ticks - modTicks : 0;
        var rounded = new DateTimeOffset(tz.Ticks + delta, tz.Offset);
        return TimeZoneInfo.ConvertTime(rounded, TimeZoneInfo.Utc);
    }
}
