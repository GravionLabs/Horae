using System.Globalization;

namespace Horae;

/// <summary>
///     ISO 8601 week-calendar extension methods on <see cref="DateOnly" /> and <see cref="DateTimeOffset" />.
///     Inspired by Luxon's <c>weekYear</c>, <c>weekNumber</c>, and <c>weekday</c>.
/// </summary>
public static class IsoWeekExtensions
{
    /// <summary>
    ///     Returns the ISO 8601 week number (1–53) of <paramref name="date" />.
    ///     Week 1 is the week containing the first Thursday of the year.
    /// </summary>
    public static int IsoWeekNumber(this DateOnly date) =>
        ISOWeek.GetWeekOfYear(date.ToDateTime(TimeOnly.MinValue));

    /// <summary>
    ///     Returns the ISO 8601 week number (1–53) of <paramref name="dto" /> (UTC).
    ///     Week 1 is the week containing the first Thursday of the year.
    /// </summary>
    public static int IsoWeekNumber(this DateTimeOffset dto) =>
        ISOWeek.GetWeekOfYear(dto.UtcDateTime);

    /// <summary>
    ///     Returns the ISO week-year of <paramref name="date" />.
    ///     This may differ from the calendar year for dates in early January or late December
    ///     (e.g. 2025-12-29 belongs to ISO week year 2026).
    /// </summary>
    public static int IsoWeekYear(this DateOnly date) =>
        ISOWeek.GetYear(date.ToDateTime(TimeOnly.MinValue));

    /// <summary>
    ///     Returns the ISO week-year of <paramref name="dto" /> (UTC).
    ///     This may differ from the calendar year for dates in early January or late December.
    /// </summary>
    public static int IsoWeekYear(this DateTimeOffset dto) =>
        ISOWeek.GetYear(dto.UtcDateTime);

    /// <summary>
    ///     Returns the ISO weekday of <paramref name="date" /> as an integer:
    ///     1 = Monday, 2 = Tuesday, …, 7 = Sunday.
    /// </summary>
    public static int IsoWeekday(this DateOnly date) =>
        date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek;

    /// <summary>
    ///     Returns the ISO weekday of <paramref name="dto" /> (UTC) as an integer:
    ///     1 = Monday, 2 = Tuesday, …, 7 = Sunday.
    /// </summary>
    public static int IsoWeekday(this DateTimeOffset dto) =>
        dto.UtcDateTime.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)dto.UtcDateTime.DayOfWeek;
}
