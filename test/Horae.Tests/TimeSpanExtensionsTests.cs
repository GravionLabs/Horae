using Shouldly;

namespace Horae.Tests;

[TestFixture(Category = "UnitTest")]
public class TimeSpanExtensionsTests
{
    [Test]
    public void TicksPerMicrosecond_ShouldBe10() =>
        TimeSpanExtensions.TicksPerMicrosecond.ShouldBe(10L);

    [Test]
    public void TicksPerNanosecond_ShouldBe0Point01() =>
        TimeSpanExtensions.TicksPerNanosecond.ShouldBe(0.01d);


    [Test]
    public void Ticks_Int_ShouldReturnCorrectTimeSpan() =>
        100.Ticks().ShouldBe(TimeSpan.FromTicks(100));

    [Test]
    public void Ticks_Long_ShouldReturnCorrectTimeSpan() =>
        100L.Ticks().ShouldBe(TimeSpan.FromTicks(100));


    [Test]
    public void Nanoseconds_Int_100_ShouldEqual1Tick() =>
        // 100 ns = 1 tick (1 tick = 100 ns)
        100.Nanoseconds().Ticks.ShouldBe(1L);

    [Test]
    public void Nanoseconds_Long_1000_ShouldEqual10Ticks() => 1000L.Nanoseconds().Ticks.ShouldBe(10L);

    [Test]
    public void Nanoseconds_Int_SubResolution_ShouldRound() =>
        // 50 ns → 0.5 ticks → rounds to 1 tick (Math.Round default = round-half-to-even, but 0.5 rounds to 0 here)
        // 150 ns → 1.5 ticks → rounds to 2 ticks
        150.Nanoseconds().Ticks.ShouldBe(2L);


    [Test]
    public void Microseconds_Int_ShouldReturn10TicksPerMicrosecond() =>
        // 1 µs = 10 ticks
        1.Microseconds().Ticks.ShouldBe(10L);

    [Test]
    public void Microseconds_Long_ShouldReturnCorrectTimeSpan() =>
        500L.Microseconds().ShouldBe(TimeSpan.FromTicks(5000));


    [Test]
    public void Milliseconds_Int_ShouldReturnCorrectTimeSpan() =>
        250.Milliseconds().ShouldBe(TimeSpan.FromMilliseconds(250));

    [Test]
    public void Milliseconds_Double_ShouldReturnCorrectTimeSpan() =>
        1.5d.Milliseconds().ShouldBe(TimeSpan.FromMilliseconds(1.5));


    [Test]
    public void Seconds_Int_ShouldReturnCorrectTimeSpan() =>
        30.Seconds().ShouldBe(TimeSpan.FromSeconds(30));

    [Test]
    public void Seconds_Double_ShouldReturnCorrectTimeSpan() =>
        1.5d.Seconds().ShouldBe(TimeSpan.FromSeconds(1.5));

    [Test]
    public void Seconds_WithOffset_ShouldAddOffset()
    {
        var result = 30.Seconds(10.Seconds());

        result.ShouldBe(TimeSpan.FromSeconds(40));
    }


    [Test]
    public void Minutes_Int_ShouldReturnCorrectTimeSpan() =>
        45.Minutes().ShouldBe(TimeSpan.FromMinutes(45));

    [Test]
    public void Minutes_Double_ShouldReturnCorrectTimeSpan() =>
        0.5d.Minutes().ShouldBe(TimeSpan.FromMinutes(0.5));

    [Test]
    public void Minutes_WithOffset_ShouldAddOffset()
    {
        var result = 5.Minutes(30.Seconds());

        result.ShouldBe(TimeSpan.FromSeconds(330));
    }


    [Test]
    public void Hours_Int_ShouldReturnCorrectTimeSpan() =>
        3.Hours().ShouldBe(TimeSpan.FromHours(3));

    [Test]
    public void Hours_Double_ShouldReturnCorrectTimeSpan() =>
        1.5d.Hours().ShouldBe(TimeSpan.FromHours(1.5));

    [Test]
    public void Hours_WithOffset_ShouldAddOffset()
    {
        var result = 2.Hours(30.Minutes());

        result.ShouldBe(TimeSpan.FromMinutes(150));
    }


    [Test]
    public void Days_Int_ShouldReturnCorrectTimeSpan() =>
        7.Days().ShouldBe(TimeSpan.FromDays(7));

    [Test]
    public void Days_Double_ShouldReturnCorrectTimeSpan() =>
        0.5d.Days().ShouldBe(TimeSpan.FromHours(12));

    [Test]
    public void Days_WithOffset_ShouldAddOffset()
    {
        var result = 1.Days(6.Hours());

        result.ShouldBe(TimeSpan.FromHours(30));
    }


    [Test]
    public void And_ShouldCombineTimeSpans()
    {
        var result = 1.Hours().And(30.Minutes());

        result.ShouldBe(TimeSpan.FromMinutes(90));
    }

    [Test]
    public void And_ShouldSupportChaining()
    {
        var result = 1.Days().And(2.Hours()).And(30.Minutes()).And(15.Seconds());

        result.ShouldBe(TimeSpan.FromDays(1) + TimeSpan.FromHours(2) + TimeSpan.FromMinutes(30) +
                        TimeSpan.FromSeconds(15));
    }
}
