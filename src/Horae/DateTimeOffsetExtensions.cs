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
    public static DateTimeOffset StartOfDayAheadUtc(
        this DateTimeOffset dateTimeOffset,
        TimeZoneInfo? timeZone = null) =>
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
    ///     <paramref name="resolution" /> in the specified time zone and returns the result as UTC.
    ///     Convenience overload that accepts a <see cref="Duration" /> instead of a raw
    ///     <see cref="TimeSpan" />.
    /// </summary>
    /// <param name="dateTimeOffset">The value to round.</param>
    /// <param name="resolution">The granularity to round down to.</param>
    /// <param name="timeZone">
    ///     The time zone in which rounding is performed.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>The rounded value expressed in UTC.</returns>
    public static DateTimeOffset RoundDown(this DateTimeOffset dateTimeOffset,
        Duration resolution,
        TimeZoneInfo? timeZone = null) =>
        dateTimeOffset.RoundDown(resolution.Value, timeZone);

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
    ///     <paramref name="resolution" /> in the specified time zone and returns the result as UTC.
    ///     Convenience overload that accepts a <see cref="Duration" /> instead of a raw
    ///     <see cref="TimeSpan" />.
    /// </summary>
    /// <param name="dateTimeOffset">The value to round.</param>
    /// <param name="resolution">The granularity to round to.</param>
    /// <param name="timeZone">
    ///     The time zone in which rounding is performed.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>The rounded value expressed in UTC.</returns>
    public static DateTimeOffset RoundToNearest(this DateTimeOffset dateTimeOffset,
        Duration resolution,
        TimeZoneInfo? timeZone = null) =>
        dateTimeOffset.RoundToNearest(resolution.Value, timeZone);

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
    ///     <paramref name="resolution" /> in the specified time zone and returns the result as UTC.
    ///     Convenience overload that accepts a <see cref="Duration" /> instead of a raw
    ///     <see cref="TimeSpan" />.
    /// </summary>
    /// <param name="dateTimeOffset">The value to round up.</param>
    /// <param name="resolution">The granularity to round up to.</param>
    /// <param name="timeZone">
    ///     The time zone in which rounding is performed.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>The rounded value expressed in UTC.</returns>
    public static DateTimeOffset RoundUp(this DateTimeOffset dateTimeOffset,
        Duration resolution,
        TimeZoneInfo? timeZone = null) =>
        dateTimeOffset.RoundUp(resolution.Value, timeZone);

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


    /// <summary>
    ///     Gets the start of the ISO week (Monday midnight) for a <see cref="DateTimeOffset" /> value
    ///     in the specified time zone.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar week.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of the Monday that starts the ISO week containing <paramref name="dateTimeOffset" />.</returns>
    public static DateTimeOffset StartOfWeek(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null)
    {
        var tz = timeZone ?? HoareSettings.CurrentTimeZone;
        var inTz = TimeZoneInfo.ConvertTime(dateTimeOffset, tz);
        var dow = inTz.DayOfWeek;
        var daysFromMonday = dow == DayOfWeek.Sunday ? 6 : (int)dow - 1;
        var monday = new DateOnly(inTz.Year, inTz.Month, inTz.Day).AddDays(-daysFromMonday);
        return monday.AsDateTimeOffset(tz);
    }

    /// <summary>
    ///     Gets the start of the ISO week expressed as UTC.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar week.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of the Monday that starts the ISO week, expressed in UTC.</returns>
    public static DateTimeOffset StartOfWeekUtc(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null) =>
        TimeZoneInfo.ConvertTime(dateTimeOffset.StartOfWeek(timeZone), TimeZoneInfo.Utc);

    /// <summary>
    ///     Gets the end of the ISO week for a <see cref="DateTimeOffset" /> value in the specified
    ///     time zone. The returned value is the exclusive upper boundary (i.e. midnight of the
    ///     following Monday).
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar week.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of the Monday following the current ISO week.</returns>
    public static DateTimeOffset EndOfWeek(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null)
    {
        var tz = timeZone ?? HoareSettings.CurrentTimeZone;
        var monday = DateOnly.FromDateTime(dateTimeOffset.StartOfWeek(tz).ToTimeZone(tz).DateTime);
        return monday.AddDays(7).AsDateTimeOffset(tz);
    }

    /// <summary>
    ///     Gets the end of the ISO week expressed as UTC.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar week.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of the Monday following the current ISO week, expressed in UTC.</returns>
    public static DateTimeOffset EndOfWeekUtc(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null) =>
        TimeZoneInfo.ConvertTime(dateTimeOffset.EndOfWeek(timeZone), TimeZoneInfo.Utc);


    /// <summary>
    ///     Gets the start of the calendar month (midnight of the first day) for a
    ///     <see cref="DateTimeOffset" /> value in the specified time zone.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar month.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of the first day of the month in the given time zone.</returns>
    public static DateTimeOffset StartOfMonth(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null)
    {
        var tz = timeZone ?? HoareSettings.CurrentTimeZone;
        var inTz = TimeZoneInfo.ConvertTime(dateTimeOffset, tz);
        return new DateOnly(inTz.Year, inTz.Month, 1).AsDateTimeOffset(tz);
    }

    /// <summary>
    ///     Gets the start of the calendar month expressed as UTC.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar month.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of the first day of the month, expressed in UTC.</returns>
    public static DateTimeOffset StartOfMonthUtc(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null) =>
        TimeZoneInfo.ConvertTime(dateTimeOffset.StartOfMonth(timeZone), TimeZoneInfo.Utc);

    /// <summary>
    ///     Gets the end of the calendar month for a <see cref="DateTimeOffset" /> value in the
    ///     specified time zone. The returned value is the exclusive upper boundary (i.e. midnight
    ///     of the first day of the following month).
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar month.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of the first day of the next calendar month in the given time zone.</returns>
    public static DateTimeOffset EndOfMonth(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null)
    {
        var tz = timeZone ?? HoareSettings.CurrentTimeZone;
        var inTz = TimeZoneInfo.ConvertTime(dateTimeOffset, tz);
        var firstOfNextMonth = inTz.Month == 12
            ? new DateOnly(inTz.Year + 1, 1, 1)
            : new DateOnly(inTz.Year, inTz.Month + 1, 1);
        return firstOfNextMonth.AsDateTimeOffset(tz);
    }

    /// <summary>
    ///     Gets the end of the calendar month expressed as UTC.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar month.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of the first day of the next calendar month, expressed in UTC.</returns>
    public static DateTimeOffset EndOfMonthUtc(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null) =>
        TimeZoneInfo.ConvertTime(dateTimeOffset.EndOfMonth(timeZone), TimeZoneInfo.Utc);


    /// <summary>
    ///     Gets the start of the calendar year (midnight of 1 January) for a
    ///     <see cref="DateTimeOffset" /> value in the specified time zone.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar year.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of 1 January of the current year in the given time zone.</returns>
    public static DateTimeOffset StartOfYear(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null)
    {
        var tz = timeZone ?? HoareSettings.CurrentTimeZone;
        var inTz = TimeZoneInfo.ConvertTime(dateTimeOffset, tz);
        return new DateOnly(inTz.Year, 1, 1).AsDateTimeOffset(tz);
    }

    /// <summary>
    ///     Gets the start of the calendar year expressed as UTC.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar year.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of 1 January of the current year, expressed in UTC.</returns>
    public static DateTimeOffset StartOfYearUtc(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null) =>
        TimeZoneInfo.ConvertTime(dateTimeOffset.StartOfYear(timeZone), TimeZoneInfo.Utc);

    /// <summary>
    ///     Gets the end of the calendar year for a <see cref="DateTimeOffset" /> value in the
    ///     specified time zone. The returned value is the exclusive upper boundary (i.e. midnight
    ///     of 1 January of the following year).
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar year.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of 1 January of the next year in the given time zone.</returns>
    public static DateTimeOffset EndOfYear(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null)
    {
        var tz = timeZone ?? HoareSettings.CurrentTimeZone;
        var inTz = TimeZoneInfo.ConvertTime(dateTimeOffset, tz);
        return new DateOnly(inTz.Year + 1, 1, 1).AsDateTimeOffset(tz);
    }

    /// <summary>
    ///     Gets the end of the calendar year expressed as UTC.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar year.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of 1 January of the next year, expressed in UTC.</returns>
    public static DateTimeOffset EndOfYearUtc(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null) =>
        TimeZoneInfo.ConvertTime(dateTimeOffset.EndOfYear(timeZone), TimeZoneInfo.Utc);


    /// <summary>
    ///     Gets the start of the calendar quarter (midnight of the first day of the quarter's first
    ///     month) for a <see cref="DateTimeOffset" /> value in the specified time zone.
    ///     Quarters are Q1 = Jan–Mar, Q2 = Apr–Jun, Q3 = Jul–Sep, Q4 = Oct–Dec.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar quarter.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of the first day of the current quarter in the given time zone.</returns>
    public static DateTimeOffset StartOfQuarter(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null)
    {
        var tz = timeZone ?? HoareSettings.CurrentTimeZone;
        var inTz = TimeZoneInfo.ConvertTime(dateTimeOffset, tz);
        var firstMonthOfQuarter = (inTz.Month - 1) / 3 * 3 + 1;
        return new DateOnly(inTz.Year, firstMonthOfQuarter, 1).AsDateTimeOffset(tz);
    }

    /// <summary>
    ///     Gets the start of the calendar quarter expressed as UTC.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar quarter.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of the first day of the current quarter, expressed in UTC.</returns>
    public static DateTimeOffset StartOfQuarterUtc(this DateTimeOffset dateTimeOffset,
        TimeZoneInfo? timeZone = null) =>
        TimeZoneInfo.ConvertTime(dateTimeOffset.StartOfQuarter(timeZone), TimeZoneInfo.Utc);

    /// <summary>
    ///     Gets the end of the calendar quarter for a <see cref="DateTimeOffset" /> value in the
    ///     specified time zone. The returned value is the exclusive upper boundary (i.e. midnight
    ///     of the first day of the following quarter).
    ///     Quarters are Q1 = Jan–Mar, Q2 = Apr–Jun, Q3 = Jul–Sep, Q4 = Oct–Dec.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar quarter.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of the first day of the next quarter in the given time zone.</returns>
    public static DateTimeOffset EndOfQuarter(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null)
    {
        var tz = timeZone ?? HoareSettings.CurrentTimeZone;
        var inTz = TimeZoneInfo.ConvertTime(dateTimeOffset, tz);
        var firstMonthOfNextQuarter = (inTz.Month - 1) / 3 * 3 + 4;
        var (year, month) = firstMonthOfNextQuarter > 12
            ? (inTz.Year + 1, firstMonthOfNextQuarter - 12)
            : (inTz.Year, firstMonthOfNextQuarter);
        return new DateOnly(year, month, 1).AsDateTimeOffset(tz);
    }

    /// <summary>
    ///     Gets the end of the calendar quarter expressed as UTC.
    /// </summary>
    /// <param name="dateTimeOffset">The source value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar quarter.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>Midnight of the first day of the next quarter, expressed in UTC.</returns>
    public static DateTimeOffset EndOfQuarterUtc(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null) =>
        TimeZoneInfo.ConvertTime(dateTimeOffset.EndOfQuarter(timeZone), TimeZoneInfo.Utc);
}
