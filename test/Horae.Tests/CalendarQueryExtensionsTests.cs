using Shouldly;

namespace Horae.Tests;

[TestFixture(Category = "UnitTest")]
public class CalendarQueryExtensionsTests
{
    // IsLeapYear

    [Test]
    [TestCase(2024, true)]
    [TestCase(2025, false)]
    [TestCase(2000, true)]
    [TestCase(1900, false)]
    public void IsLeapYear_DateOnly_ShouldReturnCorrectResult(int year, bool expected)
    {
        var date = new DateOnly(year, 6, 15);
        date.IsLeapYear().ShouldBe(expected);
    }

    [Test]
    [TestCase(2024, true)]
    [TestCase(2025, false)]
    public void IsLeapYear_DateTimeOffset_ShouldUseUtcYear(int year, bool expected)
    {
        var dto = new DateTimeOffset(year, 6, 15, 0, 0, 0, TimeSpan.Zero);
        dto.IsLeapYear().ShouldBe(expected);
    }

    // DaysInMonth

    [Test]
    [TestCase(2026, 1, 31)]
    [TestCase(2026, 2, 28)]
    [TestCase(2024, 2, 29)]
    [TestCase(2026, 4, 30)]
    [TestCase(2026, 12, 31)]
    public void DaysInMonth_DateOnly_ShouldReturnCorrectCount(int year, int month, int expected)
    {
        var date = new DateOnly(year, month, 1);
        date.DaysInMonth().ShouldBe(expected);
    }

    [Test]
    [TestCase(2026, 1, 31)]
    [TestCase(2024, 2, 29)]
    [TestCase(2026, 4, 30)]
    public void DaysInMonth_DateTimeOffset_ShouldUseUtcMonth(int year, int month, int expected)
    {
        var dto = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero);
        dto.DaysInMonth().ShouldBe(expected);
    }

    // DaysInYear

    [Test]
    [TestCase(2024, 366)]
    [TestCase(2025, 365)]
    public void DaysInYear_DateOnly_ShouldReturnCorrectCount(int year, int expected)
    {
        var date = new DateOnly(year, 3, 1);
        date.DaysInYear().ShouldBe(expected);
    }

    [Test]
    [TestCase(2024, 366)]
    [TestCase(2025, 365)]
    public void DaysInYear_DateTimeOffset_ShouldUseUtcYear(int year, int expected)
    {
        var dto = new DateTimeOffset(year, 3, 1, 0, 0, 0, TimeSpan.Zero);
        dto.DaysInYear().ShouldBe(expected);
    }

    // Quarter

    [Test]
    [TestCase(1, 1)]
    [TestCase(3, 1)]
    [TestCase(4, 2)]
    [TestCase(6, 2)]
    [TestCase(7, 3)]
    [TestCase(9, 3)]
    [TestCase(10, 4)]
    [TestCase(12, 4)]
    public void Quarter_DateOnly_ShouldReturnCorrectQuarter(int month, int expectedQuarter)
    {
        var date = new DateOnly(2026, month, 15);
        date.Quarter().ShouldBe(expectedQuarter);
    }

    [Test]
    [TestCase(1, 1)]
    [TestCase(4, 2)]
    [TestCase(7, 3)]
    [TestCase(10, 4)]
    public void Quarter_DateTimeOffset_ShouldReturnCorrectQuarter(int month, int expectedQuarter)
    {
        var dto = new DateTimeOffset(2026, month, 15, 0, 0, 0, TimeSpan.Zero);
        dto.Quarter().ShouldBe(expectedQuarter);
    }

    // DayOfYear

    [Test]
    [TestCase(2026, 1, 1, 1)]
    [TestCase(2026, 1, 31, 31)]
    [TestCase(2026, 2, 1, 32)]
    [TestCase(2026, 12, 31, 365)]
    [TestCase(2024, 12, 31, 366)]
    public void DayOfYear_DateOnly_ShouldReturnCorrectOrdinal(int year, int month, int day, int expected)
    {
        var date = new DateOnly(year, month, day);
        date.DayOfYear().ShouldBe(expected);
    }

    [Test]
    [TestCase(2026, 1, 1, 1)]
    [TestCase(2026, 12, 31, 365)]
    [TestCase(2024, 12, 31, 366)]
    public void DayOfYear_DateTimeOffset_ShouldReturnCorrectOrdinal(int year, int month, int day, int expected)
    {
        var dto = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero);
        dto.DayOfYear().ShouldBe(expected);
    }

    // IsWeekend / IsWeekday for DateOnly

    [Test]
    [TestCase(2026, 5, 9, true)]   // Saturday
    [TestCase(2026, 5, 10, true)]  // Sunday
    [TestCase(2026, 5, 11, false)] // Monday
    [TestCase(2026, 5, 15, false)] // Friday
    public void IsWeekend_DateOnly_ShouldReturnCorrectResult(int year, int month, int day, bool expected)
    {
        var date = new DateOnly(year, month, day);
        date.IsWeekend().ShouldBe(expected);
    }

    [Test]
    [TestCase(2026, 5, 9, false)]  // Saturday → not weekday
    [TestCase(2026, 5, 11, true)]  // Monday
    public void IsWeekday_DateOnly_ShouldReturnOppositeOfIsWeekend(int year, int month, int day, bool expected)
    {
        var date = new DateOnly(year, month, day);
        date.IsWeekday().ShouldBe(expected);
    }

    // IsWeekend / IsWeekday for DateTimeOffset (timezone-aware)

    [Test]
    public void IsWeekend_DateTimeOffset_ShouldRespectTimeZone()
    {
        // Sunday 23:00 UTC = Monday 08:00 in UTC+9 (Japan) → not a weekend day in JST
        var sundayEvening = new DateTimeOffset(2026, 5, 10, 23, 0, 0, TimeSpan.Zero);
        sundayEvening.IsWeekend(TimeZoneInfo.Utc).ShouldBeTrue();
        sundayEvening.IsWeekend(TimeZones.Japan).ShouldBeFalse(); // already Monday in JST
    }

    [Test]
    public void IsWeekend_DateTimeOffset_ShouldFallBackToCurrentTimeZone()
    {
        HoareSettings.CurrentTimeZone = TimeZoneInfo.Utc;
        var sunday = new DateTimeOffset(2026, 5, 10, 0, 0, 0, TimeSpan.Zero);
        sunday.IsWeekend().ShouldBeTrue();
        HoareSettings.CurrentTimeZone = null;
    }

    [Test]
    public void IsWeekday_DateTimeOffset_ShouldReturnOppositeOfIsWeekend()
    {
        var monday = new DateTimeOffset(2026, 5, 11, 12, 0, 0, TimeSpan.Zero); // Monday UTC
        monday.IsWeekday(TimeZoneInfo.Utc).ShouldBeTrue();
        monday.IsWeekend(TimeZoneInfo.Utc).ShouldBeFalse();
    }
}
