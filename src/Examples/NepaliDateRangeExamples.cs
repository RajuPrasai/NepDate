
namespace NepDate.Examples;

/// <summary>
/// Examples demonstrating the functionality of the NepaliDateRange struct
/// </summary>
public class NepaliDateRangeExamples
{
    public static void RunAllExamples()
    {
        Console.WriteLine("=== NepaliDateRange Functionality Examples ===\n");

        CreateDateRanges();
        RangeProperties();
        DateRangeOperations();
        IteratingRanges();
        RangeSplitting();
    }

    public static void CreateDateRanges()
    {
        Console.WriteLine("--- Creating NepaliDateRange Objects ---");

        // Create from start and end dates
        NepaliDate startDate = new(2080, 1, 1);
        NepaliDate endDate = new(2080, 1, 15);
        NepaliDateRange range = new(startDate, endDate);
        Console.WriteLine($"Range from start/end dates: {range}");

        // Create for a specific month
        NepaliDateRange monthRange = NepaliDateRange.ForMonth(2080, 1);
        Console.WriteLine($"Range for month (2080-01): {monthRange}");

        // Create for a fiscal year
        NepaliDateRange fiscalYearRange = NepaliDateRange.ForFiscalYear(2080);
        Console.WriteLine($"Range for fiscal year 2080: {fiscalYearRange}");

        Console.WriteLine();
    }

    public static void RangeProperties()
    {
        Console.WriteLine("--- NepaliDateRange Properties ---");

        NepaliDate startDate = new(2080, 1, 1);
        NepaliDate endDate = new(2080, 1, 30);
        NepaliDateRange range = new(startDate, endDate);

        Console.WriteLine($"Range: {range}");
        Console.WriteLine($"Start date: {range.Start}");
        Console.WriteLine($"End date: {range.End}");
        Console.WriteLine($"Length in days: {range.Length}");
        Console.WriteLine($"Is empty: {range.IsEmpty}");

        // Empty range
        NepaliDateRange emptyRange = new(endDate, startDate); // end before start = empty
        Console.WriteLine($"Empty range: {emptyRange}");
        Console.WriteLine($"Is empty: {emptyRange.IsEmpty}");
        Console.WriteLine($"Length: {emptyRange.Length}");

        Console.WriteLine();
    }

    public static void DateRangeOperations()
    {
        Console.WriteLine("--- NepaliDateRange Operations ---");

        // Create two overlapping ranges
        NepaliDateRange range1 = new(
            new NepaliDate(2080, 1, 1),
            new NepaliDate(2080, 1, 15)
        );

        NepaliDateRange range2 = new(
            new NepaliDate(2080, 1, 10),
            new NepaliDate(2080, 1, 25)
        );

        Console.WriteLine($"Range 1: {range1}");
        Console.WriteLine($"Range 2: {range2}");

        // Intersection
        NepaliDateRange intersection = range1.Intersect(range2);
        Console.WriteLine($"Intersection: {intersection}");

        // Union
        NepaliDateRange union = range1.Union(range2);
        Console.WriteLine($"Union: {union}");

        // Contains check
        NepaliDate testDate = new(2080, 1, 12);
        Console.WriteLine($"Range 1 contains {testDate}: {range1.Contains(testDate)}");
        Console.WriteLine($"Range 1 contains range2: {range1.Contains(range2)}");

        // Overlaps check
        Console.WriteLine($"Range 1 overlaps range2: {range1.Overlaps(range2)}");

        // Non-overlapping range
        NepaliDateRange range3 = new(
            new NepaliDate(2080, 2, 1),
            new NepaliDate(2080, 2, 15)
        );
        Console.WriteLine($"Range 1 overlaps range3 ({range3}): {range1.Overlaps(range3)}");

        Console.WriteLine();
    }

    public static void IteratingRanges()
    {
        Console.WriteLine("--- Iterating Through NepaliDateRange ---");

        // Create a small range for demonstration
        NepaliDateRange range = new(
            new NepaliDate(2080, 1, 1),
            new NepaliDate(2080, 1, 5)
        );

        Console.WriteLine($"Iterating through range {range}:");

        // Method 1: Using foreach
        Console.WriteLine("Using foreach:");
        foreach (NepaliDate date in range)
        {
            Console.WriteLine($"  {date}");
        }

        // Method 2: Using LINQ
        Console.WriteLine("Using LINQ Take(3):");
        foreach (NepaliDate date in range.Take(3))
        {
            Console.WriteLine($"  {date}");
        }

        Console.WriteLine();
    }

    public static void RangeSplitting()
    {
        Console.WriteLine("--- Splitting NepaliDateRange ---");

        // Create a range spanning multiple months
        NepaliDateRange multiMonthRange = new(
            new NepaliDate(2080, 1, 15),
            new NepaliDate(2080, 3, 15)
        );

        Console.WriteLine($"Original multi-month range: {multiMonthRange}");

        // Split by month
        Console.WriteLine("Split by month:");
        foreach (var monthRange in multiMonthRange.SplitByMonth())
        {
            Console.WriteLine($"  {monthRange}");
        }

        // Create a range spanning multiple fiscal years
        NepaliDateRange multiFYRange = new(
            new NepaliDate(2079, 12, 15),
            new NepaliDate(2080, 5, 15)
        );

        Console.WriteLine($"Original multi-fiscal year range: {multiFYRange}");

        Console.WriteLine();
    }
}