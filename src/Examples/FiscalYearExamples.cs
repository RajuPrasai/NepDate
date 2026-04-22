
namespace NepDate.Examples;

/// <summary>
/// Examples demonstrating the Fiscal Year functionality of the NepaliDate struct
/// </summary>
public class FiscalYearExamples
{
    public static void RunAllExamples()
    {
        Console.WriteLine("=== Fiscal Year Functionality Examples ===\n");

        FiscalYearBasics();
        FiscalYearDates();
        FiscalYearQuarters();
    }

    public static void FiscalYearBasics()
    {
        Console.WriteLine("--- Fiscal Year Basics ---");

        // Current date in Nepali calendar
        NepaliDate today = new(DateTime.Now);
        Console.WriteLine($"Today in Nepali calendar: {today}");

        // Get current fiscal year
        int fiscalYear = today.FiscalYearStartDate().Year;
        Console.WriteLine($"Current fiscal year: {fiscalYear}");

        Console.WriteLine();
    }

    public static void FiscalYearDates()
    {
        Console.WriteLine("--- Fiscal Year Start and End Dates ---");

        NepaliDate date = new(2080, 5, 15); // Bhadra (month 5)

        // Get fiscal year start and end dates
        var (fyStart, fyEnd) = date.FiscalYearStartAndEndDate();
        Console.WriteLine($"Fiscal year for {date}: {fyStart} to {fyEnd}");

        // Get previous fiscal year
        var (prevFyStart, prevFyEnd) = date.FiscalYearStartAndEndDate(-1);
        Console.WriteLine($"Previous fiscal year: {prevFyStart} to {prevFyEnd}");

        // Get next fiscal year
        var (nextFyStart, nextFyEnd) = date.FiscalYearStartAndEndDate(1);
        Console.WriteLine($"Next fiscal year: {nextFyStart} to {nextFyEnd}");

        Console.WriteLine();
    }

    public static void FiscalYearQuarters()
    {
        Console.WriteLine("--- Fiscal Year Quarters ---");

        // First Quarter (Shrawan, Bhadra, Ashwin - months 4, 5, 6)
        NepaliDate q1Date = new(2080, 4, 15);
        var (q1Start, q1End) = q1Date.FiscalYearQuarterStartAndEndDate();
        Console.WriteLine($"First Quarter ({q1Date}): {q1Start} to {q1End}");

        // Second Quarter (Kartik, Mangsir, Poush - months 7, 8, 9)
        NepaliDate q2Date = new(2080, 7, 15);
        var (q2Start, q2End) = q2Date.FiscalYearQuarterStartAndEndDate();
        Console.WriteLine($"Second Quarter ({q2Date}): {q2Start} to {q2End}");

        // Third Quarter (Magh, Falgun, Chaitra - months 10, 11, 12)
        NepaliDate q3Date = new(2080, 10, 15);
        var (q3Start, q3End) = q3Date.FiscalYearQuarterStartAndEndDate();
        Console.WriteLine($"Third Quarter ({q3Date}): {q3Start} to {q3End}");

        // Fourth Quarter (Baisakh, Jestha, Ashadh - months 1, 2, 3)
        NepaliDate q4Date = new(2080, 1, 15);
        var (q4Start, q4End) = q4Date.FiscalYearQuarterStartAndEndDate();
        Console.WriteLine($"Fourth Quarter ({q4Date}): {q4Start} to {q4End}");

        Console.WriteLine();
    }
}