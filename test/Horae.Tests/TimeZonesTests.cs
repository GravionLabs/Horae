using Shouldly;

namespace Horae.Tests;

[TestFixture, Category("UnitTest")]
public class TimeZonesTests
{
    // ── UTC ──────────────────────────────────────────────────────────────────

    [Test]
    public void Utc_ShouldNotBeNull() => TimeZones.Utc.ShouldNotBeNull();

    [Test]
    public void Utc_ShouldHaveCorrectId() => TimeZones.Utc.Id.ShouldBe("UTC");

    // ── Europe ───────────────────────────────────────────────────────────────

    [Test]
    public void CentralEuropean_ShouldNotBeNull() => TimeZones.CentralEuropean.ShouldNotBeNull();

    [Test]
    public void CentralEuropean_ShouldHaveCorrectId() => TimeZones.CentralEuropean.Id.ShouldBe("Europe/Berlin");

    [Test]
    public void London_ShouldNotBeNull() => TimeZones.London.ShouldNotBeNull();

    [Test]
    public void London_ShouldHaveCorrectId() => TimeZones.London.Id.ShouldBe("Europe/London");

    [Test]
    public void Lisbon_ShouldNotBeNull() => TimeZones.Lisbon.ShouldNotBeNull();

    [Test]
    public void Lisbon_ShouldHaveCorrectId() => TimeZones.Lisbon.Id.ShouldBe("Europe/Lisbon");

    [Test]
    public void EasternEurope_ShouldNotBeNull() => TimeZones.EasternEurope.ShouldNotBeNull();

    [Test]
    public void EasternEurope_ShouldHaveCorrectId() => TimeZones.EasternEurope.Id.ShouldBe("Europe/Athens");

    [Test]
    public void Moscow_ShouldNotBeNull() => TimeZones.Moscow.ShouldNotBeNull();

    [Test]
    public void Moscow_ShouldHaveCorrectId() => TimeZones.Moscow.Id.ShouldBe("Europe/Moscow");

    [Test]
    public void Istanbul_ShouldNotBeNull() => TimeZones.Istanbul.ShouldNotBeNull();

    [Test]
    public void Istanbul_ShouldHaveCorrectId() => TimeZones.Istanbul.Id.ShouldBe("Europe/Istanbul");

    // ── Americas ─────────────────────────────────────────────────────────────

    [Test]
    public void EasternUs_ShouldNotBeNull() => TimeZones.EasternUs.ShouldNotBeNull();

    [Test]
    public void EasternUs_ShouldHaveCorrectId() => TimeZones.EasternUs.Id.ShouldBe("America/New_York");

    [Test]
    public void PacificUs_ShouldNotBeNull() => TimeZones.PacificUs.ShouldNotBeNull();

    [Test]
    public void PacificUs_ShouldHaveCorrectId() => TimeZones.PacificUs.Id.ShouldBe("America/Los_Angeles");

    // ── Asia ─────────────────────────────────────────────────────────────────

    [Test]
    public void Dubai_ShouldNotBeNull() => TimeZones.Dubai.ShouldNotBeNull();

    [Test]
    public void Dubai_ShouldHaveCorrectId() => TimeZones.Dubai.Id.ShouldBe("Asia/Dubai");

    [Test]
    public void India_ShouldNotBeNull() => TimeZones.India.ShouldNotBeNull();

    [Test]
    public void India_ShouldHaveCorrectId() => TimeZones.India.Id.ShouldBe("Asia/Kolkata");

    [Test]
    public void Japan_ShouldNotBeNull() => TimeZones.Japan.ShouldNotBeNull();

    [Test]
    public void Japan_ShouldHaveCorrectId() => TimeZones.Japan.Id.ShouldBe("Asia/Tokyo");

    // ── Pacific ──────────────────────────────────────────────────────────────

    [Test]
    public void AustraliaEastern_ShouldNotBeNull() => TimeZones.AustraliaEastern.ShouldNotBeNull();

    [Test]
    public void AustraliaEastern_ShouldHaveCorrectId() => TimeZones.AustraliaEastern.Id.ShouldBe("Australia/Sydney");
}
