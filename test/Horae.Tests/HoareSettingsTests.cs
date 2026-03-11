using Shouldly;

namespace Horae.Tests;

[TestFixture, Category("UnitTest")]
public class HoareSettingsTests
{
    // Store the original value so each test starts clean and does not affect others.
    private TimeZoneInfo _originalTimeZone = null!;

    [SetUp]
    public void SetUp() => _originalTimeZone = HoareSettings.CurrentTimeZone;

    [TearDown]
    public void TearDown() => HoareSettings.CurrentTimeZone = _originalTimeZone;

    [Test]
    public void CurrentTimeZone_ShouldDefaultToLocalTimeZone() =>
        HoareSettings.CurrentTimeZone.ShouldBe(TimeZoneInfo.Local);

    [Test]
    public void CurrentTimeZone_ShouldReturnNewValueAfterBeingSet()
    {
        HoareSettings.CurrentTimeZone = TimeZones.CentralEuropean;

        HoareSettings.CurrentTimeZone.ShouldBe(TimeZones.CentralEuropean);
    }

    [Test]
    public void CurrentTimeZone_ShouldBeUsedByStartOfDay_WhenNoTimeZoneIsProvided()
    {
        HoareSettings.CurrentTimeZone = TimeZones.CentralEuropean;

        var dto = new DateTimeOffset(2026, 6, 15, 14, 30, 0, TimeSpan.FromHours(2));
        var result = dto.StartOfDay();

        // Midnight in CET+2 (summer time)
        result.ShouldBe(new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.FromHours(2)));
    }

    [Test]
    public void CurrentTimeZone_ShouldBeUsedByEndOfDay_WhenNoTimeZoneIsProvided()
    {
        HoareSettings.CurrentTimeZone = TimeZones.CentralEuropean;

        var dto = new DateTimeOffset(2026, 6, 15, 14, 30, 0, TimeSpan.FromHours(2));
        var result = dto.EndOfDay();

        // EndOfDay returns the first instant of the next day (exclusive upper boundary).
        result.ShouldBe(new DateTimeOffset(2026, 6, 16, 0, 0, 0, TimeSpan.FromHours(2)));
    }
}
