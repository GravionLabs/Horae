using Shouldly;

namespace Horae.Tests;

[TestFixture(Category = "UnitTest")]
public class IsoWeekExtensionsTests
{
    // IsoWeekNumber

    [Test]
    [TestCase(2025, 12, 29, 1)]  // Dec 29 2025 is ISO week 1 of 2026
    [TestCase(2026, 1, 1, 1)]    // Jan 1 2026 is still week 1
    [TestCase(2026, 5, 11, 20)]  // May 11 2026 = week 20
    [TestCase(2026, 12, 28, 53)] // Dec 28 2026 = week 53
    public void IsoWeekNumber_DateOnly_ShouldReturnCorrectWeek(int year, int month, int day, int expected)
    {
        var date = new DateOnly(year, month, day);
        date.IsoWeekNumber().ShouldBe(expected);
    }

    [Test]
    [TestCase(2025, 12, 29, 1)]
    [TestCase(2026, 5, 11, 20)]
    public void IsoWeekNumber_DateTimeOffset_ShouldUseUtcDate(int year, int month, int day, int expected)
    {
        var dto = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero);
        dto.IsoWeekNumber().ShouldBe(expected);
    }

    // IsoWeekYear

    [Test]
    [TestCase(2025, 12, 29, 2026)] // belongs to ISO week year 2026
    [TestCase(2026, 1, 1, 2026)]
    [TestCase(2026, 5, 11, 2026)]
    [TestCase(2015, 12, 28, 2015)] // Dec 28 2015 = ISO week year 2015
    public void IsoWeekYear_DateOnly_ShouldReturnCorrectYear(int year, int month, int day, int expected)
    {
        var date = new DateOnly(year, month, day);
        date.IsoWeekYear().ShouldBe(expected);
    }

    [Test]
    [TestCase(2025, 12, 29, 2026)]
    [TestCase(2026, 1, 1, 2026)]
    public void IsoWeekYear_DateTimeOffset_ShouldUseUtcDate(int year, int month, int day, int expected)
    {
        var dto = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero);
        dto.IsoWeekYear().ShouldBe(expected);
    }

    // IsoWeekday

    [Test]
    [TestCase(2026, 5, 11, 1)] // Monday = 1
    [TestCase(2026, 5, 12, 2)] // Tuesday = 2
    [TestCase(2026, 5, 13, 3)] // Wednesday = 3
    [TestCase(2026, 5, 14, 4)] // Thursday = 4
    [TestCase(2026, 5, 15, 5)] // Friday = 5
    [TestCase(2026, 5, 16, 6)] // Saturday = 6
    [TestCase(2026, 5, 17, 7)] // Sunday = 7
    public void IsoWeekday_DateOnly_ShouldReturn1ForMonday7ForSunday(int year, int month, int day, int expected)
    {
        var date = new DateOnly(year, month, day);
        date.IsoWeekday().ShouldBe(expected);
    }

    [Test]
    [TestCase(2026, 5, 11, 1)] // Monday = 1
    [TestCase(2026, 5, 17, 7)] // Sunday = 7
    public void IsoWeekday_DateTimeOffset_ShouldUseUtcDayOfWeek(int year, int month, int day, int expected)
    {
        var dto = new DateTimeOffset(year, month, day, 12, 0, 0, TimeSpan.Zero);
        dto.IsoWeekday().ShouldBe(expected);
    }
}
