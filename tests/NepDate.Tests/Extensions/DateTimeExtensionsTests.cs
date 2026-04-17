namespace NepDate.Tests.Extensions;

public class DateTimeExtensionsTests
{
    [Fact]
    public void ToNepaliDate_ValidDateTime_ReturnsCorrectNepaliDate()
    {
        var englishDate = new DateTime(2023, 8, 30);

        var nepaliDate = englishDate.ToNepaliDate();

        Assert.Equal(2080, nepaliDate.Year);
        Assert.Equal(5, nepaliDate.Month);
        Assert.Equal(13, nepaliDate.Day);
    }

    [Fact]
    public void ToNepaliDate_MinValue_ThrowsArgumentOutOfRangeException()
    {
        var minDate = DateTime.MinValue;

        Assert.Throws<ArgumentOutOfRangeException>(() => minDate.ToNepaliDate());
    }

    [Fact]
    public void ToNepaliDate_MaxValue_ThrowsArgumentOutOfRangeException()
    {
        var maxDate = DateTime.MaxValue;

        Assert.Throws<ArgumentOutOfRangeException>(() => maxDate.ToNepaliDate());
    }

    [Fact]
    public void ToNepaliDate_DateBeforeMinSupported_ThrowsArgumentOutOfRangeException()
    {
        var tooEarlyDate = new DateTime(1843, 1, 1); // Before NepaliDate.MinValue

        Assert.Throws<ArgumentOutOfRangeException>(() => tooEarlyDate.ToNepaliDate());
    }

    [Fact]
    public void ToNepaliDate_DateAfterMaxSupported_ThrowsArgumentOutOfRangeException()
    {
        var tooLateDate = new DateTime(2200, 1, 1); // After NepaliDate.MaxValue

        Assert.Throws<ArgumentOutOfRangeException>(() => tooLateDate.ToNepaliDate());
    }
}