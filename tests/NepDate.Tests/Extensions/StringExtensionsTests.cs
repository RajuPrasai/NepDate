namespace NepDate.Tests.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void ToNepaliDate_ValidDateString_ReturnsNepaliDate()
    {
        string dateString = "2080/05/15";

        var result = dateString.ToNepaliDate();

        Assert.Equal(new NepaliDate(2080, 5, 15), result);
    }

    [Fact]
    public void ToNepaliDate_NepaliDigits_ReturnsNepaliDate()
    {
        string dateString = "२०८०/०५/१५";

        var result = dateString.ToNepaliDate();

        Assert.Equal(new NepaliDate(2080, 5, 15), result);
    }

    [Fact]
    public void ToNepaliDate_InvalidDateString_ThrowsFormatException()
    {
        string invalidDateString = "not a date";

        Assert.Throws<FormatException>(() => invalidDateString.ToNepaliDate());
    }

    [Fact]
    public void TryToNepaliDate_ValidDateString_ReturnsTrue()
    {
        string dateString = "2080/05/15";

        bool success = dateString.TryToNepaliDate(out var result);

        Assert.True(success);
        Assert.Equal(new NepaliDate(2080, 5, 15), result);
    }

    [Fact]
    public void TryToNepaliDate_InvalidDateString_ReturnsFalse()
    {
        string invalidDateString = "not a date";

        bool success = invalidDateString.TryToNepaliDate(out var result);

        Assert.False(success);
        Assert.Equal(default, result);
    }
}