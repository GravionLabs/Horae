using Shouldly;

namespace Horae.Tests;

[TestFixture, Category("UnitTest")]
public class RegionExtensionsTests
{
    // A fixed UTC reference instant used across all tests.
    private static readonly DateTimeOffset _reference = new(2026, 6, 15, 12, 0, 0, TimeSpan.Zero);

    [Test]
    public void CentralEuropeanToUtc_ShouldResultInUtcOffset()
    {
        var utc = _reference.CentralEuropeanToUtc();
        utc.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Test]
    public void LondonToUtc_ShouldResultInUtcOffset()
    {
        var utc = _reference.LondonToUtc();
        utc.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Test]
    public void LisbonToUtc_ShouldResultInUtcOffset()
    {
        var utc = _reference.LisbonToUtc();
        utc.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Test]
    public void EasternEuropeToUtc_ShouldResultInUtcOffset()
    {
        var utc = _reference.EasternEuropeToUtc();
        utc.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Test]
    public void MoscowToUtc_ShouldResultInUtcOffset()
    {
        var utc = _reference.MoscowToUtc();
        utc.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Test]
    public void IstanbulToUtc_ShouldResultInUtcOffset()
    {
        var utc = _reference.IstanbulToUtc();
        utc.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Test]
    public void EasternUsToUtc_ShouldResultInUtcOffset()
    {
        var utc = _reference.EasternUsToUtc();
        utc.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Test]
    public void PacificUsToUtc_ShouldResultInUtcOffset()
    {
        var utc = _reference.PacificUsToUtc();
        utc.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Test]
    public void DubaiToUtc_ShouldResultInUtcOffset()
    {
        var utc = _reference.DubaiToUtc();
        utc.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Test]
    public void IndiaToUtc_ShouldResultInUtcOffset()
    {
        var utc = _reference.IndiaToUtc();
        utc.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Test]
    public void JapanToUtc_ShouldResultInUtcOffset()
    {
        var utc = _reference.JapanToUtc();
        utc.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Test]
    public void AustraliaEasternToUtc_ShouldResultInUtcOffset()
    {
        var utc = _reference.AustraliaEasternToUtc();
        utc.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Test]
    public void ToUtc_ShouldConvertCorrectly_ForJapanStandardTime()
    {
        // Japan is UTC+9 with no DST. 12:00 JST = 03:00 UTC.
        var jst = new DateTimeOffset(2026, 1, 15, 12, 0, 0, TimeSpan.FromHours(9));
        var utc = jst.ToUtc(TimeZones.Japan);

        utc.Offset.ShouldBe(TimeSpan.Zero);
        utc.Hour.ShouldBe(3);
    }
}

