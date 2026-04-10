using NepDate.Extensions;

namespace NepDate.Tests.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void ToNepaliDate_ValidDateString_ReturnsNepaliDate()
    {
        // Arrange
        string dateString = "2080/05/15";

        // Act
        var result = dateString.ToNepaliDate();

        // Assert
        Assert.Equal(new NepaliDate(2080, 5, 15), result);
    }

    [Fact]
    public void ToNepaliDate_NepaliDigits_ReturnsNepaliDate()
    {
        // Arrange
        string dateString = "२०८०/०५/१५";

        // Act
        var result = dateString.ToNepaliDate();

        // Assert
        Assert.Equal(new NepaliDate(2080, 5, 15), result);
    }

    [Fact]
    public void ToNepaliDate_InvalidDateString_ThrowsFormatException()
    {
        // Arrange
        string invalidDateString = "not a date";

        // Act & Assert
        Assert.Throws<FormatException>(() => invalidDateString.ToNepaliDate());
    }

    [Fact]
    public void TryToNepaliDate_ValidDateString_ReturnsTrue()
    {
        // Arrange
        string dateString = "2080/05/15";

        // Act
        bool success = dateString.TryToNepaliDate(out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(new NepaliDate(2080, 5, 15), result);
    }

    [Fact]
    public void TryToNepaliDate_InvalidDateString_ReturnsFalse()
    {
        // Arrange
        string invalidDateString = "not a date";

        // Act
        bool success = invalidDateString.TryToNepaliDate(out var result);

        // Assert
        Assert.False(success);
        Assert.Equal(default, result);
    }
}