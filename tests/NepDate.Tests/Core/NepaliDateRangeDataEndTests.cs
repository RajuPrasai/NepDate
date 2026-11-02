namespace NepDate.Tests.Core;

/// <summary>
/// Covers ranges that touch the ends of the supported date range. Moving past
/// 2199-12-30 or before 1901-01-01 is never needed to produce the result, so these
/// operations must not throw for a valid range.
/// </summary>
public class NepaliDateRangeDataEndTests
{
    private static readonly NepaliDate FirstDate = new NepaliDate(1901, 1, 1);
    private static readonly NepaliDate LastDate = new NepaliDate(2199, 12, 30);

    // ---- SplitByMonth ----

    [Fact]
    public void SplitByMonth_RangeEndingAtLastDate_ReturnsLastPieceEndingAtRangeEnd()
    {
        var range = new NepaliDateRange(new NepaliDate(2199, 11, 30), LastDate);

        var parts = range.SplitByMonth();

        Assert.Equal(2, parts.Length);
        Assert.Equal(new NepaliDate(2199, 11, 30), parts[0].Start);
        Assert.Equal(new NepaliDate(2199, 11, 30), parts[0].End);
        Assert.Equal(new NepaliDate(2199, 12, 1), parts[1].Start);
        Assert.Equal(LastDate, parts[1].End);
    }

    // ---- SplitByFiscalQuarter ----

    [Fact]
    public void SplitByFiscalQuarter_RangeEndingAtLastDate_ReturnsSingleRange()
    {
        var range = new NepaliDateRange(new NepaliDate(2199, 12, 25), LastDate);

        var parts = range.SplitByFiscalQuarter();

        Assert.Single(parts);
        Assert.Equal(range.Start, parts[0].Start);
        Assert.Equal(LastDate, parts[0].End);
    }

    // ---- GetEnumerator ----

    [Fact]
    public void GetEnumerator_SingleDayAtLastDate_YieldsThatDate()
    {
        var range = NepaliDateRange.SingleDay(LastDate);

        var dates = range.ToList();

        Assert.Single(dates);
        Assert.Equal(LastDate, dates[0]);
    }

    // ---- WorkingDays ----

    [Fact]
    public void WorkingDays_SingleDayAtLastDate_YieldsThatDate()
    {
        var range = NepaliDateRange.SingleDay(LastDate);

        var dates = range.WorkingDays().ToList();

        Assert.Single(dates);
        Assert.Equal(LastDate, dates[0]);
    }

    // ---- WeekendDays ----

    [Fact]
    public void WeekendDays_SingleDayAtLastDate_YieldsNothing()
    {
        var range = NepaliDateRange.SingleDay(LastDate);

        Assert.Empty(range.WeekendDays().ToList());
    }

    // ---- DatesWithInterval ----

    [Fact]
    public void DatesWithInterval_IntervalOfOne_SingleDayAtLastDate_YieldsThatDate()
    {
        var range = NepaliDateRange.SingleDay(LastDate);

        var dates = range.DatesWithInterval(1).ToList();

        Assert.Single(dates);
        Assert.Equal(LastDate, dates[0]);
    }

    [Fact]
    public void DatesWithInterval_NextIntervalWouldPassLastDate_StopsAtLastDate()
    {
        var range = new NepaliDateRange(new NepaliDate(2199, 12, 25), LastDate);

        var dates = range.DatesWithInterval(3).ToList();

        Assert.Equal(2, dates.Count);
        Assert.Equal(new NepaliDate(2199, 12, 25), dates[0]);
        Assert.Equal(new NepaliDate(2199, 12, 28), dates[1]);
    }

    // ---- IsAdjacentTo ----

    [Fact]
    public void IsAdjacentTo_ThisRangeEndsAtLastDate_ReturnsFalse()
    {
        var range = new NepaliDateRange(new NepaliDate(2199, 12, 25), LastDate);
        var other = new NepaliDateRange(new NepaliDate(2081, 4, 10), new NepaliDate(2081, 4, 20));

        Assert.False(range.IsAdjacentTo(other));
    }

    [Fact]
    public void IsAdjacentTo_OtherRangeEndsAtLastDate_ReturnsFalse()
    {
        var range = new NepaliDateRange(new NepaliDate(2081, 4, 10), new NepaliDate(2081, 4, 20));
        var other = new NepaliDateRange(new NepaliDate(2199, 12, 25), LastDate);

        Assert.False(range.IsAdjacentTo(other));
    }

    [Fact]
    public void IsAdjacentTo_ThisRangeStartsAtFirstDate_ReturnsFalse()
    {
        var range = new NepaliDateRange(FirstDate, new NepaliDate(1901, 1, 5));
        var other = new NepaliDateRange(new NepaliDate(2081, 4, 10), new NepaliDate(2081, 4, 20));

        Assert.False(range.IsAdjacentTo(other));
    }

    // ---- Intersect ----

    [Fact]
    public void Intersect_DisjointRanges_ReceiverStartsAtFirstDate_ReturnsEmptyRange()
    {
        var range = new NepaliDateRange(FirstDate, new NepaliDate(1901, 1, 5));
        var other = new NepaliDateRange(new NepaliDate(2081, 4, 10), new NepaliDate(2081, 4, 20));

        var intersection = range.Intersect(other);

        Assert.True(intersection.IsEmpty);
        Assert.Equal(0, intersection.Length);
        Assert.Equal("Empty Range", intersection.ToString());
    }
}
