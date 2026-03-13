using Shouldly;

namespace Horae.Tests;

[TestFixture] [Category("UnitTest")]
public class HoareSettingsTests
{
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

    [Test]
    public void Use_ShouldOverrideCurrentTimeZoneWithinScope()
    {
        var before = HoareSettings.CurrentTimeZone;

        using (HoareSettings.Use(TimeZones.Japan))
            HoareSettings.CurrentTimeZone.ShouldBe(TimeZones.Japan);

        HoareSettings.CurrentTimeZone.ShouldBe(before);
    }

    [Test]
    public void Use_ShouldRestoreTimeZone_EvenWhenExceptionIsThrown()
    {
        var before = HoareSettings.CurrentTimeZone;

        try
        {
            using (HoareSettings.Use(TimeZones.Japan))
                throw new InvalidOperationException("simulated");
        }
        catch (InvalidOperationException) { }

        HoareSettings.CurrentTimeZone.ShouldBe(before);
    }
}
