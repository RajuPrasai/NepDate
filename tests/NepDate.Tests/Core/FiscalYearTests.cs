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
}