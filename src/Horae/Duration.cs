using System.Text.Json.Serialization;

using Horae.Converters;

namespace Horae;

/// <summary>
///     Represents an ISO 8601 duration (e.g. <c>PT15M</c>, <c>P1D</c>), wrapping a positive <see cref="TimeSpan" />.
///     Commonly used as a step size for interval splitting and time rounding.
/// </summary>
/// <example>
///     <code>
///         var r = Duration.QuarterHour;          // PT15M
///         TimeSpan ts = r;                          // implicit conversion
///         string json = JsonSerializer.Serialize(r); // "PT15M"
///
///         var parsed = Duration.Parse("PT15M");
///     </code>
/// </example>
[JsonConverter(typeof(DurationConverter))]
public readonly record struct Duration
{
    /// <summary>One minute — <c>PT1M</c>.</summary>
    public static readonly Duration Minute = new(TimeSpan.FromMinutes(1));

    /// <summary>Five minutes — <c>PT5M</c>.</summary>
    public static readonly Duration FiveMinutes = new(TimeSpan.FromMinutes(5));

    /// <summary>Fifteen minutes — <c>PT15M</c>.</summary>
    public static readonly Duration QuarterHour = new(TimeSpan.FromMinutes(15));

    /// <summary>Thirty minutes — <c>PT30M</c>.</summary>
    public static readonly Duration HalfHour = new(TimeSpan.FromMinutes(30));

    /// <summary>One hour — <c>PT1H</c>.</summary>
    public static readonly Duration Hour = new(TimeSpan.FromHours(1));

    /// <summary>One day — <c>P1D</c>.</summary>
    public static readonly Duration Day = new(TimeSpan.FromDays(1));

    /// <summary>
    ///     Initializes a new <see cref="Duration" /> with the given duration.
    /// </summary>
    /// <param name="value">The duration of one resolution step. Must be positive.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when <paramref name="value" /> is zero or negative.
    /// </exception>
    public Duration(TimeSpan value)
    {
        if (value <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(value),
                value,
                "Duration must be a positive time span.");
        }

        Value = value;
    }

    /// <summary>Gets the duration of one resolution step.</summary>
    public TimeSpan Value { get; }

    /// <summary>
    ///     Implicitly converts a <see cref="Duration" /> to its underlying <see cref="TimeSpan" />.
    /// </summary>
    public static implicit operator TimeSpan(Duration resolution) => resolution.Value;

    /// <summary>
    ///     Implicitly converts a <see cref="TimeSpan" /> to a <see cref="Duration" />.
    /// </summary>
    public static implicit operator Duration(TimeSpan timeSpan) => new(timeSpan);

    /// <summary>
    ///     Parses an ISO 8601 duration string (e.g. <c>PT15M</c>, <c>P1D</c>, <c>PT1H</c>)
    ///     into a <see cref="Duration" />.
    /// </summary>
    /// <param name="value">The ISO 8601 duration string to parse.</param>
    /// <returns>The parsed <see cref="Duration" />.</returns>
    /// <exception cref="FormatException">
    ///     Thrown when <paramref name="value" /> is not a recognised ISO 8601 duration.
    /// </exception>
    public static Duration Parse(string value)
    {
        if (!TryParse(value, out var result))
            throw new FormatException($"'{value}' is not a valid ISO 8601 duration.");

        return result;
    }

    /// <summary>
    ///     Tries to parse an ISO 8601 duration string into a <see cref="Duration" />.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <param name="result">
    ///     When this method returns <see langword="true" />, contains the parsed value;
    ///     otherwise the default value.
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if parsing succeeded; otherwise <see langword="false" />.
    /// </returns>
    public static bool TryParse(string? value, out Duration result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value) || value[0] != 'P')
            return false;

        // Reject year/month components: P{n}Y, P{n}M (calendar-dependent, not fixed durations).
        if (value.Contains('Y') || (!value.Contains('T') && value.Contains('M')))
            return false;

        var timeSpan = ParseIso8601(value);
        if (timeSpan is null || timeSpan.Value <= TimeSpan.Zero)
            return false;

        result = new Duration(timeSpan.Value);
        return true;
    }

    /// <summary>
    ///     Returns the ISO 8601 duration string for this resolution
    ///     (e.g. <c>PT15M</c> for 15 minutes, <c>P1D</c> for 1 day).
    /// </summary>
    public override string ToString() => ToIso8601(Value);

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static string ToIso8601(TimeSpan ts)
    {
        if (ts.TotalSeconds % 86400 == 0)
            return $"P{(long)ts.TotalDays}D";

        if (ts.TotalSeconds % 3600 == 0)
            return $"PT{(long)ts.TotalHours}H";

        if (ts.TotalSeconds % 60 == 0)
            return $"PT{(long)ts.TotalMinutes}M";

        return $"PT{(long)ts.TotalSeconds}S";
    }

    private static TimeSpan? ParseIso8601(string s)
    {
        // Supported forms: P{n}D  PT{n}H  PT{n}M  PT{n}S  PT{h}H{m}M{s}S (hours+min+sec)
        // We use a small hand-rolled parser for the subset we care about.
        var span = s.AsSpan()[1..]; // skip leading 'P'
        var days = 0L;
        var hours = 0L;
        var minutes = 0L;
        var seconds = 0L;
        var inTime = false;

        while (!span.IsEmpty)
        {
            if (span[0] == 'T')
            {
                inTime = true;
                span = span[1..];
                continue;
            }

            // Read numeric part.
            var numEnd = 0;
            while (numEnd < span.Length && char.IsDigit(span[numEnd]))
                numEnd++;

            if (numEnd == 0 || numEnd >= span.Length)
                return null;

            if (!long.TryParse(span[..numEnd], out var num))
                return null;

            var designator = span[numEnd];
            span = span[(numEnd + 1)..];

            switch (designator)
            {
                case 'D' when !inTime:
                    days = num;
                    break;
                case 'H' when inTime:
                    hours = num;
                    break;
                case 'M' when inTime:
                    minutes = num;
                    break;
                case 'S' when inTime:
                    seconds = num;
                    break;
                default:
                    return null;
            }
        }

        return TimeSpan.FromSeconds(days * 86400 + hours * 3600 + minutes * 60 + seconds);
    }
}
