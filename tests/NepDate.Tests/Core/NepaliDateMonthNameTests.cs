namespace NepDate.Tests.Core;

/// <summary>
/// Verifies that MonthName returns the correct NepaliMonths enum for every month,
/// and that the value is purely a function of the month number, independent of year and day.
///
/// Expected mapping (from the Nepali calendar):
///   1=Baishakh  2=Jestha   3=Ashad    4=Shrawan
///   5=Bhadra    6=Ashoj    7=Kartik   8=Mangsir
///   9=Poush    10=Magh    11=Falgun  12=Chaitra
/// </summary>
public class NepaliDateMonthNameTests
{
    // ---- Each month returns the right enum value ----

    [Theory]
    [InlineData(1, NepaliMonths.Baishakh)]
    [InlineData(2, NepaliMonths.Jestha)]
    [InlineData(3, NepaliMonths.Ashad)]
    [InlineData(4, NepaliMonths.Shrawan)]
    [InlineData(5, NepaliMonths.Bhadra)]
    [InlineData(6, NepaliMonths.Ashoj)]
    [InlineData(7, NepaliMonths.Kartik)]
    [InlineData(8, NepaliMonths.Mangsir)]
    [InlineData(9, NepaliMonths.Poush)]
    [InlineData(10, NepaliMonths.Magh)]
    [InlineData(11, NepaliMonths.Falgun)]
    [InlineData(12, NepaliMonths.Chaitra)]
    public void MonthName_AllMonths_ReturnCorrectEnum(int month, NepaliMonths expected)
    {
        var date = new NepaliDate(2080, month, 1);
        Assert.Equal(expected, date.MonthName);
    }

    // ---- Month name is not affected by the day within the month ----

    [Fact]
    public void MonthName_SameMonthDifferentDays_AllReturnSameEnum()
    {
        // Shrawan 2080 (month 4); all three days must report the same month name.
        var day1 = new NepaliDate(2080, 4, 1);
        var day15 = new NepaliDate(2080, 4, 15);
        var dayEnd = new NepaliDate(2080, 4, day1.MonthEndDay);

        Assert.Equal(NepaliMonths.Shrawan, day1.MonthName);
        Assert.Equal(NepaliMonths.Shrawan, day15.MonthName);
        Assert.Equal(NepaliMonths.Shrawan, dayEnd.MonthName);
    }

    // ---- Month name is not affected by the year ----

    [Theory]
    [InlineData(2070)]
    [InlineData(2080)]
    [InlineData(2090)]
    [InlineData(2100)]
    public void MonthName_SameMonthDifferentYears_AllReturnSameEnum(int year)
    {
        // Month 9 must always be Poush regardless of which year.
        var date = new NepaliDate(year, 9, 1);
        Assert.Equal(NepaliMonths.Poush, date.MonthName);
    }

    // ---- Sequential months cycle correctly (spot-check) ----

    [Fact]
    public void MonthName_AddingOneMonth_AdvancesEnum()
    {
        // Adding exactly one month to 2080/11/01 (Falgun) → 2080/12/01 (Chaitra)
        var falgun = new NepaliDate(2080, 11, 1);
        var chaitra = falgun.AddMonths(1);

        Assert.Equal(NepaliMonths.Falgun, falgun.MonthName);
        Assert.Equal(NepaliMonths.Chaitra, chaitra.MonthName);
    }

    [Fact]
    public void MonthName_YearBoundary_WrapsFromChaitraToBasikhakh()
    {
        // Last month of 2080 then first month of 2081
        var chaitra2080 = new NepaliDate(2080, 12, 1);
        var baishakh2081 = new NepaliDate(2081, 1, 1);

        Assert.Equal(NepaliMonths.Chaitra, chaitra2080.MonthName);
        Assert.Equal(NepaliMonths.Baishakh, baishakh2081.MonthName);
    }

    // ---- Enum integer values match month numbers (sanity check independent of library) ----

    [Theory]
    [InlineData(NepaliMonths.Baishakh, 1)]
    [InlineData(NepaliMonths.Jestha, 2)]
    [InlineData(NepaliMonths.Ashad, 3)]
    [InlineData(NepaliMonths.Shrawan, 4)]
    [InlineData(NepaliMonths.Bhadra, 5)]
    [InlineData(NepaliMonths.Ashoj, 6)]
    [InlineData(NepaliMonths.Kartik, 7)]
    [InlineData(NepaliMonths.Mangsir, 8)]
    [InlineData(NepaliMonths.Poush, 9)]
    [InlineData(NepaliMonths.Magh, 10)]
    [InlineData(NepaliMonths.Falgun, 11)]
    [InlineData(NepaliMonths.Chaitra, 12)]
    public void NepaliMonthsEnum_IntegerValueMatchesMonthNumber(NepaliMonths month, int expectedNumber)
    {
        // The enum is cast from the Month integer, so they must be equal.
        Assert.Equal(expectedNumber, (int)month);
    }
}
