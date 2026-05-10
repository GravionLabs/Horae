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


    // ── StartOfWeek / EndOfWeek ───────────────────────────────────────────────

    [Test]
    public void StartOfWeek_ShouldReturnMondayMidnight_InExplicitTimeZone()
    {
        // 2026-06-15 (Monday) 10:00 UTC = 2026-06-15 19:00 JST → Monday midnight JST
        var result = _utcRef.StartOfWeek(TimeZones.Japan);

        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.FromHours(9)));
    }

    [Test]
    public void StartOfWeek_ShouldGoBackToMonday_WhenMidWeek()
    {
        // 2026-06-18 (Thursday) 10:00 UTC = 2026-06-18 19:00 JST → Monday = 2026-06-15
        var thursday = new DateTimeOffset(2026, 6, 18, 10, 0, 0, TimeSpan.Zero);
        var result = thursday.StartOfWeek(TimeZones.Japan);

        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.FromHours(9)));
    }

    [Test]
    public void StartOfWeek_ShouldGoBackToMonday_WhenSunday()
    {
        // 2026-06-21 (Sunday) → ISO week starts on Monday 2026-06-15
        var sunday = new DateTimeOffset(2026, 6, 21, 10, 0, 0, TimeSpan.Zero);
        var result = sunday.StartOfWeek(TimeZones.Japan);

        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.FromHours(9)));
    }

    [Test]
    public void StartOfWeek_ShouldUseCurrentTimeZone_WhenNullIsPassed()
    {
        HoareSettings.CurrentTimeZone = TimeZones.Japan;

        var result = _utcRef.StartOfWeek();

        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.FromHours(9)));
    }

    [Test]
    public void StartOfWeekUtc_ShouldReturnMondayMidnightAsUtc()
    {
        // Monday midnight JST = 2026-06-14 15:00 UTC
        var result = _utcRef.StartOfWeekUtc(TimeZones.Japan);

        result.Offset.ShouldBe(TimeSpan.Zero);
        result.ShouldBe(new DateTimeOffset(2026, 6, 14, 15, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void EndOfWeek_ShouldReturnFollowingMondayMidnight()
    {
        // Week of 2026-06-15 → end = 2026-06-22 00:00 JST
        var result = _utcRef.EndOfWeek(TimeZones.Japan);

        result.ShouldBe(new DateTimeOffset(2026, 6, 22, 0, 0, 0, TimeSpan.FromHours(9)));
    }

    [Test]
    public void EndOfWeekUtc_ShouldReturnFollowingMondayMidnightAsUtc()
    {
        // 2026-06-22 00:00 JST = 2026-06-21 15:00 UTC
        var result = _utcRef.EndOfWeekUtc(TimeZones.Japan);

        result.Offset.ShouldBe(TimeSpan.Zero);
        result.ShouldBe(new DateTimeOffset(2026, 6, 21, 15, 0, 0, TimeSpan.Zero));
    }


    // ── StartOfMonth / EndOfMonth ─────────────────────────────────────────────

    [Test]
    public void StartOfMonth_ShouldReturnFirstDayMidnight_InExplicitTimeZone()
    {
        // 2026-06-15 10:00 UTC = 2026-06-15 19:00 JST → start of June JST = 2026-06-01 00:00 +09:00
        var result = _utcRef.StartOfMonth(TimeZones.Japan);

        result.ShouldBe(new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.FromHours(9)));
    }

    [Test]
    public void StartOfMonth_ShouldUseCurrentTimeZone_WhenNullIsPassed()
    {
        HoareSettings.CurrentTimeZone = TimeZones.Dubai;

        var result = _utcRef.StartOfMonth();

        result.ShouldBe(new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.FromHours(4)));
    }

    [Test]
    public void StartOfMonthUtc_ShouldReturnFirstDayMidnightAsUtc()
    {
        // 2026-06-01 00:00 JST = 2026-05-31 15:00 UTC
        var result = _utcRef.StartOfMonthUtc(TimeZones.Japan);

        result.Offset.ShouldBe(TimeSpan.Zero);
        result.ShouldBe(new DateTimeOffset(2026, 5, 31, 15, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void EndOfMonth_ShouldReturnFirstDayOfNextMonthMidnight()
    {
        // June → July 1 midnight JST
        var result = _utcRef.EndOfMonth(TimeZones.Japan);

        result.ShouldBe(new DateTimeOffset(2026, 7, 1, 0, 0, 0, TimeSpan.FromHours(9)));
    }

    [Test]
    public void EndOfMonth_ShouldWrapToJanuary_WhenInDecember()
    {
        // 2026-12-15 10:00 UTC = 2026-12-15 19:00 JST → end of December = 2027-01-01 00:00 JST
        var december = new DateTimeOffset(2026, 12, 15, 10, 0, 0, TimeSpan.Zero);
        var result = december.EndOfMonth(TimeZones.Japan);

        result.ShouldBe(new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.FromHours(9)));
    }

    [Test]
    public void EndOfMonthUtc_ShouldReturnFirstDayOfNextMonthAsUtc()
    {
        // 2026-07-01 00:00 JST = 2026-06-30 15:00 UTC
        var result = _utcRef.EndOfMonthUtc(TimeZones.Japan);

        result.Offset.ShouldBe(TimeSpan.Zero);
        result.ShouldBe(new DateTimeOffset(2026, 6, 30, 15, 0, 0, TimeSpan.Zero));
    }


    // ── StartOfYear / EndOfYear ───────────────────────────────────────────────

    [Test]
    public void StartOfYear_ShouldReturnJanuaryFirstMidnight_InExplicitTimeZone()
    {
        // 2026-06-15 → 2026-01-01 00:00 JST
        var result = _utcRef.StartOfYear(TimeZones.Japan);

        result.ShouldBe(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.FromHours(9)));
    }

    [Test]
    public void StartOfYear_ShouldUseCurrentTimeZone_WhenNullIsPassed()
    {
        HoareSettings.CurrentTimeZone = TimeZones.Dubai;

        var result = _utcRef.StartOfYear();

        result.ShouldBe(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.FromHours(4)));
    }

    [Test]
    public void StartOfYearUtc_ShouldReturnJanuaryFirstMidnightAsUtc()
    {
        // 2026-01-01 00:00 JST = 2025-12-31 15:00 UTC
        var result = _utcRef.StartOfYearUtc(TimeZones.Japan);

        result.Offset.ShouldBe(TimeSpan.Zero);
        result.ShouldBe(new DateTimeOffset(2025, 12, 31, 15, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void EndOfYear_ShouldReturnJanuaryFirstOfNextYearMidnight()
    {
        // 2026 → 2027-01-01 00:00 JST
        var result = _utcRef.EndOfYear(TimeZones.Japan);

        result.ShouldBe(new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.FromHours(9)));
    }

    [Test]
    public void EndOfYearUtc_ShouldReturnJanuaryFirstOfNextYearMidnightAsUtc()
    {
        // 2027-01-01 00:00 JST = 2026-12-31 15:00 UTC
        var result = _utcRef.EndOfYearUtc(TimeZones.Japan);

        result.Offset.ShouldBe(TimeSpan.Zero);
        result.ShouldBe(new DateTimeOffset(2026, 12, 31, 15, 0, 0, TimeSpan.Zero));
    }


    // ── StartOfQuarter / EndOfQuarter ─────────────────────────────────────────

    [Test]
    [TestCase(1, 2026, 1)]   // Q1: Jan 1
    [TestCase(2, 2026, 1)]   // Q1: Jan 1
    [TestCase(5, 2026, 4)]   // Q2: Apr 1
    [TestCase(7, 2026, 7)]   // Q3: Jul 1
    [TestCase(10, 2026, 10)] // Q4: Oct 1
    [TestCase(12, 2026, 10)] // Q4: Oct 1
    public void StartOfQuarter_ShouldReturnFirstDayOfQuarter(int refMonth, int expectedYear, int expectedMonth)
    {
        var dto = new DateTimeOffset(2026, refMonth, 15, 10, 0, 0, TimeSpan.Zero);
        var result = dto.StartOfQuarter(TimeZones.Japan);

        result.ShouldBe(new DateTimeOffset(expectedYear, expectedMonth, 1, 0, 0, 0, TimeSpan.FromHours(9)));
    }

    [Test]
    public void StartOfQuarter_ShouldUseCurrentTimeZone_WhenNullIsPassed()
    {
        // June → Q2, start = April 1
        HoareSettings.CurrentTimeZone = TimeZones.Dubai;

        var result = _utcRef.StartOfQuarter();

        result.ShouldBe(new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.FromHours(4)));
    }

    [Test]
    public void StartOfQuarterUtc_ShouldReturnFirstDayOfQuarterAsUtc()
    {
        // Q2 start: 2026-04-01 00:00 JST = 2026-03-31 15:00 UTC
        var result = _utcRef.StartOfQuarterUtc(TimeZones.Japan);

        result.Offset.ShouldBe(TimeSpan.Zero);
        result.ShouldBe(new DateTimeOffset(2026, 3, 31, 15, 0, 0, TimeSpan.Zero));
    }

    [Test]
    [TestCase(1, 2026, 4)]    // Q1 ends → Q2 starts Apr 1
    [TestCase(3, 2026, 4)]    // Q1 ends → Apr 1
    [TestCase(4, 2026, 7)]    // Q2 ends → Jul 1
    [TestCase(9, 2026, 10)]   // Q3 ends → Oct 1
    [TestCase(10, 2027, 1)]   // Q4 ends → next year Jan 1
    [TestCase(12, 2027, 1)]   // Q4 ends → next year Jan 1
    public void EndOfQuarter_ShouldReturnFirstDayOfNextQuarter(int refMonth, int expectedYear, int expectedMonth)
    {
        var dto = new DateTimeOffset(2026, refMonth, 15, 10, 0, 0, TimeSpan.Zero);
        var result = dto.EndOfQuarter(TimeZones.Japan);

        result.ShouldBe(new DateTimeOffset(expectedYear, expectedMonth, 1, 0, 0, 0, TimeSpan.FromHours(9)));
    }

    [Test]
    public void EndOfQuarterUtc_ShouldReturnFirstDayOfNextQuarterAsUtc()
    {
        // Q2 end: 2026-07-01 00:00 JST = 2026-06-30 15:00 UTC
        var result = _utcRef.EndOfQuarterUtc(TimeZones.Japan);

        result.Offset.ShouldBe(TimeSpan.Zero);
        result.ShouldBe(new DateTimeOffset(2026, 6, 30, 15, 0, 0, TimeSpan.Zero));
    }
}
