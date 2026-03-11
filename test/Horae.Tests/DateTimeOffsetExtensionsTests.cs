using Shouldly;

namespace Horae.Tests;

/// <summary>
///     Tests for <see cref="DateTimeOffsetExtensions" />.
///     Uses timezones with fixed, DST-free offsets (Japan UTC+9, Dubai UTC+4) wherever possible
///     to keep expected values deterministic.
/// </summary>
[TestFixture] [Category("UnitTest")]
public class DateTimeOffsetExtensionsTests
{
    // 2026-06-15 10:00:00 UTC — a mid-summer reference instant.
    private static readonly DateTimeOffset _utcRef = new(2026, 6, 15, 10, 0, 0, TimeSpan.Zero);

    private TimeZoneInfo _originalTimeZone = null!;

    [SetUp]
    public void SetUp() => _originalTimeZone = HoareSettings.CurrentTimeZone;

    [TearDown]
    public void TearDown() => HoareSettings.CurrentTimeZone = _originalTimeZone;


    [Test]
    public void ToTimeZone_ShouldExpressInstantInTargetTimeZone()
    {
        // Japan is UTC+9 (no DST). 10:00 UTC → 19:00 JST.
        var result = _utcRef.ToTimeZone(TimeZones.Japan);

        result.Offset.ShouldBe(TimeSpan.FromHours(9));
        result.Hour.ShouldBe(19);
    }

    [Test]
    public void ToTimeZone_ShouldPreserveInstant()
    {
        var result = _utcRef.ToTimeZone(TimeZones.Dubai);

        result.UtcDateTime.ShouldBe(_utcRef.UtcDateTime);
    }


    [Test]
    public void ToCentralEuropeanTime_ShouldConvertToCet()
    {
        var result = _utcRef.ToCentralEuropeanTime();

        // June is CEST (UTC+2).
        result.Offset.ShouldBe(TimeSpan.FromHours(2));
        result.UtcDateTime.ShouldBe(_utcRef.UtcDateTime);
    }


    [Test]
    public void StartOfDay_ShouldReturnMidnightInExplicitTimeZone()
    {
        // 10:00 UTC = 19:00 JST → start of day in JST = 2026-06-15 00:00 +09:00
        var result = _utcRef.StartOfDay(TimeZones.Japan);

        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.FromHours(9)));
    }

    [Test]
    public void StartOfDay_ShouldUseCurrentTimeZone_WhenNullIsPassed()
    {
        HoareSettings.CurrentTimeZone = TimeZones.Dubai; // UTC+4

        // 10:00 UTC = 14:00 GST → start of day = 2026-06-15 00:00 +04:00
        var result = _utcRef.StartOfDay();

        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.FromHours(4)));
    }


    [Test]
    public void StartOfDayUtc_ShouldReturnMidnightAsUtc()
    {
        // Start of day in JST = 2026-06-15 00:00 +09:00 = 2026-06-14 15:00 UTC
        var result = _utcRef.StartOfDayUtc(TimeZones.Japan);

        result.Offset.ShouldBe(TimeSpan.Zero);
        result.ShouldBe(new DateTimeOffset(2026, 6, 14, 15, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void StartOfDayUtc_ShouldUseCurrentTimeZone_WhenNullIsPassed()
    {
        HoareSettings.CurrentTimeZone = TimeZones.Japan;

        var result = _utcRef.StartOfDayUtc();

        result.Offset.ShouldBe(TimeSpan.Zero);
        result.ShouldBe(new DateTimeOffset(2026, 6, 14, 15, 0, 0, TimeSpan.Zero));
    }


    [Test]
    public void EndOfDay_ShouldReturnFirstInstantOfNextDay()
    {
        // Start of day in JST = 2026-06-15 00:00 +09:00 → EndOfDay = 2026-06-16 00:00 +09:00
        var result = _utcRef.EndOfDay(TimeZones.Japan);

        result.ShouldBe(new DateTimeOffset(2026, 6, 16, 0, 0, 0, TimeSpan.FromHours(9)));
    }

    [Test]
    public void EndOfDay_ShouldUseCurrentTimeZone_WhenNullIsPassed()
    {
        HoareSettings.CurrentTimeZone = TimeZones.Dubai;

        var result = _utcRef.EndOfDay();

        result.ShouldBe(new DateTimeOffset(2026, 6, 16, 0, 0, 0, TimeSpan.FromHours(4)));
    }


    [Test]
    public void EndOfDayUtc_ShouldReturnFirstInstantOfNextDayAsUtc()
    {
        // EndOfDay in JST = 2026-06-16 00:00 +09:00 = 2026-06-15 15:00 UTC
        var result = _utcRef.EndOfDayUtc(TimeZones.Japan);

        result.Offset.ShouldBe(TimeSpan.Zero);
        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 15, 0, 0, TimeSpan.Zero));
    }


    [Test]
    public void StartOfIntraday_ShouldReturnMidnightInExplicitTimeZone()
    {
        var result = _utcRef.StartOfIntraday(TimeZones.Japan);

        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.FromHours(9)));
    }

    [Test]
    public void StartOfIntraday_ShouldUseCurrentTimeZone_WhenNullIsPassed()
    {
        HoareSettings.CurrentTimeZone = TimeZones.Dubai;

        var result = _utcRef.StartOfIntraday();

        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.FromHours(4)));
    }


    [Test]
    public void StartOfIntradayUtc_ShouldReturnMidnightAsUtc()
    {
        var result = _utcRef.StartOfIntradayUtc(TimeZones.Japan);

        result.Offset.ShouldBe(TimeSpan.Zero);
        result.ShouldBe(new DateTimeOffset(2026, 6, 14, 15, 0, 0, TimeSpan.Zero));
    }


    [Test]
    public void EndOfIntraday_ShouldReturnFirstInstantOfNextDay()
    {
        var result = _utcRef.EndOfIntraday(TimeZones.Japan);

        result.ShouldBe(new DateTimeOffset(2026, 6, 16, 0, 0, 0, TimeSpan.FromHours(9)));
    }

    [Test]
    public void EndOfIntraday_ShouldUseCurrentTimeZone_WhenNullIsPassed()
    {
        HoareSettings.CurrentTimeZone = TimeZones.Dubai;

        var result = _utcRef.EndOfIntraday();

        result.ShouldBe(new DateTimeOffset(2026, 6, 16, 0, 0, 0, TimeSpan.FromHours(4)));
    }


    [Test]
    public void EndOfIntradayUtc_ShouldReturnFirstInstantOfNextDayAsUtc()
    {
        var result = _utcRef.EndOfIntradayUtc(TimeZones.Japan);

        result.Offset.ShouldBe(TimeSpan.Zero);
        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 15, 0, 0, TimeSpan.Zero));
    }


    [Test]
    public void StartOfDayAhead_ShouldReturnMidnightOfNextDay()
    {
        // Same as EndOfIntraday
        var result = _utcRef.StartOfDayAhead(TimeZones.Japan);

        result.ShouldBe(new DateTimeOffset(2026, 6, 16, 0, 0, 0, TimeSpan.FromHours(9)));
    }

    [Test]
    public void StartOfDayAheadUtc_ShouldReturnMidnightOfNextDayAsUtc()
    {
        var result = _utcRef.StartOfDayAheadUtc(TimeZones.Japan);

        result.Offset.ShouldBe(TimeSpan.Zero);
        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 15, 0, 0, TimeSpan.Zero));
    }


    [Test]
    public void EndOfDayAhead_ShouldReturnMidnightTwoDaysAhead()
    {
        var result = _utcRef.EndOfDayAhead(TimeZones.Japan);

        result.ShouldBe(new DateTimeOffset(2026, 6, 17, 0, 0, 0, TimeSpan.FromHours(9)));
    }

    [Test]
    public void EndOfDayAheadUtc_ShouldReturnMidnightTwoDaysAheadAsUtc()
    {
        var result = _utcRef.EndOfDayAheadUtc(TimeZones.Japan);

        result.Offset.ShouldBe(TimeSpan.Zero);
        result.ShouldBe(new DateTimeOffset(2026, 6, 16, 15, 0, 0, TimeSpan.Zero));
    }


    [Test]
    public void RoundDown_ShouldRoundInExplicitTimeZone()
    {
        // 10:00 UTC = 14:00 GST (Dubai UTC+4). RoundDown to 1h → 14:00 GST = 10:00 UTC (unchanged).
        var result = _utcRef.RoundDown(TimeSpan.FromHours(1), TimeZones.Dubai);

        result.Offset.ShouldBe(TimeSpan.Zero);
        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void RoundDown_ShouldTruncate_WhenNotOnBoundary()
    {
        // 10:35 UTC = 14:35 GST. RoundDown to 1h → 14:00 GST = 10:00 UTC.
        var dto = new DateTimeOffset(2026, 6, 15, 10, 35, 0, TimeSpan.Zero);
        var result = dto.RoundDown(TimeSpan.FromHours(1), TimeZones.Dubai);

        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void RoundDown_ShouldUseCurrentTimeZone_WhenNullIsPassed()
    {
        HoareSettings.CurrentTimeZone = TimeZones.Dubai;

        var dto = new DateTimeOffset(2026, 6, 15, 10, 35, 0, TimeSpan.Zero);
        var result = dto.RoundDown(TimeSpan.FromHours(1));

        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero));
    }


    [Test]
    public void RoundUp_ShouldRoundToNextBoundary()
    {
        // 10:35 UTC = 14:35 GST. RoundUp to 1h → 15:00 GST = 11:00 UTC.
        var dto = new DateTimeOffset(2026, 6, 15, 10, 35, 0, TimeSpan.Zero);
        var result = dto.RoundUp(TimeSpan.FromHours(1), TimeZones.Dubai);

        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 11, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void RoundUp_ShouldNotChangeValue_WhenAlreadyOnBoundary()
    {
        // 10:00 UTC = 14:00 GST — already on the hour.
        var result = _utcRef.RoundUp(TimeSpan.FromHours(1), TimeZones.Dubai);

        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void RoundUp_ShouldUseCurrentTimeZone_WhenNullIsPassed()
    {
        HoareSettings.CurrentTimeZone = TimeZones.Dubai;

        var dto = new DateTimeOffset(2026, 6, 15, 10, 35, 0, TimeSpan.Zero);
        var result = dto.RoundUp(TimeSpan.FromHours(1));

        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 11, 0, 0, TimeSpan.Zero));
    }


    [Test]
    public void RoundToNearest_ShouldRoundDown_WhenBelowMidpoint()
    {
        // 10:29 UTC = 14:29 GST. Nearest hour = 14:00 GST = 10:00 UTC.
        var dto = new DateTimeOffset(2026, 6, 15, 10, 29, 0, TimeSpan.Zero);
        var result = dto.RoundToNearest(TimeSpan.FromHours(1), TimeZones.Dubai);

        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void RoundToNearest_ShouldRoundUp_WhenAboveMidpoint()
    {
        // 10:31 UTC = 14:31 GST. Nearest hour = 15:00 GST = 11:00 UTC.
        var dto = new DateTimeOffset(2026, 6, 15, 10, 31, 0, TimeSpan.Zero);
        var result = dto.RoundToNearest(TimeSpan.FromHours(1), TimeZones.Dubai);

        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 11, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void RoundToNearest_ShouldUseCurrentTimeZone_WhenNullIsPassed()
    {
        HoareSettings.CurrentTimeZone = TimeZones.Dubai;

        var dto = new DateTimeOffset(2026, 6, 15, 10, 29, 0, TimeSpan.Zero);
        var result = dto.RoundToNearest(TimeSpan.FromHours(1));

        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero));
    }
}
