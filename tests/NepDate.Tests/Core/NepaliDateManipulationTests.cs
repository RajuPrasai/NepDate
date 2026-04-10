namespace NepDate.Tests.Core;

public class NepaliDateManipulationTests
{
    [Fact]
    public void MonthEndDate_ReturnsCorrectDate()
    {
        // Arrange
        var nepaliDate = new NepaliDate(2080, 5, 15);

        // Act
        var monthEndDate = nepaliDate.MonthEndDate();

        // Assert
        Assert.Equal(2080, monthEndDate.Year);
        Assert.Equal(5, monthEndDate.Month);
        Assert.Equal(31, monthEndDate.Day);
    }

    [Theory]
    [InlineData(2080, 5, 15, 1, 2080, 6, 15)]   // Add 1 month
    [InlineData(2080, 5, 15, -1, 2080, 4, 15)]  // Subtract 1 month
    [InlineData(2080, 5, 15, 12, 2081, 5, 15)]  // Add 12 months (1 year)
    [InlineData(2081, 4, 32, 5, 2081, 9, 29)]  // Month end day adjustment
    [InlineData(2081, 4, 32, 5, 2081, 10, 3, true)] // Away from month end
    public void AddMonths_ReturnsCorrectDate(int year, int month, int day, int monthsToAdd, int expectedYear, int expectedMonth, int expectedDay, bool awayFromMonthEnd = false)
    {
        // Arrange
        var nepaliDate = new NepaliDate(year, month, day);

        // Act
        var result = nepaliDate.AddMonths(monthsToAdd, awayFromMonthEnd);

        // Assert
        Assert.Equal(expectedYear, result.Year);
        Assert.Equal(expectedMonth, result.Month);
        Assert.Equal(expectedDay, result.Day);
    }

    [Theory]
    [InlineData(2080, 5, 15, 5, 2080, 5, 20)]  // Add 5 days
    [InlineData(2080, 5, 15, -5, 2080, 5, 10)] // Subtract 5 days
    [InlineData(2080, 5, 31, 1, 2080, 6, 1)]   // Cross month boundary
    [InlineData(2080, 12, 30, 5, 2081, 1, 5)]  // Cross year boundary
    public void AddDays_ReturnsCorrectDate(int year, int month, int day, int daysToAdd, int expectedYear, int expectedMonth, int expectedDay)
    {
        // Arrange
        var nepaliDate = new NepaliDate(year, month, day);

        // Act
        var result = nepaliDate.AddDays(daysToAdd);

        // Assert
        Assert.Equal(expectedYear, result.Year);
        Assert.Equal(expectedMonth, result.Month);
        Assert.Equal(expectedDay, result.Day);
    }

    [Fact]
    public void Subtract_TwoDates_ReturnsCorrectTimeSpan()
    {
        var earlier = new NepaliDate(2080, 5, 1);
        var later = new NepaliDate(2080, 5, 16);
        Assert.Equal(TimeSpan.FromDays(15), later.Subtract(earlier));
        Assert.Equal(TimeSpan.FromDays(-15), earlier.Subtract(later));
    }

    [Fact]
    public void Subtract_SameDate_ReturnsZero()
    {
        var date = new NepaliDate(2080, 5, 15);
        Assert.Equal(TimeSpan.Zero, date.Subtract(date));
    }
}