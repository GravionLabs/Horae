namespace Horae;

/// <summary>
///     DST-awareness extension methods on <see cref="DateTimeOffset" />.
///     Inspired by Luxon's <c>isInDST</c> and <c>getPossibleOffsets()</c>.
/// </summary>
public static class DstQueryExtensions
{
    /// <summary>
    ///     Returns <see langword="true" /> if <paramref name="dto" /> represents an instant
    ///     that is in daylight saving time in the given <paramref name="timeZone" />.
    /// </summary>
    /// <param name="dto">The instant to query.</param>
    /// <param name="timeZone">The time zone to evaluate DST against.</param>
    public static bool IsInDst(this DateTimeOffset dto, TimeZoneInfo timeZone) =>
        timeZone.IsDaylightSavingTime(dto);

    /// <summary>
    ///     Returns <see langword="true" /> if <paramref name="dto" /> represents an instant
    ///     whose local wall-clock time in <paramref name="timeZone" /> is ambiguous —
    ///     i.e. it falls in the repeated hour during a fall-back DST transition.
    /// </summary>
    /// <param name="dto">The instant to query.</param>
    /// <param name="timeZone">The time zone in which to check for ambiguity.</param>
    public static bool IsDstAmbiguous(this DateTimeOffset dto, TimeZoneInfo timeZone) =>
        timeZone.IsAmbiguousTime(dto);
}
