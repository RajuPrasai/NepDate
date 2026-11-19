namespace NepDate.Tests.Core;

public class FiscalYearTests
{
    [Fact]
    public void GetFiscalYearStartAndEndDate_ValidYear_ReturnsCorrectDates()
    {
        int fiscalYear = 2080;

        var (startDate, endDate) = NepaliDate.GetFiscalYearStartAndEndDate(fiscalYear);

        Assert.Equal(new NepaliDate(2080, 4, 1), startDate);
        Assert.Equal(new NepaliDate(2081, 3, new NepaliDate(2081, 3, 1).MonthEndDay), endDate);
    }

    [Theory]
    [InlineData(1843)] // Below minimum
    [InlineData(2200)] // Above maximum
    public void GetFiscalYearStartAndEndDate_InvalidYear_ThrowsException(int year)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NepaliDate.GetFiscalYearStartAndEndDate(year));
    }

    [Fact]
    public void GetFiscalYearStartDate_ValidYear_ReturnsCorrectDate()
    {
        int fiscalYear = 2080;

        var startDate = NepaliDate.GetFiscalYearStartDate(fiscalYear);

        Assert.Equal(new NepaliDate(2080, 4, 1), startDate);
    }

    [Fact]
    public void GetFiscalYearEndDate_ValidYear_ReturnsCorrectDate()
    {
        int fiscalYear = 2080;

        var endDate = NepaliDate.GetFiscalYearEndDate(fiscalYear);

        Assert.Equal(new NepaliDate(2081, 3, new NepaliDate(2081, 3, 1).MonthEndDay), endDate);
    }

    [Fact]
    public void FiscalYearStartAndEndDate_OnNepaliDateInstance_ReturnsCorrectDates()
    {
        var date = new NepaliDate(2080, 6, 15);

        var (startDate, endDate) = date.FiscalYearStartAndEndDate();

        Assert.Equal(new NepaliDate(2080, 4, 1), startDate);
        Assert.Equal(new NepaliDate(2081, 3, new NepaliDate(2081, 3, 1).MonthEndDay), endDate);
    }

    [Fact]
    public void FiscalYearStartDate_OnNepaliDateInstance_ReturnsCorrectDate()
    {
        var date = new NepaliDate(2080, 6, 15);

        var startDate = date.FiscalYearStartDate();

        Assert.Equal(new NepaliDate(2080, 4, 1), startDate);
    }

    [Fact]
    public void FiscalYearEndDate_OnNepaliDateInstance_ReturnsCorrectDate()
    {
        var date = new NepaliDate(2080, 6, 15);

        var endDate = date.FiscalYearEndDate();

        Assert.Equal(new NepaliDate(2081, 3, new NepaliDate(2081, 3, 1).MonthEndDay), endDate);
    }

    [Fact]
    public void GetFiscalYearQuarterStartAndEndDate_ValidYearAndMonth_ReturnsCorrectDates()
    {
        int fiscalYear = 2080;
        int month = 5; // Bhadra, first quarter

        var (startDate, endDate) = NepaliDate.GetFiscalYearQuarterStartAndEndDate(fiscalYear, month);

        Assert.Equal(new NepaliDate(2080, 4, 1), startDate); // Quarter starts with Shrawan (month 4)
        Assert.Equal(new NepaliDate(2080, 6, new NepaliDate(2080, 6, 1).MonthEndDay), endDate); // Ends with Ashwin (month 6)
    }

    [Fact]
    public void GetFiscalYearQuarterStartDate_ValidYearAndMonth_ReturnsCorrectDate()
    {
        int fiscalYear = 2080;
        int month = 8; // Mangsir, second quarter

        var startDate = NepaliDate.GetFiscalYearQuarterStartDate(fiscalYear, month);

        Assert.Equal(new NepaliDate(2080, 7, 1), startDate); // Quarter starts with Kartik (month 7)
    }

    [Fact]
    public void GetFiscalYearQuarterEndDate_ValidYearAndMonth_ReturnsCorrectDate()
    {
        int fiscalYear = 2080;
        int month = 11; // Falgun, third quarter

        var endDate = NepaliDate.GetFiscalYearQuarterEndDate(fiscalYear, month);

        Assert.Equal(new NepaliDate(2080, 12, new NepaliDate(2080, 12, 1).MonthEndDay), endDate); // Ends with Chaitra (month 12)
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void GetFiscalYearQuarter_FourthQuarterMonths_FallInTheNextBsYear(int month)
    {
        // Baisakh to Ashad close fiscal year 2080, so its fourth quarter is 2081/01/01 to the end of Ashad 2081.
        var expectedStart = new NepaliDate(2081, 1, 1);
        var expectedEnd = new NepaliDate(2081, 3, 1).MonthEndDate();

        Assert.Equal(expectedStart, NepaliDate.GetFiscalYearQuarterStartDate(2080, month));
        Assert.Equal(expectedEnd, NepaliDate.GetFiscalYearQuarterEndDate(2080, month));
        Assert.Equal((expectedStart, expectedEnd), NepaliDate.GetFiscalYearQuarterStartAndEndDate(2080, month));
        Assert.Equal(NepaliDate.GetFiscalYearEndDate(2080), NepaliDate.GetFiscalYearQuarterEndDate(2080, month));
    }

    [Fact]
    public void GetFiscalYearQuarter_EveryMonth_MatchesTheInstanceMethodsOfThatMonth()
    {
        for (int fiscalYear = 1901; fiscalYear <= 2198; fiscalYear++)
        {
            for (int month = 1; month <= 12; month++)
            {
                var date = new NepaliDate(month <= 3 ? fiscalYear + 1 : fiscalYear, month, 1);

                Assert.Equal(date.FiscalYearQuarterStartAndEndDate(), NepaliDate.GetFiscalYearQuarterStartAndEndDate(fiscalYear, month));
                Assert.Equal(NepaliDate.GetFiscalYearStartDate(fiscalYear), date.FiscalYearStartDate());
            }
        }
    }
}