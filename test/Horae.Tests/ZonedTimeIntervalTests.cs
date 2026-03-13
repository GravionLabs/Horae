using Shouldly;

namespace Horae.Tests;

/// <summary>
///     Tests for <see cref="ZonedTimeInterval" />.
/// </summary>
[TestFixture] [Category("UnitTest")]
public class ZonedTimeIntervalTests
{
    private static readonly TimeZoneInfo Cet  = TimeZones.CentralEuropean;
    private static readonly TimeZoneInfo Jst  = TimeZones.Japan;

    // ── Constructor ──────────────────────────────────────────────────────────

    [Test]
    public void Constructor_ShouldConvertStartAndEndToZoneTime()
    {
        // UTC midnight → CET = +01:00 in January
        var utcStart = new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero);
        var utcEnd   = new DateTimeOffset(2026, 1, 2, 23, 0, 0, TimeSpan.Zero);

        var interval = new ZonedTimeInterval(utcStart, utcEnd, Cet);

        interval.Start.ShouldBe(new DateTimeOffset(2026, 1, 2,  0, 0, 0, TimeSpan.FromHours(1)));
        interval.End.ShouldBe(  new DateTimeOffset(2026, 1, 3,  0, 0, 0, TimeSpan.FromHours(1)));
        interval.TimeZone.ShouldBe(Cet);
    }

    [Test]
    public void Constructor_ShouldApplySummerOffset_ForCest()
    {
        // In summer CET is +02:00 (CEST)
        var utcStart = new DateTimeOffset(2026, 6, 15, 22, 0, 0, TimeSpan.Zero);
        var utcEnd   = new DateTimeOffset(2026, 6, 16, 22, 0, 0, TimeSpan.Zero);

        var interval = new ZonedTimeInterval(utcStart, utcEnd, Cet);

        interval.Start.Offset.ShouldBe(TimeSpan.FromHours(2));
        interval.End.Offset.ShouldBe(  TimeSpan.FromHours(2));
    }

    [Test]
    public void Constructor_ShouldThrow_WhenStartIsAfterEnd()
    {
        var start = new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.FromHours(1));
        var end   = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.FromHours(1));

        Should.Throw<ArgumentException>(() => new ZonedTimeInterval(start, end, Cet));
    }

    [Test]
    public void Constructor_ShouldThrow_WhenTimeZoneIsNull()
    {
        var dto = DateTimeOffset.UtcNow;

        Should.Throw<ArgumentNullException>(() => new ZonedTimeInterval(dto, dto, null!));
    }

    // ── Day factory ──────────────────────────────────────────────────────────

    [Test]
    public void Day_ShouldProduceMidnightToMidnightInZone_Winter()
    {
        var interval = ZonedTimeInterval.Day(new DateOnly(2026, 1, 15), Cet);

        interval.Start.ShouldBe(new DateTimeOffset(2026, 1, 15, 0, 0, 0, TimeSpan.FromHours(1)));
        interval.End.ShouldBe(  new DateTimeOffset(2026, 1, 16, 0, 0, 0, TimeSpan.FromHours(1)));
    }

    [Test]
    public void Day_ShouldProduceMidnightToMidnightInZone_Summer()
    {
        var interval = ZonedTimeInterval.Day(new DateOnly(2026, 6, 15), Cet);

        interval.Start.ShouldBe(new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.FromHours(2)));
        interval.End.ShouldBe(  new DateTimeOffset(2026, 6, 16, 0, 0, 0, TimeSpan.FromHours(2)));
    }

    // ── Contains ─────────────────────────────────────────────────────────────

    [Test]
    public void Contains_ShouldReturnTrue_ForInstantWithinInterval()
    {
        var interval = ZonedTimeInterval.Day(new DateOnly(2026, 1, 1), Cet);
        var inside   = new DateTimeOffset(2026, 1, 1, 14, 0, 0, TimeSpan.FromHours(1));

        interval.Contains(inside).ShouldBeTrue();
    }

    [Test]
    public void Contains_ShouldReturnFalse_ForInstantAfterEnd()
    {
        var interval = ZonedTimeInterval.Day(new DateOnly(2026, 1, 1), Cet);
        var after    = new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.FromHours(1));

        interval.Contains(after).ShouldBeFalse();
    }

    [Test]
    public void Contains_ShouldReturnTrue_ForStartInstant()
    {
        var interval = ZonedTimeInterval.Day(new DateOnly(2026, 1, 1), Cet);

        interval.Contains(interval.Start).ShouldBeTrue();
    }

    // ── Deconstruct ──────────────────────────────────────────────────────────

    [Test]
    public void Deconstruct_ShouldProvideStartAndEnd()
    {
        var interval = ZonedTimeInterval.Day(new DateOnly(2026, 1, 1), Cet);

        var (start, end) = interval;

        start.ShouldBe(interval.Start);
        end.ShouldBe(interval.End);
    }

    // ── ToUtc ────────────────────────────────────────────────────────────────

    [Test]
    public void ToUtc_ShouldReturnUtcNormalisedTimeInterval()
    {
        var interval = ZonedTimeInterval.Day(new DateOnly(2026, 1, 15), Cet);

        var utc = interval.ToUtc();

        utc.Start.ShouldBe(new DateTimeOffset(2026, 1, 14, 23, 0, 0, TimeSpan.Zero));
        utc.End.ShouldBe(  new DateTimeOffset(2026, 1, 15, 23, 0, 0, TimeSpan.Zero));
    }

    // ── SplitBy ──────────────────────────────────────────────────────────────

    [Test]
    public void SplitBy_QuarterHour_ShouldReturn96Slots_ForOneDay()
    {
        var interval = ZonedTimeInterval.Day(new DateOnly(2026, 1, 15), Cet);

        var slots = interval.SplitBy(Duration.QuarterHour).ToList();

        slots.Count.ShouldBe(96);
    }

    [Test]
    public void SplitBy_QuarterHour_ShouldReturnSlotsInZoneTime()
    {
        var interval = ZonedTimeInterval.Day(new DateOnly(2026, 1, 15), Cet);

        var first = interval.SplitBy(Duration.QuarterHour).First();

        // Slots should be in CET (+01:00 in January)
        first.Offset.ShouldBe(TimeSpan.FromHours(1));
        first.ShouldBe(new DateTimeOffset(2026, 1, 15, 0, 0, 0, TimeSpan.FromHours(1)));
    }

    [Test]
    public void SplitBy_DstSpringDay_ShouldReturn92Slots_ForCetToCest()
    {
        // 2026-03-29: CET→CEST spring forward — day is 23 hours = 92 quarter-hours
        var interval = ZonedTimeInterval.Day(new DateOnly(2026, 3, 29), Cet);

        var slots = interval.SplitBy(Duration.QuarterHour).ToList();

        slots.Count.ShouldBe(92);
    }

    // ── SplitByDays ──────────────────────────────────────────────────────────

    [Test]
    public void SplitByDays_ShouldReturnOneMidnightPerDay()
    {
        var start    = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.FromHours(1));
        var end      = new DateTimeOffset(2026, 1, 4, 0, 0, 0, TimeSpan.FromHours(1));
        var interval = new ZonedTimeInterval(start, end, Cet);

        var days = interval.SplitByDays().ToList();

        days.Count.ShouldBe(3);
        days[0].ShouldBe(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.FromHours(1)));
        days[1].ShouldBe(new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.FromHours(1)));
        days[2].ShouldBe(new DateTimeOffset(2026, 1, 3, 0, 0, 0, TimeSpan.FromHours(1)));
    }

    [Test]
    public void SplitByDays_ShouldReturnSlotsInZoneTime()
    {
        var interval = ZonedTimeInterval.Day(new DateOnly(2026, 6, 15), Cet);

        var days = interval.SplitByDays().ToList();

        days.Single().Offset.ShouldBe(TimeSpan.FromHours(2)); // CEST
    }

    [Test]
    public void SplitByDays_DstSpringTransition_ShouldHandleOffsetChange()
    {
        // 2026-03-29 starts at +01:00 (CET) but the day switches to +02:00 (CEST)
        // SplitByDays across transition should give correct midnights
        var start    = new DateTimeOffset(2026, 3, 28, 0, 0, 0, TimeSpan.FromHours(1));
        var end      = new DateTimeOffset(2026, 3, 30, 0, 0, 0, TimeSpan.FromHours(2));
        var interval = new ZonedTimeInterval(start, end, Cet);

        var days = interval.SplitByDays().ToList();

        days.Count.ShouldBe(2);
        days[0].ShouldBe(new DateTimeOffset(2026, 3, 28, 0, 0, 0, TimeSpan.FromHours(1)));
        days[1].ShouldBe(new DateTimeOffset(2026, 3, 29, 0, 0, 0, TimeSpan.FromHours(1)));
    }
}
