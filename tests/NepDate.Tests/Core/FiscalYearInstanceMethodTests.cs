namespace NepDate.Tests.Core;

/// <summary>
/// Tests for the instance-level fiscal year methods on NepaliDate that are not covered
/// by the existing FiscalYearTests.cs (which only tests static helpers and a single Q1 scenario).
///
/// Coverage:
///   - FiscalYearStartDate / FiscalYearEndDate for dates in every region (months 1-3 and 4-12)
///   - FiscalYearStartDate / FiscalYearEndDate with positive and negative yearOffset
///   - FiscalYearQuarterStartDate / FiscalYearQuarterEndDate with every explicit quarter enum
///   - FiscalYearQuarterStartAndEndDate combined tuple methods
///   - FiscalYearQuarterStartDate / FiscalYearQuarterEndDate with Current for Q1, Q2, Q3 dates
///
/// All expected values derived from Nepal's fiscal year rules:
///   Fiscal year Y runs from 1 Shrawan (month 4) of year Y
///                          to last day of Ashadh (month 3) of year Y+1.
///   Q1 = months 4, 5, 6   Q2 = months 7, 8, 9
///   Q3 = months 10, 11, 12 Q4 = months 1, 2, 3 (of the next calendar year)
/// </summary>
public class FiscalYearInstanceMethodTests
{
    // ---- FiscalYearStartDate ----

    [Fact]
    public void FiscalYearStartDate_DateInShrawan_ReturnsSameyearShrawan1()
    {
        // Shrawan 2080 (month 4): fiscal year 2080 starts at 2080/04/01.
        var date = new NepaliDate(2080, 4, 15);
        var start = date.FiscalYearStartDate();
        Assert.Equal(new NepaliDate(2080, 4, 1), start);
    }

    [Fact]
    public void FiscalYearStartDate_DateInChaitra_ReturnsSameYearShrawan1()
    {
        // Chaitra 2080 (month 12): still in fiscal year 2080 → starts 2080/04/01.
        var date = new NepaliDate(2080, 12, 15);
        var start = date.FiscalYearStartDate();
        Assert.Equal(new NepaliDate(2080, 4, 1), start);
    }

    [Fact]
    public void FiscalYearStartDate_DateInBasikhakh_ReturnsPreviousYearShrawan1()
    {
        // Baishakh 2080 (month 1): this is in fiscal year 2079 → starts 2079/04/01.
        var date = new NepaliDate(2080, 1, 15);
        var start = date.FiscalYearStartDate();
        Assert.Equal(new NepaliDate(2079, 4, 1), start);
    }

    [Fact]
    public void FiscalYearStartDate_DateInAshadh_ReturnsPreviousYearShrawan1()
    {
        // Ashadh 2080 (month 3): last month of fiscal year 2079 → starts 2079/04/01.
        var date = new NepaliDate(2080, 3, 10);
        var start = date.FiscalYearStartDate();
        Assert.Equal(new NepaliDate(2079, 4, 1), start);
    }

    [Fact]
    public void FiscalYearStartDate_WithPositiveYearOffset_ShiftsForward()
    {
        // Date in Shrawan 2080 (fiscal year 2080) + offset 1 → fiscal year 2081 starts 2081/04/01.
        var date = new NepaliDate(2080, 4, 1);
        var start = date.FiscalYearStartDate(yearOffset: 1);
        Assert.Equal(new NepaliDate(2081, 4, 1), start);
    }

    [Fact]
    public void FiscalYearStartDate_WithNegativeYearOffset_ShiftsBackward()
    {
        // Date in Shrawan 2080 (fiscal year 2080) - offset 1 → fiscal year 2079 starts 2079/04/01.
        var date = new NepaliDate(2080, 4, 1);
        var start = date.FiscalYearStartDate(yearOffset: -1);
        Assert.Equal(new NepaliDate(2079, 4, 1), start);
    }

    // ---- FiscalYearEndDate ----

    [Fact]
    public void FiscalYearEndDate_DateInShrawan_ReturnsNextYearAshad()
    {
        // Shrawan 2080 is in fiscal year 2080 → ends last day of Ashadh 2081.
        var date = new NepaliDate(2080, 4, 15);
        var end = date.FiscalYearEndDate();
        Assert.Equal(2081, end.Year);
        Assert.Equal(3, end.Month);
        Assert.Equal(new NepaliDate(2081, 3, 1).MonthEndDay, end.Day);
    }

    [Fact]
    public void FiscalYearEndDate_DateInChaitra_ReturnsNextYearAshadh()
    {
        // Chaitra 2080 is still in fiscal year 2080 → ends last day of Ashadh 2081.
        var date = new NepaliDate(2080, 12, 15);
        var end = date.FiscalYearEndDate();
        Assert.Equal(2081, end.Year);
        Assert.Equal(3, end.Month);
    }

    [Fact]
    public void FiscalYearEndDate_DateInBasikhakh_ReturnsSameYearAshadh()
    {
        // Baishakh 2080 (month 1) is in fiscal year 2079 → ends last day of Ashadh 2080.
        var date = new NepaliDate(2080, 1, 15);
        var end = date.FiscalYearEndDate();
        Assert.Equal(2080, end.Year);
        Assert.Equal(3, end.Month);
        Assert.Equal(new NepaliDate(2080, 3, 1).MonthEndDay, end.Day);
    }

    [Fact]
    public void FiscalYearEndDate_WithPositiveYearOffset_ShiftsForward()
    {
        // Shrawan 2080 (FY 2080) + 1 offset → next FY ends last day of Ashadh 2082.
        var date = new NepaliDate(2080, 4, 1);
        var end = date.FiscalYearEndDate(yearOffset: 1);
        Assert.Equal(2082, end.Year);
        Assert.Equal(3, end.Month);
    }

    // ---- FiscalYearStartAndEndDate combined ----

    [Fact]
    public void FiscalYearStartAndEndDate_DateInQ2_ReturnsBothBoundaries()
    {
        // Mangsir 2080 (month 8) is in fiscal year 2080: start=2080/04/01, end=last day of 2081/03.
        var date = new NepaliDate(2080, 8, 15);
        var (start, end) = date.FiscalYearStartAndEndDate();

        Assert.Equal(new NepaliDate(2080, 4, 1), start);
        Assert.Equal(2081, end.Year);
        Assert.Equal(3, end.Month);
    }

    [Fact]
    public void FiscalYearStartAndEndDate_DateInQ4_ReturnsPreviousFiscalYearBoundaries()
    {
        // Jestha 2081 (month 2) is in fiscal year 2080: start=2080/04/01, end=last day of 2081/03.
        var date = new NepaliDate(2081, 2, 10);
        var (start, end) = date.FiscalYearStartAndEndDate();

        Assert.Equal(new NepaliDate(2080, 4, 1), start);
        Assert.Equal(2081, end.Year);
        Assert.Equal(3, end.Month);
    }

    // ---- FiscalYearQuarterStartDate with explicit quarters ----
    // Base date: 2080/05/15 (Bhadra 15, Q1 of FY 2080)

    [Fact]
    public void FiscalYearQuarterStartDate_ExplicitFirst_ReturnsShrawan1()
    {
        // Q1 starts on 1 Shrawan of the current year, regardless of which month the base date is in.
        var date = new NepaliDate(2080, 5, 15);
        var start = date.FiscalYearQuarterStartDate(FiscalYearQuarters.First);
        Assert.Equal(new NepaliDate(2080, 4, 1), start);
    }

    [Fact]
    public void FiscalYearQuarterStartDate_ExplicitSecond_ReturnsKartik1()
    {
        // Q2 starts on 1 Kartik (month 7).
        var date = new NepaliDate(2080, 5, 15);
        var start = date.FiscalYearQuarterStartDate(FiscalYearQuarters.Second);
        Assert.Equal(new NepaliDate(2080, 7, 1), start);
    }

    [Fact]
    public void FiscalYearQuarterStartDate_ExplicitThird_ReturnsMagh1()
    {
        // Q3 starts on 1 Magh (month 10).
        var date = new NepaliDate(2080, 5, 15);
        var start = date.FiscalYearQuarterStartDate(FiscalYearQuarters.Third);
        Assert.Equal(new NepaliDate(2080, 10, 1), start);
    }

    [Fact]
    public void FiscalYearQuarterStartDate_ExplicitFourth_ReturnsNextYearBasikhakh1()
    {
        // Q4 starts on 1 Baishakh (month 1) of year+1, because months 1-3 belong to next calendar year.
        var date = new NepaliDate(2080, 5, 15);
        var start = date.FiscalYearQuarterStartDate(FiscalYearQuarters.Fourth);
        Assert.Equal(new NepaliDate(2081, 1, 1), start);
    }

    // ---- FiscalYearQuarterEndDate with explicit quarters ----

    [Fact]
    public void FiscalYearQuarterEndDate_ExplicitFirst_ReturnsLastDayOfAshoj()
    {
        // Q1 ends last day of Ashoj (month 6).
        var date = new NepaliDate(2080, 5, 15);
        var end = date.FiscalYearQuarterEndDate(FiscalYearQuarters.First);
        Assert.Equal(2080, end.Year);
        Assert.Equal(6, end.Month);
        Assert.Equal(new NepaliDate(2080, 6, 1).MonthEndDay, end.Day);
    }

    [Fact]
    public void FiscalYearQuarterEndDate_ExplicitSecond_ReturnsLastDayOfPoush()
    {
        // Q2 ends last day of Poush (month 9).
        var date = new NepaliDate(2080, 5, 15);
        var end = date.FiscalYearQuarterEndDate(FiscalYearQuarters.Second);
        Assert.Equal(2080, end.Year);
        Assert.Equal(9, end.Month);
        Assert.Equal(new NepaliDate(2080, 9, 1).MonthEndDay, end.Day);
    }

    [Fact]
    public void FiscalYearQuarterEndDate_ExplicitThird_ReturnsLastDayOfChaitra()
    {
        // Q3 ends last day of Chaitra (month 12).
        var date = new NepaliDate(2080, 5, 15);
        var end = date.FiscalYearQuarterEndDate(FiscalYearQuarters.Third);
        Assert.Equal(2080, end.Year);
        Assert.Equal(12, end.Month);
        Assert.Equal(new NepaliDate(2080, 12, 1).MonthEndDay, end.Day);
    }

    [Fact]
    public void FiscalYearQuarterEndDate_ExplicitFourth_ReturnsNextYearLastDayOfAshadh()
    {
        // Q4 ends last day of Ashadh (month 3) of year+1.
        var date = new NepaliDate(2080, 5, 15);
        var end = date.FiscalYearQuarterEndDate(FiscalYearQuarters.Fourth);
        Assert.Equal(2081, end.Year);
        Assert.Equal(3, end.Month);
        Assert.Equal(new NepaliDate(2081, 3, 1).MonthEndDay, end.Day);
    }

    // ---- FiscalYearQuarterStartAndEndDate combined ----

    [Fact]
    public void FiscalYearQuarterStartAndEndDate_ExplicitSecond_ReturnsBothBoundaries()
    {
        var date = new NepaliDate(2080, 5, 15);
        var (start, end) = date.FiscalYearQuarterStartAndEndDate(FiscalYearQuarters.Second);

        Assert.Equal(new NepaliDate(2080, 7, 1), start);
        Assert.Equal(9, end.Month);
        Assert.Equal(2080, end.Year);
    }

    [Fact]
    public void FiscalYearQuarterStartAndEndDate_ExplicitThird_ReturnsBothBoundaries()
    {
        var date = new NepaliDate(2080, 5, 15);
        var (start, end) = date.FiscalYearQuarterStartAndEndDate(FiscalYearQuarters.Third);

        Assert.Equal(new NepaliDate(2080, 10, 1), start);
        Assert.Equal(12, end.Month);
    }

    // ---- FiscalYearQuarterStartDate / EndDate with Current on known quarters ----

    [Fact]
    public void FiscalYearQuarterStartDate_Current_DateInQ1_ReturnsShrawan1()
    {
        // Bhadra 2080 (month 5) is in Q1. Current quarter start = 2080/04/01.
        var date = new NepaliDate(2080, 5, 15);
        var start = date.FiscalYearQuarterStartDate(FiscalYearQuarters.Current);
        Assert.Equal(new NepaliDate(2080, 4, 1), start);
    }

    [Fact]
    public void FiscalYearQuarterEndDate_Current_DateInQ1_ReturnsLastDayOfAshoj()
    {
        var date = new NepaliDate(2080, 5, 15);
        var end = date.FiscalYearQuarterEndDate(FiscalYearQuarters.Current);
        Assert.Equal(6, end.Month);
        Assert.Equal(2080, end.Year);
    }

    [Fact]
    public void FiscalYearQuarterStartDate_Current_DateInQ2_ReturnsKartik1()
    {
        // Mangsir 2080 (month 8) is in Q2. Current quarter start = 2080/07/01.
        var date = new NepaliDate(2080, 8, 15);
        var start = date.FiscalYearQuarterStartDate(FiscalYearQuarters.Current);
        Assert.Equal(new NepaliDate(2080, 7, 1), start);
    }

    [Fact]
    public void FiscalYearQuarterStartDate_Current_DateInQ3_ReturnsMagh1()
    {
        // Falgun 2080 (month 11) is in Q3. Current quarter start = 2080/10/01.
        var date = new NepaliDate(2080, 11, 15);
        var start = date.FiscalYearQuarterStartDate(FiscalYearQuarters.Current);
        Assert.Equal(new NepaliDate(2080, 10, 1), start);
    }

    [Fact]
    public void FiscalYearQuarterEndDate_Current_DateInQ3_ReturnsLastDayOfChaitra()
    {
        var date = new NepaliDate(2080, 11, 15);
        var end = date.FiscalYearQuarterEndDate(FiscalYearQuarters.Current);
        Assert.Equal(12, end.Month);
        Assert.Equal(2080, end.Year);
    }

    // ---- FiscalYearQuarterStartDate with yearOffset ----

    [Fact]
    public void FiscalYearQuarterStartDate_ExplicitFirstWithOffset1_ReturnsNextYearShrawan1()
    {
        var date = new NepaliDate(2080, 5, 15);
        var start = date.FiscalYearQuarterStartDate(FiscalYearQuarters.First, yearOffset: 1);
        Assert.Equal(new NepaliDate(2081, 4, 1), start);
    }

    [Fact]
    public void FiscalYearQuarterStartDate_ExplicitFirstWithNegativeOffset_ReturnsPreviousYearShrawan1()
    {
        var date = new NepaliDate(2080, 5, 15);
        var start = date.FiscalYearQuarterStartDate(FiscalYearQuarters.First, yearOffset: -1);
        Assert.Equal(new NepaliDate(2079, 4, 1), start);
    }

    // ---- Consistency: FiscalYearStartAndEndDate matches manually constructed dates ----

    [Theory]
    [InlineData(4)]   // Shrawan — first month of fiscal year
    [InlineData(6)]   // Ashoj   — last month of Q1
    [InlineData(7)]   // Kartik  — first month of Q2
    [InlineData(12)]  // Chaitra — last month of Q3
    public void FiscalYearStartDate_AllFiscalMonths_Return2080Shrawan1(int month)
    {
        // All months 4-12 of 2080 belong to fiscal year 2080.
        var date = new NepaliDate(2080, month, 1);
        var start = date.FiscalYearStartDate();
        Assert.Equal(new NepaliDate(2080, 4, 1), start);
    }

    [Theory]
    [InlineData(1)]  // Baishakh
    [InlineData(2)]  // Jestha
    [InlineData(3)]  // Ashadh
    public void FiscalYearStartDate_Q4CalendarMonths_ReturnPreviousYearShrawan1(int month)
    {
        // Months 1-3 of 2080 belong to fiscal year 2079.
        var date = new NepaliDate(2080, month, 1);
        var start = date.FiscalYearStartDate();
        Assert.Equal(new NepaliDate(2079, 4, 1), start);
    }

    // ---- FiscalYearQuarterStartDate / EndDate from a Q4 base date (regression) ----
    // Base date: 2081/02/10 (Jestha 10, Q4 of FY 2080).
    // All explicit quarter requests must resolve against FY 2080, not FY 2081.

    [Fact]
    public void FiscalYearQuarterStartDate_FromQ4Date_ExplicitFirst_ReturnsFY2080Q1Start()
    {
        // From a Q4 date, Q1 of the current fiscal year (FY2080) starts on 2080/04/01.
        var date = new NepaliDate(2081, 2, 10);
        var start = date.FiscalYearQuarterStartDate(FiscalYearQuarters.First);
        Assert.Equal(new NepaliDate(2080, 4, 1), start);
    }

    [Fact]
    public void FiscalYearQuarterStartDate_FromQ4Date_ExplicitSecond_ReturnsFY2080Q2Start()
    {
        var date = new NepaliDate(2081, 2, 10);
        var start = date.FiscalYearQuarterStartDate(FiscalYearQuarters.Second);
        Assert.Equal(new NepaliDate(2080, 7, 1), start);
    }

    [Fact]
    public void FiscalYearQuarterStartDate_FromQ4Date_ExplicitThird_ReturnsFY2080Q3Start()
    {
        var date = new NepaliDate(2081, 2, 10);
        var start = date.FiscalYearQuarterStartDate(FiscalYearQuarters.Third);
        Assert.Equal(new NepaliDate(2080, 10, 1), start);
    }

    [Fact]
    public void FiscalYearQuarterStartDate_FromQ4Date_ExplicitFourth_ReturnsFY2080Q4Start()
    {
        // Q4 of FY2080 starts on 2081/01/01.
        var date = new NepaliDate(2081, 2, 10);
        var start = date.FiscalYearQuarterStartDate(FiscalYearQuarters.Fourth);
        Assert.Equal(new NepaliDate(2081, 1, 1), start);
    }

    [Fact]
    public void FiscalYearQuarterStartDate_FromQ4Date_Current_ReturnsFY2080Q4Start()
    {
        // Current quarter of 2081/02/10 is Q4; its start is 2081/01/01.
        var date = new NepaliDate(2081, 2, 10);
        var start = date.FiscalYearQuarterStartDate(FiscalYearQuarters.Current);
        Assert.Equal(new NepaliDate(2081, 1, 1), start);
    }

    [Fact]
    public void FiscalYearQuarterEndDate_FromQ4Date_ExplicitFirst_ReturnsFY2080Q1End()
    {
        var date = new NepaliDate(2081, 2, 10);
        var end = date.FiscalYearQuarterEndDate(FiscalYearQuarters.First);
        Assert.Equal(2080, end.Year);
        Assert.Equal(6, end.Month);
        Assert.Equal(new NepaliDate(2080, 6, 1).MonthEndDay, end.Day);
    }

    [Fact]
    public void FiscalYearQuarterEndDate_FromQ4Date_ExplicitThird_ReturnsFY2080Q3End()
    {
        var date = new NepaliDate(2081, 2, 10);
        var end = date.FiscalYearQuarterEndDate(FiscalYearQuarters.Third);
        Assert.Equal(2080, end.Year);
        Assert.Equal(12, end.Month);
        Assert.Equal(new NepaliDate(2080, 12, 1).MonthEndDay, end.Day);
    }

    [Fact]
    public void FiscalYearQuarterEndDate_FromQ4Date_ExplicitFourth_ReturnsFY2080Q4End()
    {
        // Q4 of FY2080 ends on last day of 2081/03.
        var date = new NepaliDate(2081, 2, 10);
        var end = date.FiscalYearQuarterEndDate(FiscalYearQuarters.Fourth);
        Assert.Equal(2081, end.Year);
        Assert.Equal(3, end.Month);
        Assert.Equal(new NepaliDate(2081, 3, 1).MonthEndDay, end.Day);
    }

    [Fact]
    public void FiscalYearQuarterEndDate_FromQ4Date_Current_ReturnsFY2080Q4End()
    {
        var date = new NepaliDate(2081, 2, 10);
        var end = date.FiscalYearQuarterEndDate(FiscalYearQuarters.Current);
        Assert.Equal(2081, end.Year);
        Assert.Equal(3, end.Month);
        Assert.Equal(new NepaliDate(2081, 3, 1).MonthEndDay, end.Day);
    }
}
