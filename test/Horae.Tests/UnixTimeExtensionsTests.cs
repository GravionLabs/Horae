using Shouldly;

namespace Horae.Tests;

[TestFixture(Category = "UnitTest")]
public class UnixTimeExtensionsTests
{
    // FromUnixSeconds

    [Test]
    public void FromUnixSeconds_ShouldReturnEpoch_WhenZero()
    {
        var result = UnixTimeExtensions.FromUnixSeconds(0);
        result.ShouldBe(new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void FromUnixSeconds_ShouldReturnCorrectInstant()
    {
        var result = UnixTimeExtensions.FromUnixSeconds(1_700_000_000L);
        result.UtcDateTime.ShouldBe(new DateTime(2023, 11, 14, 22, 13, 20, DateTimeKind.Utc));
    }

    [Test]
    public void FromUnixSeconds_ShouldHandleNegativeValues()
    {
        var result = UnixTimeExtensions.FromUnixSeconds(-1L);
        result.ShouldBe(new DateTimeOffset(1969, 12, 31, 23, 59, 59, TimeSpan.Zero));
    }

    // FromUnixMilliseconds

    [Test]
    public void FromUnixMilliseconds_ShouldReturnEpoch_WhenZero()
    {
        var result = UnixTimeExtensions.FromUnixMilliseconds(0);
        result.ShouldBe(new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void FromUnixMilliseconds_ShouldReturnCorrectInstant()
    {
        var result = UnixTimeExtensions.FromUnixMilliseconds(1_700_000_000_000L);
        result.UtcDateTime.ShouldBe(new DateTime(2023, 11, 14, 22, 13, 20, DateTimeKind.Utc));
    }

    [Test]
    public void FromUnixMilliseconds_ShouldPreserveSubSecondPrecision()
    {
        var result = UnixTimeExtensions.FromUnixMilliseconds(1_700_000_000_500L);
        result.UtcDateTime.Millisecond.ShouldBe(500);
    }

    // ToUnixSeconds

    [Test]
    public void ToUnixSeconds_ShouldReturnZero_ForEpoch()
    {
        var epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        epoch.ToUnixSeconds().ShouldBe(0);
    }

    [Test]
    public void ToUnixSeconds_ShouldTruncateSubSecondPrecision()
    {
        var dto = new DateTimeOffset(2023, 11, 14, 22, 13, 20, 999, TimeSpan.Zero);
        dto.ToUnixSeconds().ShouldBe(1_700_000_000L);
    }

    [Test]
    public void ToUnixSeconds_RoundTrip_FromUnixSeconds()
    {
        const long original = 1_700_000_000L;
        UnixTimeExtensions.FromUnixSeconds(original).ToUnixSeconds().ShouldBe(original);
    }

    // ToUnixMilliseconds

    [Test]
    public void ToUnixMilliseconds_ShouldReturnZero_ForEpoch()
    {
        var epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        epoch.ToUnixMilliseconds().ShouldBe(0);
    }

    [Test]
    public void ToUnixMilliseconds_ShouldIncludeSubSecondPrecision()
    {
        var dto = new DateTimeOffset(2023, 11, 14, 22, 13, 20, 500, TimeSpan.Zero);
        dto.ToUnixMilliseconds().ShouldBe(1_700_000_000_500L);
    }

    [Test]
    public void ToUnixMilliseconds_RoundTrip_FromUnixMilliseconds()
    {
        const long original = 1_700_000_000_500L;
        UnixTimeExtensions.FromUnixMilliseconds(original).ToUnixMilliseconds().ShouldBe(original);
    }
}
