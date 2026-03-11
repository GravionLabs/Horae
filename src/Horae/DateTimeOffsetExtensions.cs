namespace Horae;

/// <summary>
///     Provides extension methods for <see cref="DateTimeOffset" /> to handle Central European Time and UTC conversions,
///     as well as rounding and day boundary calculations.
/// </summary>
public static class DateTimeOffsetExtensions
{
    /// <summary>
    ///     Converts a <see cref="DateTimeOffset" /> value from Central European Time to UTC.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset" /> value to convert.</param>
    /// <returns>The converted <see cref="DateTimeOffset" /> value in UTC.</returns>
    public static DateTimeOffset CentralEuropeanToUtcTime(this DateTimeOffset dateTimeOffset) =>
        TimeZoneInfo.ConvertTime(dateTimeOffset, TimeZoneInfo.Utc);

    /// <summary>
    ///     Gets the end of the day for a <see cref="DateTimeOffset" /> value in the specified time zone.
    ///     The returned value is the exclusive upper boundary (i.e. the first instant of the following day),
    ///     which is the conventional pattern for half-open interval comparisons in trading systems.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset" /> value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar day. When <see langword="null"/>,
    ///     <see cref="HoareSettings.CurrentTimeZone"/> is used.
    /// </param>
    /// <returns>The first instant of the next calendar day in the given time zone.</returns>
    public static DateTimeOffset EndOfDay(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null) =>
        dateTimeOffset.StartOfDay(timeZone).AddDays(1);

    /// <summary>
    ///     Gets the end of the day ahead for a <see cref="DateTimeOffset" /> value in Central European Time.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset" /> value.</param>
    /// <returns>The <see cref="DateTimeOffset" /> value representing the end of the day ahead in Central European Time.</returns>
    public static DateTimeOffset EndOfDayAhead(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.EndOfIntraday().AddDays(1);

    /// <summary>
    ///     Converts a <see cref="DateTimeOffset" /> value to UTC and gets the end of the day ahead.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset" /> value.</param>
    /// <returns>The <see cref="DateTimeOffset" /> value representing the end of the day ahead in UTC.</returns>
    public static DateTimeOffset EndOfDayAheadUtc(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.EndOfDayAhead().CentralEuropeanToUtcTime();

    /// <summary>
    ///     Converts a <see cref="DateTimeOffset" /> value to UTC and gets the end of the day.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset" /> value.</param>
    /// <returns>The <see cref="DateTimeOffset" /> value representing the end of the day in UTC.</returns>
    public static DateTimeOffset EndOfDayUtc(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.EndOfDay().CentralEuropeanToUtcTime();

    /// <summary>
    ///     Gets the end of the intraday for a <see cref="DateTimeOffset" /> value in Central European Time.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset" /> value.</param>
    /// <returns>The <see cref="DateTimeOffset" /> value representing the end of the intraday in Central European Time.</returns>
    public static DateTimeOffset EndOfIntraday(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.StartOfIntraday().AddDays(1);

    /// <summary>
    ///     Converts a <see cref="DateTimeOffset" /> value to UTC and gets the end of the intraday.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset" /> value.</param>
    /// <returns>The <see cref="DateTimeOffset" /> value representing the end of the intraday in UTC.</returns>
    public static DateTimeOffset EndOfIntradayUtc(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.EndOfIntraday().CentralEuropeanToUtcTime();

    /// <summary>
    ///     Rounds down a <see cref="DateTimeOffset" /> value to the nearest specified <see cref="TimeSpan" /> in Central
    ///     European Time.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset" /> value to round down.</param>
    /// <param name="timeSpan">The <see cref="TimeSpan" /> to round down to.</param>
    /// <returns>The rounded down <see cref="DateTimeOffset" /> value in UTC.</returns>
    public static DateTimeOffset RoundDown(this DateTimeOffset dateTimeOffset, TimeSpan timeSpan)
    {
        var cet = dateTimeOffset.ToCentralEuropeanTime();
        var delta = cet.Ticks % timeSpan.Ticks;
        var rounded = new DateTimeOffset(cet.Ticks - delta, cet.Offset);
        return rounded.CentralEuropeanToUtcTime();
    }

    /// <summary>
    ///     Rounds a <see cref="DateTimeOffset" /> value to the nearest specified <see cref="TimeSpan" /> in Central European
    ///     Time.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset" /> value to round.</param>
    /// <param name="timeSpan">The <see cref="TimeSpan" /> to round to.</param>
    /// <returns>The rounded <see cref="DateTimeOffset" /> value in UTC.</returns>
    public static DateTimeOffset RoundToNearest(this DateTimeOffset dateTimeOffset, TimeSpan timeSpan)
    {
        var cet = dateTimeOffset.ToCentralEuropeanTime();
        var delta = cet.Ticks % timeSpan.Ticks;
        var roundUp = delta > timeSpan.Ticks / 2;
        var offset = roundUp ? timeSpan.Ticks : 0;
        var rounded = new DateTimeOffset(cet.Ticks + offset - delta, cet.Offset);
        return rounded.CentralEuropeanToUtcTime();
    }

    /// <summary>
    ///     Rounds up a <see cref="DateTimeOffset" /> value to the nearest specified <see cref="TimeSpan" /> in Central
    ///     European Time.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset" /> value to round up.</param>
    /// <param name="timeSpan">The <see cref="TimeSpan" /> to round up to.</param>
    /// <returns>The rounded up <see cref="DateTimeOffset" /> value in UTC.</returns>
    public static DateTimeOffset RoundUp(this DateTimeOffset dateTimeOffset, TimeSpan timeSpan)
    {
        var cet = dateTimeOffset.ToCentralEuropeanTime();
        var modTicks = cet.Ticks % timeSpan.Ticks;
        var delta = modTicks != 0 ? timeSpan.Ticks - modTicks : 0;
        var rounded = new DateTimeOffset(cet.Ticks + delta, cet.Offset);
        return rounded.CentralEuropeanToUtcTime();
    }

    /// <summary>
    ///     Gets the start of the day for a <see cref="DateTimeOffset" /> value in the specified time zone.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset" /> value.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the calendar day. When <see langword="null"/>,
    ///     <see cref="HoareSettings.CurrentTimeZone"/> is used.
    /// </param>
    /// <returns>The <see cref="DateTimeOffset" /> value representing midnight of the same calendar day.</returns>
    public static DateTimeOffset StartOfDay(this DateTimeOffset dateTimeOffset, TimeZoneInfo? timeZone = null)
    {
        var tz = timeZone ?? HoareSettings.CurrentTimeZone;
        var inTz = TimeZoneInfo.ConvertTime(dateTimeOffset, tz);
        return new DateTimeOffset(inTz.Year, inTz.Month, inTz.Day, 0, 0, 0, inTz.Offset);
    }

    /// <summary>
    ///     Gets the start of the day ahead for a <see cref="DateTimeOffset" /> value in Central European Time.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset" /> value.</param>
    /// <returns>The <see cref="DateTimeOffset" /> value representing the start of the day ahead in Central European Time.</returns>
    public static DateTimeOffset StartOfDayAhead(this DateTimeOffset dateTimeOffset) => dateTimeOffset.EndOfIntraday();

    /// <summary>
    ///     Converts a <see cref="DateTimeOffset" /> value to UTC and gets the start of the day ahead.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset" /> value.</param>
    /// <returns>The <see cref="DateTimeOffset" /> value representing the start of the day ahead in UTC.</returns>
    public static DateTimeOffset StartOfDayAheadUtc(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.StartOfDayAhead().CentralEuropeanToUtcTime();

    /// <summary>
    ///     Converts a <see cref="DateTimeOffset" /> value to UTC and gets the start of the day.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset" /> value.</param>
    /// <returns>The <see cref="DateTimeOffset" /> value representing the start of the day in UTC.</returns>
    public static DateTimeOffset StartOfDayUtc(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.StartOfDay().CentralEuropeanToUtcTime();

    /// <summary>
    ///     Gets the start of the intraday for a <see cref="DateTimeOffset" /> value in Central European Time.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset" /> value.</param>
    /// <returns>The <see cref="DateTimeOffset" /> value representing the start of the intraday in Central European Time.</returns>
    public static DateTimeOffset StartOfIntraday(this DateTimeOffset dateTimeOffset)
    {
        var cet = dateTimeOffset.ToCentralEuropeanTime();
        var date = new DateTimeOffset(cet.Year, cet.Month, cet.Day, 0, 0, 0, cet.Offset);
        return date;
    }

    /// <summary>
    ///     Converts a <see cref="DateTimeOffset" /> value to UTC and gets the start of the intraday.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset" /> value.</param>
    /// <returns>The <see cref="DateTimeOffset" /> value representing the start of the intraday in UTC.</returns>
    public static DateTimeOffset StartOfIntradayUtc(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.StartOfIntraday().CentralEuropeanToUtcTime();

    /// <summary>
    ///     Converts a <see cref="DateTimeOffset" /> value to Central European Time.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset" /> value to convert.</param>
    /// <returns>The converted <see cref="DateTimeOffset" /> value in Central European Time.</returns>
    public static DateTimeOffset ToCentralEuropeanTime(this DateTimeOffset dateTimeOffset) =>
        TimeZoneInfo.ConvertTime(dateTimeOffset, TimeZones.CentralEuropean);
}
