using Shouldly;

namespace Horae.Tests;

[TestFixture] [Category("UnitTest")]
public class RegionExtensionsTests
{
    // A fixed UTC reference instant — mid-summer so CET is CEST (UTC+2).
    private static readonly DateTimeOffset _reference = new(2026, 6, 15, 12, 0, 0, TimeSpan.Zero);

    [Test]
    public void ToUtc_ShouldReturnUtcOffset()
    {
        // Japan is UTC+9 with no DST. 12:00 JST = 03:00 UTC.
        var jst = new DateTimeOffset(2026, 1, 15, 12, 0, 0, TimeSpan.FromHours(9));

        var utc = jst.ToUtc();

        utc.Offset.ShouldBe(TimeSpan.Zero);
        utc.Hour.ShouldBe(3);
    }

    [Test]
    public void ToCentralEuropeanTime_ShouldExpressInCet()
    {
        // June is CEST (UTC+2). 12:00 UTC → 14:00 CEST.
        var result = _reference.ToCentralEuropeanTime();

        result.Offset.ShouldBe(TimeSpan.FromHours(2));
        result.Hour.ShouldBe(14);
    }

    [Test]
    public void ToLondonTime_ShouldExpressInBst()
    {
        // June: London is BST (UTC+1). 12:00 UTC → 13:00 BST.
        var result = _reference.ToLondonTime();

        result.Offset.ShouldBe(TimeSpan.FromHours(1));
        result.Hour.ShouldBe(13);
    }

    [Test]
    public void ToLisbonTime_ShouldExpressInWest()
    {
        // June: Lisbon is WEST (UTC+1). 12:00 UTC → 13:00 WEST.
        var result = _reference.ToLisbonTime();

        result.Offset.ShouldBe(TimeSpan.FromHours(1));
        result.Hour.ShouldBe(13);
    }

    [Test]
    public void ToEasternEuropeTime_ShouldExpressInEest()
    {
        // June: EET is EEST (UTC+3). 12:00 UTC → 15:00 EEST.
        var result = _reference.ToEasternEuropeTime();

        result.Offset.ShouldBe(TimeSpan.FromHours(3));
        result.Hour.ShouldBe(15);
    }

    [Test]
    public void ToMoscowTime_ShouldExpressInMsk()
    {
        // Moscow is always UTC+3, no DST. 12:00 UTC → 15:00 MSK.
        var result = _reference.ToMoscowTime();

        result.Offset.ShouldBe(TimeSpan.FromHours(3));
        result.Hour.ShouldBe(15);
    }

    [Test]
    public void ToIstanbulTime_ShouldExpressInTrt()
    {
        // Istanbul is always UTC+3, no DST. 12:00 UTC → 15:00 TRT.
        var result = _reference.ToIstanbulTime();

        result.Offset.ShouldBe(TimeSpan.FromHours(3));
        result.Hour.ShouldBe(15);
    }

    [Test]
    public void ToEasternUsTime_ShouldExpressInEdt()
    {
        // June: US Eastern is EDT (UTC-4). 12:00 UTC → 08:00 EDT.
        var result = _reference.ToEasternUsTime();

        result.Offset.ShouldBe(TimeSpan.FromHours(-4));
        result.Hour.ShouldBe(8);
    }

    [Test]
    public void ToPacificUsTime_ShouldExpressInPdt()
    {
        // June: US Pacific is PDT (UTC-7). 12:00 UTC → 05:00 PDT.
        var result = _reference.ToPacificUsTime();

        result.Offset.ShouldBe(TimeSpan.FromHours(-7));
        result.Hour.ShouldBe(5);
    }

    [Test]
    public void ToDubaiTime_ShouldExpressInGst()
    {
        // Dubai is always UTC+4, no DST. 12:00 UTC → 16:00 GST.
        var result = _reference.ToDubaiTime();

        result.Offset.ShouldBe(TimeSpan.FromHours(4));
        result.Hour.ShouldBe(16);
    }

    [Test]
    public void ToIndiaTime_ShouldExpressInIst()
    {
        // India is always UTC+5:30, no DST. 12:00 UTC → 17:30 IST.
        var result = _reference.ToIndiaTime();

        result.Offset.ShouldBe(TimeSpan.FromHours(5.5));
        result.Hour.ShouldBe(17);
        result.Minute.ShouldBe(30);
    }

    [Test]
    public void ToJapanTime_ShouldExpressInJst()
    {
        // Japan is always UTC+9, no DST. 12:00 UTC → 21:00 JST.
        var result = _reference.ToJapanTime();

        result.Offset.ShouldBe(TimeSpan.FromHours(9));
        result.Hour.ShouldBe(21);
    }

    [Test]
    public void ToAustraliaEasternTime_ShouldExpressInAest()
    {
        // June: Sydney is AEST (UTC+10, no DST in winter). 12:00 UTC → 22:00 AEST.
        var result = _reference.ToAustraliaEasternTime();

        result.Offset.ShouldBe(TimeSpan.FromHours(10));
        result.Hour.ShouldBe(22);
    }

    [Test]
    public void ToUtc_ShouldPreserveInstant()
    {
        var cest = new DateTimeOffset(2026, 6, 15, 14, 0, 0, TimeSpan.FromHours(2));

        var utc = cest.ToUtc();

        utc.UtcDateTime.ShouldBe(cest.UtcDateTime);
        utc.Offset.ShouldBe(TimeSpan.Zero);
    }
}
