using Shouldly;

namespace Horae.Tests;

[TestFixture(Category = "UnitTest")]
public class DstQueryExtensionsTests
{
    // IsInDst

    [Test]
    public void IsInDst_ShouldReturnTrue_DuringSummerTime()
    {
        // July 1, 2026 12:00 UTC — CET is in CEST (UTC+2) → DST active
        var summerUtc = new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero);
        summerUtc.IsInDst(TimeZones.CentralEuropean).ShouldBeTrue();
    }

    [Test]
    public void IsInDst_ShouldReturnFalse_DuringWinterTime()
    {
        // January 1, 2026 12:00 UTC — CET is standard time (UTC+1)
        var winterUtc = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        winterUtc.IsInDst(TimeZones.CentralEuropean).ShouldBeFalse();
    }

    [Test]
    public void IsInDst_ShouldReturnFalse_ForNoDstZone()
    {
        // Japan has no DST — always returns false
        var anyInstant = new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero);
        anyInstant.IsInDst(TimeZones.Japan).ShouldBeFalse();
    }

    // IsDstAmbiguous

    [Test]
    public void IsDstAmbiguous_ShouldReturnTrue_DuringFallBackHour()
    {
        // Europe/Berlin falls back on Oct 25, 2026 at 03:00 CEST → 02:00 CET (01:00 UTC).
        // UTC 00:30 Oct 25 = 02:30 CEST — falls in the repeated 02:00-03:00 window → ambiguous.
        var ambiguous = new DateTimeOffset(2026, 10, 25, 0, 30, 0, TimeSpan.Zero);
        ambiguous.IsDstAmbiguous(TimeZones.CentralEuropean).ShouldBeTrue();
    }

    [Test]
    public void IsDstAmbiguous_ShouldReturnFalse_OutsideFallBackHour()
    {
        // July — well outside any DST transition
        var clear = new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero);
        clear.IsDstAmbiguous(TimeZones.CentralEuropean).ShouldBeFalse();
    }

    [Test]
    public void IsDstAmbiguous_ShouldReturnFalse_ForNoDstZone()
    {
        // Japan has no DST — never ambiguous
        var anyInstant = new DateTimeOffset(2026, 10, 25, 0, 30, 0, TimeSpan.Zero);
        anyInstant.IsDstAmbiguous(TimeZones.Japan).ShouldBeFalse();
    }
}
