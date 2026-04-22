namespace NepDate.Tests.Core;

public class NepaliDatePropertiesTests
{
    [Fact]
    public void EnglishDate_Conversion_ReturnsCorrectDate()
    {
        var nepaliDate = new NepaliDate(2080, 5, 15);

        var englishDate = nepaliDate.EnglishDate;

        Assert.Equal(2023, englishDate.Year);
        Assert.Equal(9, englishDate.Month);
        Assert.Equal(1, englishDate.Day);
    }

    [Fact]
    public void DayOfWeek_ReturnsCorrectValue()
    {
        var nepaliDate = new NepaliDate(2080, 5, 15);

        var dayOfWeek = nepaliDate.DayOfWeek;

        Assert.Equal(DayOfWeek.Friday, dayOfWeek);
    }

    [Fact]
    public void MonthEndDay_ReturnsCorrectValue()
    {
        var nepaliDate = new NepaliDate(2080, 5, 15);

        var monthEndDay = nepaliDate.MonthEndDay;

        Assert.Equal(31, monthEndDay);
    }

    [Fact]
    public void MonthName_ReturnsCorrectEnum()
    {
        var nepaliDate = new NepaliDate(2080, 5, 15);

        var monthName = nepaliDate.MonthName;

        Assert.Equal(NepaliMonths.Bhadra, monthName);
    }

    [Fact]
    public void Today_ReturnsCurrentNepaliDate()
    {
        var today = DateTime.Today;
        var expectedNepaliDate = new NepaliDate(today);

        var todayNepaliDate = NepaliDate.Today;

        Assert.Equal(expectedNepaliDate, todayNepaliDate);
    }

    [Fact]
    public void DayOfYear_FirstDayOfYear_Returns1()
    {
        // Baisakh 1 is the first day of the Nepali calendar year.
        var date = new NepaliDate(2080, 1, 1);
        Assert.Equal(1, date.DayOfYear);
    }

    [Fact]
    public void DayOfYear_FirstDayOfMonth4_EqualsSum_Of_Months1To3_Plus1()
    {
        // DayOfYear for the first day of month 4 should equal (m1 + m2 + m3) + 1.
        var m1 = new NepaliDate(2080, 1, 1).MonthEndDay;
        var m2 = new NepaliDate(2080, 2, 1).MonthEndDay;
        var m3 = new NepaliDate(2080, 3, 1).MonthEndDay;
        int expected = m1 + m2 + m3 + 1;
        Assert.Equal(expected, new NepaliDate(2080, 4, 1).DayOfYear);
    }

    [Fact]
    public void Equals_NullObject_ReturnsFalse()
    {
        var date = new NepaliDate(2080, 5, 15);
        Assert.False(date.Equals(null));
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        var date = new NepaliDate(2080, 5, 15);
        Assert.False(date.Equals("2080/05/15"));
        Assert.False(date.Equals(42));
    }
}