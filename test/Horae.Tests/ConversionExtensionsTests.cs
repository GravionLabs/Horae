namespace Horae.Tests;

[TestFixture(Category="UnitTest")]
public class ConversionExtensionsTests
{
    [Test]
    public void AsDateOnly_Should_Return_Correct_DateOnly()
    {
        var dto = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var dateOnly = dto.AsDateOnly();
        Assert.That(dateOnly, Is.EqualTo(new DateOnly(2026, 1, 1)));
    }
}
