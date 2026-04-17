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
    public void Now_ReturnsCurrentNepaliDate()
    {
        var today = DateTime.Today;
        var expectedNepaliDate = new NepaliDate(today);

        var nowNepaliDate = NepaliDate.Now;

        Assert.Equal(expectedNepaliDate, nowNepaliDate);
    }

    [Fact]
    public void DayOfYear_MatchesEnglishDateDayOfYear()
    {
        var date = new NepaliDate(2080, 1, 1);
        Assert.Equal(date.EnglishDate.DayOfYear, date.DayOfYear);
    }

    [Fact]
    public void DayOfYear_KnownDate_ReturnsCorrectValue()
    {
        // 2080/1/1 = April 14, 2023 = day 104 of 2023
        Assert.Equal(104, new NepaliDate(2080, 1, 1).DayOfYear);
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