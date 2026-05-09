using System.Text.Json;

using Shouldly;

namespace Horae.Tests;

/// <summary>
///     Tests for <see cref="DayOffset" />.
/// </summary>
[TestFixture]
[Category("UnitTest")]
public class DayOffsetTests
{
    private static readonly DateTimeOffset Reference =
        new(2026, 5, 9, 12, 0, 0, TimeSpan.Zero); // 2026-05-09 12:00 UTC (Saturday)

    [Test]
    public void Presets_ShouldHaveCorrectDays()
    {
        DayOffset.Yesterday.Days.ShouldBe(-1);
        DayOffset.Today.Days.ShouldBe(0);
        DayOffset.DayAhead.Days.ShouldBe(1);
    }

    [Test]
    public void Presets_ShouldHaveNoEmbeddedTimeZone()
    {
        DayOffset.Yesterday.TimeZone.ShouldBeNull();
        DayOffset.Today.TimeZone.ShouldBeNull();
        DayOffset.DayAhead.TimeZone.ShouldBeNull();
    }

    [TestCase(-1, 2026, 5, 8)]
    [TestCase(0, 2026, 5, 9)]
    [TestCase(1, 2026, 5, 10)]
    [TestCase(2, 2026, 5, 11)]
    [TestCase(7, 2026, 5, 16)]
    public void Resolve_ShouldReturnCorrectDate(int days, int year, int month, int day)
    {
        var offset = new DayOffset(days);
        offset.Resolve(Reference, TimeZones.Utc).ShouldBe(new DateOnly(year, month, day));
    }

    [Test]
    public void Resolve_ShouldUseExplicitTzArgFirst()
    {
        // Reference is 23:00 UTC on May 9 → May 10 in Japan
        var lateRef = new DateTimeOffset(2026, 5, 9, 23, 0, 0, TimeSpan.Zero);
        var offset = new DayOffset(0, TimeZones.CentralEuropean);

        // Explicit arg (Japan) should win over embedded tz (CentralEuropean)
        offset.Resolve(lateRef, TimeZones.Japan).ShouldBe(new DateOnly(2026, 5, 10));
    }

    [Test]
    public void Resolve_ShouldUseEmbeddedTzWhenNoExplicitArg()
    {
        // Reference is 23:00 UTC on May 9 → May 10 in Japan
        var lateRef = new DateTimeOffset(2026, 5, 9, 23, 0, 0, TimeSpan.Zero);
        var offset = new DayOffset(0, TimeZones.Japan);

        offset.Resolve(lateRef).ShouldBe(new DateOnly(2026, 5, 10));
    }

    [Test]
    public void Resolve_ShouldFallBackToHoareSettings_WhenNoTzAnywhere()
    {
        using (HoareSettings.Use(TimeZones.Japan))
        {
            var lateRef = new DateTimeOffset(2026, 5, 9, 23, 0, 0, TimeSpan.Zero);
            var offset = new DayOffset(0); // no embedded tz
            offset.Resolve(lateRef).ShouldBe(new DateOnly(2026, 5, 10));
        }
    }

    [Test]
    public void Resolve_AcrossDstBoundary_ShouldAddCalendarDays()
    {
        // CET → CEST transition in 2026: March 29 at 02:00 local (= 01:00 UTC)
        // Reference: 2026-03-28 22:00 UTC = 2026-03-28 23:00 CET
        var beforeTransition = new DateTimeOffset(2026, 3, 28, 22, 0, 0, TimeSpan.Zero);

        // +1 day should give March 29, not March 30 (no double-count despite 23-hour day)
        new DayOffset(1).Resolve(beforeTransition, TimeZones.CentralEuropean)
            .ShouldBe(new DateOnly(2026, 3, 29));

        // Reference: 2026-10-24 21:00 UTC = 2026-10-24 23:00 CEST (before CEST → CET)
        var beforeFallBack = new DateTimeOffset(2026, 10, 24, 21, 0, 0, TimeSpan.Zero);

        // +1 day should give Oct 25 (25-hour day, but DateOnly.AddDays is timezone-agnostic)
        new DayOffset(1).Resolve(beforeFallBack, TimeZones.CentralEuropean)
            .ShouldBe(new DateOnly(2026, 10, 25));
    }

    [Test]
    public void Resolve_AcrossMidnight_ShouldReflectTimezone()
    {
        // 2026-01-01T23:30:00Z is still Jan 1 in UTC, but Jan 2 in Japan (UTC+9)
        var ref2 = new DateTimeOffset(2026, 1, 1, 23, 30, 0, TimeSpan.Zero);

        new DayOffset(0).Resolve(ref2, TimeZones.Utc).ShouldBe(new DateOnly(2026, 1, 1));
        new DayOffset(0).Resolve(ref2, TimeZones.Japan).ShouldBe(new DateOnly(2026, 1, 2));
    }

    [Test]
    public void ImplicitConversion_ShouldCreateDayOffsetWithNoDays()
    {
        DayOffset offset = 5;
        offset.Days.ShouldBe(5);
        offset.TimeZone.ShouldBeNull();
    }

    [TestCase(0, null, "+0")]
    [TestCase(1, null, "+1")]
    [TestCase(-1, null, "-1")]
    [TestCase(7, null, "+7")]
    public void ToString_ShouldFormatCorrectly_WhenNoTimeZone(int days, string? _, string expected)
    {
        new DayOffset(days).ToString().ShouldBe(expected);
    }

    [Test]
    public void ToString_ShouldIncludeTimeZoneId_WhenEmbedded()
    {
        new DayOffset(1, TimeZones.CentralEuropean).ToString().ShouldBe("+1 (Europe/Berlin)");
        new DayOffset(-1, TimeZones.Japan).ToString().ShouldBe("-1 (Asia/Tokyo)");
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(-1)]
    [TestCase(365)]
    public void JsonRoundTrip_Integer_ShouldPreserveValue(int days)
    {
        var offset = new DayOffset(days);
        var json = JsonSerializer.Serialize(offset);
        var restored = JsonSerializer.Deserialize<DayOffset>(json);

        json.ShouldBe($"{days}");
        restored.Days.ShouldBe(days);
        restored.TimeZone.ShouldBeNull();
    }

    [Test]
    public void JsonRoundTrip_Object_ShouldPreserveValueAndTimezone()
    {
        var offset = new DayOffset(1, TimeZones.CentralEuropean);
        var json = JsonSerializer.Serialize(offset);
        var restored = JsonSerializer.Deserialize<DayOffset>(json);

        json.ShouldBe("""{"days":1,"timezone":"Europe/Berlin"}""");
        restored.Days.ShouldBe(1);
        restored.TimeZone.ShouldNotBeNull();
        restored.TimeZone!.Id.ShouldBe("Europe/Berlin");
    }

    [Test]
    public void JsonDeserialize_ShouldAcceptIntegerForObjectExpectingDayOffset()
    {
        // Backward-compat: when the timezone was not yet embedded, the JSON is a plain int
        var restored = JsonSerializer.Deserialize<DayOffset>("3");
        restored.Days.ShouldBe(3);
        restored.TimeZone.ShouldBeNull();
    }

    [Test]
    public void JsonDeserialize_ShouldAcceptObjectWithoutTimezone()
    {
        var restored = JsonSerializer.Deserialize<DayOffset>("""{"days":2}""");
        restored.Days.ShouldBe(2);
        restored.TimeZone.ShouldBeNull();
    }

    [Test]
    public void EqualityAndHashCode_ShouldBehaveAsValueType()
    {
        var a = new DayOffset(1, TimeZones.CentralEuropean);
        var b = new DayOffset(1, TimeZones.CentralEuropean);
        var c = new DayOffset(1, TimeZones.Japan);

        a.ShouldBe(b);
        a.ShouldNotBe(c);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }
}
