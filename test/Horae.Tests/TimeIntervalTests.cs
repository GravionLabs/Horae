using Shouldly;

namespace Horae.Tests;

/// <summary>
///     Tests for <see cref="TimeInterval" />.
///     Uses fixed-offset instants and Japan (UTC+9, no DST) where timezone-aware behavior
///     must be deterministic, and Central European Time for delivery-date tests.
/// </summary>
[TestFixture] [Category("UnitTest")]
public class TimeIntervalTests
{
    [Test]
    public void Empty_ShouldHaveMinValueBoundaries()
    {
        TimeInterval.Empty.Start.ShouldBe(DateTimeOffset.MinValue);
        TimeInterval.Empty.End.ShouldBe(DateTimeOffset.MinValue);
    }

    [Test]
    public void Constructor_ShouldNormaliseToUtc()
    {
        var cetStart = new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.FromHours(2));
        var cetEnd = new DateTimeOffset(2026, 6, 15, 12, 0, 0, TimeSpan.FromHours(2));

        var interval = new TimeInterval(cetStart, cetEnd);

        interval.Start.Offset.ShouldBe(TimeSpan.Zero);
        interval.End.Offset.ShouldBe(TimeSpan.Zero);
        interval.Start.ShouldBe(cetStart.ToUniversalTime());
        interval.End.ShouldBe(cetEnd.ToUniversalTime());
    }

    [Test]
    public void Constructor_ShouldThrow_WhenStartIsAfterEnd()
    {
        var start = new DateTimeOffset(2026, 6, 15, 12, 0, 0, TimeSpan.Zero);
        var end = new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero);

        Should.Throw<ArgumentException>(() => new TimeInterval(start, end));
    }

    [Test]
    public void Constructor_ShouldNotThrow_WhenStartEqualsEnd()
    {
        var point = new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero);

        Should.NotThrow(() => new TimeInterval(point, point));
    }

    [Test]
    public void Create_ShouldReturnCorrectInterval()
    {
        var start = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var end = new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero);

        var interval = TimeInterval.Create(start, end);

        interval.Start.ShouldBe(start);
        interval.End.ShouldBe(end);
    }

    [Test]
    public void Day_ShouldSpanFullUtcDay_WhenTimeZoneIsUtc()
    {
        var date = new DateOnly(2026, 6, 15);

        var interval = TimeInterval.Day(date, TimeZoneInfo.Utc);

        interval.Start.ShouldBe(new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero));
        interval.End.ShouldBe(new DateTimeOffset(2026, 6, 16, 0, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void Day_ShouldSpanJapanDay_WhenTimeZoneIsJapan()
    {
        // Japan is UTC+9. 2026-06-15 midnight JST = 2026-06-14 15:00 UTC.
        var date = new DateOnly(2026, 6, 15);

        var interval = TimeInterval.Day(date, TimeZones.Japan);

        interval.Start.ShouldBe(new DateTimeOffset(2026, 6, 14, 15, 0, 0, TimeSpan.Zero));
        interval.End.ShouldBe(new DateTimeOffset(2026, 6, 15, 15, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void Intraday_ShouldSpanCurrentCalendarDay()
    {
        // Use Japan (UTC+9, no DST) for deterministic results.
        // 2026-06-15 10:00 UTC = 2026-06-15 19:00 JST → intraday = 2026-06-15 in JST.
        var now = new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero);

        var interval = TimeInterval.Intraday(now, TimeZones.Japan);

        // JST midnight 2026-06-15 = UTC 2026-06-14 15:00
        interval.Start.ShouldBe(new DateTimeOffset(2026, 6, 14, 15, 0, 0, TimeSpan.Zero));
        interval.End.ShouldBe(new DateTimeOffset(2026, 6, 15, 15, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void DayAhead_ShouldSpanNextCalendarDay()
    {
        // 2026-06-15 10:00 UTC = 2026-06-15 19:00 JST → day-ahead = 2026-06-16 in JST.
        var now = new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero);

        var interval = TimeInterval.DayAhead(now, TimeZones.Japan);

        // JST midnight 2026-06-16 = UTC 2026-06-15 15:00
        interval.Start.ShouldBe(new DateTimeOffset(2026, 6, 15, 15, 0, 0, TimeSpan.Zero));
        interval.End.ShouldBe(new DateTimeOffset(2026, 6, 16, 15, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void Contains_ShouldReturnTrue_WhenValueIsWithinInterval()
    {
        var interval = new TimeInterval(
            new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 6, 16, 0, 0, 0, TimeSpan.Zero));

        interval.Contains(new DateTimeOffset(2026, 6, 15, 12, 0, 0, TimeSpan.Zero)).ShouldBeTrue();
    }

    [Test]
    public void Contains_ShouldReturnTrue_WhenValueEqualsStart()
    {
        var start = new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero);
        var interval = new TimeInterval(start, start.AddDays(1));

        interval.Contains(start).ShouldBeTrue();
    }

    [Test]
    public void Contains_ShouldReturnFalse_WhenValueEqualsEnd()
    {
        var start = new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero);
        var end = start.AddDays(1);
        var interval = new TimeInterval(start, end);

        // Half-open interval: End is exclusive.
        interval.Contains(end).ShouldBeFalse();
    }

    [Test]
    public void Contains_ShouldReturnFalse_WhenValueIsBeforeStart()
    {
        var start = new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero);
        var interval = new TimeInterval(start, start.AddDays(1));

        interval.Contains(start.AddSeconds(-1)).ShouldBeFalse();
    }

    [Test]
    public void Deconstruct_ShouldReturnStartAndEnd()
    {
        var start = new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero);
        var end = start.AddDays(1);
        var interval = new TimeInterval(start, end);

        var (s, e) = interval;

        s.ShouldBe(start);
        e.ShouldBe(end);
    }

    [Test]
    public void SplitBy_ShouldReturnCorrectSlotCount()
    {
        // 24-hour interval split into 1-hour slots → 24 slots.
        var start = new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero);
        var interval = new TimeInterval(start, start.AddHours(24));

        var slots = interval.SplitBy(TimeSpan.FromHours(1), TimeZoneInfo.Utc).ToList();

        slots.Count.ShouldBe(24);
        slots[0].ShouldBe(start);
        slots[^1].ShouldBe(start.AddHours(23));
    }

    [Test]
    public void SplitBy_ShouldRoundBoundariesOutward()
    {
        // Interval 00:15 – 00:45 split by 1 hour → rounds to 00:00 – 01:00 → 1 slot.
        HoareSettings.CurrentTimeZone = TimeZoneInfo.Utc;
        var start = new DateTimeOffset(2026, 6, 15, 0, 15, 0, TimeSpan.Zero);
        var end = new DateTimeOffset(2026, 6, 15, 0, 45, 0, TimeSpan.Zero);
        var interval = new TimeInterval(start, end);

        var slots = interval.SplitBy(TimeSpan.FromHours(1)).ToList();

        slots.Count.ShouldBe(1);
        slots[0].ShouldBe(new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void SplitBy_ResolutionDay_ShouldBeDstSafe_AtCetToCestTransition()
    {
        // 2026-03-29 is the CET → CEST spring transition (23-hour day).
        // Interval covers exactly March 29 and March 30 in CET:
        //   start = 2026-03-28 23:00 UTC  (= March 29 00:00 CET, +01:00)
        //   end   = 2026-03-30 22:00 UTC  (= March 31 00:00 CEST, +02:00)
        //
        // A naïve +24 h step would land at 2026-03-29 23:00 UTC for March 30 (wrong).
        // The DST-safe DateOnly path yields 2026-03-29 22:00 UTC (correct midnight).
        var start = new DateTimeOffset(2026, 3, 28, 23, 0, 0, TimeSpan.Zero); // March 29 00:00 CET
        var end = new DateTimeOffset(2026, 3, 30, 22, 0, 0, TimeSpan.Zero); // March 31 00:00 CEST
        var interval = new TimeInterval(start, end);

        var slots = interval.SplitBy(Duration.Day, TimeZones.CentralEuropean).ToList();

        slots.Count.ShouldBe(2);
        slots[0].ShouldBe(new DateTimeOffset(2026, 3, 28, 23, 0, 0, TimeSpan.Zero)); // March 29 00:00 CET (+1)
        slots[1].ShouldBe(new DateTimeOffset(2026, 3, 29, 22, 0, 0, TimeSpan.Zero)); // March 30 00:00 CEST (+2)
    }

    [Test]
    public void SplitByDays_ShouldReturnOneDatePerCetCalendarDay()
    {
        // Interval is exactly CET June 15 (CEST = UTC+2):
        //   start = 2026-06-14 22:00 UTC  (= June 15 00:00 CEST)
        //   end   = 2026-06-15 22:00 UTC  (= June 16 00:00 CEST)
        //
        // New DST-safe algorithm: startDate = June 15, endDate = June 15 (End == midnight)
        // → yields midnight of June 15 in CEST = 2026-06-14 22:00 UTC (1 date).
        var start = new DateTimeOffset(2026, 6, 14, 22, 0, 0, TimeSpan.Zero);
        var end = new DateTimeOffset(2026, 6, 15, 22, 0, 0, TimeSpan.Zero);
        var interval = new TimeInterval(start, end);

        var dates = interval.SplitByDays(TimeZones.CentralEuropean).ToList();

        dates.Count.ShouldBe(1);
        dates[0].ShouldBe(new DateTimeOffset(2026, 6, 14, 22, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void SplitByDays_ShouldReturnMultipleDates_WhenIntervalSpansMultipleCetDays()
    {
        // Three CET days: June 15, 16, 17 (summer CEST, UTC+2).
        //   start = 2026-06-14 22:00 UTC  (= June 15 00:00 CEST)
        //   end   = 2026-06-17 22:00 UTC  (= June 18 00:00 CEST) — End == midnight → excluded
        //
        // → startDate = June 15, endDate = June 17 → 3 dates.
        var start = new DateTimeOffset(2026, 6, 14, 22, 0, 0, TimeSpan.Zero);
        var end = new DateTimeOffset(2026, 6, 17, 22, 0, 0, TimeSpan.Zero);
        var interval = new TimeInterval(start, end);

        var dates = interval.SplitByDays(TimeZones.CentralEuropean).ToList();

        dates.Count.ShouldBe(3);
        dates[0].ShouldBe(new DateTimeOffset(2026, 6, 14, 22, 0, 0, TimeSpan.Zero)); // June 15 00:00 CEST
        dates[1].ShouldBe(new DateTimeOffset(2026, 6, 15, 22, 0, 0, TimeSpan.Zero)); // June 16 00:00 CEST
        dates[2].ShouldBe(new DateTimeOffset(2026, 6, 16, 22, 0, 0, TimeSpan.Zero)); // June 17 00:00 CEST
    }

    [Test]
    public void SplitByDays_ShouldBeDstSafe_AtCetToCestTransition()
    {
        // 2026-03-29 is the CET → CEST spring transition (clocks forward, 23-hour day).
        //   March 29 midnight CET  = 2026-03-28 23:00 UTC (+01:00)
        //   March 30 midnight CEST = 2026-03-29 22:00 UTC (+02:00)  ← only 23 h apart!
        //
        // A naïve +24 h step would land at 2026-03-29 23:00 UTC (wrong).
        // The DST-safe DateOnly iteration correctly yields 2026-03-29 22:00 UTC.
        var start = new DateTimeOffset(2026, 3, 28, 23, 0, 0, TimeSpan.Zero); // March 29 00:00 CET
        var end = new DateTimeOffset(2026, 3, 30, 22, 0, 0, TimeSpan.Zero); // March 31 00:00 CEST
        var interval = new TimeInterval(start, end);

        var dates = interval.SplitByDays(TimeZones.CentralEuropean).ToList();

        dates.Count.ShouldBe(2);
        dates[0].ShouldBe(new DateTimeOffset(2026, 3, 28, 23, 0, 0, TimeSpan.Zero)); // March 29 00:00 CET (+1)
        dates[1].ShouldBe(new DateTimeOffset(2026, 3, 29, 22, 0, 0, TimeSpan.Zero)); // March 30 00:00 CEST (+2)
    }

    // Duration

    [Test]
    public void Duration_ShouldReturnEndMinusStart()
    {
        var a = new TimeInterval(D(0), D(12));
        a.Duration.ShouldBe(TimeSpan.FromHours(12));
    }

    // Overlaps

    [Test]
    public void Overlaps_ShouldReturnTrue_WhenIntervalsShare()
    {
        var a = new TimeInterval(D(0), D(12));
        var b = new TimeInterval(D(10), D(20));
        a.Overlaps(b).ShouldBeTrue();
        b.Overlaps(a).ShouldBeTrue();
    }

    [Test]
    public void Overlaps_ShouldReturnFalse_WhenAdjacent()
    {
        var a = new TimeInterval(D(0), D(6));
        var b = new TimeInterval(D(6), D(12));
        a.Overlaps(b).ShouldBeFalse();
    }

    [Test]
    public void Overlaps_ShouldReturnFalse_WhenDisjoint()
    {
        var a = new TimeInterval(D(0), D(4));
        var b = new TimeInterval(D(8), D(12));
        a.Overlaps(b).ShouldBeFalse();
    }

    // Engulfs

    [Test]
    public void Engulfs_ShouldReturnTrue_WhenFullyContained()
    {
        var outer = new TimeInterval(D(0), D(24));
        var inner = new TimeInterval(D(6), D(18));
        outer.Engulfs(inner).ShouldBeTrue();
        inner.Engulfs(outer).ShouldBeFalse();
    }

    [Test]
    public void Engulfs_ShouldReturnTrue_WhenEqual()
    {
        var a = new TimeInterval(D(0), D(12));
        a.Engulfs(a).ShouldBeTrue();
    }

    // Intersection

    [Test]
    public void Intersection_ShouldReturnOverlap()
    {
        var a = new TimeInterval(D(0), D(12));
        var b = new TimeInterval(D(8), D(20));
        var result = a.Intersection(b);
        result.ShouldNotBeNull();
        result!.Value.Start.ShouldBe(D(8));
        result.Value.End.ShouldBe(D(12));
    }

    [Test]
    public void Intersection_ShouldReturnNull_WhenDisjoint()
    {
        var a = new TimeInterval(D(0), D(6));
        var b = new TimeInterval(D(6), D(12));
        a.Intersection(b).ShouldBeNull();
    }

    // Union

    [Test]
    public void Union_ShouldSpanBothIntervals()
    {
        var a = new TimeInterval(D(0), D(6));
        var b = new TimeInterval(D(10), D(20));
        var u = a.Union(b);
        u.Start.ShouldBe(D(0));
        u.End.ShouldBe(D(20));
    }

    [Test]
    public void Union_ShouldReturnSelf_WhenEqual()
    {
        var a = new TimeInterval(D(0), D(12));
        a.Union(a).ShouldBe(a);
    }

    // Week / Month / Year factories

    [Test]
    public void Week_ShouldReturnSevenDayInterval_InUtcZone()
    {
        // Monday May 11 UTC
        var monday = new DateOnly(2026, 5, 11);
        var interval = TimeInterval.Week(monday, TimeZoneInfo.Utc);
        interval.Start.ShouldBe(new DateTimeOffset(2026, 5, 11, 0, 0, 0, TimeSpan.Zero));
        interval.End.ShouldBe(new DateTimeOffset(2026, 5, 18, 0, 0, 0, TimeSpan.Zero));
        interval.Duration.ShouldBe(TimeSpan.FromDays(7));
    }

    [Test]
    public void Month_ShouldReturnFullCalendarMonth_InJapan()
    {
        // Japan UTC+9, no DST → February 2026 = 28 days
        var interval = TimeInterval.Month(2026, 2, TimeZones.Japan);
        var localStart = TimeZoneInfo.ConvertTime(interval.Start, TimeZones.Japan);
        var localEnd   = TimeZoneInfo.ConvertTime(interval.End,   TimeZones.Japan);
        localStart.ShouldBe(new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.FromHours(9)));
        localEnd.ShouldBe(new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.FromHours(9)));
        interval.Duration.ShouldBe(TimeSpan.FromDays(28));
    }

    [Test]
    public void Year_ShouldReturnFullCalendarYear()
    {
        var interval = TimeInterval.Year(2025, TimeZoneInfo.Utc);
        interval.Start.ShouldBe(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));
        interval.End.ShouldBe(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        interval.Duration.ShouldBe(TimeSpan.FromDays(365));
    }

    // SplitByWeeks

    [Test]
    public void SplitByWeeks_ShouldYieldMondaysWithinInterval()
    {
        // Interval: May 10 (Sunday) → May 25 (Monday) UTC
        var start = new DateTimeOffset(2026, 5, 10, 0, 0, 0, TimeSpan.Zero);
        var end   = new DateTimeOffset(2026, 5, 25, 0, 0, 0, TimeSpan.Zero);
        var interval = new TimeInterval(start, end);

        var mondays = interval.SplitByWeeks(TimeZoneInfo.Utc).ToList();

        // Mondays in range (exclusive of end=May 25): May 11, May 18
        mondays.Count.ShouldBe(2);
        mondays[0].ShouldBe(new DateTimeOffset(2026, 5, 11, 0, 0, 0, TimeSpan.Zero));
        mondays[1].ShouldBe(new DateTimeOffset(2026, 5, 18, 0, 0, 0, TimeSpan.Zero));
    }

    // SplitByMonths

    [Test]
    public void SplitByMonths_ShouldYieldMonthStartsWithinInterval()
    {
        // Q1 2026 in UTC
        var interval = TimeInterval.Year(2026, TimeZoneInfo.Utc);
        var months = interval.SplitByMonths(TimeZoneInfo.Utc).ToList();

        months.Count.ShouldBe(12);
        months[0].ShouldBe(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        months[11].ShouldBe(new DateTimeOffset(2026, 12, 1, 0, 0, 0, TimeSpan.Zero));
    }

    private static DateTimeOffset D(int hours) =>
        new DateTimeOffset(2026, 5, 11, 0, 0, 0, TimeSpan.Zero).AddHours(hours);
}
