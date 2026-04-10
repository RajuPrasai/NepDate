
namespace NepDate.Examples;

/// <summary>
/// Examples demonstrating the functionality of the SmartDateParser class
/// </summary>
public class SmartDateParserExamples
{
    public static void RunAllExamples()
    {
        Console.WriteLine("=== SmartDateParser Functionality Examples ===\n");

        BasicParsing();
        ParseVariousFormats();
        ParseWithMonthNames();
        ParseWithAmbiguity();
        ParseWithDefaults();
    }

    public static void BasicParsing()
    {
        Console.WriteLine("--- Basic Date Parsing ---");

        // Basic string formats
        string[] dateStrings = {
            "2080/01/15",
            "2080-01-15",
            "15/01/2080",
            "15-01-2080"
        };

        foreach (var dateStr in dateStrings)
        {
            if (SmartDateParser.TryParse(dateStr, out NepaliDate result))
            {
                Console.WriteLine($"Successfully parsed '{dateStr}' to {result}");
            }
            else
            {
                Console.WriteLine($"Failed to parse '{dateStr}'");
            }
        }

        // Parse with exception handling
        try
        {
            NepaliDate date = SmartDateParser.Parse("2080/01/15");
            Console.WriteLine($"Parsed with direct method: {date}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Parse exception: {ex.Message}");
        }

        Console.WriteLine();
    }

    public static void ParseVariousFormats()
    {
        Console.WriteLine("--- Parsing Various Formats ---");

        // Different separators and formats
        string[] dateFormats = {
            "2080.1.15",           // Period separator
            "2080,1,15",           // Comma separator
            "2080 1 15",           // Space separator
            "15.1.2080",           // Day first with period
            "15 Baisakh 2080",     // With month name
            "15th Baisakh, 2080",  // With ordinal and month name
            "2080 Baisakh 15",     // Year first with month name
            "Baisakh 15, 2080"     // Month first format
        };

        foreach (var format in dateFormats)
        {
            if (SmartDateParser.TryParse(format, out NepaliDate result))
            {
                Console.WriteLine($"Successfully parsed '{format}' to {result}");
            }
            else
            {
                Console.WriteLine($"Failed to parse '{format}'");
            }
        }

        Console.WriteLine();
    }

    public static void ParseWithMonthNames()
    {
        Console.WriteLine("--- Parsing with Month Names ---");

        // Different month name variations (for Baisakh - Month 1)
        string[] monthVariations = {
            "15 Baisakh 2080",
            "15 Baishakh 2080",
            "15 Baisak 2080",
            "15 Vaisakh 2080",
            "15 वैशाख 2080"  // Unicode Nepali
        };

        foreach (var dateStr in monthVariations)
        {
            if (SmartDateParser.TryParse(dateStr, out NepaliDate result))
            {
                Console.WriteLine($"Successfully parsed '{dateStr}' to {result}");
            }
            else
            {
                Console.WriteLine($"Failed to parse '{dateStr}'");
            }
        }

        Console.WriteLine();
    }

    public static void ParseWithAmbiguity()
    {
        Console.WriteLine("--- Parsing with Ambiguity Resolution ---");

        // Ambiguous formats where the parser must determine if month or day comes first
        string[] ambiguousFormats = {
            "1/2/2080",    // Could be Jan 2 or Feb 1
            "2/1/2080",    // Could be Feb 1 or Jan 2
            "15/12/2080",  // Less ambiguous (day > 12)
            "12/10/2080"   // Both could be month or day
        };

        // Default behavior (MDY preference)
        Console.WriteLine("Default behavior (MDY preference):");
        foreach (var format in ambiguousFormats)
        {
            if (SmartDateParser.TryParse(format, out NepaliDate result))
            {
                Console.WriteLine($"'{format}' → {result}");
            }
        }

        Console.WriteLine();
    }

    public static void ParseWithDefaults()
    {
        Console.WriteLine("--- Parsing with Default Values ---");

        // Current Nepali date for reference
        NepaliDate today = new(DateTime.Now);
        Console.WriteLine($"Current Nepali date: {today}");

        // Parse with missing components that will use defaults
        string[] incompleteFormats = {
            "15",            // Just the day (uses current month and year)
            "Baisakh",       // Just the month (uses 1st day and current year)
            "Baisakh 15",    // Month and day (uses current year)
            "2080"           // Just the year (uses 1st day of 1st month)
        };

        foreach (var format in incompleteFormats)
        {
            if (SmartDateParser.TryParse(format, out NepaliDate result))
            {
                Console.WriteLine($"Parsed incomplete format '{format}' → {result}");
            }
            else
            {
                Console.WriteLine($"Failed to parse '{format}'");
            }
        }

        Console.WriteLine();
    }
}