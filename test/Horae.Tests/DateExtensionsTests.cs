using Shouldly;

namespace Horae.Tests;

[TestFixture(Category = "UnitTest")]
public class DateExtensionsTests
{
    [TestCase(1, 1, Description = "January")]
    [TestCase(2, 2, Description = "February")]
    [TestCase(3, 3, Description = "March")]
    [TestCase(4, 4, Description = "April")]
    [TestCase(5, 5, Description = "May")]
    [TestCase(6, 6, Description = "June")]
    [TestCase(7, 7, Description = "July")]
    [TestCase(8, 8, Description = "August")]
    [TestCase(9, 9, Description = "September")]
    [TestCase(10, 10, Description = "October")]
    [TestCase(11, 11, Description = "November")]
    [TestCase(12, 12, Description = "December")]
    public void MonthMethod_ShouldReturnMidnightUtc(int month, int expectedMonth)
    {
        var result = month switch
        {
            1 => 15.January(2026),
            2 => 15.February(2026),
            3 => 15.March(2026),
            4 => 15.April(2026),
            5 => 15.May(2026),
            6 => 15.June(2026),
            7 => 15.July(2026),
            8 => 15.August(2026),
            9 => 15.September(2026),
            10 => 15.October(2026),
            11 => 15.November(2026),
            12 => 15.December(2026),
            _ => throw new InvalidOperationException()
        };

        result.Month.ShouldBe(expectedMonth);
        result.Day.ShouldBe(15);
        result.Year.ShouldBe(2026);
        result.TimeOfDay.ShouldBe(TimeSpan.Zero);
        result.Offset.ShouldBe(TimeSpan.Zero);
    }


    [Test]
    public void At_WithTimeSpan_ShouldSetTimeOfDay()
    {
        var result = 15.March(2026).At(new TimeSpan(9, 30, 0));

        result.Hour.ShouldBe(9);
        result.Minute.ShouldBe(30);
        result.Second.ShouldBe(0);
    }

    [Test]
    public void At_WithTimeSpan_ShouldPreserveOffset()
    {
        var source = new DateTimeOffset(2026, 3, 15, 0, 0, 0, TimeSpan.FromHours(4)); // Dubai
        var result = source.At(new TimeSpan(14, 0, 0));

        result.Offset.ShouldBe(TimeSpan.FromHours(4));
        result.Hour.ShouldBe(14);
    }


    [Test]
    public void At_WithComponents_ShouldSetTimeCorrectly()
    {
        var result = 15.March(2026).At(9, 30, 15, 500);

        result.Hour.ShouldBe(9);
        result.Minute.ShouldBe(30);
        result.Second.ShouldBe(15);
        result.Millisecond.ShouldBe(500);
    }

    [Test]
    public void At_WithMicroseconds_ShouldIncludeMicroseconds()
    {
        var result = 15.March(2026).At(10, 0, 0, 0, 500);

        result.Microsecond.ShouldBe(500);
    }

    [Test]
    public void At_WithNanoseconds_ShouldAddNanoseconds()
    {
        var result = 15.March(2026).At(10, 0, 0, 0, 0, 100);

        // 100 ns = 1 tick; verify total ticks advanced by at least 1 tick
        var baseline = 15.March(2026).At(10, 0);
        (result.Ticks - baseline.Ticks).ShouldBe(1L);
    }

    [Test]
    public void At_ShouldPreserveOffset_WhenUsingComponents()
    {
        var source = new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.FromHours(4));
        var result = source.At(8, 0);

        result.Offset.ShouldBe(TimeSpan.FromHours(4));
    }

    [Test]
    public void At_ShouldThrow_WhenMicrosecondsOutOfRange() =>
        Should.Throw<ArgumentOutOfRangeException>(() => 1.January(2026).At(0, 0, microseconds: 1000));

    [Test]
    public void At_ShouldThrow_WhenNanosecondsOutOfRange() =>
        Should.Throw<ArgumentOutOfRangeException>(() => 1.January(2026).At(0, 0, nanoseconds: -1));


    [Test]
    public void Before_ShouldReturnCorrectDateTime()
    {
        var reference = new DateTimeOffset(2026, 3, 15, 12, 0, 0, TimeSpan.Zero);

        var result = TimeSpan.FromHours(2).Before(reference);

        result.ShouldBe(new DateTimeOffset(2026, 3, 15, 10, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void Before_ShouldPreserveOffset()
    {
        var reference = new DateTimeOffset(2026, 3, 15, 12, 0, 0, TimeSpan.FromHours(4));

        var result = TimeSpan.FromHours(1).Before(reference);

        result.Offset.ShouldBe(TimeSpan.FromHours(4));
        result.Hour.ShouldBe(11);
    }

    [Test]
    public void Before_WithDays_ShouldSubtractCorrectly()
    {
        var reference = new DateTimeOffset(2026, 3, 15, 0, 0, 0, TimeSpan.Zero);

        var result = TimeSpan.FromDays(5).Before(reference);

        result.ShouldBe(new DateTimeOffset(2026, 3, 10, 0, 0, 0, TimeSpan.Zero));
    }


    [Test]
    public void After_ShouldReturnCorrectDateTime()
    {
        var reference = new DateTimeOffset(2026, 3, 15, 12, 0, 0, TimeSpan.Zero);

        var result = TimeSpan.FromHours(3).After(reference);

        result.ShouldBe(new DateTimeOffset(2026, 3, 15, 15, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void After_ShouldPreserveOffset()
    {
        var reference = new DateTimeOffset(2026, 3, 15, 10, 0, 0, TimeSpan.FromHours(9));

        var result = TimeSpan.FromMinutes(90).After(reference);

        result.Offset.ShouldBe(TimeSpan.FromHours(9));
        result.Hour.ShouldBe(11);
        result.Minute.ShouldBe(30);
    }


    [Test]
    public void AddNanoseconds_ShouldAddCorrectly()
    {
        var dto = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        var result = dto.AddNanoseconds(1000); // 1000 ns = 10 ticks

        (result.Ticks - dto.Ticks).ShouldBe(10L);
    }

    [Test]
    public void AddNanoseconds_ShouldSupportNegativeValues()
    {
        var dto = new DateTimeOffset(2026, 1, 1, 0, 0, 1, TimeSpan.Zero); // 1 second in

        var result = dto.AddNanoseconds(-1_000_000_000L); // -1 second in ns

        result.Second.ShouldBe(0);
    }
}
