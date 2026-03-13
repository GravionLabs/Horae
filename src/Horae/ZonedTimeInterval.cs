namespace Horae;

/// <summary>
///     Represents a half-open time interval [<see cref="Start" />, <see cref="End" />) whose
///     boundaries are expressed in a specific time zone rather than normalised to UTC.
///     Use this type when the local time context must be preserved — for example, a trading-day
///     boundary in CET or a session window such as "15:00–18:00 Europe/Berlin".
/// </summary>
/// <remarks>
///     For UTC-normalised intervals, use <see cref="TimeInterval" /> instead.
///     <para>
///         The constructor converts <c>start</c> and <c>end</c> to the supplied
///         <paramref name="timeZone" />'s local time, so <see cref="Start" /> and <see cref="End" />
///         always reflect the correct UTC offset for that instant (e.g. <c>+01:00</c> in winter,
///         <c>+02:00</c> in summer for CET/CEST).
///     </para>
///     <para>
///         <see cref="TimeZoneInfo" /> is required because a raw UTC offset (e.g. <c>+01:00</c>)
///         is insufficient for DST-safe operations — the same offset is shared by many time zones
///         with different daylight-saving rules.
///     </para>
/// </remarks>
/// <example>
///     <code>
///         // Trading day in CET:
///         var day = ZonedTimeInterval.Day(new DateOnly(2026, 6, 15), TimeZones.CentralEuropean);
///         // day.Start = 2026-06-15T00:00:00+02:00   (CEST)
///         // day.End   = 2026-06-16T00:00:00+02:00
///
///         // Session window:
///         var session = ZonedTimeInterval.Create(
///             new DateTimeOffset(2026, 1, 1, 15, 0, 0, TimeSpan.FromHours(1)),
///             new DateTimeOffset(2026, 1, 1, 18, 0, 0, TimeSpan.FromHours(1)),
///             TimeZones.CentralEuropean);
///
///         TimeInterval utc = day.ToUtc();
///     </code>
/// </example>
public readonly record struct ZonedTimeInterval
{
    /// <summary>
    ///     Initializes a new <see cref="ZonedTimeInterval" />.
    ///     Both endpoints are converted to the local time of <paramref name="timeZone" />.
    /// </summary>
    /// <param name="start">The inclusive start of the interval.</param>
    /// <param name="end">The exclusive end of the interval.</param>
    /// <param name="timeZone">
    ///     The time zone in which the interval is expressed.
    ///     Required for DST-safe operations such as <see cref="SplitByDays" />.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="timeZone" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="start" /> is greater than <paramref name="end" />.
    /// </exception>
    public ZonedTimeInterval(DateTimeOffset start, DateTimeOffset end, TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeZone);

        // Convert to the zone's local time so the offset reflects DST at that instant.
        Start    = TimeZoneInfo.ConvertTime(start, timeZone);
        End      = TimeZoneInfo.ConvertTime(end,   timeZone);
        TimeZone = timeZone;

        if (Start > End)
            throw new ArgumentException($"{nameof(start)} must be less than or equal to {nameof(end)}.");
    }

    /// <summary>Gets the inclusive start of the interval, expressed in <see cref="TimeZone" />'s local time.</summary>
    public DateTimeOffset Start { get; }

    /// <summary>Gets the exclusive end of the interval, expressed in <see cref="TimeZone" />'s local time.</summary>
    public DateTimeOffset End { get; }

    /// <summary>Gets the time zone in which the interval boundaries are expressed.</summary>
    public TimeZoneInfo TimeZone { get; }

    /// <summary>
    ///     Creates a new <see cref="ZonedTimeInterval" /> from the given endpoints.
    /// </summary>
    /// <param name="start">The inclusive start of the interval.</param>
    /// <param name="end">The exclusive end of the interval.</param>
    /// <param name="timeZone">The time zone for the interval.</param>
    /// <returns>A new <see cref="ZonedTimeInterval" /> instance.</returns>
    public static ZonedTimeInterval Create(DateTimeOffset start, DateTimeOffset end, TimeZoneInfo timeZone) =>
        new(start, end, timeZone);

    /// <summary>
    ///     Creates a <see cref="ZonedTimeInterval" /> spanning the full calendar day for the given
    ///     date in the specified time zone (midnight to the following midnight).
    /// </summary>
    /// <param name="date">The calendar date.</param>
    /// <param name="timeZone">The time zone in which the day boundaries are determined.</param>
    /// <returns>
    ///     A <see cref="ZonedTimeInterval" /> from midnight to the following midnight in
    ///     <paramref name="timeZone" />.
    /// </returns>
    public static ZonedTimeInterval Day(DateOnly date, TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeZone);
        var start = date.AsDateTimeOffset(timeZone);
        var end   = date.AddDays(1).AsDateTimeOffset(timeZone);
        return new ZonedTimeInterval(start, end, timeZone);
    }

    /// <summary>
    ///     Determines whether the interval contains the given instant.
    ///     Uses a half-open interval check: <c>Start ≤ value &lt; End</c>.
    ///     Comparison is offset-aware: <see cref="DateTimeOffset" /> equality accounts for UTC equivalence.
    /// </summary>
    /// <param name="value">The instant to test.</param>
    /// <returns>
    ///     <see langword="true" /> when <paramref name="value" /> falls within [Start, End);
    ///     otherwise <see langword="false" />.
    /// </returns>
    public bool Contains(DateTimeOffset value) => Start <= value && value < End;

    /// <summary>
    ///     Deconstructs the interval into its start and end endpoints.
    /// </summary>
    /// <param name="start">Receives the inclusive start of the interval.</param>
    /// <param name="end">Receives the exclusive end of the interval.</param>
    public void Deconstruct(out DateTimeOffset start, out DateTimeOffset end)
    {
        start = Start;
        end   = End;
    }

    /// <summary>
    ///     Returns a UTC-normalised <see cref="TimeInterval" /> with the same boundaries.
    ///     Both endpoints are converted to UTC (<c>+00:00</c> offset).
    /// </summary>
    /// <returns>A <see cref="TimeInterval" /> representing the same interval in UTC.</returns>
    public TimeInterval ToUtc() => new(Start, End);

    /// <summary>
    ///     Splits the interval into a sequence of instants at the given resolution.
    ///     Convenience overload that accepts a <see cref="Duration" /> instead of a raw
    ///     <see cref="TimeSpan" />.
    /// </summary>
    /// <param name="resolution">The step size to split by.</param>
    /// <returns>A sequence of <see cref="DateTimeOffset" /> values in <see cref="TimeZone" />'s local time.</returns>
    public IEnumerable<DateTimeOffset> SplitBy(Duration resolution) =>
        SplitBy(resolution.Value);

    /// <summary>
    ///     Splits the interval into a sequence of instants at the given resolution.
    ///     Boundaries are rounded outward to the nearest multiple of <paramref name="resolution" />.
    ///     For resolutions of one day or longer, iteration uses calendar-date arithmetic so that
    ///     DST transitions are handled correctly.
    /// </summary>
    /// <param name="resolution">The step size to split by.</param>
    /// <returns>A sequence of <see cref="DateTimeOffset" /> values in <see cref="TimeZone" />'s local time.</returns>
    public IEnumerable<DateTimeOffset> SplitBy(TimeSpan resolution)
    {
        var start = Start.RoundDown(resolution, TimeZone);
        var end   = End.RoundUp(resolution, TimeZone);

        if (resolution.TotalDays >= 1.0)
        {
            var days      = (int)resolution.TotalDays;
            var startDate = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(start, TimeZone).DateTime);
            var endDate   = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(end,   TimeZone).DateTime);

            for (var date = startDate; date < endDate; date = date.AddDays(days))
                yield return TimeZoneInfo.ConvertTime(date.AsDateTimeOffset(TimeZone), TimeZone);
        }
        else
        {
            for (var current = start; current < end; current = current.Add(resolution))
                yield return TimeZoneInfo.ConvertTime(current, TimeZone);
        }
    }

    /// <summary>
    ///     Splits the interval into a sequence of instants representing calendar-day boundaries
    ///     (midnight → midnight) in <see cref="TimeZone" />.
    ///     Iteration is performed over calendar dates so that DST transitions are handled correctly —
    ///     a spring day may be 23 hours and an autumn day 25 hours.
    /// </summary>
    /// <returns>
    ///     A sequence of <see cref="DateTimeOffset" /> values, one per calendar day, each representing
    ///     midnight in <see cref="TimeZone" />.
    /// </returns>
    public IEnumerable<DateTimeOffset> SplitByDays()
    {
        var startInTz = TimeZoneInfo.ConvertTime(Start, TimeZone);
        var endInTz   = TimeZoneInfo.ConvertTime(End,   TimeZone);

        var startDate = new DateOnly(startInTz.Year, startInTz.Month, startInTz.Day);
        var endDate   = new DateOnly(endInTz.Year,   endInTz.Month,   endInTz.Day);

        var endMidnight = endDate.AsDateTimeOffset(TimeZone);
        if (End <= endMidnight)
            endDate = endDate.AddDays(-1);

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
            yield return date.AsDateTimeOffset(TimeZone);
    }
}
