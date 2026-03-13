using System.Text.Json;
using Shouldly;

namespace Horae.Tests;

/// <summary>
///     Tests for <see cref="Duration" />.
/// </summary>
[TestFixture] [Category("UnitTest")]
public class ResolutionTests
{
    private TimeZoneInfo _originalTimeZone = null!;

    [SetUp]
    public void SetUp() => _originalTimeZone = HoareSettings.CurrentTimeZone;

    [TearDown]
    public void TearDown() => HoareSettings.CurrentTimeZone = _originalTimeZone;

    [TestCase("PT1M",  1,    0,  0)]
    [TestCase("PT5M",  5,    0,  0)]
    [TestCase("PT15M", 15,   0,  0)]
    [TestCase("PT30M", 30,   0,  0)]
    [TestCase("PT1H",  0,    1,  0)]
    [TestCase("P1D",   0,    0,  1)]
    [TestCase("P7D",   0,    0,  7)]
    public void ToString_ShouldReturnIso8601Duration(string expected, int minutes, int hours, int days)
    {
        var resolution = new Duration(TimeSpan.FromMinutes(minutes) + TimeSpan.FromHours(hours) +
                                        TimeSpan.FromDays(days));

        resolution.ToString().ShouldBe(expected);
    }

    [TestCase("PT1M")]
    [TestCase("PT5M")]
    [TestCase("PT15M")]
    [TestCase("PT30M")]
    [TestCase("PT1H")]
    [TestCase("P1D")]
    [TestCase("PT45S")]
    [TestCase("PT2H")]
    [TestCase("P7D")]
    public void Parse_ShouldRoundTrip(string iso)
    {
        var resolution = Duration.Parse(iso);
        resolution.ToString().ShouldBe(iso);
    }

    [Test]
    public void StaticInstances_ShouldHaveCorrectValues()
    {
        Duration.Minute.Value.ShouldBe(TimeSpan.FromMinutes(1));
        Duration.FiveMinutes.Value.ShouldBe(TimeSpan.FromMinutes(5));
        Duration.QuarterHour.Value.ShouldBe(TimeSpan.FromMinutes(15));
        Duration.HalfHour.Value.ShouldBe(TimeSpan.FromMinutes(30));
        Duration.Hour.Value.ShouldBe(TimeSpan.FromHours(1));
        Duration.Day.Value.ShouldBe(TimeSpan.FromDays(1));
        Duration.Week.Value.ShouldBe(TimeSpan.FromDays(7));
    }

    [Test]
    public void StaticInstances_ShouldSerialiseToExpectedStrings()
    {
        Duration.Minute.ToString().ShouldBe("PT1M");
        Duration.FiveMinutes.ToString().ShouldBe("PT5M");
        Duration.QuarterHour.ToString().ShouldBe("PT15M");
        Duration.HalfHour.ToString().ShouldBe("PT30M");
        Duration.Hour.ToString().ShouldBe("PT1H");
        Duration.Day.ToString().ShouldBe("P1D");
        Duration.Week.ToString().ShouldBe("P7D");
    }

    [Test]
    public void Constructor_ShouldThrow_WhenValueIsZero()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new Duration(TimeSpan.Zero));
    }

    [Test]
    public void Constructor_ShouldThrow_WhenValueIsNegative()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new Duration(TimeSpan.FromMinutes(-1)));
    }

    [TestCase("")]
    [TestCase("invalid")]
    [TestCase("P1M")]      // month — calendar-dependent, not supported
    [TestCase("P1Y")]      // year — calendar-dependent, not supported
    [TestCase("PT0M")]     // zero duration
    [TestCase("PT-15M")]   // negative
    public void TryParse_ShouldReturnFalse_ForInvalidInput(string input)
    {
        Duration.TryParse(input, out _).ShouldBeFalse();
    }

    [Test]
    public void ImplicitConversion_ToTimeSpan_ShouldReturnValue()
    {
        TimeSpan ts = Duration.QuarterHour;
        ts.ShouldBe(TimeSpan.FromMinutes(15));
    }

    [Test]
    public void ImplicitConversion_FromTimeSpan_ShouldWrapValue()
    {
        Duration r = TimeSpan.FromHours(4);
        r.Value.ShouldBe(TimeSpan.FromHours(4));
    }

    [Test]
    public void JsonSerialise_ShouldProduceQuotedIso8601String()
    {
        var json = JsonSerializer.Serialize(Duration.QuarterHour);
        json.ShouldBe("\"PT15M\"");
    }

    [Test]
    public void JsonDeserialise_ShouldParseIso8601String()
    {
        var r = JsonSerializer.Deserialize<Duration>("\"P7D\"");
        r.ShouldBe(Duration.Week);
    }

    [Test]
    public void JsonRoundTrip_ShouldPreserveValue()
    {
        var original = Duration.HalfHour;
        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<Duration>(json);
        restored.ShouldBe(original);
    }

    [Test]
    public void TimeInterval_SplitBy_Resolution_ShouldReturnCorrectSlots()
    {
        // 2-hour interval split into quarter-hours → 8 slots.
        HoareSettings.CurrentTimeZone = TimeZoneInfo.Utc;
        var start = new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero);
        var interval = new TimeInterval(start, start.AddHours(2));

        var slots = interval.SplitBy(Duration.QuarterHour).ToList();

        slots.Count.ShouldBe(8);
        slots[0].ShouldBe(start);
        slots[^1].ShouldBe(start.AddMinutes(105));
    }

    [Test]
    public void RoundDown_Resolution_ShouldDelegateToTimeSpanOverload()
    {
        HoareSettings.CurrentTimeZone = TimeZoneInfo.Utc;
        var dto = new DateTimeOffset(2026, 6, 15, 10, 37, 0, TimeSpan.Zero);

        var viaResolution = dto.RoundDown(Duration.HalfHour);
        var viaTimeSpan   = dto.RoundDown(TimeSpan.FromMinutes(30));

        viaResolution.ShouldBe(viaTimeSpan);
        viaResolution.ShouldBe(new DateTimeOffset(2026, 6, 15, 10, 30, 0, TimeSpan.Zero));
    }

    [Test]
    public void RoundUp_Resolution_ShouldDelegateToTimeSpanOverload()
    {
        HoareSettings.CurrentTimeZone = TimeZoneInfo.Utc;
        var dto = new DateTimeOffset(2026, 6, 15, 10, 37, 0, TimeSpan.Zero);

        var viaResolution = dto.RoundUp(Duration.HalfHour);
        var viaTimeSpan   = dto.RoundUp(TimeSpan.FromMinutes(30));

        viaResolution.ShouldBe(viaTimeSpan);
        viaResolution.ShouldBe(new DateTimeOffset(2026, 6, 15, 11, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void RoundToNearest_Resolution_ShouldDelegateToTimeSpanOverload()
    {
        HoareSettings.CurrentTimeZone = TimeZoneInfo.Utc;
        var dto = new DateTimeOffset(2026, 6, 15, 10, 22, 0, TimeSpan.Zero);

        var viaResolution = dto.RoundToNearest(Duration.QuarterHour);
        var viaTimeSpan   = dto.RoundToNearest(TimeSpan.FromMinutes(15));

        viaResolution.ShouldBe(viaTimeSpan);
        viaResolution.ShouldBe(new DateTimeOffset(2026, 6, 15, 10, 15, 0, TimeSpan.Zero));
    }
}
