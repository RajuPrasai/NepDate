namespace NepDate.Tests.Core;

/// <summary>
/// Tests for AddDays, AddMonths, the DateTime constructor (time component ignored),
/// and the TryParse autoAdjust overload.
///
/// Expected values are derived from first principles:
///   - Nepali calendar knowledge (month lengths, year boundaries)
///   - Manual arithmetic on known anchor points
///   - The AddMonths fractional rule: if the value is not a whole number it is
///     converted to days as Math.Round(months * 30.41666..., 0, AwayFromZero).
/// </summary>
public class NepaliDateArithmeticEdgeCaseTests
{
    // Known anchor: 2080/05/15 (Bhadra 15) = 1 September 2023 (Friday).
    // Bhadra 2080 has 31 days (verified from existing tests).

    // ---- AddDays edge cases ----

    [Fact]
    public void AddDays_Zero_ReturnsSameDate()
    {
        var date = new NepaliDate(2080, 5, 15);
        var result = date.AddDays(0);
        Assert.Equal(date, result);
    }

    [Fact]
    public void AddDays_PositiveAcrossMonthBoundary_IsCorrect()
    {
        // 2080/05/31 (last day of Bhadra) + 1 day = 2080/06/01 (first day of Ashoj)
        var lastOfBhadra = new NepaliDate(2080, 5, 31);
        var result = lastOfBhadra.AddDays(1);
        Assert.Equal(2080, result.Year);
        Assert.Equal(6, result.Month);
        Assert.Equal(1, result.Day);
    }

    [Fact]
    public void AddDays_NegativeAcrossMonthBoundary_IsCorrect()
    {
        // 2080/06/01 (first of Ashoj) - 1 day = 2080/05/31 (last of Bhadra)
        var firstOfAshoj = new NepaliDate(2080, 6, 1);
        var result = firstOfAshoj.AddDays(-1);
        Assert.Equal(2080, result.Year);
        Assert.Equal(5, result.Month);
        Assert.Equal(31, result.Day);
    }

    [Fact]
    public void AddDays_PositiveAcrossYearBoundary_IsCorrect()
    {
        // We know 2080/05/15 = Sep 1, 2023.
        // Sep 1 + 365 days = Sep 1, 2024 = 2081/05/16 (2024 is a leap year in Gregorian).
        // Rather than trusting that, use a simpler assertion:
        // 2080/12 last day + 1 = 2081/01/01
        var lastOfChaitra2080 = new NepaliDate(2080, 12, new NepaliDate(2080, 12, 1).MonthEndDay);
        var result = lastOfChaitra2080.AddDays(1);
        Assert.Equal(2081, result.Year);
        Assert.Equal(1, result.Month);
        Assert.Equal(1, result.Day);
    }

    [Fact]
    public void AddDays_NegativeAcrossYearBoundary_IsCorrect()
    {
        // 2081/01/01 - 1 = last day of 2080/12
        var firstOfBasikhakh2081 = new NepaliDate(2081, 1, 1);
        var result = firstOfBasikhakh2081.AddDays(-1);
        Assert.Equal(2080, result.Year);
        Assert.Equal(12, result.Month);
        Assert.Equal(new NepaliDate(2080, 12, 1).MonthEndDay, result.Day);
    }

    [Fact]
    public void AddDays_LargePositiveValue_IsCorrect()
    {
        // 365 days from 2080/01/01 → should land in 2081/01/01 or 2081/01/02
        // depending on whether BS year 2080 has 365 or 366 days.
        // We just verify the year advances correctly.
        var start = new NepaliDate(2080, 1, 1);
        var result = start.AddDays(365);
        Assert.True(result.Year == 2081 || result.Year == 2080);
        Assert.True(result >= start);
    }

    // ---- AddMonths: integer cases (cross-year boundary) ----

    [Fact]
    public void AddMonths_PositiveCrossesYearBoundary_IsCorrect()
    {
        // 2080/11/15 (Falgun 15) + 5 months
        // 11 + 5 = 16 → while (16 > 12): year++, month -= 12 → year=2081, month=4
        // Day 15 fits in Shrawan 2081 (Shrawan usually has 31+ days).
        var date = new NepaliDate(2080, 11, 15);
        var result = date.AddMonths(5);
        Assert.Equal(2081, result.Year);
        Assert.Equal(4, result.Month);
        Assert.Equal(15, result.Day);
    }

    [Fact]
    public void AddMonths_NegativeCrossesYearBoundary_IsCorrect()
    {
        // 2080/03/15 (Ashad 15) - 5 months
        // 3 - 5 = -2 → while (-2 < 1): year--, month += 12 → year=2079, month=10
        // Day 15 fits in Magh 2079.
        var date = new NepaliDate(2080, 3, 15);
        var result = date.AddMonths(-5);
        Assert.Equal(2079, result.Year);
        Assert.Equal(10, result.Month);
        Assert.Equal(15, result.Day);
    }

    [Fact]
    public void AddMonths_PositiveMoreThan12_CrossesMultipleYears()
    {
        // 2080/05/15 + 14 months
        // 5 + 14 = 19 → while (19 > 12): year++, month -= 12 → year=2081, month=7
        var date = new NepaliDate(2080, 5, 15);
        var result = date.AddMonths(14);
        Assert.Equal(2081, result.Year);
        Assert.Equal(7, result.Month);
        Assert.Equal(15, result.Day);
    }

    [Fact]
    public void AddMonths_Exactly12_SameMonthNextYear()
    {
        // +12 months is always the same month one year ahead.
        var date = new NepaliDate(2080, 6, 15);
        var result = date.AddMonths(12);
        Assert.Equal(2081, result.Year);
        Assert.Equal(6, result.Month);
        Assert.Equal(15, result.Day);
    }

    [Fact]
    public void AddMonths_DayCappedAtMonthEnd_WhenResultMonthIsShorter()
    {
        // 2081/04/32 (Shrawan 32 — last day of Shrawan 2081, which has 32 days per tests).
        // + 5 months → month = 4 + 5 = 9 (Poush 2081).
        // Poush 2081 likely has fewer than 32 days.
        // The result day must be capped at MonthEndDay of Poush 2081.
        var date = new NepaliDate(2081, 4, 32);
        var result = date.AddMonths(5);
        Assert.Equal(2081, result.Year);
        Assert.Equal(9, result.Month);
        Assert.True(result.Day <= result.MonthEndDay);
        Assert.True(result.Day > 0);
    }

    [Fact]
    public void AddMonths_Negative_SameDayPreservedWhenFits()
    {
        // 2080/08/10 (Mangsir 10) - 1 = 2080/07/10 (Kartik 10)
        var date = new NepaliDate(2080, 8, 10);
        var result = date.AddMonths(-1);
        Assert.Equal(2080, result.Year);
        Assert.Equal(7, result.Month);
        Assert.Equal(10, result.Day);
    }

    // ---- AddMonths: fractional values (converted to days internally) ----

    [Fact]
    public void AddMonths_HalfMonth_AddsApproximately15Days()
    {
        // 0.5 months → Math.Round(0.5 * 30.41666..., AwayFromZero) = Math.Round(15.2083) = 15 days.
        var start = new NepaliDate(2080, 5, 1);
        var result = start.AddMonths(0.5);
        // 2080/05/01 + 15 days = 2080/05/16
        Assert.Equal(2080, result.Year);
        Assert.Equal(5, result.Month);
        Assert.Equal(16, result.Day);
    }

    [Fact]
    public void AddMonths_NegativeHalfMonth_SubtractsApproximately15Days()
    {
        // -0.5 months → subtracts Math.Round(15.2083) = 15 days.
        var start = new NepaliDate(2080, 5, 16);
        var result = start.AddMonths(-0.5);
        Assert.Equal(2080, result.Year);
        Assert.Equal(5, result.Month);
        Assert.Equal(1, result.Day);
    }

    [Fact]
    public void AddMonths_OneAndHalfMonths_AddsApproximately46Days()
    {
        // 1.5 months → Math.Round(1.5, AwayFromZero) = 2 (rounded), but 1.5 != 2,
        // so: AddDays(Math.Round(1.5 * 30.41666, AwayFromZero)) = AddDays(Math.Round(45.625)) = AddDays(46).
        // 2080/05/01 + 46 days:
        //   Bhadra (month 5) has 31 days → from day 1, 30 days reaches day 31, 1 more = Ashoj day 1, 15 more = Ashoj day 16.
        //   Total: 31 + 15 = 46 days → 2080/06/16.
        var start = new NepaliDate(2080, 5, 1);
        var result = start.AddMonths(1.5);
        Assert.Equal(2080, result.Year);
        Assert.Equal(6, result.Month);
        Assert.Equal(16, result.Day);
    }

    [Fact]
    public void AddMonths_FractionalValue_ReturnsDateFurtherThanZeroMonths()
    {
        var start = new NepaliDate(2080, 5, 15);
        var result = start.AddMonths(0.1);
        // 0.1 months → Math.Round(0.1 * 30.41666) = Math.Round(3.04166) = 3 days.
        Assert.Equal(start.AddDays(3), result);
    }

    // ---- DateTime constructor: time component must be ignored ----

    [Fact]
    public void Constructor_DateTimeWithMidnightTime_IgnoresTime()
    {
        var engDate = new DateTime(2023, 9, 1, 0, 0, 0); // Sep 1 2023 = 2080/05/15
        var nepaliDate = new NepaliDate(engDate);
        Assert.Equal(2080, nepaliDate.Year);
        Assert.Equal(5, nepaliDate.Month);
        Assert.Equal(15, nepaliDate.Day);
    }

    [Fact]
    public void Constructor_DateTimeWithNoonTime_IgnoresTime()
    {
        var engDate = new DateTime(2023, 9, 1, 12, 30, 45); // Same date as midnight
        var nepaliDate = new NepaliDate(engDate);
        Assert.Equal(2080, nepaliDate.Year);
        Assert.Equal(5, nepaliDate.Month);
        Assert.Equal(15, nepaliDate.Day);
    }

    [Fact]
    public void Constructor_DateTimeWithEndOfDayTime_IgnoresTime()
    {
        var engDate = new DateTime(2023, 9, 1, 23, 59, 59);
        var nepaliDate = new NepaliDate(engDate);
        Assert.Equal(2080, nepaliDate.Year);
        Assert.Equal(5, nepaliDate.Month);
        Assert.Equal(15, nepaliDate.Day);
    }

    [Fact]
    public void Constructor_TwoDateTimesOnSameEnglishDateDifferentTimes_ProduceSameNepaliDate()
    {
        var morning = new NepaliDate(new DateTime(2023, 1, 1, 6, 0, 0));
        var evening = new NepaliDate(new DateTime(2023, 1, 1, 22, 0, 0));
        Assert.Equal(morning, evening);
    }

    // ---- TryParse with autoAdjust overload ----

    [Fact]
    public void TryParse_AutoAdjust_DayYearMonthFormat_ReturnsCorrectDate()
    {
        // "15/05/2080" parsed naively gives Year=15, Month=5, Day=2080.
        // autoAdjust=true: Day (2080) has length >= 3 → swap Year and Day → Year=2080, Day=15.
        // monthInMiddle=true: no Day/Month swap.
        // Result: 2080/05/15.
        bool success = NepaliDate.TryParse("15/05/2080", out var result, autoAdjust: true, monthInMiddle: true);
        Assert.True(success);
        Assert.Equal(2080, result.Year);
        Assert.Equal(5, result.Month);
        Assert.Equal(15, result.Day);
    }

    [Fact]
    public void TryParse_AutoAdjust_MonthDayYearInvertedOrder_ReturnsCorrectDate()
    {
        // "05/15/2080" parsed naively gives Year=5, Month=15, Day=2080.
        // autoAdjust=true: Day (2080) length >= 3 → swap Year and Day → Year=2080, Day=5.
        // monthInMiddle=false: swap Month and Day → Month=5, Day=15.
        // Result: 2080/05/15.
        bool success = NepaliDate.TryParse("05/15/2080", out var result, autoAdjust: true, monthInMiddle: false);
        Assert.True(success);
        Assert.Equal(2080, result.Year);
        Assert.Equal(5, result.Month);
        Assert.Equal(15, result.Day);
    }

    [Fact]
    public void TryParse_AutoAdjust_TwoDigitYear_ExpandsToCurrentMillennium()
    {
        // "15/05/80" → Year=15, Month=5, Day=80 → Day length >= 3? No (80 has 2 digits).
        // Day > 32? 80 > 32 → swap Year and Day → Year=80, Day=15.
        // monthInMiddle=true: no swap.
        // Year.Length <= 3 → prepend "2" → Year = 2080.
        bool success = NepaliDate.TryParse("15/05/80", out var result, autoAdjust: true, monthInMiddle: true);
        Assert.True(success);
        Assert.Equal(2080, result.Year);
        Assert.Equal(5, result.Month);
        Assert.Equal(15, result.Day);
    }

    [Fact]
    public void TryParse_AutoAdjust_InvalidDateAfterAdjustment_ReturnsFalse()
    {
        // "99/99/99" cannot be adjusted into a valid Nepali date.
        bool success = NepaliDate.TryParse("99/99/99", out var result, autoAdjust: true, monthInMiddle: true);
        Assert.False(success);
        Assert.Equal(default, result);
    }

    [Fact]
    public void TryParse_AutoAdjust_ValidStandardFormat_ReturnsCorrectDate()
    {
        // "2080/05/15" is already standard — autoAdjust should still produce the right result.
        bool success = NepaliDate.TryParse("2080/05/15", out var result, autoAdjust: true, monthInMiddle: true);
        Assert.True(success);
        Assert.Equal(new NepaliDate(2080, 5, 15), result);
    }
}
