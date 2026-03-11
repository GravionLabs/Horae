namespace Horae;

/// <summary>
///     Global configuration settings for the Horae library.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="CurrentTimeZone" /> serves as the ambient default time zone for all library
///         operations that accept an optional <see cref="TimeZoneInfo" /> parameter. When a caller
///         does not supply an explicit time zone, the library falls back to this value.
///     </para>
///     <para>
///         <b>Thread safety:</b> This class uses a simple static property without synchronization.
///         Changing <see cref="CurrentTimeZone" /> while other threads are executing time-zone-aware
///         operations is not thread-safe. Set the value once at application start-up before any
///         concurrent access occurs.
///     </para>
/// </remarks>
public static class HoareSettings
{
    /// <summary>
    ///     Gets or sets the default time zone used by Horae extension methods when no explicit
    ///     <see cref="TimeZoneInfo" /> argument is provided.
    ///     Defaults to <see cref="TimeZoneInfo.Local" /> (the system's local time zone).
    /// </summary>
    /// <example>
    ///     <code>
    ///         // Set a global default once at application start-up:
    ///         HoareSettings.CurrentTimeZone = TimeZones.CentralEuropean;
    ///
    ///         // All subsequent calls without an explicit time zone will use CentralEuropean:
    ///         var end = myDateTimeOffset.EndOfDay();
    ///     </code>
    /// </example>
    public static TimeZoneInfo CurrentTimeZone { get; set; } = TimeZoneInfo.Local;
}
