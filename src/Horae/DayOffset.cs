using System.Text.Json.Serialization;

using Horae.Converters;

namespace Horae;

/// <summary>
///     Represents a calendar day relative to a reference instant, expressed as an integer day offset.
///     Resolves to a <see cref="DateOnly" /> in a specific time zone at the time of the call.
/// </summary>
/// <remarks>
///     <para>
///         A <see cref="DayOffset" /> remains valid indefinitely — it describes a relationship
///         ("one day ahead") rather than a fixed date. The concrete <see cref="DateOnly" /> is
///         obtained by calling <see cref="Resolve(TimeZoneInfo?)" /> or
///         <see cref="Resolve(DateTimeOffset, TimeZoneInfo?)" />.
///     </para>
///     <para>
///         Time zone resolution priority (highest to lowest):
///         <list type="number">
///             <item><description>The explicit <c>tz</c> argument passed to <c>Resolve</c>.</description></item>
///             <item><description>The <see cref="TimeZone" /> embedded in this instance.</description></item>
///             <item><description><see cref="HoareSettings.CurrentTimeZone" /> (ambient default).</description></item>
///         </list>
///     </para>
/// </remarks>
/// <example>
///     <code>
///         // Named presets
///         DateOnly today     = DayOffset.Today.Resolve(TimeZones.CentralEuropean);
///         DateOnly dayAhead  = DayOffset.DayAhead.Resolve(TimeZones.CentralEuropean);
///         DateOnly yesterday = DayOffset.Yesterday.Resolve(TimeZones.CentralEuropean);
///
///         // Arbitrary offset (implicit int conversion)
///         DateOnly twoDaysOut = ((DayOffset)2).Resolve(TimeZones.CentralEuropean);
///
///         // Self-contained with embedded timezone — resolves without any argument
///         var dto = new DayOffset(1, TimeZones.CentralEuropean);
///         DateOnly date = dto.Resolve();
///         // Serializes as: { "days": 1, "timezone": "Europe/Berlin" }
///
///         // Inject reference instant (e.g. in tests)
///         DateOnly pinned = new DayOffset(1).Resolve(
///             new DateTimeOffset(2026, 5, 9, 0, 0, 0, TimeSpan.Zero),
///             TimeZones.CentralEuropean);
///         // → DateOnly(2026, 5, 10)
///     </code>
/// </example>
[JsonConverter(typeof(DayOffsetConverter))]
public readonly record struct DayOffset
{
    /// <summary>Yesterday — offset of <c>-1</c> day, no embedded time zone.</summary>
    public static readonly DayOffset Yesterday = new(-1);

    /// <summary>Today — offset of <c>0</c> days, no embedded time zone.</summary>
    public static readonly DayOffset Today = new(0);

    /// <summary>Day-ahead — offset of <c>+1</c> day, no embedded time zone.</summary>
    public static readonly DayOffset DayAhead = new(1);

    /// <summary>
    ///     Initializes a new <see cref="DayOffset" /> with the given offset and an optional embedded time zone.
    /// </summary>
    /// <param name="days">
    ///     The number of calendar days relative to today. Negative values refer to past days.
    /// </param>
    /// <param name="timeZone">
    ///     The time zone used to determine "today" when no explicit <c>tz</c> is passed to
    ///     <see cref="Resolve(DateTimeOffset, TimeZoneInfo?)" />.
    ///     When <see langword="null" />, <see cref="HoareSettings.CurrentTimeZone" /> is used as fallback.
    /// </param>
    public DayOffset(int days, TimeZoneInfo? timeZone = null)
    {
        Days = days;
        TimeZone = timeZone;
    }

    /// <summary>Gets the number of calendar days relative to today. Negative values refer to past days.</summary>
    public int Days { get; }

    /// <summary>
    ///     Gets the time zone embedded in this instance, or <see langword="null" /> if none was specified.
    ///     When <see langword="null" />, <see cref="Resolve(DateTimeOffset, TimeZoneInfo?)" /> falls back
    ///     to <see cref="HoareSettings.CurrentTimeZone" />.
    /// </summary>
    public TimeZoneInfo? TimeZone { get; }

    /// <summary>
    ///     Resolves this offset to a <see cref="DateOnly" /> relative to the given
    ///     <paramref name="reference" /> instant.
    /// </summary>
    /// <param name="reference">
    ///     The reference instant used to determine "today". Typically <see cref="DateTimeOffset.UtcNow" />.
    /// </param>
    /// <param name="tz">
    ///     The time zone used to convert <paramref name="reference" /> to a calendar date.
    ///     When <see langword="null" />, the <see cref="TimeZone" /> embedded in this instance is used,
    ///     or <see cref="HoareSettings.CurrentTimeZone" /> as the final fallback.
    /// </param>
    /// <returns>The calendar date that is <see cref="Days" /> days from today in the effective time zone.</returns>
    public DateOnly Resolve(DateTimeOffset reference, TimeZoneInfo? tz = null)
    {
        var effectiveTz = tz ?? TimeZone ?? HoareSettings.CurrentTimeZone;
        var local = TimeZoneInfo.ConvertTime(reference, effectiveTz);
        return DateOnly.FromDateTime(local.DateTime).AddDays(Days);
    }

    /// <summary>
    ///     Resolves this offset to a <see cref="DateOnly" /> relative to <see cref="DateTimeOffset.UtcNow" />.
    /// </summary>
    /// <param name="tz">
    ///     The time zone used to convert the current instant to a calendar date.
    ///     When <see langword="null" />, the <see cref="TimeZone" /> embedded in this instance is used,
    ///     or <see cref="HoareSettings.CurrentTimeZone" /> as the final fallback.
    /// </param>
    /// <returns>The calendar date that is <see cref="Days" /> days from today in the effective time zone.</returns>
    public DateOnly Resolve(TimeZoneInfo? tz = null) => Resolve(DateTimeOffset.UtcNow, tz);

    /// <summary>
    ///     Implicitly converts an <see cref="int" /> to a <see cref="DayOffset" /> with no embedded time zone.
    /// </summary>
    public static implicit operator DayOffset(int days) => new(days);

    /// <summary>
    ///     Returns a string representation of this offset, e.g. <c>"+1"</c>, <c>"-1"</c>,
    ///     or <c>"+1 (Europe/Berlin)"</c> when a time zone is embedded.
    /// </summary>
    public override string ToString()
    {
        var sign = Days >= 0 ? $"+{Days}" : $"{Days}";
        return TimeZone is null ? sign : $"{sign} ({TimeZone.Id})";
    }
}
