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
    public void Seconds_Int_ShouldReturnDuration() =>
        30.Seconds().ShouldBe(new Duration(TimeSpan.FromSeconds(30)));

    [Test]
    public void Seconds_Double_ShouldReturnDuration() =>
        1.5d.Seconds().ShouldBe(new Duration(TimeSpan.FromSeconds(1.5)));

    [Test]
    public void Seconds_WithOffset_ShouldAddOffset()
    {
        var result = 30.Seconds(10.Seconds());

        result.ShouldBe(TimeSpan.FromSeconds(40));
    }


    [Test]
    public void Minutes_Int_ShouldReturnDuration() =>
        45.Minutes().ShouldBe(new Duration(TimeSpan.FromMinutes(45)));

    [Test]
    public void Minutes_Double_ShouldReturnDuration() =>
        0.5d.Minutes().ShouldBe(new Duration(TimeSpan.FromMinutes(0.5)));

    [Test]
    public void Minutes_WithOffset_ShouldAddOffset()
    {
        var result = 5.Minutes(30.Seconds());

        result.ShouldBe(TimeSpan.FromSeconds(330));
    }


    [Test]
    public void Hours_Int_ShouldReturnDuration() =>
        3.Hours().ShouldBe(new Duration(TimeSpan.FromHours(3)));

    [Test]
    public void Hours_Double_ShouldReturnDuration() =>
        1.5d.Hours().ShouldBe(new Duration(TimeSpan.FromHours(1.5)));

    [Test]
    public void Hours_WithOffset_ShouldAddOffset()
    {
        var result = 2.Hours(30.Minutes());

        result.ShouldBe(TimeSpan.FromMinutes(150));
    }


    [Test]
    public void Days_Int_ShouldReturnDuration() =>
        7.Days().ShouldBe(new Duration(TimeSpan.FromDays(7)));

    [Test]
    public void Days_Double_ShouldReturnDuration() =>
        0.5d.Days().ShouldBe(new Duration(TimeSpan.FromHours(12)));

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

        result.ShouldBe(new Duration(TimeSpan.FromMinutes(90)));
    }

    [Test]
    public void And_ShouldSupportChaining()
    {
        var result = 1.Days().And(2.Hours()).And(30.Minutes()).And(15.Seconds());

        result.ShouldBe(new Duration(TimeSpan.FromDays(1) + TimeSpan.FromHours(2) + TimeSpan.FromMinutes(30) +
                                     TimeSpan.FromSeconds(15)));
    }

    [Test]
    public void Days_NegativeInput_ShouldThrowArgumentOutOfRangeException() =>
        Should.Throw<ArgumentOutOfRangeException>(() => (-1).Days());

    [Test]
    public void Hours_NegativeInput_ShouldThrowArgumentOutOfRangeException() =>
        Should.Throw<ArgumentOutOfRangeException>(() => (-1).Hours());

    [Test]
    public void Minutes_NegativeInput_ShouldThrowArgumentOutOfRangeException() =>
        Should.Throw<ArgumentOutOfRangeException>(() => (-5).Minutes());

    [Test]
    public void Seconds_NegativeInput_ShouldThrowArgumentOutOfRangeException() =>
        Should.Throw<ArgumentOutOfRangeException>(() => (-30).Seconds());

    // And(TimeSpan, TimeSpan) overload — invoked when the argument is a raw TimeSpan

    [Test]
    public void And_TimeSpan_ShouldAddTwoTimeSpans()
    {
        // And(this TimeSpan, TimeSpan) is called when the receiver is a raw TimeSpan.
        var result = TimeSpan.FromHours(1).And(TimeSpan.FromMilliseconds(500));

        result.ShouldBe(TimeSpan.FromHours(1).Add(TimeSpan.FromMilliseconds(500)));
    }
}
