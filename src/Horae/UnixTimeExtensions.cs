namespace Horae;

/// <summary>
///     Unix epoch timestamp helpers for <see cref="DateTimeOffset" />.
///     Inspired by Luxon's <c>DateTime.fromSeconds()</c>, <c>DateTime.fromMillis()</c>,
///     and <c>.toSeconds()</c> / <c>.toMillis()</c>.
/// </summary>
public static class UnixTimeExtensions
{
    /// <summary>
    ///     Creates a <see cref="DateTimeOffset" /> from a Unix timestamp expressed in whole seconds
    ///     since 1970-01-01T00:00:00Z.
    /// </summary>
    /// <param name="seconds">Seconds since the Unix epoch (may be negative for dates before 1970).</param>
    public static DateTimeOffset FromUnixSeconds(long seconds) =>
        DateTimeOffset.FromUnixTimeSeconds(seconds);

    /// <summary>
    ///     Creates a <see cref="DateTimeOffset" /> from a Unix timestamp expressed in milliseconds
    ///     since 1970-01-01T00:00:00Z.
    /// </summary>
    /// <param name="milliseconds">Milliseconds since the Unix epoch (may be negative for dates before 1970).</param>
    public static DateTimeOffset FromUnixMilliseconds(long milliseconds) =>
        DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);

    /// <summary>
    ///     Returns the number of whole seconds elapsed since the Unix epoch (1970-01-01T00:00:00Z).
    ///     Sub-second precision is truncated.
    /// </summary>
    public static long ToUnixSeconds(this DateTimeOffset dto) => dto.ToUnixTimeSeconds();

    /// <summary>
    ///     Returns the number of milliseconds elapsed since the Unix epoch (1970-01-01T00:00:00Z).
    ///     Sub-millisecond precision is truncated.
    /// </summary>
    public static long ToUnixMilliseconds(this DateTimeOffset dto) => dto.ToUnixTimeMilliseconds();
}
