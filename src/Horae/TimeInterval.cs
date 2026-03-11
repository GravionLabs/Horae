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
    ///     The interval boundaries are first rounded outward (down for <see cref="Start" />,
    ///     up for <see cref="End" />) to the nearest multiple of <paramref name="resolution" />.
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
        var start = Start.RoundDown(resolution, timeZone);
        var end = End.RoundUp(resolution, timeZone);

        for (var current = start; current < end; current = current.Add(resolution))
            yield return current;
    }

    /// <summary>
    ///     Splits the interval into a sequence of UTC instants representing CET delivery-date
    ///     boundaries (midnight CET → midnight CET, one per calendar day in Central European Time).
    ///     This is the conventional split used for energy-market delivery periods.
    /// </summary>
    /// <returns>
    ///     A sequence of UTC <see cref="DateTimeOffset" /> values, one per CET calendar day,
    ///     each representing the start of that day in UTC.
    /// </returns>
    public IEnumerable<DateTimeOffset> SplitByDeliveryDates()
    {
        var day = TimeSpan.FromDays(1);

        // Round outward to UTC day boundaries, then express in CET for iteration.
        var startDate = Start.RoundDown(day, TimeZoneInfo.Utc).ToCentralEuropeanTime();
        var endDate = End.RoundUp(day, TimeZoneInfo.Utc).ToCentralEuropeanTime();

        for (var current = startDate; current < endDate; current = current.Add(day))
            yield return current.CentralEuropeanToUtcTime();
    }
}
