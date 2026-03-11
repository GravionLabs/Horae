namespace Horae;

/// <summary>
///     Provides a collection of commonly used time zones as <see cref="TimeZoneInfo" /> instances.
///     All time zones use IANA IDs, which are supported on all platforms with .NET 6 and later.
/// </summary>
public static class TimeZones
{
    /// <summary>
    ///     Gets the Coordinated Universal Time (UTC) time zone.
    ///     This is the same instance as <see cref="TimeZoneInfo.Utc" />.
    /// </summary>
    public static readonly TimeZoneInfo Utc = TimeZoneInfo.Utc;


    /// <summary>
    ///     Gets the Central European time zone (CET/CEST, UTC+1/UTC+2).
    ///     Example cities: Berlin, Paris, Rome, Vienna, Warsaw.
    /// </summary>
    public static readonly TimeZoneInfo CentralEuropean = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");

    /// <summary>
    ///     Gets the Western European / London time zone (GMT/BST, UTC+0/UTC+1).
    /// </summary>
    public static readonly TimeZoneInfo London = TimeZoneInfo.FindSystemTimeZoneById("Europe/London");

    /// <summary>
    ///     Gets the Western European time zone for Lisbon (WET/WEST, UTC+0/UTC+1).
    /// </summary>
    public static readonly TimeZoneInfo Lisbon = TimeZoneInfo.FindSystemTimeZoneById("Europe/Lisbon");

    /// <summary>
    ///     Gets the Eastern European time zone (EET/EEST, UTC+2/UTC+3).
    ///     Example cities: Athens, Helsinki, Kyiv, Bucharest.
    /// </summary>
    public static readonly TimeZoneInfo EasternEurope = TimeZoneInfo.FindSystemTimeZoneById("Europe/Athens");

    /// <summary>
    ///     Gets the Moscow Standard Time (MSK, UTC+3, no DST).
    /// </summary>
    public static readonly TimeZoneInfo Moscow = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");

    /// <summary>
    ///     Gets the Turkey Time zone for Istanbul (TRT, UTC+3, no DST).
    /// </summary>
    public static readonly TimeZoneInfo Istanbul = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");


    /// <summary>
    ///     Gets the Eastern Time zone for the US and Canada (EST/EDT, UTC-5/UTC-4).
    ///     Example cities: New York, Toronto, Boston.
    /// </summary>
    public static readonly TimeZoneInfo EasternUs = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");

    /// <summary>
    ///     Gets the Pacific Time zone for the US and Canada (PST/PDT, UTC-8/UTC-7).
    ///     Example cities: Los Angeles, Seattle, Vancouver.
    /// </summary>
    public static readonly TimeZoneInfo PacificUs = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");


    /// <summary>
    ///     Gets the Gulf Standard Time for Dubai (GST, UTC+4, no DST).
    /// </summary>
    public static readonly TimeZoneInfo Dubai = TimeZoneInfo.FindSystemTimeZoneById("Asia/Dubai");

    /// <summary>
    ///     Gets the India Standard Time (IST, UTC+5:30, no DST).
    /// </summary>
    public static readonly TimeZoneInfo India = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");

    /// <summary>
    ///     Gets the Japan Standard Time (JST, UTC+9, no DST).
    /// </summary>
    public static readonly TimeZoneInfo Japan = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");


    /// <summary>
    ///     Gets the Australian Eastern Time zone (AEST/AEDT, UTC+10/UTC+11).
    ///     Example cities: Sydney, Melbourne, Brisbane.
    /// </summary>
    public static readonly TimeZoneInfo AustraliaEastern = TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney");
}
