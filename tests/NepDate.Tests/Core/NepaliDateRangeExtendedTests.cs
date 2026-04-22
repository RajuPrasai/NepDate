namespace NepDate.Tests.Core;

/// <summary>
/// Covers all NepaliDateRange functionality not exercised by NepaliDateRangeTests.cs:
///   ForCalendarYear, CurrentMonth/FiscalYear/CalendarYear (structure only),
///   SplitByFiscalQuarter, DatesWithInterval, WeekendDays, WorkingDays,
///   edge cases for Contains/IsAdjacentTo/Intersect/Union/Except,
///   GetEnumerator, ToString overloads, Equals, GetHashCode, and the == / != operators.
///
/// All expected values are computed from first-principles calendar knowledge:
///   - 2080/05/15 = Friday 1 September 2023 (confirmed by existing tests)
///   - Bhadra (month 5) 2080 has 31 days (confirmed by existing tests)
/// </summary>
public class NepaliDateRangeExtendedTests
{
    // ---- ForCalendarYear ----

    [Fact]
    public void ForCalendarYear_StartsAtBaishakhOne_EndsAtChaitraEnd()
    {
        var range = NepaliDateRange.ForCalendarYear(2080);

        Assert.Equal(new NepaliDate(2080, 1, 1), range.Start);
        Assert.Equal(12, range.End.Month);
        Assert.Equal(2080, range.End.Year);
        Assert.Equal(range.End.MonthEndDay, range.End.Day); // last day of Chaitra
    }

    [Fact]
    public void ForCalendarYear_LengthIsReasonable()
    {
        var range = NepaliDateRange.ForCalendarYear(2080);
        // A Nepali year is 365 or 366 days.
        Assert.InRange(range.Length, 365, 366);
    }

    // ---- CurrentMonth / CurrentFiscalYear / CurrentCalendarYear (structural) ----

    [Fact]
    public void CurrentMonth_StartIsFirstDayOfCurrentMonth()
    {
        var range = NepaliDateRange.CurrentMonth();
        var today = new NepaliDate(DateTime.Today);
        Assert.Equal(today.Year, range.Start.Year);
        Assert.Equal(today.Month, range.Start.Month);
        Assert.Equal(1, range.Start.Day);
    }

    [Fact]
    public void CurrentMonth_EndIsLastDayOfCurrentMonth()
    {
        var range = NepaliDateRange.CurrentMonth();
        var today = new NepaliDate(DateTime.Today);
        Assert.Equal(today.Year, range.End.Year);
        Assert.Equal(today.Month, range.End.Month);
        Assert.Equal(range.End.MonthEndDay, range.End.Day);
    }

    [Fact]
    public void CurrentMonth_ContainsToday()
    {
        var range = NepaliDateRange.CurrentMonth();
        Assert.True(range.Contains(new NepaliDate(DateTime.Today)));
    }

    [Fact]
    public void CurrentFiscalYear_ContainsToday()
    {
        var range = NepaliDateRange.CurrentFiscalYear();
        Assert.True(range.Contains(new NepaliDate(DateTime.Today)));
    }

    [Fact]
    public void CurrentFiscalYear_StartIsShrawan()
    {
        var range = NepaliDateRange.CurrentFiscalYear();
        Assert.Equal(4, range.Start.Month); // Shrawan = month 4
        Assert.Equal(1, range.Start.Day);
    }

    [Fact]
    public void CurrentFiscalYear_EndIsAshadh()
    {
        var range = NepaliDateRange.CurrentFiscalYear();
        Assert.Equal(3, range.End.Month); // Ashadh = month 3
        Assert.Equal(range.End.MonthEndDay, range.End.Day);
    }

    [Fact]
    public void CurrentCalendarYear_ContainsToday()
    {
        var range = NepaliDateRange.CurrentCalendarYear();
        Assert.True(range.Contains(new NepaliDate(DateTime.Today)));
    }

    [Fact]
    public void CurrentCalendarYear_StartIsBaishakh1()
    {
        var range = NepaliDateRange.CurrentCalendarYear();
        Assert.Equal(1, range.Start.Month);
        Assert.Equal(1, range.Start.Day);
    }

    // ---- SplitByFiscalQuarter ----

    [Fact]
    public void SplitByFiscalQuarter_EmptyRange_ReturnsEmptyArray()
    {
        var empty = new NepaliDateRange(new NepaliDate(2080, 5, 10), new NepaliDate(2080, 5, 1));
        Assert.Empty(empty.SplitByFiscalQuarter());
    }

    [Fact]
    public void SplitByFiscalQuarter_RangeWithinOneQuarter_ReturnsSingleRange()
    {
        // 2080/05/01 to 2080/05/20 is entirely within Q1 (months 4-6).
        var range = new NepaliDateRange(new NepaliDate(2080, 5, 1), new NepaliDate(2080, 5, 20));
        var parts = range.SplitByFiscalQuarter();

        Assert.Single(parts);
        Assert.Equal(new NepaliDate(2080, 5, 1), parts[0].Start);
        Assert.Equal(new NepaliDate(2080, 5, 20), parts[0].End);
    }

    [Fact]
    public void SplitByFiscalQuarter_RangeSpanningTwoQuarters_ReturnsTwoRanges()
    {
        // 2080/05/01 to 2080/08/01 spans Q1 (ends month 6) and starts Q2 (month 7).
        var range = new NepaliDateRange(new NepaliDate(2080, 5, 1), new NepaliDate(2080, 8, 1));
        var parts = range.SplitByFiscalQuarter();

        Assert.Equal(2, parts.Length);

        // First part: 2080/05/01 to last day of 2080/06 (Ashoj)
        Assert.Equal(new NepaliDate(2080, 5, 1), parts[0].Start);
        Assert.Equal(6, parts[0].End.Month);
        Assert.Equal(new NepaliDate(2080, 6, 1).MonthEndDay, parts[0].End.Day);

        // Second part: 2080/07/01 to 2080/08/01 (range end)
        Assert.Equal(new NepaliDate(2080, 7, 1), parts[1].Start);
        Assert.Equal(new NepaliDate(2080, 8, 1), parts[1].End);
    }

    [Fact]
    public void SplitByFiscalQuarter_FullFiscalYear_ReturnsFourRanges()
    {
        // Fiscal year 2080: 2080/04/01 to last day of 2081/03.
        var fiscalYearRange = NepaliDateRange.ForFiscalYear(2080);
        var parts = fiscalYearRange.SplitByFiscalQuarter();

        Assert.Equal(4, parts.Length);

        // Q1: Shrawan–Ashoj 2080
        Assert.Equal(new NepaliDate(2080, 4, 1), parts[0].Start);
        Assert.Equal(6, parts[0].End.Month);

        // Q2: Kartik–Poush 2080
        Assert.Equal(new NepaliDate(2080, 7, 1), parts[1].Start);
        Assert.Equal(9, parts[1].End.Month);

        // Q3: Magh–Chaitra 2080
        Assert.Equal(new NepaliDate(2080, 10, 1), parts[2].Start);
        Assert.Equal(12, parts[2].End.Month);

        // Q4: Baishakh–Ashadh 2081
        Assert.Equal(new NepaliDate(2081, 1, 1), parts[3].Start);
        Assert.Equal(3, parts[3].End.Month);
    }

    [Fact]
    public void SplitByFiscalQuarter_RangeInQ4Months_SingleRange()
    {
        // 2080/01/01 to 2080/03/30 is entirely within Q4 (months 1-3).
        var range = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 3, 30));
        var parts = range.SplitByFiscalQuarter();

        Assert.Single(parts);
        Assert.Equal(new NepaliDate(2080, 1, 1), parts[0].Start);
        Assert.Equal(new NepaliDate(2080, 3, 30), parts[0].End);
    }

    // ---- DatesWithInterval ----

    [Fact]
    public void DatesWithInterval_Zero_ThrowsArgumentOutOfRangeException()
    {
        var range = NepaliDateRange.FromDayCount(new NepaliDate(2080, 1, 1), 10);
        Assert.Throws<ArgumentOutOfRangeException>(() => range.DatesWithInterval(0).ToList());
    }

    [Fact]
    public void DatesWithInterval_NegativeInterval_ThrowsArgumentOutOfRangeException()
    {
        var range = NepaliDateRange.FromDayCount(new NepaliDate(2080, 1, 1), 10);
        Assert.Throws<ArgumentOutOfRangeException>(() => range.DatesWithInterval(-1).ToList());
    }

    [Fact]
    public void DatesWithInterval_IntervalOfOne_YieldsEveryDay()
    {
        // A 5-day range with interval 1 must yield exactly 5 dates.
        var range = NepaliDateRange.FromDayCount(new NepaliDate(2080, 5, 1), 5);
        var dates = range.DatesWithInterval(1).ToList();
        Assert.Equal(5, dates.Count);
        for (int i = 0; i < 5; i++)
        {
            Assert.Equal(new NepaliDate(2080, 5, 1 + i), dates[i]);
        }
    }

    [Fact]
    public void DatesWithInterval_IntervalOfTwo_YieldsOddDays()
    {
        // 2080/01/01 to 2080/01/07 (7 days) with interval 2:
        // yields: 01/01, 01/03, 01/05, 01/07 → 4 dates.
        var range = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 7));
        var dates = range.DatesWithInterval(2).ToList();

        Assert.Equal(4, dates.Count);
        Assert.Equal(new NepaliDate(2080, 1, 1), dates[0]);
        Assert.Equal(new NepaliDate(2080, 1, 3), dates[1]);
        Assert.Equal(new NepaliDate(2080, 1, 5), dates[2]);
        Assert.Equal(new NepaliDate(2080, 1, 7), dates[3]);
    }

    [Fact]
    public void DatesWithInterval_IntervalLargerThanRange_YieldsOnlyStartDate()
    {
        // A 3-day range with interval 10 yields only the start date.
        var range = NepaliDateRange.FromDayCount(new NepaliDate(2080, 5, 1), 3);
        var dates = range.DatesWithInterval(10).ToList();
        Assert.Single(dates);
        Assert.Equal(new NepaliDate(2080, 5, 1), dates[0]);
    }

    [Fact]
    public void DatesWithInterval_EmptyRange_YieldsNothing()
    {
        var empty = new NepaliDateRange(new NepaliDate(2080, 5, 10), new NepaliDate(2080, 5, 1));
        Assert.Empty(empty.DatesWithInterval(1).ToList());
    }

    // ---- WeekendDays ----
    // Anchor: 2080/05/15 = Friday. So: 05/16 = Sat, 05/17 = Sun.

    [Fact]
    public void WeekendDays_IncludeSundayTrue_ReturnsBothSaturdayAndSunday()
    {
        // Range: Thursday–Monday (5 days), only Sat and Sun are weekends.
        var range = new NepaliDateRange(new NepaliDate(2080, 5, 14), new NepaliDate(2080, 5, 18));
        var weekends = range.WeekendDays(includeSunday: true).ToList();

        Assert.Equal(2, weekends.Count);
        Assert.Equal(new NepaliDate(2080, 5, 16), weekends[0]); // Saturday
        Assert.Equal(new NepaliDate(2080, 5, 17), weekends[1]); // Sunday
    }

    [Fact]
    public void WeekendDays_IncludeSundayFalse_ReturnsOnlySaturday()
    {
        var range = new NepaliDateRange(new NepaliDate(2080, 5, 14), new NepaliDate(2080, 5, 18));
        var weekends = range.WeekendDays(includeSunday: false).ToList();

        Assert.Single(weekends);
        Assert.Equal(new NepaliDate(2080, 5, 16), weekends[0]); // Saturday only
    }

    [Fact]
    public void WeekendDays_RangeWithNoWeekend_ReturnsEmpty()
    {
        // Mon–Fri: 2080/05/15(Fri) back to 2080/05/11(Mon) ... 
        // Use 2080/05/11 (Mon) to 2080/05/15 (Fri) = no Sat/Sun.
        // 2080/05/15 = Fri, so 05/11 = Mon.
        var range = new NepaliDateRange(new NepaliDate(2080, 5, 11), new NepaliDate(2080, 5, 15));
        var weekends = range.WeekendDays(includeSunday: true).ToList();
        Assert.Empty(weekends);
    }

    [Fact]
    public void WeekendDays_EmptyRange_ReturnsEmpty()
    {
        var empty = new NepaliDateRange(new NepaliDate(2080, 5, 10), new NepaliDate(2080, 5, 1));
        Assert.Empty(empty.WeekendDays().ToList());
    }

    // ---- WorkingDays ----

    [Fact]
    public void WorkingDays_ExcludeSundayFalse_ExcludesOnlySaturday()
    {
        // Thu(14), Fri(15), Sat(16), Sun(17), Mon(18) — excludes only Sat → 4 working days.
        var range = new NepaliDateRange(new NepaliDate(2080, 5, 14), new NepaliDate(2080, 5, 18));
        var working = range.WorkingDays(excludeSunday: false).ToList();

        Assert.Equal(4, working.Count);
        Assert.DoesNotContain(new NepaliDate(2080, 5, 16), working); // Saturday excluded
        Assert.Contains(new NepaliDate(2080, 5, 17), working);       // Sunday included
    }

    [Fact]
    public void WorkingDays_ExcludeSundayTrue_ExcludesBothSatAndSun()
    {
        // Thu(14), Fri(15), Sat(16), Sun(17), Mon(18) — excludes Sat and Sun → 3 working days.
        var range = new NepaliDateRange(new NepaliDate(2080, 5, 14), new NepaliDate(2080, 5, 18));
        var working = range.WorkingDays(excludeSunday: true).ToList();

        Assert.Equal(3, working.Count);
        Assert.Contains(new NepaliDate(2080, 5, 14), working); // Thursday
        Assert.Contains(new NepaliDate(2080, 5, 15), working); // Friday
        Assert.Contains(new NepaliDate(2080, 5, 18), working); // Monday
        Assert.DoesNotContain(new NepaliDate(2080, 5, 16), working); // Saturday excluded
        Assert.DoesNotContain(new NepaliDate(2080, 5, 17), working); // Sunday excluded
    }

    [Fact]
    public void WorkingDays_EmptyRange_ReturnsEmpty()
    {
        var empty = new NepaliDateRange(new NepaliDate(2080, 5, 10), new NepaliDate(2080, 5, 1));
        Assert.Empty(empty.WorkingDays().ToList());
    }

    // ---- FromDayCount edge cases ----

    [Fact]
    public void FromDayCount_ZeroDays_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            NepaliDateRange.FromDayCount(new NepaliDate(2080, 5, 1), 0));
    }

    [Fact]
    public void FromDayCount_NegativeDays_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            NepaliDateRange.FromDayCount(new NepaliDate(2080, 5, 1), -5));
    }

    // ---- Intersect edge cases ----

    [Fact]
    public void Intersect_NonOverlappingRanges_ReturnsEmptyRange()
    {
        var range1 = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 10));
        var range2 = new NepaliDateRange(new NepaliDate(2080, 1, 15), new NepaliDate(2080, 1, 25));

        var intersection = range1.Intersect(range2);

        Assert.True(intersection.IsEmpty);
    }

    [Fact]
    public void Intersect_AdjacentRanges_ReturnsEmptyRange()
    {
        // Range1 ends on day 10, range2 starts on day 11 — no shared date.
        var range1 = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 10));
        var range2 = new NepaliDateRange(new NepaliDate(2080, 1, 11), new NepaliDate(2080, 1, 20));

        Assert.True(range1.Intersect(range2).IsEmpty);
    }

    [Fact]
    public void Intersect_OneRangeFullyInsideOther_ReturnsInnerRange()
    {
        var outer = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 30));
        var inner = new NepaliDateRange(new NepaliDate(2080, 1, 10), new NepaliDate(2080, 1, 20));

        var intersection = outer.Intersect(inner);

        Assert.Equal(inner.Start, intersection.Start);
        Assert.Equal(inner.End, intersection.End);
    }

    // ---- Union edge cases ----

    [Fact]
    public void Union_LeftRangeIsEmpty_ReturnsRightRange()
    {
        var empty = new NepaliDateRange(new NepaliDate(2080, 5, 10), new NepaliDate(2080, 5, 1));
        var actual = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 15));

        var union = empty.Union(actual);

        Assert.Equal(actual.Start, union.Start);
        Assert.Equal(actual.End, union.End);
    }

    [Fact]
    public void Union_RightRangeIsEmpty_ReturnsLeftRange()
    {
        var actual = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 15));
        var empty = new NepaliDateRange(new NepaliDate(2080, 5, 10), new NepaliDate(2080, 5, 1));

        var union = actual.Union(empty);

        Assert.Equal(actual.Start, union.Start);
        Assert.Equal(actual.End, union.End);
    }

    [Fact]
    public void Union_NonOverlappingRanges_SpansEntireGap()
    {
        // Range1: 01/01–01/10, Range2: 01/20–01/30 — union covers 01/01–01/30 including the gap.
        var range1 = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 10));
        var range2 = new NepaliDateRange(new NepaliDate(2080, 1, 20), new NepaliDate(2080, 1, 30));

        var union = range1.Union(range2);

        Assert.Equal(new NepaliDate(2080, 1, 1), union.Start);
        Assert.Equal(new NepaliDate(2080, 1, 30), union.End);
    }

    // ---- Contains boundary cases ----

    [Fact]
    public void Contains_ExactStartDate_ReturnsTrue()
    {
        var range = new NepaliDateRange(new NepaliDate(2080, 1, 5), new NepaliDate(2080, 1, 20));
        Assert.True(range.Contains(new NepaliDate(2080, 1, 5)));
    }

    [Fact]
    public void Contains_ExactEndDate_ReturnsTrue()
    {
        var range = new NepaliDateRange(new NepaliDate(2080, 1, 5), new NepaliDate(2080, 1, 20));
        Assert.True(range.Contains(new NepaliDate(2080, 1, 20)));
    }

    [Fact]
    public void Contains_OneDayBeforeStart_ReturnsFalse()
    {
        var range = new NepaliDateRange(new NepaliDate(2080, 1, 5), new NepaliDate(2080, 1, 20));
        Assert.False(range.Contains(new NepaliDate(2080, 1, 4)));
    }

    [Fact]
    public void Contains_OneDayAfterEnd_ReturnsFalse()
    {
        var range = new NepaliDateRange(new NepaliDate(2080, 1, 5), new NepaliDate(2080, 1, 20));
        Assert.False(range.Contains(new NepaliDate(2080, 1, 21)));
    }

    [Fact]
    public void Contains_EmptyRange_ContainsNothing()
    {
        var empty = new NepaliDateRange(new NepaliDate(2080, 5, 10), new NepaliDate(2080, 5, 1));
        Assert.False(empty.Contains(new NepaliDate(2080, 5, 5)));
    }

    // ---- IsAdjacentTo false cases ----

    [Fact]
    public void IsAdjacentTo_GapBetweenRanges_ReturnsFalse()
    {
        // Range1 ends 01/10, Range2 starts 01/12 — gap of one day means not adjacent.
        var range1 = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 10));
        var range2 = new NepaliDateRange(new NepaliDate(2080, 1, 12), new NepaliDate(2080, 1, 20));

        Assert.False(range1.IsAdjacentTo(range2));
    }

    [Fact]
    public void IsAdjacentTo_OverlappingRanges_ReturnsFalse()
    {
        var range1 = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 15));
        var range2 = new NepaliDateRange(new NepaliDate(2080, 1, 10), new NepaliDate(2080, 1, 20));

        Assert.False(range1.IsAdjacentTo(range2));
    }

    [Fact]
    public void IsAdjacentTo_EmptyRange_ReturnsFalse()
    {
        var actual = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 10));
        var empty = new NepaliDateRange(new NepaliDate(2080, 1, 15), new NepaliDate(2080, 1, 12));

        Assert.False(actual.IsAdjacentTo(empty));
        Assert.False(empty.IsAdjacentTo(actual));
    }

    // ---- Except edge cases ----

    [Fact]
    public void Except_NonOverlappingExcludeRange_ReturnsOriginalRange()
    {
        var range = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 10));
        var exclude = new NepaliDateRange(new NepaliDate(2080, 2, 1), new NepaliDate(2080, 2, 15));

        var result = range.Except(exclude);

        Assert.Single(result);
        Assert.Equal(range.Start, result[0].Start);
        Assert.Equal(range.End, result[0].End);
    }

    [Fact]
    public void Except_ExcludeRangeFullyContainsThis_ReturnsEmptyArray()
    {
        var range = new NepaliDateRange(new NepaliDate(2080, 1, 5), new NepaliDate(2080, 1, 15));
        var exclude = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 20));

        var result = range.Except(exclude);

        Assert.Empty(result);
    }

    [Fact]
    public void Except_ExcludeOverlapsStart_ReturnsRightPart()
    {
        // Range: 01/01–01/20, Exclude: 01/01–01/10 → leaves 01/11–01/20.
        var range = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 20));
        var exclude = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 10));

        var result = range.Except(exclude);

        Assert.Single(result);
        Assert.Equal(new NepaliDate(2080, 1, 11), result[0].Start);
        Assert.Equal(new NepaliDate(2080, 1, 20), result[0].End);
    }

    [Fact]
    public void Except_ExcludeOverlapsEnd_ReturnsLeftPart()
    {
        // Range: 01/01–01/20, Exclude: 01/15–01/20 → leaves 01/01–01/14.
        var range = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 20));
        var exclude = new NepaliDateRange(new NepaliDate(2080, 1, 15), new NepaliDate(2080, 1, 20));

        var result = range.Except(exclude);

        Assert.Single(result);
        Assert.Equal(new NepaliDate(2080, 1, 1), result[0].Start);
        Assert.Equal(new NepaliDate(2080, 1, 14), result[0].End);
    }

    // ---- GetEnumerator (foreach) ----

    [Fact]
    public void GetEnumerator_ThreeDayRange_YieldsExactlyThreeDates()
    {
        var range = new NepaliDateRange(new NepaliDate(2080, 5, 13), new NepaliDate(2080, 5, 15));
        var dates = new List<NepaliDate>();
        foreach (var d in range)
        {
            dates.Add(d);
        }

        Assert.Equal(3, dates.Count);
        Assert.Equal(new NepaliDate(2080, 5, 13), dates[0]);
        Assert.Equal(new NepaliDate(2080, 5, 14), dates[1]);
        Assert.Equal(new NepaliDate(2080, 5, 15), dates[2]);
    }

    [Fact]
    public void GetEnumerator_EmptyRange_YieldsNoDates()
    {
        var empty = new NepaliDateRange(new NepaliDate(2080, 5, 10), new NepaliDate(2080, 5, 1));
        var dates = empty.ToList(); // uses IEnumerable<NepaliDate>
        Assert.Empty(dates);
    }

    [Fact]
    public void GetEnumerator_SingleDayRange_YieldsOneDate()
    {
        var single = NepaliDateRange.SingleDay(new NepaliDate(2080, 5, 15));
        var dates = single.ToList();
        Assert.Single(dates);
        Assert.Equal(new NepaliDate(2080, 5, 15), dates[0]);
    }

    // ---- ToString ----

    [Fact]
    public void ToString_DefaultFormat_IsStartDashEnd()
    {
        var range = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 15));
        // NepaliDate.ToString() produces "YYYY/MM/DD"
        Assert.Equal("2080/01/01 - 2080/01/15", range.ToString());
    }

    [Fact]
    public void ToString_EmptyRange_ReturnsEmptyRangeString()
    {
        var empty = new NepaliDateRange(new NepaliDate(2080, 5, 10), new NepaliDate(2080, 5, 1));
        Assert.Equal("Empty Range", empty.ToString());
    }

    [Fact]
    public void ToString_WithDateFormat_UsesSpecifiedFormat()
    {
        var range = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 15));
        // DayMonthYear with Dash → "01-01-2080 - 15-01-2080"
        var result = range.ToString(DateFormats.DayMonthYear, Separators.Dash);
        Assert.Equal("01-01-2080 - 15-01-2080", result);
    }

    [Fact]
    public void ToString_WithDateFormat_EmptyRange_ReturnsEmptyRangeString()
    {
        var empty = new NepaliDateRange(new NepaliDate(2080, 5, 10), new NepaliDate(2080, 5, 1));
        Assert.Equal("Empty Range", empty.ToString(DateFormats.YearMonthDay, Separators.Dash));
    }

    // ---- Equals and GetHashCode ----

    [Fact]
    public void Equals_IdenticalRanges_ReturnsTrue()
    {
        var a = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 15));
        var b = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 15));
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_DifferentStart_ReturnsFalse()
    {
        var a = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 15));
        var b = new NepaliDateRange(new NepaliDate(2080, 1, 2), new NepaliDate(2080, 1, 15));
        Assert.False(a.Equals(b));
    }

    [Fact]
    public void Equals_DifferentEnd_ReturnsFalse()
    {
        var a = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 15));
        var b = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 16));
        Assert.False(a.Equals(b));
    }

    [Fact]
    public void Equals_BothEmpty_ReturnsTrue()
    {
        var empty1 = new NepaliDateRange(new NepaliDate(2080, 5, 10), new NepaliDate(2080, 5, 1));
        var empty2 = new NepaliDateRange(new NepaliDate(2079, 3, 10), new NepaliDate(2079, 3, 1));
        Assert.True(empty1.Equals(empty2));
    }

    [Fact]
    public void Equals_ObjectOverload_SameRange_ReturnsTrue()
    {
        var a = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 15));
        object b = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 15));
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_ObjectOverload_WrongType_ReturnsFalse()
    {
        var a = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 15));
        Assert.False(a.Equals("not a range"));
    }

    [Fact]
    public void GetHashCode_EqualRanges_SameHash()
    {
        var a = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 15));
        var b = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 15));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    // ---- Operators == and != ----

    [Fact]
    public void OperatorEquals_IdenticalRanges_ReturnsTrue()
    {
        var a = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 15));
        var b = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 15));
        Assert.True(a == b);
        Assert.False(a != b);
    }

    [Fact]
    public void OperatorNotEquals_DifferentRanges_ReturnsTrue()
    {
        var a = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 15));
        var b = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 1, 16));
        Assert.True(a != b);
        Assert.False(a == b);
    }
}
