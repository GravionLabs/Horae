namespace Horae;

/// <summary>
///     Extension methods for converting <see cref="DateTimeOffset" /> values to specific
///     regional time zones or to UTC.
/// </summary>
public static class RegionExtensions
{
    /// <summary>
    ///     Converts a <see cref="DateTimeOffset" /> value to UTC
    ///     (<see cref="TimeSpan.Zero" /> offset).
    /// </summary>
    /// <param name="dto">The value to convert.</param>
    /// <returns>The equivalent instant expressed as UTC.</returns>
    public static DateTimeOffset ToUtc(this DateTimeOffset dto) =>
        TimeZoneInfo.ConvertTime(dto, TimeZoneInfo.Utc);

    /// <summary>Converts to Central European Time (CET/CEST, UTC+1/UTC+2).</summary>
    public static DateTimeOffset ToCentralEuropeanTime(this DateTimeOffset dto) =>
        dto.ToTimeZone(TimeZones.CentralEuropean);

    /// <summary>Converts to London / Western European Time (GMT/BST, UTC+0/UTC+1).</summary>
    public static DateTimeOffset ToLondonTime(this DateTimeOffset dto) =>
        dto.ToTimeZone(TimeZones.London);

    /// <summary>Converts to Lisbon / Western European Time (WET/WEST, UTC+0/UTC+1).</summary>
    public static DateTimeOffset ToLisbonTime(this DateTimeOffset dto) =>
        dto.ToTimeZone(TimeZones.Lisbon);

    /// <summary>Converts to Eastern European Time (EET/EEST, UTC+2/UTC+3).</summary>
    public static DateTimeOffset ToEasternEuropeTime(this DateTimeOffset dto) =>
        dto.ToTimeZone(TimeZones.EasternEurope);

    /// <summary>Converts to Moscow Standard Time (MSK, UTC+3, no DST).</summary>
    public static DateTimeOffset ToMoscowTime(this DateTimeOffset dto) =>
        dto.ToTimeZone(TimeZones.Moscow);

    /// <summary>Converts to Turkey Time / Istanbul (TRT, UTC+3, no DST).</summary>
    public static DateTimeOffset ToIstanbulTime(this DateTimeOffset dto) =>
        dto.ToTimeZone(TimeZones.Istanbul);

    /// <summary>Converts to US Eastern Time (EST/EDT, UTC-5/UTC-4).</summary>
    public static DateTimeOffset ToEasternUsTime(this DateTimeOffset dto) =>
        dto.ToTimeZone(TimeZones.EasternUs);

    /// <summary>Converts to US Pacific Time (PST/PDT, UTC-8/UTC-7).</summary>
    public static DateTimeOffset ToPacificUsTime(this DateTimeOffset dto) =>
        dto.ToTimeZone(TimeZones.PacificUs);

    /// <summary>Converts to Gulf Standard Time / Dubai (GST, UTC+4, no DST).</summary>
    public static DateTimeOffset ToDubaiTime(this DateTimeOffset dto) =>
        dto.ToTimeZone(TimeZones.Dubai);

    /// <summary>Converts to India Standard Time (IST, UTC+5:30, no DST).</summary>
    public static DateTimeOffset ToIndiaTime(this DateTimeOffset dto) =>
        dto.ToTimeZone(TimeZones.India);

    /// <summary>Converts to Japan Standard Time (JST, UTC+9, no DST).</summary>
    public static DateTimeOffset ToJapanTime(this DateTimeOffset dto) =>
        dto.ToTimeZone(TimeZones.Japan);

    /// <summary>Converts to Australian Eastern Time (AEST/AEDT, UTC+10/UTC+11).</summary>
    public static DateTimeOffset ToAustraliaEasternTime(this DateTimeOffset dto) =>
        dto.ToTimeZone(TimeZones.AustraliaEastern);
}

