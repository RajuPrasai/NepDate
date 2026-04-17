
namespace NepDate.Examples;

/// <summary>
/// Examples demonstrating the basic functionality of the NepaliDate struct
/// </summary>
public class NepaliDateExamples
{
    public static void RunAllExamples()
    {
        Console.WriteLine("=== NepaliDate Basic Functionality Examples ===\n");

        CreateNepaliDates();
        ConvertBetweenCalendars();
        DateComparisons();
        DateProperties();
        DateManipulation();
        FormatAndParse();
    }

    public static void CreateNepaliDates()
    {
        Console.WriteLine("--- Creating NepaliDate Objects ---");

        // Create from Nepali date components
        NepaliDate fromComponents = new(2080, 1, 15);
        Console.WriteLine($"From components (2080, 1, 15): {fromComponents}");

        // Create from current DateTime
        NepaliDate today = new(DateTime.Now);
        Console.WriteLine($"From today's date: {today}");

        // Create from string
        NepaliDate fromString = new("2080/01/15");
        Console.WriteLine($"From string '2080/01/15': {fromString}");

        // Min and Max values
        Console.WriteLine($"Minimum supported date: {NepaliDate.MinValue}");
        Console.WriteLine($"Maximum supported date: {NepaliDate.MaxValue}");

        Console.WriteLine();
    }

    public static void ConvertBetweenCalendars()
    {
        Console.WriteLine("--- Converting Between Calendar Systems ---");

        // Nepali to English conversion
        NepaliDate nepDate = new(2080, 1, 1);
        DateTime engDate = nepDate.EnglishDate;
        Console.WriteLine($"Nepali date {nepDate} = English date {engDate:yyyy-MM-dd}");

        // English to Nepali conversion
        DateTime someEnglishDate = new(2023, 5, 15);
        NepaliDate convertedNepDate = new(someEnglishDate);
        Console.WriteLine($"English date {someEnglishDate:yyyy-MM-dd} = Nepali date {convertedNepDate}");

        Console.WriteLine();
    }

    public static void DateComparisons()
    {
        Console.WriteLine("--- Date Comparisons ---");

        NepaliDate date1 = new(2080, 1, 1);
        NepaliDate date2 = new(2080, 1, 15);
        NepaliDate date3 = new(2080, 1, 1);

        Console.WriteLine($"{date1} == {date3}: {date1 == date3}");
        Console.WriteLine($"{date1} != {date2}: {date1 != date2}");
        Console.WriteLine($"{date1} < {date2}: {date1 < date2}");
        Console.WriteLine($"{date2} > {date1}: {date2 > date1}");

        Console.WriteLine($"Comparing {date1} and {date2}: {date1.CompareTo(date2)}");
        Console.WriteLine($"Comparing {date2} and {date1}: {date2.CompareTo(date1)}");
        Console.WriteLine($"Comparing {date1} and {date3}: {date1.CompareTo(date3)}");

        Console.WriteLine();
    }

    public static void DateProperties()
    {
        Console.WriteLine("--- Date Properties ---");

        NepaliDate date = new(2080, 5, 15);

        Console.WriteLine($"Year: {date.Year}");
        Console.WriteLine($"Month: {date.Month}");
        Console.WriteLine($"Day: {date.Day}");
        Console.WriteLine($"Day of Week: {date.DayOfWeek}");
        Console.WriteLine($"Day of Year: {date.DayOfYear}");
        Console.WriteLine($"Month End Day: {date.MonthEndDay}");
        Console.WriteLine($"Is Leap Year: {date.IsLeapYear()}");
        Console.WriteLine($"Is Today: {date.IsToday()}");

        // Calendar data (available for 2001-2089 BS)
        NepaliDate calDate = new(2081, 1, 1);
        var calInfo = calDate.GetCalendarInfo();
        Console.WriteLine($"\nCalendar data for {calDate}:");
        Console.WriteLine($"  Tithi:             {calDate.TithiNp} ({calDate.TithiEn})");
        Console.WriteLine($"  Is Public Holiday: {calDate.IsPublicHoliday}");
        Console.WriteLine($"  Events (Np):       {string.Join(", ", calInfo.EventsNp)}");
        Console.WriteLine($"  Events (En):       {string.Join(", ", calInfo.EventsEn)}");

        Console.WriteLine();
    }

    public static void DateManipulation()
    {
        Console.WriteLine("--- Date Manipulation ---");

        NepaliDate date = new(2080, 1, 15);

        // Adding days, months, years
        Console.WriteLine($"Original date: {date}");
        Console.WriteLine($"Add 10 days: {date.AddDays(10)}");
        Console.WriteLine($"Add 2 months: {date.AddMonths(2)}");
        Console.WriteLine($"Add 1 year: {date.AddMonths(12)}");

        // Get month end date
        Console.WriteLine($"Month end date: {date.MonthEndDate()}");

        // Calculating difference between dates
        NepaliDate laterDate = date.AddDays(30);
        TimeSpan diff = laterDate.Subtract(date);
        Console.WriteLine($"Difference between {date} and {laterDate}: {diff.Days} days");

        Console.WriteLine();
    }

    public static void FormatAndParse()
    {
        Console.WriteLine("--- Formatting and Parsing ---");

        NepaliDate date = new(2080, 1, 15);

        // Different formats
        Console.WriteLine($"Default format: {date}");
        Console.WriteLine($"yyyy/MM/dd format: {date.ToString(DateFormats.YearMonthDay)}");
        Console.WriteLine($"dd-MM-yyyy format: {date.ToString(DateFormats.DayMonthYear)}");

        // Parsing from string
        if (NepaliDate.TryParse("2080/02/15", out NepaliDate parsedDate))
        {
            Console.WriteLine($"Successfully parsed '2080/02/15' to {parsedDate}");
        }

        Console.WriteLine();
    }
}