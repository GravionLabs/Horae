namespace Horae;

/// <summary>
///     Extension methods on <see cref="int" />, <see cref="long" />, <see cref="double" /> and <see cref="TimeSpan" />
///     for a fluent, readable way of constructing <see cref="Duration" /> and <see cref="TimeSpan" /> values.
///     Methods that represent whole-unit step sizes (<c>Seconds</c>, <c>Minutes</c>, <c>Hours</c>, <c>Days</c>)
///     return <see cref="Duration" />; precision methods (<c>Ticks</c>, <c>Nanoseconds</c>, etc.) return
///     <see cref="TimeSpan" />.
/// </summary>
/// <example>
///     <code>
/// // Fluent Duration construction:
/// Duration d = 12.Hours();
/// Duration d = 15.Minutes();
///
/// // Compose with And (Duration chain):
/// Duration d = 23.Hours().And(59.Minutes()).And(30.Seconds());
///
/// // Mix with TimeSpan (And returns TimeSpan via implicit conversion):
/// TimeSpan ts = 1.Days().And(500.Milliseconds());
/// </code>
/// </example>
public static class TimeSpanExtensions
{
    /// <summary>Number of ticks in one microsecond (= 10).</summary>
    public const long TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;

    /// <summary>
    ///     Number of ticks in one nanosecond (= 0.01).
    ///     <see cref="TimeSpan" /> has 100 ns resolution, so sub-100 ns values are rounded.
    /// </summary>
    public const double TicksPerNanosecond = TicksPerMicrosecond / 1000d;

    /// <summary>Returns a <see cref="TimeSpan" /> for the given number of <paramref name="ticks" />.</summary>
    public static TimeSpan Ticks(this int ticks) => TimeSpan.FromTicks(ticks);

    /// <summary>Returns a <see cref="TimeSpan" /> for the given number of <paramref name="ticks" />.</summary>
    public static TimeSpan Ticks(this long ticks) => TimeSpan.FromTicks(ticks);

    /// <summary>
    ///     Returns a <see cref="TimeSpan" /> for the given number of nanoseconds.
    ///     Sub-100 ns precision is lost because <see cref="TimeSpan" /> is tick-based (1 tick = 100 ns).
    /// </summary>
    public static TimeSpan Nanoseconds(this int nanoseconds) =>
        ((long)Math.Round(nanoseconds * TicksPerNanosecond)).Ticks();

    /// <summary>
    ///     Returns a <see cref="TimeSpan" /> for the given number of nanoseconds.
    ///     Sub-100 ns precision is lost because <see cref="TimeSpan" /> is tick-based (1 tick = 100 ns).
    /// </summary>
    public static TimeSpan Nanoseconds(this long nanoseconds) =>
        ((long)Math.Round(nanoseconds * TicksPerNanosecond)).Ticks();

    /// <summary>Returns a <see cref="TimeSpan" /> for the given number of microseconds.</summary>
    public static TimeSpan Microseconds(this int microseconds) =>
        (microseconds * TicksPerMicrosecond).Ticks();

    /// <summary>Returns a <see cref="TimeSpan" /> for the given number of microseconds.</summary>
    public static TimeSpan Microseconds(this long microseconds) =>
        (microseconds * TicksPerMicrosecond).Ticks();

    /// <summary>Returns a <see cref="TimeSpan" /> for the given number of milliseconds.</summary>
    public static TimeSpan Milliseconds(this int milliseconds) =>
        TimeSpan.FromMilliseconds(milliseconds);

    /// <summary>Returns a <see cref="TimeSpan" /> for the given number of milliseconds.</summary>
    public static TimeSpan Milliseconds(this double milliseconds) =>
        TimeSpan.FromMilliseconds(milliseconds);

    /// <summary>Returns a <see cref="Duration" /> for the given number of seconds.</summary>
    public static Duration Seconds(this int seconds) =>
        new(TimeSpan.FromSeconds(seconds));

    /// <summary>Returns a <see cref="Duration" /> for the given number of seconds.</summary>
    public static Duration Seconds(this double seconds) =>
        new(TimeSpan.FromSeconds(seconds));

    /// <summary>
    ///     Returns a <see cref="TimeSpan" /> for the given number of seconds, plus <paramref name="offset" />.
    ///     Useful for composing durations inline: <c>30.Seconds(5.Minutes())</c>.
    /// </summary>
    public static TimeSpan Seconds(this int seconds, TimeSpan offset) =>
        TimeSpan.FromSeconds(seconds).Add(offset);


    /// <summary>Returns a <see cref="Duration" /> for the given number of minutes.</summary>
    public static Duration Minutes(this int minutes) =>
        new(TimeSpan.FromMinutes(minutes));

    /// <summary>Returns a <see cref="Duration" /> for the given number of minutes.</summary>
    public static Duration Minutes(this double minutes) =>
        new(TimeSpan.FromMinutes(minutes));

    /// <summary>
    ///     Returns a <see cref="TimeSpan" /> for the given number of minutes, plus <paramref name="offset" />.
    /// </summary>
    public static TimeSpan Minutes(this int minutes, TimeSpan offset) =>
        TimeSpan.FromMinutes(minutes).Add(offset);


    /// <summary>Returns a <see cref="Duration" /> for the given number of hours.</summary>
    public static Duration Hours(this int hours) =>
        new(TimeSpan.FromHours(hours));

    /// <summary>Returns a <see cref="Duration" /> for the given number of hours.</summary>
    public static Duration Hours(this double hours) =>
        new(TimeSpan.FromHours(hours));

    /// <summary>
    ///     Returns a <see cref="TimeSpan" /> for the given number of hours, plus <paramref name="offset" />.
    /// </summary>
    public static TimeSpan Hours(this int hours, TimeSpan offset) =>
        TimeSpan.FromHours(hours).Add(offset);


    /// <summary>Returns a <see cref="Duration" /> for the given number of days.</summary>
    public static Duration Days(this int days) =>
        new(TimeSpan.FromDays(days));

    /// <summary>Returns a <see cref="Duration" /> for the given number of days.</summary>
    public static Duration Days(this double days) =>
        new(TimeSpan.FromDays(days));

    /// <summary>
    ///     Returns a <see cref="TimeSpan" /> for the given number of days, plus <paramref name="offset" />.
    /// </summary>
    public static TimeSpan Days(this int days, TimeSpan offset) =>
        TimeSpan.FromDays(days).Add(offset);

    /// <summary>
    ///     Adds <paramref name="offset" /> to this <see cref="TimeSpan" />.
    ///     Enables readable chaining: <c>23.Hours().And(59.Minutes())</c>.
    /// </summary>
    /// <example>
    ///     <code>
    /// TimeSpan duration = 1.Days().And(12.Hours()).And(30.Minutes());
    /// </code>
    /// </example>
    public static TimeSpan And(this TimeSpan sourceTime, TimeSpan offset) =>
        sourceTime.Add(offset);

    /// <summary>
    ///     Adds <paramref name="offset" /> to this <see cref="Duration" />, returning a new <see cref="Duration" />.
    ///     Enables fully-typed Duration chains: <c>1.Days().And(2.Hours()).And(30.Minutes())</c>.
    /// </summary>
    /// <example>
    ///     <code>
    /// Duration d = 1.Days().And(2.Hours()).And(30.Minutes());
    /// </code>
    /// </example>
    public static Duration And(this Duration duration, Duration offset) =>
        new(duration.Value.Add(offset.Value));
}
