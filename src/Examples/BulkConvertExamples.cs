using System.Diagnostics;

namespace NepDate.Examples;

/// <summary>
/// Examples demonstrating the BulkConvert functionality for batch processing of dates
/// </summary>
public class BulkConvertExamples
{
    public static void RunAllExamples()
    {
        Console.WriteLine("=== BulkConvert Functionality Examples ===\n");

        BasicBulkConversion();
        PerformanceComparison();
        BatchProcessing();
    }

    public static void BasicBulkConversion()
    {
        Console.WriteLine("--- Basic Bulk Conversion ---");

        // Create a list of English dates
        List<DateTime> englishDates =
        [
            new(2023, 4, 14),
            new(2023, 4, 15),
            new(2023, 4, 16),
            new(2023, 4, 17),
            new(2023, 4, 18)
        ];

        Console.WriteLine("Original English dates:");
        foreach (var date in englishDates)
        {
            Console.WriteLine($"  {date:yyyy-MM-dd}");
        }

        // Bulk convert to Nepali dates
        IEnumerable<NepaliDate> nepaliDates = NepaliDate.BulkConvert.ToNepaliDates(englishDates);

        Console.WriteLine("\nConverted to Nepali dates:");
        foreach (var date in nepaliDates)
        {
            Console.WriteLine($"  {date}");
        }

        // Convert back to English dates
        IEnumerable<DateTime> convertedBackDates = NepaliDate.BulkConvert.ToEnglishDates(nepaliDates);

        Console.WriteLine("\nConverted back to English dates:");
        foreach (var date in convertedBackDates)
        {
            Console.WriteLine($"  {date:yyyy-MM-dd}");
        }

        Console.WriteLine();
    }

    public static void PerformanceComparison()
    {
        Console.WriteLine("--- Performance Comparison ---");

        // Create a larger set of dates for performance testing
        int count = 1000;
        List<DateTime> englishDates = new(count);
        DateTime startDate = new(2023, 1, 1);

        for (int i = 0; i < count; i++)
        {
            englishDates.Add(startDate.AddDays(i));
        }

        Console.WriteLine($"Converting {count} dates using different methods:");

        // Method 1: Individual conversion
        Stopwatch sw = Stopwatch.StartNew();
        List<NepaliDate> individualConversion = new(count);

        foreach (var date in englishDates)
        {
            individualConversion.Add(new NepaliDate(date));
        }

        sw.Stop();
        Console.WriteLine($"Individual conversion: {sw.ElapsedMilliseconds} ms");

        // Method 2: Bulk conversion
        sw.Restart();
        var bulk = NepaliDate.BulkConvert.ToNepaliDates(englishDates).ToList();
        sw.Stop();
        Console.WriteLine($"Bulk conversion: {sw.ElapsedMilliseconds} ms");

        // Verify results are the same
        bool allSame = individualConversion.SequenceEqual(bulk);

        Console.WriteLine($"All results are identical: {allSame}");

        Console.WriteLine();
    }

    public static void BatchProcessing()
    {
        Console.WriteLine("--- Batch Processing ---");

        // Create a larger set of dates for batch processing
        int count = 10000;
        List<DateTime> englishDates = new(count);
        DateTime startDate = new(2023, 1, 1);

        for (int i = 0; i < count; i++)
        {
            englishDates.Add(startDate.AddDays(i));
        }

        Console.WriteLine($"Processing {count} dates in batches:");

        // Process in batches of 2500
        int batchSize = 2500;
        int totalProcessed = 0;

        Stopwatch sw = Stopwatch.StartNew();

        for (int i = 0; i < count; i += batchSize)
        {
            int currentBatchSize = Math.Min(batchSize, count - i);
            var batch = englishDates.GetRange(i, currentBatchSize);

            // Process the batch
            var nepaliDates = NepaliDate.BulkConvert.ToNepaliDates(batch).ToList();

            totalProcessed += nepaliDates.Count;
            Console.WriteLine($"  Processed batch {i / batchSize + 1}: {nepaliDates.Count} dates");
        }

        sw.Stop();
        Console.WriteLine($"Total processed: {totalProcessed} dates in {sw.ElapsedMilliseconds} ms");

        Console.WriteLine();
    }
}