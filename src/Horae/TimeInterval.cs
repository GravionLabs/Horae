namespace Horae;

/// <summary>
///     Represents a half-open time interval [<see cref="Start" />, <see cref="End" />) with
///     timezone-aware boundaries. Both endpoints are stored as UTC (<see cref="TimeSpan.Zero" /> offset).
/// </summary>
/// <example>
///     <code>
///         var interval = TimeInterval.Day(new DateOnly(2026, 6, 15));
///         bool isNow = interval.Contains(DateTimeOffset.UtcNow);
///
///         foreach (var slot in interval.SplitBy(TimeSpan.FromHours(1)))
///             Console.WriteLine(slot);
///     </code>
/// </example>
public readonly record struct TimeInterval
{
    /// <summary>
    ///     Represents an empty time interval with both endpoints set to <see cref="DateTimeOffset.MinValue" />.
    /// </summary>
    public static readonly TimeInterval Empty = new(DateTimeOffset.MinValue, DateTimeOffset.MinValue);

    /// <summary>
    ///     Initializes a new instance of <see cref="TimeInterval" />.
    ///     Both endpoints are normalised to UTC.
    /// </summary>
    /// <param name="start">The inclusive start of the interval.</param>
    /// <param name="end">The exclusive end of the interval.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="start" /> is greater than <paramref name="end" />.
    /// </exception>
    public TimeInterval(DateTimeOffset start, DateTimeOffset end)
    {
        start = start.ToUniversalTime();
        end = end.ToUniversalTime();

        if (start > end)
            throw new ArgumentException($"{nameof(start)} must be less than or equal to {nameof(end)}.");

        Start = start;
        End = end;
    }

    /// <summary>Gets the inclusive start of the interval, expressed in UTC.</summary>
    public DateTimeOffset Start { get; }

    /// <summary>Gets the exclusive end of the interval, expressed in UTC.</summary>
    public DateTimeOffset End { get; }

    /// <summary>
    ///     Creates a new <see cref="TimeInterval" /> from the given endpoints.
    /// </summary>
    /// <param name="start">The inclusive start of the interval.</param>
    /// <param name="end">The exclusive end of the interval.</param>
    /// <returns>A new <see cref="TimeInterval" /> instance.</returns>
    public static TimeInterval Create(DateTimeOffset start, DateTimeOffset end) => new(start, end);

    /// <summary>
    ///     Creates a <see cref="TimeInterval" /> spanning the UTC calendar day for the given date.
    /// </summary>
    /// <param name="date">The calendar date.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the day boundaries.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>
    ///     A <see cref="TimeInterval" /> from midnight to the following midnight, expressed in UTC.
    /// </returns>
    public static TimeInterval Day(DateOnly date, TimeZoneInfo? timeZone = null)
    {
        var midnight = date.AsDateTimeOffset(timeZone);
        return new TimeInterval(midnight.StartOfDayUtc(timeZone), midnight.EndOfDayUtc(timeZone));
    }

    /// <summary>
    ///     Creates a <see cref="TimeInterval" /> for the intraday session containing the given instant.
    /// </summary>
    /// <param name="now">Any instant within the desired intraday session.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the session boundaries.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>
    ///     A <see cref="TimeInterval" /> from the start to the end of the intraday session, in UTC.
    /// </returns>
    public static TimeInterval Intraday(DateTimeOffset now, TimeZoneInfo? timeZone = null) =>
        new(now.StartOfIntradayUtc(timeZone), now.EndOfIntradayUtc(timeZone));

    /// <summary>
    ///     Creates a <see cref="TimeInterval" /> for the day-ahead session relative to the given instant.
    /// </summary>
    /// <param name="now">Any instant within the current day.</param>
    /// <param name="timeZone">
    ///     The time zone used to determine the session boundaries.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>
    ///     A <see cref="TimeInterval" /> covering the next calendar day in UTC.
    /// </returns>
    public static TimeInterval DayAhead(DateTimeOffset now, TimeZoneInfo? timeZone = null) =>
        new(now.StartOfDayAheadUtc(timeZone), now.EndOfDayAheadUtc(timeZone));

    /// <summary>
    ///     Determines whether the interval contains the given instant.
    ///     Uses a half-open interval check: <c>Start &lt;= value &lt; End</c>.
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
        end = End;
    }

    /// <summary>
    ///     Splits the interval into a sequence of UTC instants at the given resolution.
    ///     Convenience overload that accepts a <see cref="Resolution" /> instead of a raw
    ///     <see cref="TimeSpan" />.
    /// </summary>
    /// <param name="resolution">The step size to split by.</param>
    /// <param name="timeZone">
    ///     The time zone used for rounding.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>
    ///     A sequence of UTC <see cref="DateTimeOffset" /> values, one per slot.
    /// </returns>
    public IEnumerable<DateTimeOffset> SplitBy(Resolution resolution, TimeZoneInfo? timeZone = null) =>
        SplitBy(resolution.Value, timeZone);

    /// <summary>
    ///     Splits the interval into a sequence of UTC instants at the given resolution.
    ///     The interval boundaries are first rounded outward (down for <see cref="Start" />,
    ///     up for <see cref="End" />) to the nearest multiple of <paramref name="resolution" />.
    ///     For resolutions of one day or longer, iteration is performed via calendar-date
    ///     arithmetic so that DST transitions are handled correctly (a spring day is 23 h,
    ///     an autumn day is 25 h — not always 24 h).
    /// </summary>
    /// <param name="resolution">The step size to split by.</param>
    /// <param name="timeZone">
    ///     The time zone used for rounding.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>
    ///     A sequence of UTC <see cref="DateTimeOffset" /> values, one per slot.
    /// </returns>
    public IEnumerable<DateTimeOffset> SplitBy(TimeSpan resolution, TimeZoneInfo? timeZone = null)
    {
        var tz = timeZone ?? HoareSettings.CurrentTimeZone;
        var start = Start.RoundDown(resolution, tz);
        var end   = End.RoundUp(resolution, tz);

        // For day-granularity resolutions, adding a fixed TimeSpan is not DST-safe:
        // a CET spring day is only 23 h and an autumn day is 25 h.
        // Iterating via DateOnly.AddDays() in the target timezone avoids this.
        if (resolution.TotalDays >= 1.0)
        {
            var days      = (int)resolution.TotalDays;
            var startDate = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(start, tz).DateTime);
            var endDate   = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(end,   tz).DateTime);

            for (var date = startDate; date < endDate; date = date.AddDays(days))
                yield return date.AsDateTimeOffset(tz).ToUniversalTime();
        }
        else
        {
            for (var current = start; current < end; current = current.Add(resolution))
                yield return current;
        }
    }

    /// <summary>
    ///     Splits the interval into a sequence of UTC instants representing calendar-day
    ///     boundaries (midnight → midnight) in the specified time zone.
    ///     Iteration is performed over calendar dates in the given time zone, so each yielded
    ///     value is the exact UTC equivalent of midnight for that date — correctly accounting
    ///     for DST transitions where a day may be 23 or 25 hours long.
    /// </summary>
    /// <param name="timeZone">
    ///     The time zone used to determine calendar-day boundaries.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used.
    /// </param>
    /// <returns>
    ///     A sequence of UTC <see cref="DateTimeOffset" /> values, one per calendar day
    ///     in the given time zone, each representing the start (midnight) of that day in UTC.
    /// </returns>
    public IEnumerable<DateTimeOffset> SplitByDays(TimeZoneInfo? timeZone = null)
    {
        var tz = timeZone ?? HoareSettings.CurrentTimeZone;

        // Determine the first and last calendar dates in the target timezone.
        var startInTz = TimeZoneInfo.ConvertTime(Start, tz);
        var endInTz   = TimeZoneInfo.ConvertTime(End,   tz);

        var startDate = new DateOnly(startInTz.Year, startInTz.Month, startInTz.Day);
        var endDate   = new DateOnly(endInTz.Year,   endInTz.Month,   endInTz.Day);

        // If End falls exactly on midnight, that boundary day is excluded (half-open interval).
        var endMidnight = endDate.AsDateTimeOffset(tz);
        if (End <= endMidnight)
            endDate = endDate.AddDays(-1);

        // Yield midnight of each calendar day in tz as UTC.
        // Using DateOnly.AddDays(1) ensures DST-safe iteration: each step advances
        // exactly one calendar day, regardless of whether that day is 23 or 25 hours.
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
            yield return date.AsDateTimeOffset(tz).ToUniversalTime();
    }
}
