namespace Horae;

/// <summary>
///     Extension methods for converting <see cref="DateTimeOffset"/> values between regional
///     time zones and UTC.
/// </summary>
public static class RegionExtensions
{
    // ── Generic helper ────────────────────────────────────────────────────────

    /// <summary>
    ///     Converts a <see cref="DateTimeOffset"/> value to UTC.
    /// </summary>
    /// <param name="dto">The value to convert.</param>
    /// <param name="sourceTimeZone">
    ///     The time zone that <paramref name="dto"/> is semantically expressed in.
    ///     For <see cref="DateTimeOffset"/> values, the UTC equivalent is already unambiguously
    ///     determined by the embedded offset; this parameter serves as documentation of intent.
    /// </param>
    /// <returns>The equivalent instant expressed as UTC (<see cref="TimeSpan.Zero"/> offset).</returns>
    public static DateTimeOffset ToUtc(this DateTimeOffset dto, TimeZoneInfo sourceTimeZone) =>
        TimeZoneInfo.ConvertTime(dto, TimeZoneInfo.Utc);

    // ── Europe ───────────────────────────────────────────────────────────────

    /// <summary>Converts from Central European Time (CET/CEST) to UTC.</summary>
    public static DateTimeOffset CentralEuropeanToUtc(this DateTimeOffset dto) =>
        dto.ToUtc(TimeZones.CentralEuropean);

    /// <summary>Converts from London / Western European Time (GMT/BST) to UTC.</summary>
    public static DateTimeOffset LondonToUtc(this DateTimeOffset dto) =>
        dto.ToUtc(TimeZones.London);

    /// <summary>Converts from Lisbon / Western European Time (WET/WEST) to UTC.</summary>
    public static DateTimeOffset LisbonToUtc(this DateTimeOffset dto) =>
        dto.ToUtc(TimeZones.Lisbon);

    /// <summary>Converts from Eastern European Time (EET/EEST) to UTC.</summary>
    public static DateTimeOffset EasternEuropeToUtc(this DateTimeOffset dto) =>
        dto.ToUtc(TimeZones.EasternEurope);

    /// <summary>Converts from Moscow Standard Time (MSK) to UTC.</summary>
    public static DateTimeOffset MoscowToUtc(this DateTimeOffset dto) =>
        dto.ToUtc(TimeZones.Moscow);

    /// <summary>Converts from Turkey Time / Istanbul (TRT) to UTC.</summary>
    public static DateTimeOffset IstanbulToUtc(this DateTimeOffset dto) =>
        dto.ToUtc(TimeZones.Istanbul);

    // ── Americas ─────────────────────────────────────────────────────────────

    /// <summary>Converts from US Eastern Time (EST/EDT) to UTC.</summary>
    public static DateTimeOffset EasternUsToUtc(this DateTimeOffset dto) =>
        dto.ToUtc(TimeZones.EasternUs);

    /// <summary>Converts from US Pacific Time (PST/PDT) to UTC.</summary>
    public static DateTimeOffset PacificUsToUtc(this DateTimeOffset dto) =>
        dto.ToUtc(TimeZones.PacificUs);

    // ── Asia ─────────────────────────────────────────────────────────────────

    /// <summary>Converts from Gulf Standard Time / Dubai (GST) to UTC.</summary>
    public static DateTimeOffset DubaiToUtc(this DateTimeOffset dto) =>
        dto.ToUtc(TimeZones.Dubai);

    /// <summary>Converts from India Standard Time (IST) to UTC.</summary>
    public static DateTimeOffset IndiaToUtc(this DateTimeOffset dto) =>
        dto.ToUtc(TimeZones.India);

    /// <summary>Converts from Japan Standard Time (JST) to UTC.</summary>
    public static DateTimeOffset JapanToUtc(this DateTimeOffset dto) =>
        dto.ToUtc(TimeZones.Japan);

    // ── Pacific ──────────────────────────────────────────────────────────────

    /// <summary>Converts from Australian Eastern Time (AEST/AEDT) to UTC.</summary>
    public static DateTimeOffset AustraliaEasternToUtc(this DateTimeOffset dto) =>
        dto.ToUtc(TimeZones.AustraliaEastern);
}

