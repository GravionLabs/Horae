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
///         <b>Isolation:</b> <see cref="CurrentTimeZone" /> is stored in an
///         <see cref="System.Threading.AsyncLocal{T}" />, which provides per-execution-context
///         isolation. A value set in one async context (e.g. a unit test or a background task)
///         is not visible in sibling contexts. Child contexts <em>inherit</em> the value of their
///         parent at the time they are started, but subsequent changes in the child do not
///         propagate back to the parent.
///     </para>
///     <para>
///         The recommended pattern is to set <see cref="CurrentTimeZone" /> once during
///         application start-up (before any async work is started) so that all operations
///         inherit the configured value. Alternatively, use <see cref="Use" /> for a scoped
///         override that is automatically reverted when the scope is disposed.
///     </para>
/// </remarks>
public static class HoareSettings
{
    // AsyncLocal provides per-execution-context storage. Each async test method and each
    // independently started Task gets its own slot, so mutations in one context do not
    // bleed into sibling or parent contexts.
    private static readonly AsyncLocal<TimeZoneInfo?> _currentTimeZone = new();

    /// <summary>
    ///     Gets or sets the default time zone used by Horae extension methods when no explicit
    ///     <see cref="TimeZoneInfo" /> argument is provided.
    ///     Defaults to <see cref="TimeZoneInfo.Local" /> (the system's local time zone).
    /// </summary>
    /// <remarks>
    ///     The value is stored per execution context (see <see cref="System.Threading.AsyncLocal{T}" />).
    ///     Set it once at application start-up so all async operations inherit it. For a
    ///     short-lived override, prefer <see cref="Use" /> instead.
    /// </remarks>
    /// <example>
    ///     <code>
    ///         // Set a global default once at application start-up:
    ///         HoareSettings.CurrentTimeZone = TimeZones.CentralEuropean;
    ///
    ///         // All subsequent calls without an explicit time zone will use CentralEuropean:
    ///         var end = myDateTimeOffset.EndOfDay();
    ///     </code>
    /// </example>
    public static TimeZoneInfo CurrentTimeZone
    {
        get => _currentTimeZone.Value ?? TimeZoneInfo.Local;
        set => _currentTimeZone.Value = value;
    }

    /// <summary>
    ///     Temporarily overrides <see cref="CurrentTimeZone" /> for the current execution context
    ///     and restores the previous value when the returned scope is disposed.
    /// </summary>
    /// <param name="timeZone">The time zone to use within the scope.</param>
    /// <returns>
    ///     An <see cref="IDisposable" /> that restores the previous <see cref="CurrentTimeZone" />
    ///     when disposed.
    /// </returns>
    /// <example>
    ///     <code>
    ///         using (HoareSettings.Use(TimeZones.CentralEuropean))
    ///         {
    ///             var start = DateTimeOffset.UtcNow.StartOfDay(); // uses CentralEuropean
    ///         }
    ///         // CurrentTimeZone is restored here
    ///     </code>
    /// </example>
    public static IDisposable Use(TimeZoneInfo timeZone) => new TimeZoneScope(timeZone);

    private sealed class TimeZoneScope : IDisposable
    {
        private readonly TimeZoneInfo _previous;

        internal TimeZoneScope(TimeZoneInfo timeZone)
        {
            _previous = CurrentTimeZone;
            CurrentTimeZone = timeZone;
        }

        public void Dispose() => CurrentTimeZone = _previous;
    }
}
