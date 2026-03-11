namespace Horae.Tests;

[TestFixture(Category="UnitTest")]
public class DateExtensionsTests
{
    [Test]
    public void January_Should_Create_Correct_DateTimeOffset()
    {
        var dto = 1.January(2026);
        Assert.That(dto, Is.EqualTo(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)));
    }
}
