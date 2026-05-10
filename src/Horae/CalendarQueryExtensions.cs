namespace Horae;

/// <summary>
///     Calendar query extension methods on <see cref="DateOnly" /> and <see cref="DateTimeOffset" />.
///     Inspired by Luxon's <c>isInLeapYear</c>, <c>daysInMonth</c>, <c>daysInYear</c>, <c>quarter</c>,
///     <c>ordinal</c>, and <c>isWeekend</c>.
/// </summary>
public static class CalendarQueryExtensions
{
    /// <summary>Returns <see langword="true" /> if the year of <paramref name="date" /> is a leap year.</summary>
    public static bool IsLeapYear(this DateOnly date) => DateTime.IsLeapYear(date.Year);

    /// <summary>Returns <see langword="true" /> if the UTC year of <paramref name="dto" /> is a leap year.</summary>
    public static bool IsLeapYear(this DateTimeOffset dto) => DateTime.IsLeapYear(dto.UtcDateTime.Year);

    /// <summary>Returns the number of days in the month of <paramref name="date" />.</summary>
    public static int DaysInMonth(this DateOnly date) => DateTime.DaysInMonth(date.Year, date.Month);

    /// <summary>Returns the number of days in the UTC month of <paramref name="dto" />.</summary>
    public static int DaysInMonth(this DateTimeOffset dto) =>
        DateTime.DaysInMonth(dto.UtcDateTime.Year, dto.UtcDateTime.Month);

    /// <summary>Returns the number of days in the year of <paramref name="date" /> (365 or 366).</summary>
    public static int DaysInYear(this DateOnly date) => DateTime.IsLeapYear(date.Year) ? 366 : 365;

    /// <summary>Returns the number of days in the UTC year of <paramref name="dto" /> (365 or 366).</summary>
    public static int DaysInYear(this DateTimeOffset dto) =>
        DateTime.IsLeapYear(dto.UtcDateTime.Year) ? 366 : 365;

    /// <summary>Returns the calendar quarter (1–4) of <paramref name="date" />.</summary>
    public static int Quarter(this DateOnly date) => (date.Month - 1) / 3 + 1;

    /// <summary>Returns the calendar quarter (1–4) of <paramref name="dto" /> (UTC).</summary>
    public static int Quarter(this DateTimeOffset dto) => (dto.UtcDateTime.Month - 1) / 3 + 1;

    /// <summary>Returns the day of the year (1–366, ordinal) of <paramref name="date" />.</summary>
    public static int DayOfYear(this DateOnly date) => date.DayOfYear;

    /// <summary>Returns the day of the year (1–366, ordinal) of <paramref name="dto" /> (UTC).</summary>
    public static int DayOfYear(this DateTimeOffset dto) => dto.UtcDateTime.DayOfYear;

    /// <summary>Returns <see langword="true" /> if <paramref name="date" /> falls on a Saturday or Sunday.</summary>
    public static bool IsWeekend(this DateOnly date) =>
        date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    /// <summary>
    ///     Returns <see langword="true" /> if <paramref name="dto" /> falls on a Saturday or Sunday
    ///     in the given <paramref name="timeZone" /> (falls back to <see cref="HoareSettings.CurrentTimeZone" />).
    /// </summary>
    public static bool IsWeekend(this DateTimeOffset dto, TimeZoneInfo? timeZone = null)
    {
        var tz = timeZone ?? HoareSettings.CurrentTimeZone ?? TimeZoneInfo.Utc;
        var local = TimeZoneInfo.ConvertTime(dto, tz);
        return local.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }

    /// <summary>Returns <see langword="true" /> if <paramref name="date" /> falls on a weekday (Monday–Friday).</summary>
    public static bool IsWeekday(this DateOnly date) => !date.IsWeekend();

    /// <summary>
    ///     Returns <see langword="true" /> if <paramref name="dto" /> falls on a weekday (Monday–Friday)
    ///     in the given <paramref name="timeZone" /> (falls back to <see cref="HoareSettings.CurrentTimeZone" />).
    /// </summary>
    public static bool IsWeekday(this DateTimeOffset dto, TimeZoneInfo? timeZone = null) =>
        !dto.IsWeekend(timeZone);
}
