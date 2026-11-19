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

    [Theory]
    [InlineData(1901, 1, 1902, 32)]
    [InlineData(1901, 2, 1902, 32)]
    [InlineData(1901, 3, 1902, 32)]
    [InlineData(2079, 1, 2080, 31)]
    [InlineData(2079, 2, 2080, 31)]
    [InlineData(2079, 3, 2080, 31)]
    [InlineData(2081, 1, 2082, 32)]
    [InlineData(2081, 2, 2082, 32)]
    [InlineData(2081, 3, 2082, 32)]
    [InlineData(2198, 1, 2199, 32)]
    [InlineData(2198, 2, 2199, 32)]
    [InlineData(2198, 3, 2199, 32)]
    public void GetFiscalYearQuarter_FourthQuarter_UsesFollowingYearsAshadhLength(
        int fiscalYear, int month, int calendarYear, int lastDay)
    {
        // Ashadh has 31 or 32 days depending on the BS year, not the fiscal year label.
        var expectedStart = new NepaliDate(calendarYear, 1, 1);
        var expectedEnd = new NepaliDate(calendarYear, 3, lastDay);

        Assert.Equal(expectedStart, NepaliDate.GetFiscalYearQuarterStartDate(fiscalYear, month));
        Assert.Equal(expectedEnd, NepaliDate.GetFiscalYearQuarterEndDate(fiscalYear, month));
        Assert.Equal((expectedStart, expectedEnd), NepaliDate.GetFiscalYearQuarterStartAndEndDate(fiscalYear, month));
    }

    [Theory]
    [InlineData(4, 4, 6, 30)]
    [InlineData(5, 4, 6, 30)]
    [InlineData(6, 4, 6, 30)]
    [InlineData(7, 7, 9, 30)]
    [InlineData(8, 7, 9, 30)]
    [InlineData(9, 7, 9, 30)]
    [InlineData(10, 10, 12, 30)]
    [InlineData(11, 10, 12, 30)]
    [InlineData(12, 10, 12, 30)]
    public void GetFiscalYearQuarter_LastSupportedYear_FirstThreeQuartersRemainAvailable(
        int month, int startMonth, int endMonth, int lastDay)
    {
        // These quarters do not need a date in the unsupported year 2200.
        var expectedStart = new NepaliDate(2199, startMonth, 1);
        var expectedEnd = new NepaliDate(2199, endMonth, lastDay);

        Assert.Equal(expectedStart, NepaliDate.GetFiscalYearQuarterStartDate(2199, month));
        Assert.Equal(expectedEnd, NepaliDate.GetFiscalYearQuarterEndDate(2199, month));
        Assert.Equal((expectedStart, expectedEnd), NepaliDate.GetFiscalYearQuarterStartAndEndDate(2199, month));
    }

    [Theory]
    [InlineData(2199, 1)]
    [InlineData(2199, 2)]
    [InlineData(2199, 3)]
    [InlineData(1899, 1)]
    [InlineData(1899, 4)]
    [InlineData(2200, 1)]
    [InlineData(2200, 4)]
    [InlineData(int.MinValue, 1)]
    [InlineData(int.MaxValue, 1)]
    public void GetFiscalYearQuarter_ResolvedYearOutsideSupportedRange_Throws(int fiscalYear, int month)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NepaliDate.GetFiscalYearQuarterStartDate(fiscalYear, month));
        Assert.Throws<ArgumentOutOfRangeException>(() => NepaliDate.GetFiscalYearQuarterEndDate(fiscalYear, month));
        Assert.Throws<ArgumentOutOfRangeException>(() => NepaliDate.GetFiscalYearQuarterStartAndEndDate(fiscalYear, month));
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(13)]
    [InlineData(int.MaxValue)]
    public void GetFiscalYearQuarter_InvalidMonth_Throws(int month)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NepaliDate.GetFiscalYearQuarterStartDate(2080, month));
        Assert.Throws<ArgumentOutOfRangeException>(() => NepaliDate.GetFiscalYearQuarterEndDate(2080, month));
        Assert.Throws<ArgumentOutOfRangeException>(() => NepaliDate.GetFiscalYearQuarterStartAndEndDate(2080, month));
    }
}
