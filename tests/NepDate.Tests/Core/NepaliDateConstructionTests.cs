using NepDate.Exceptions;

namespace NepDate.Tests.Core;

public class NepaliDateConstructionTests
{
    [Fact]
    public void Constructor_ValidNepaliDate_CreatesInstance()
    {
        // Arrange & Act
        var nepaliDate = new NepaliDate(2080, 5, 15);

        // Assert
        Assert.Equal(2080, nepaliDate.Year);
        Assert.Equal(5, nepaliDate.Month);
        Assert.Equal(15, nepaliDate.Day);
    }

    [Fact]
    public void Constructor_MinValue_CreatesInstance()
    {
        // Arrange & Act
        var nepaliDate = NepaliDate.MinValue;

        // Assert
        Assert.Equal(1901, nepaliDate.Year);
        Assert.Equal(1, nepaliDate.Month);
        Assert.Equal(1, nepaliDate.Day);
    }

    [Fact]
    public void Constructor_MaxValue_CreatesInstance()
    {
        // Arrange & Act
        var nepaliDate = NepaliDate.MaxValue;

        // Assert
        Assert.Equal(2199, nepaliDate.Year);
        Assert.Equal(12, nepaliDate.Month);
        Assert.True(nepaliDate.Day > 0); // Last day of Chaitra
    }

    [Theory]
    [InlineData(2080, 0, 1)] // Month too low
    [InlineData(2080, 13, 1)] // Month too high
    [InlineData(2080, 1, 0)] // Day too low
    [InlineData(2080, 1, 33)] // Day too high
    public void Constructor_InvalidNepaliDate_ThrowsException(int year, int month, int day)
    {
        // Act & Assert
        Assert.Throws<NepDateException.InvalidNepaliDateFormatException>(() => new NepaliDate(year, month, day));
    }

    [Theory]
    [InlineData("2080/05/15")]
    [InlineData("2080-05-15")]
    [InlineData("2080.05.15")]
    [InlineData("2080_05_15")]
    [InlineData("2080\\05\\15")]
    [InlineData("2080 05 15")]
    public void Constructor_ValidStringFormats_CreatesInstance(string dateString)
    {
        // Arrange & Act
        var nepaliDate = new NepaliDate(dateString);

        // Assert
        Assert.Equal(2080, nepaliDate.Year);
        Assert.Equal(5, nepaliDate.Month);
        Assert.Equal(15, nepaliDate.Day);
    }

    [Theory]
    [InlineData("15/05/2080", true, false)] // Day, Month, Year
    [InlineData("05/15/2080", true, true)] // Month, Day, Year
    [InlineData("2080/05/15", false, false)] // Day, Year, Month
    [InlineData("80/05/15", true, true)] // 2-digit year auto-adjusted
    public void Constructor_AutoAdjustedFormats_CreatesInstance(string dateString, bool autoAdjust, bool monthInMiddle)
    {
        // Arrange & Act
        var nepaliDate = new NepaliDate(dateString, autoAdjust, monthInMiddle);

        // Assert
        Assert.Equal(2080, nepaliDate.Year);
        Assert.Equal(5, nepaliDate.Month);
        Assert.Equal(15, nepaliDate.Day);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("2080/5")]
    [InlineData("2080/5/40")]
    public void Constructor_InvalidStringFormats_ThrowsException(string dateString)
    {
        // Act & Assert
        _ = Assert.Throws<NepDateException.InvalidNepaliDateFormatException>(() => new NepaliDate(dateString));
    }

    [Fact]
    public void Constructor_EnglishDate_ConvertsCorrectly()
    {
        // Arrange
        var englishDate = new DateTime(2023, 8, 30);

        // Act
        var nepaliDate = new NepaliDate(englishDate);

        // Assert
        Assert.Equal(2080, nepaliDate.Year);
        Assert.Equal(5, nepaliDate.Month);
        Assert.Equal(13, nepaliDate.Day);
    }

    [Fact]
    public void TryParse_ValidString_ReturnsTrueWithCorrectDate()
    {
        bool success = NepaliDate.TryParse("2080/05/15", out var date);
        Assert.True(success);
        Assert.Equal(2080, date.Year);
        Assert.Equal(5, date.Month);
        Assert.Equal(15, date.Day);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("2080/5")]
    public void TryParse_InvalidString_ReturnsFalseWithDefault(string input)
    {
        bool success = NepaliDate.TryParse(input, out var date);
        Assert.False(success);
        Assert.Equal(default, date);
    }
}