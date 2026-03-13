using System.Text.Json;
using System.Text.Json.Serialization;
using Horae.Converters;
using Shouldly;

namespace Horae.Tests;

[TestFixture] [Category("UnitTest")]
public class IsoStringTests
{
    [Test]
    public void ToIsoString_DateTimeOffset_ShouldFormatWholeSecondsWithZ()
    {
        var dto = new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero);

        dto.ToIsoString().ShouldBe("2026-01-01T23:00:00Z");
    }

    [Test]
    public void ToIsoString_DateTimeOffset_ShouldConvertToUtcBeforeFormatting()
    {
        // +01:00 → UTC 22:00.
        var dto = new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.FromHours(1));

        dto.ToIsoString().ShouldBe("2026-01-01T22:00:00Z");
    }

    [Test]
    public void ToIsoString_DateTimeOffset_ShouldOmitSubSecondsWhenNonZero()
    {
        // Default is Seconds: sub-second digits are always omitted.
        var dto = new DateTimeOffset(2026, 1, 1, 23, 0, 0, 500, TimeSpan.Zero);

        dto.ToIsoString().ShouldBe("2026-01-01T23:00:00Z");
    }

    [Test]
    public void ToIsoString_DateTimeOffset_ShouldOmitSubSecondsWhenZero()
    {
        var dto = new DateTimeOffset(2026, 6, 15, 12, 30, 45, TimeSpan.Zero);

        dto.ToIsoString().ShouldNotContain(".");
    }

    [Test]
    public void ToIsoString_NullableDateTimeOffset_ShouldReturnNullForNull()
    {
        DateTimeOffset? dto = null;

        dto.ToIsoString().ShouldBeNull();
    }

    [Test]
    public void ToIsoString_NullableDateTimeOffset_ShouldFormatValue()
    {
        DateTimeOffset? dto = new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero);

        dto.ToIsoString().ShouldBe("2026-01-01T23:00:00Z");
    }

    [Test]
    public void ToIsoString_DateOnly_ShouldFormatAsIsoDate()
    {
        new DateOnly(2026, 1, 1).ToIsoString().ShouldBe("2026-01-01");
    }

    [Test]
    public void ToIsoString_DateOnly_ShouldPadMonthAndDayWithLeadingZero()
    {
        new DateOnly(2026, 3, 9).ToIsoString().ShouldBe("2026-03-09");
    }

    [Test]
    public void ToIsoString_NullableDateOnly_ShouldReturnNullForNull()
    {
        DateOnly? date = null;

        date.ToIsoString().ShouldBeNull();
    }

    [Test]
    public void ToIsoString_NullableDateOnly_ShouldFormatValue()
    {
        DateOnly? date = new DateOnly(2026, 1, 1);

        date.ToIsoString().ShouldBe("2026-01-01");
    }

    [Test]
    public void ParseIsoDateTime_ShouldParseUtcZString()
    {
        var result = "2026-01-01T23:00:00Z".ParseIsoDateTime();

        result.UtcDateTime.ShouldBe(new DateTime(2026, 1, 1, 23, 0, 0, DateTimeKind.Utc));
        result.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Test]
    public void ParseIsoDateTime_ShouldParseOffsetString()
    {
        // "+01:00" offset preserved as-is.
        var result = "2026-01-01T23:00:00+01:00".ParseIsoDateTime();

        result.Offset.ShouldBe(TimeSpan.FromHours(1));
        result.Hour.ShouldBe(23);
    }

    [Test]
    public void ParseIsoDateTime_ShouldRoundTripWithToIsoString()
    {
        var original = new DateTimeOffset(2026, 6, 15, 14, 30, 0, TimeSpan.Zero);

        var result = original.ToIsoString().ParseIsoDateTime();

        result.UtcDateTime.ShouldBe(original.UtcDateTime);
    }

    [Test]
    public void ParseIsoDateTime_ShouldThrowForInvalidString()
    {
        Should.Throw<FormatException>(() => "not-a-date".ParseIsoDateTime());
    }

    [Test]
    public void TryParseIsoDateTime_ShouldReturnTrueForValidString()
    {
        var ok = "2026-01-01T23:00:00Z".TryParseIsoDateTime(out var result);

        ok.ShouldBeTrue();
        result.UtcDateTime.ShouldBe(new DateTime(2026, 1, 1, 23, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public void TryParseIsoDateTime_ShouldReturnFalseForInvalidString()
    {
        var ok = "not-a-date".TryParseIsoDateTime(out _);

        ok.ShouldBeFalse();
    }

    [Test]
    public void ParseIsoDate_ShouldParseIsoDateString()
    {
        var result = "2026-01-01".ParseIsoDate();

        result.ShouldBe(new DateOnly(2026, 1, 1));
    }

    [Test]
    public void ParseIsoDate_ShouldRoundTripWithToIsoString()
    {
        var original = new DateOnly(2026, 6, 15);

        var result = original.ToIsoString().ParseIsoDate();

        result.ShouldBe(original);
    }

    [Test]
    public void ParseIsoDate_ShouldThrowForInvalidString()
    {
        Should.Throw<FormatException>(() => "not-a-date".ParseIsoDate());
    }

    [Test]
    public void TryParseIsoDate_ShouldReturnTrueForValidString()
    {
        var ok = "2026-01-01".TryParseIsoDate(out var result);

        ok.ShouldBeTrue();
        result.ShouldBe(new DateOnly(2026, 1, 1));
    }

    [Test]
    public void TryParseIsoDate_ShouldReturnFalseForInvalidString()
    {
        var ok = "not-a-date".TryParseIsoDate(out _);

        ok.ShouldBeFalse();
    }

    private sealed record TestModel(
        [property: JsonConverter(typeof(IsoDateTimeOffsetConverter))]
        DateTimeOffset Timestamp,
        [property: JsonConverter(typeof(NullableIsoDateTimeOffsetConverter))]
        DateTimeOffset? OptionalTimestamp,
        [property: JsonConverter(typeof(IsoDateConverter))]
        DateOnly Date,
        [property: JsonConverter(typeof(NullableIsoDateConverter))]
        DateOnly? OptionalDate);

    [Test]
    public void IsoDateTimeOffsetConverter_ShouldSerializeAsUtcZ()
    {
        var model = new TestModel(
            new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero),
            null,
            new DateOnly(2026, 1, 1),
            null);

        var json = JsonSerializer.Serialize(model);

        json.ShouldContain("\"Timestamp\":\"2026-01-01T23:00:00Z\"");
    }

    [Test]
    public void NullableIsoDateTimeOffsetConverter_ShouldSerializeNullAsNull()
    {
        var model = new TestModel(
            new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero),
            null,
            new DateOnly(2026, 1, 1),
            null);

        var json = JsonSerializer.Serialize(model);

        json.ShouldContain("\"OptionalTimestamp\":null");
    }

    [Test]
    public void NullableIsoDateTimeOffsetConverter_ShouldSerializeValueAsUtcZ()
    {
        var model = new TestModel(
            new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 6, 15, 12, 0, 0, TimeSpan.Zero),
            new DateOnly(2026, 1, 1),
            null);

        var json = JsonSerializer.Serialize(model);

        json.ShouldContain("\"OptionalTimestamp\":\"2026-06-15T12:00:00Z\"");
    }

    [Test]
    public void IsoDateConverter_ShouldSerializeAsIsoDate()
    {
        var model = new TestModel(
            new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero),
            null,
            new DateOnly(2026, 6, 15),
            null);

        var json = JsonSerializer.Serialize(model);

        json.ShouldContain("\"Date\":\"2026-06-15\"");
    }

    [Test]
    public void NullableIsoDateConverter_ShouldSerializeNullAsNull()
    {
        var model = new TestModel(
            new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero),
            null,
            new DateOnly(2026, 1, 1),
            null);

        var json = JsonSerializer.Serialize(model);

        json.ShouldContain("\"OptionalDate\":null");
    }

    [Test]
    public void JsonConverters_ShouldRoundTripCorrectly()
    {
        var original = new TestModel(
            new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 6, 15, 12, 0, 0, TimeSpan.Zero),
            new DateOnly(2026, 6, 15),
            new DateOnly(2026, 12, 31));

        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<TestModel>(json)!;

        restored.Timestamp.ShouldBe(original.Timestamp);
        restored.OptionalTimestamp.ShouldBe(original.OptionalTimestamp);
        restored.Date.ShouldBe(original.Date);
        restored.OptionalDate.ShouldBe(original.OptionalDate);
    }

    [Test]
    public void ToIsoString_PrecisionAuto_ShouldOmitFractionalForWholeSecond()
    {
        var dto = new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero);

        dto.ToIsoString(IsoDateTimePrecision.Auto).ShouldBe("2026-01-01T23:00:00Z");
    }

    [Test]
    public void ToIsoString_PrecisionAuto_ShouldIncludeFractionalWhenNonZero()
    {
        var dto = new DateTimeOffset(2026, 1, 1, 23, 0, 0, 500, TimeSpan.Zero);

        dto.ToIsoString(IsoDateTimePrecision.Auto).ShouldBe("2026-01-01T23:00:00.5Z");
    }

    [Test]
    public void ToIsoString_PrecisionSeconds_ShouldAlwaysOmitFractional()
    {
        var dto = new DateTimeOffset(2026, 1, 1, 23, 0, 0, 500, TimeSpan.Zero);

        dto.ToIsoString(IsoDateTimePrecision.Seconds).ShouldBe("2026-01-01T23:00:00Z");
    }

    [Test]
    public void ToIsoString_PrecisionFractional_ShouldAlwaysInclude7Digits()
    {
        var dto = new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero);

        dto.ToIsoString(IsoDateTimePrecision.Fractional).ShouldBe("2026-01-01T23:00:00.0000000Z");
    }

    [Test]
    public void ToIsoString_PrecisionFractional_ShouldInclude7DigitsForNonZeroSubSecond()
    {
        var dto = new DateTimeOffset(2026, 1, 1, 23, 0, 0, 500, TimeSpan.Zero);

        dto.ToIsoString(IsoDateTimePrecision.Fractional).ShouldBe("2026-01-01T23:00:00.5000000Z");
    }


    private sealed record PrecisionModel(
        [property: IsoDateTimeFormat(IsoDateTimePrecision.Fractional)]
        DateTimeOffset Fractional,
        [property: IsoDateTimeFormat(IsoDateTimePrecision.Seconds)]
        DateTimeOffset Seconds,
        [property: IsoDateTimeFormat]
        DateTimeOffset? Auto);

    [Test]
    public void IsoDateTimeFormatAttribute_Fractional_ShouldSerialize7Digits()
    {
        var model = new PrecisionModel(
            new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 1, 1, 23, 0, 0, 500, TimeSpan.Zero),
            null);

        var json = JsonSerializer.Serialize(model);

        json.ShouldContain("\"Fractional\":\"2026-01-01T23:00:00.0000000Z\"");
    }

    [Test]
    public void IsoDateTimeFormatAttribute_Seconds_ShouldOmitFractional()
    {
        var model = new PrecisionModel(
            new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 1, 1, 23, 0, 0, 500, TimeSpan.Zero),
            null);

        var json = JsonSerializer.Serialize(model);

        json.ShouldContain("\"Seconds\":\"2026-01-01T23:00:00Z\"");
    }

    [Test]
    public void IsoDateTimeFormatAttribute_Auto_NullableNull_ShouldSerializeAsNull()
    {
        var model = new PrecisionModel(
            new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero),
            null);

        var json = JsonSerializer.Serialize(model);

        json.ShouldContain("\"Auto\":null");
    }

    [Test]
    public void IsoDateTimeFormatAttribute_ShouldRoundTrip()
    {
        var original = new PrecisionModel(
            new DateTimeOffset(2026, 6, 15, 12, 30, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 6, 15, 12, 30, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 6, 15, 12, 30, 0, TimeSpan.Zero));

        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<PrecisionModel>(json)!;

        restored.Fractional.ShouldBe(original.Fractional);
        restored.Seconds.ShouldBe(original.Seconds);
        restored.Auto.ShouldBe(original.Auto);
    }
}
