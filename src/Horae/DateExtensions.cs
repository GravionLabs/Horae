namespace Horae;

/// <summary>
///     Fluent date-construction extensions on <see cref="int" /> and <see cref="DateTimeOffset" />.
///     All month methods return midnight UTC; <c>At</c>, <c>Before</c> and <c>After</c>
///     preserve (or derive from) the offset of the source value.
/// </summary>
/// <example>
///     <code>
/// DateTimeOffset dt = 15.March(2026).At(9, 30);
/// DateTimeOffset yesterday = 1.Hours().Before(DateTimeOffset.UtcNow);
/// </code>
/// </example>
public static class DateExtensions
{
    /// <summary>Returns midnight UTC on the given <paramref name="day" /> of January in <paramref name="year" />.</summary>
    public static DateTimeOffset January(this int day, int year) =>
        new(year, 1, day, 0, 0, 0, TimeSpan.Zero);

    /// <summary>Returns midnight UTC on the given <paramref name="day" /> of February in <paramref name="year" />.</summary>
    public static DateTimeOffset February(this int day, int year) =>
        new(year, 2, day, 0, 0, 0, TimeSpan.Zero);

    /// <summary>Returns midnight UTC on the given <paramref name="day" /> of March in <paramref name="year" />.</summary>
    public static DateTimeOffset March(this int day, int year) =>
        new(year, 3, day, 0, 0, 0, TimeSpan.Zero);

    /// <summary>Returns midnight UTC on the given <paramref name="day" /> of April in <paramref name="year" />.</summary>
    public static DateTimeOffset April(this int day, int year) =>
        new(year, 4, day, 0, 0, 0, TimeSpan.Zero);

    /// <summary>Returns midnight UTC on the given <paramref name="day" /> of May in <paramref name="year" />.</summary>
    public static DateTimeOffset May(this int day, int year) =>
        new(year, 5, day, 0, 0, 0, TimeSpan.Zero);

    /// <summary>Returns midnight UTC on the given <paramref name="day" /> of June in <paramref name="year" />.</summary>
    public static DateTimeOffset June(this int day, int year) =>
        new(year, 6, day, 0, 0, 0, TimeSpan.Zero);

    /// <summary>Returns midnight UTC on the given <paramref name="day" /> of July in <paramref name="year" />.</summary>
    public static DateTimeOffset July(this int day, int year) =>
        new(year, 7, day, 0, 0, 0, TimeSpan.Zero);

    /// <summary>Returns midnight UTC on the given <paramref name="day" /> of August in <paramref name="year" />.</summary>
    public static DateTimeOffset August(this int day, int year) =>
        new(year, 8, day, 0, 0, 0, TimeSpan.Zero);

    /// <summary>Returns midnight UTC on the given <paramref name="day" /> of September in <paramref name="year" />.</summary>
    public static DateTimeOffset September(this int day, int year) =>
        new(year, 9, day, 0, 0, 0, TimeSpan.Zero);

    /// <summary>Returns midnight UTC on the given <paramref name="day" /> of October in <paramref name="year" />.</summary>
    public static DateTimeOffset October(this int day, int year) =>
        new(year, 10, day, 0, 0, 0, TimeSpan.Zero);

    /// <summary>Returns midnight UTC on the given <paramref name="day" /> of November in <paramref name="year" />.</summary>
    public static DateTimeOffset November(this int day, int year) =>
        new(year, 11, day, 0, 0, 0, TimeSpan.Zero);

    /// <summary>Returns midnight UTC on the given <paramref name="day" /> of December in <paramref name="year" />.</summary>
    public static DateTimeOffset December(this int day, int year) =>
        new(year, 12, day, 0, 0, 0, TimeSpan.Zero);

    /// <summary>
    ///     Returns a new <see cref="DateTimeOffset" /> with the calendar date from <paramref name="date" />
    ///     and the time-of-day replaced by <paramref name="time" />. The original UTC offset is preserved.
    /// </summary>
    /// <example>
    ///     <code>
    /// DateTimeOffset dt = 15.March(2026).At(TimeSpan.FromHours(9.5)); // 09:30 UTC
    /// </code>
    /// </example>
    public static DateTimeOffset At(this DateTimeOffset date, TimeSpan time)
    {
        var midnight = new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, date.Offset);
        return midnight + time;
    }

    /// <summary>
    ///     Returns a new <see cref="DateTimeOffset" /> with the calendar date from <paramref name="date" />
    ///     and the time-of-day set to the specified components. The original UTC offset is preserved.
    /// </summary>
    /// <param name="date">The source date whose calendar date and UTC offset are used.</param>
    /// <param name="hours">Hours component (0–23).</param>
    /// <param name="minutes">Minutes component (0–59).</param>
    /// <param name="seconds">Seconds component (0–59). Defaults to 0.</param>
    /// <param name="milliseconds">Milliseconds component (0–999). Defaults to 0.</param>
    /// <param name="microseconds">Microseconds component (0–999). Defaults to 0.</param>
    /// <param name="nanoseconds">Nanoseconds component (0–999). Defaults to 0.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when <paramref name="microseconds" /> or <paramref name="nanoseconds" /> is outside 0–999.
    /// </exception>
    /// <example>
    ///     <code>
    /// DateTimeOffset dt = 15.March(2026).At(9, 30, 15); // 09:30:15 UTC
    /// </code>
    /// </example>
    public static DateTimeOffset At(
        this DateTimeOffset date,
        int hours,
        int minutes,
        int seconds = 0,
        int milliseconds = 0,
        int microseconds = 0,
        int nanoseconds = 0)
    {
        if (microseconds is < 0 or > 999)
            throw new ArgumentOutOfRangeException(nameof(microseconds), "Valid values are between 0 and 999");

        if (nanoseconds is < 0 or > 999)
            throw new ArgumentOutOfRangeException(nameof(nanoseconds), "Valid values are between 0 and 999");

        var value = new DateTimeOffset(date.Year,
            date.Month,
            date.Day,
            hours,
            minutes,
            seconds,
            milliseconds,
            date.Offset);

        if (microseconds != 0)
            value += TimeSpan.FromMicroseconds(microseconds);

        if (nanoseconds != 0)
            value += new TimeSpan(nanoseconds / 100); // 1 tick = 100 ns; sub-100 ns precision is lost

        return value;
    }

    /// <summary>
    ///     Returns the <see cref="DateTimeOffset" /> that is <paramref name="timeDifference" /> before
    ///     <paramref name="sourceDateTime" />.
    /// </summary>
    /// <example>
    ///     <code>
    /// DateTimeOffset yesterday = TimeSpan.FromDays(1).Before(DateTimeOffset.UtcNow);
    /// </code>
    /// </example>
    public static DateTimeOffset Before(this TimeSpan timeDifference, DateTimeOffset sourceDateTime) =>
        sourceDateTime - timeDifference;

    /// <summary>
    ///     Returns the <see cref="DateTimeOffset" /> that is <paramref name="timeDifference" /> after
    ///     <paramref name="sourceDateTime" />.
    /// </summary>
    /// <example>
    ///     <code>
    /// DateTimeOffset nextWeek = TimeSpan.FromDays(7).After(DateTimeOffset.UtcNow);
    /// </code>
    /// </example>
    public static DateTimeOffset After(this TimeSpan timeDifference, DateTimeOffset sourceDateTime) =>
        sourceDateTime + timeDifference;

    /// <summary>
    ///     Returns a new <see cref="DateTimeOffset" /> with <paramref name="nanoseconds" /> added.
    ///     Uses <see cref="TimeSpan.FromNanoseconds" /> internally; precision is limited to 100 ns (one tick).
    /// </summary>
    /// <remarks>
    ///     <see cref="DateTimeOffset" /> does not have a native <c>AddNanoseconds</c> method;
    ///     this extension fills the gap. Values below 100 ns will be rounded down to the nearest tick.
    /// </remarks>
    public static DateTimeOffset AddNanoseconds(this DateTimeOffset self, long nanoseconds) =>
        self + new TimeSpan(nanoseconds / 100); // 1 tick = 100 ns; sub-100 ns precision is lost
}
