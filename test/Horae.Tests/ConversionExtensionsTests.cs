using Shouldly;

namespace Horae.Tests;

[TestFixture(Category = "UnitTest")]
public class ConversionExtensionsTests
{
    private TimeZoneInfo _originalTimeZone = null!;

    [SetUp]
    public void SetUp() => _originalTimeZone = HoareSettings.CurrentTimeZone;

    [TearDown]
    public void TearDown() => HoareSettings.CurrentTimeZone = _originalTimeZone;


    [Test]
    public void AsDateOnly_ShouldReturnDateInExplicitTimeZone()
    {
        // 2026-01-01 23:00 UTC = 2026-01-02 08:00 JST → date is 2026-01-02 in Japan.
        var dto = new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero);

        dto.AsDateOnly(TimeZones.Japan).ShouldBe(new DateOnly(2026, 1, 2));
    }

    [Test]
    public void AsDateOnly_ShouldUseCurrentTimeZone_WhenNullIsPassed()
    {
        HoareSettings.CurrentTimeZone = TimeZones.Japan;

        var dto = new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero);

        dto.AsDateOnly().ShouldBe(new DateOnly(2026, 1, 2));
    }


    [Test]
    public void AsUtcDateOnly_ShouldReturnUtcDate()
    {
        // 2026-01-01 23:00 UTC → UTC date is still 2026-01-01.
        var dto = new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero);

        dto.AsUtcDateOnly().ShouldBe(new DateOnly(2026, 1, 1));
    }

    [Test]
    public void AsUtcDateOnly_ShouldIgnoreCurrentTimeZone()
    {
        HoareSettings.CurrentTimeZone = TimeZones.Japan;

        var dto = new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero);

        // Always UTC, regardless of HoareSettings.
        dto.AsUtcDateOnly().ShouldBe(new DateOnly(2026, 1, 1));
    }


    [Test]
    public void AsDateTime_ShouldReturnDateTimeInExplicitTimeZone()
    {
        // 10:00 UTC = 19:00 JST (UTC+9).
        var dto = new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero);

        var result = dto.AsDateTime(TimeZones.Japan);

        result.Hour.ShouldBe(19);
        result.Kind.ShouldBe(DateTimeKind.Unspecified);
    }

    [Test]
    public void AsDateTime_ShouldUseCurrentTimeZone_WhenNullIsPassed()
    {
        HoareSettings.CurrentTimeZone = TimeZones.Japan;

        var dto = new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero);

        dto.AsDateTime().Hour.ShouldBe(19);
    }


    [Test]
    public void AsUtcDateTime_ShouldReturnUtcDateTime()
    {
        var dto = new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.FromHours(2));

        var result = dto.AsUtcDateTime();

        result.ShouldBe(new DateTime(2026, 6, 15, 8, 0, 0, DateTimeKind.Utc));
        result.Kind.ShouldBe(DateTimeKind.Utc);
    }


    [Test]
    public void AsDateTimeOffset_ShouldReturnMidnightWithCorrectOffset_ForExplicitTimeZone()
    {
        var date = new DateOnly(2026, 6, 15);

        // Dubai is UTC+4 (no DST).
        var result = date.AsDateTimeOffset(TimeZones.Dubai);

        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.FromHours(4)));
    }

    [Test]
    public void AsDateTimeOffset_ShouldUseCurrentTimeZone_WhenNullIsPassed()
    {
        HoareSettings.CurrentTimeZone = TimeZones.Dubai;

        var date = new DateOnly(2026, 6, 15);

        date.AsDateTimeOffset().ShouldBe(new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.FromHours(4)));
    }


    [Test]
    public void AsUtcDateTimeOffset_ShouldReturnMidnightUtc()
    {
        var date = new DateOnly(2026, 6, 15);

        var result = date.AsUtcDateTimeOffset();

        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void AsUtcDateTimeOffset_ShouldHaveZeroOffset()
    {
        var date = new DateOnly(2026, 1, 1);

        date.AsUtcDateTimeOffset().Offset.ShouldBe(TimeSpan.Zero);
    }
}

