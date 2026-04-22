using Xunit.Abstractions;

namespace NepDate.Tests.Core;

public class NepaliDateRangeTests
{
    private readonly ITestOutputHelper _output;

    public NepaliDateRangeTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Constructor_ValidDates_CreatesRange()
    {
        var start = new NepaliDate(2080, 1, 1);
        var end = new NepaliDate(2080, 1, 15);

        var range = new NepaliDateRange(start, end);

        Assert.Equal(start, range.Start);
        Assert.Equal(end, range.End);
        Assert.False(range.IsEmpty);
        Assert.Equal(15, range.Length);
    }

    [Fact]
    public void Constructor_EndBeforeStart_CreatesEmptyRange()
    {
        var start = new NepaliDate(2080, 1, 15);
        var end = new NepaliDate(2080, 1, 1);

        var range = new NepaliDateRange(start, end);

        Assert.True(range.IsEmpty);
        Assert.Equal(0, range.Length);
    }

    [Fact]
    public void SingleDay_CreatesRangeWithOneDay()
    {
        var date = new NepaliDate(2080, 1, 1);

        var range = NepaliDateRange.SingleDay(date);

        Assert.Equal(date, range.Start);
        Assert.Equal(date, range.End);
        Assert.Equal(1, range.Length);
    }

    [Fact]
    public void FromDayCount_CreatesCorrectRange()
    {
        var start = new NepaliDate(2080, 1, 1);
        const int days = 10;

        var range = NepaliDateRange.FromDayCount(start, days);

        Assert.Equal(start, range.Start);
        Assert.Equal(start.AddDays(days - 1), range.End);
        Assert.Equal(days, range.Length);
    }

    [Fact]
    public void ForMonth_CreatesCorrectRange()
    {
        const int year = 2080;
        const int month = 1; // Baisakh

        var range = NepaliDateRange.ForMonth(year, month);

        Assert.Equal(new NepaliDate(year, month, 1), range.Start);
        var monthEndDay = new NepaliDate(year, month, 1).MonthEndDay;
        Assert.Equal(new NepaliDate(year, month, monthEndDay), range.End);

        _output.WriteLine($"Month: {year}/{month} has {monthEndDay} days");
        _output.WriteLine($"Range: {range}");
    }

    [Fact]
    public void ForFiscalYear_CreatesCorrectRange()
    {
        const int fiscalYear = 2080;

        var range = NepaliDateRange.ForFiscalYear(fiscalYear);

        Assert.Equal(new NepaliDate(fiscalYear, 4, 1), range.Start); // 1 Shrawan
        Assert.Equal(new NepaliDate(fiscalYear + 1, 3, 1).MonthEndDate(), range.End); // Last day of Ashadh

        _output.WriteLine($"Fiscal Year {fiscalYear} range: {range}");
        _output.WriteLine($"Length: {range.Length} days");
    }

    [Fact]
    public void Contains_DateInRange_ReturnsTrue()
    {
        var start = new NepaliDate(2080, 1, 1);
        var end = new NepaliDate(2080, 1, 15);
        var range = new NepaliDateRange(start, end);
        var date = new NepaliDate(2080, 1, 10);

        var contains = range.Contains(date);

        Assert.True(contains);
    }

    [Fact]
    public void Contains_DateOutsideRange_ReturnsFalse()
    {
        var start = new NepaliDate(2080, 1, 1);
        var end = new NepaliDate(2080, 1, 15);
        var range = new NepaliDateRange(start, end);
        var date = new NepaliDate(2080, 1, 20);

        var contains = range.Contains(date);

        Assert.False(contains);
    }

    [Fact]
    public void Contains_RangeFullyContained_ReturnsTrue()
    {
        var outerRange = new NepaliDateRange(
            new NepaliDate(2080, 1, 1),
            new NepaliDate(2080, 1, 30));

        var innerRange = new NepaliDateRange(
            new NepaliDate(2080, 1, 10),
            new NepaliDate(2080, 1, 20));

        var contains = outerRange.Contains(innerRange);

        Assert.True(contains);
    }

    [Fact]
    public void Overlaps_RangesOverlap_ReturnsTrue()
    {
        var range1 = new NepaliDateRange(
            new NepaliDate(2080, 1, 1),
            new NepaliDate(2080, 1, 15));

        var range2 = new NepaliDateRange(
            new NepaliDate(2080, 1, 10),
            new NepaliDate(2080, 1, 20));

        var overlaps = range1.Overlaps(range2);

        Assert.True(overlaps);
    }

    [Fact]
    public void Overlaps_RangesDontOverlap_ReturnsFalse()
    {
        var range1 = new NepaliDateRange(
            new NepaliDate(2080, 1, 1),
            new NepaliDate(2080, 1, 10));

        var range2 = new NepaliDateRange(
            new NepaliDate(2080, 1, 15),
            new NepaliDate(2080, 1, 20));

        var overlaps = range1.Overlaps(range2);

        Assert.False(overlaps);
    }

    [Fact]
    public void IsAdjacentTo_AdjacentRanges_ReturnsTrue()
    {
        var range1 = new NepaliDateRange(
            new NepaliDate(2080, 1, 1),
            new NepaliDate(2080, 1, 10));

        var range2 = new NepaliDateRange(
            new NepaliDate(2080, 1, 11),
            new NepaliDate(2080, 1, 20));

        var isAdjacent = range1.IsAdjacentTo(range2);

        Assert.True(isAdjacent);
    }

    [Fact]
    public void Intersect_OverlappingRanges_ReturnsIntersection()
    {
        var range1 = new NepaliDateRange(
            new NepaliDate(2080, 1, 1),
            new NepaliDate(2080, 1, 15));

        var range2 = new NepaliDateRange(
            new NepaliDate(2080, 1, 10),
            new NepaliDate(2080, 1, 20));

        var intersection = range1.Intersect(range2);

        Assert.Equal(new NepaliDate(2080, 1, 10), intersection.Start);
        Assert.Equal(new NepaliDate(2080, 1, 15), intersection.End);
        Assert.Equal(6, intersection.Length); // 10, 11, 12, 13, 14, 15 (inclusive)

        _output.WriteLine($"Range 1: {range1}");
        _output.WriteLine($"Range 2: {range2}");
        _output.WriteLine($"Intersection: {intersection}");
    }

    [Fact]
    public void Union_OverlappingRanges_ReturnsUnion()
    {
        var range1 = new NepaliDateRange(
            new NepaliDate(2080, 1, 1),
            new NepaliDate(2080, 1, 15));

        var range2 = new NepaliDateRange(
            new NepaliDate(2080, 1, 10),
            new NepaliDate(2080, 1, 20));

        var union = range1.Union(range2);

        Assert.Equal(new NepaliDate(2080, 1, 1), union.Start);
        Assert.Equal(new NepaliDate(2080, 1, 20), union.End);

        _output.WriteLine($"Range 1: {range1}");
        _output.WriteLine($"Range 2: {range2}");
        _output.WriteLine($"Union: {union}");
    }

    [Fact]
    public void Except_RemovingRangeFromMiddle_ReturnsTwoRanges()
    {
        var fullRange = new NepaliDateRange(
            new NepaliDate(2080, 1, 1),
            new NepaliDate(2080, 1, 30));

        var middleRange = new NepaliDateRange(
            new NepaliDate(2080, 1, 10),
            new NepaliDate(2080, 1, 20));

        var result = fullRange.Except(middleRange);

        Assert.Equal(2, result.Length);

        var firstPart = result[0];
        var secondPart = result[1];

        Assert.Equal(new NepaliDate(2080, 1, 1), firstPart.Start);
        Assert.Equal(new NepaliDate(2080, 1, 9), firstPart.End);

        Assert.Equal(new NepaliDate(2080, 1, 21), secondPart.Start);
        Assert.Equal(new NepaliDate(2080, 1, 30), secondPart.End);

        _output.WriteLine($"Full Range: {fullRange}");
        _output.WriteLine($"Range to remove: {middleRange}");
        _output.WriteLine($"Result 1: {result[0]}");
        _output.WriteLine($"Result 2: {result[1]}");
    }

    [Fact]
    public void SplitByMonth_RangeAcrossMonths_SplitsCorrectly()
    {
        var range = new NepaliDateRange(
            new NepaliDate(2080, 1, 15), // 15 Baisakh
            new NepaliDate(2080, 3, 10)); // 10 Ashadh

        var monthRanges = range.SplitByMonth();

        Assert.Equal(3, monthRanges.Length); // Parts of Baisakh, Jestha, and Ashadh

        // First range should be 15-end of Baisakh
        Assert.Equal(new NepaliDate(2080, 1, 15), monthRanges[0].Start);
        Assert.Equal(new NepaliDate(2080, 1, 1).MonthEndDate(), monthRanges[0].End);

        // Second range should be all of Jestha
        Assert.Equal(new NepaliDate(2080, 2, 1), monthRanges[1].Start);
        Assert.Equal(new NepaliDate(2080, 2, 1).MonthEndDate(), monthRanges[1].End);

        // Third range should be 1-10 Ashadh
        Assert.Equal(new NepaliDate(2080, 3, 1), monthRanges[2].Start);
        Assert.Equal(new NepaliDate(2080, 3, 10), monthRanges[2].End);

        foreach (var monthRange in monthRanges)
        {
            _output.WriteLine($"Month range: {monthRange}");
        }
    }

    [Fact]
    public void WorkingDays_ReturnsCorrectDays()
    {
        var startDate = new NepaliDate(2080, 1, 1);
        int weekendDaysExpected = 0;

        // Find what day of the week is the start date
        DayOfWeek startDayOfWeek = startDate.DayOfWeek;
        _output.WriteLine($"Start date is {startDayOfWeek}");

        // Count how many Saturdays are in the next 14 days
        for (int i = 0; i < 14; i++)
        {
            if (((int)startDayOfWeek + i) % 7 == (int)DayOfWeek.Saturday)
            {
                weekendDaysExpected++;
            }
        }

        var range = NepaliDateRange.FromDayCount(startDate, 14);

        var workingDays = range.WorkingDays().ToList();

        Assert.Equal(14 - weekendDaysExpected, workingDays.Count);
        Assert.All(workingDays, day => Assert.NotEqual(DayOfWeek.Saturday, day.DayOfWeek));

        _output.WriteLine($"Range: {range}");
        _output.WriteLine($"Working days: {workingDays.Count}");
        foreach (var day in workingDays)
        {
            _output.WriteLine($"{day} - {day.DayOfWeek}");
        }
    }

    [Fact]
    public void Enumeration_EnumeratesAllDaysInRange()
    {
        var start = new NepaliDate(2080, 1, 1);
        var end = new NepaliDate(2080, 1, 10);
        var range = new NepaliDateRange(start, end);

        var days = range.ToList();

        Assert.Equal(10, days.Count);
        for (int i = 0; i < 10; i++)
        {
            Assert.Equal(new NepaliDate(2080, 1, i + 1), days[i]);
        }
    }

    [Fact]
    public void Demo_RangeOperations()
    {
        // This test demonstrates how the NepaliDateRange can be used
        // for real-world scenarios

        // Create a fiscal year range
        var fiscalYear = NepaliDateRange.ForFiscalYear(2080);
        _output.WriteLine($"Fiscal Year 2080: {fiscalYear}");
        _output.WriteLine($"Length: {fiscalYear.Length} days");

        // Get the first quarter
        var firstQuarter = fiscalYear.SplitByFiscalQuarter()[0];
        _output.WriteLine($"First Quarter: {firstQuarter}");

        // Get all the weekends in the first quarter
        var weekends = firstQuarter.WeekendDays().ToList();
        _output.WriteLine($"Number of weekends in first quarter: {weekends.Count}");

        // Calculate working days in the first quarter
        var workingDays = firstQuarter.WorkingDays().ToList();
        _output.WriteLine($"Number of working days in first quarter: {workingDays.Count}");

        // Create a custom date range for a project
        var projectStart = new NepaliDate(2080, 4, 15); // 15 Shrawan
        var projectEnd = new NepaliDate(2080, 7, 30);   // 30 Kartik
        var projectRange = new NepaliDateRange(projectStart, projectEnd);
        _output.WriteLine($"Project duration: {projectRange.Length} days");

        // Check if the project spans across the festival season
        var dashainRange = new NepaliDateRange(
            new NepaliDate(2080, 6, 25), // Approximate Dashain start
            new NepaliDate(2080, 7, 5));  // Approximate Dashain end

        if (projectRange.Overlaps(dashainRange))
        {
            _output.WriteLine("Project timeline includes Dashain festival!");

            // Calculate workdays excluding the festival period
            var workdaysExcludingFestival = projectRange.Except(dashainRange)
                .SelectMany(r => r.WorkingDays())
                .Count();

            _output.WriteLine($"Actual working days in project (excluding Dashain): {workdaysExcludingFestival}");
        }
    }
}