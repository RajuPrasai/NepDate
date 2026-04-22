using System.Diagnostics;

namespace NepDate.Tests.Integration;

public class MemoryOptimizationBenchmark
{
    // Count of conversions to perform in each test
    private const int NumConversions = 10000;

    [Fact]
    public void CompareMemoryAndPerformance()
    {
        Console.WriteLine($"Running memory and performance benchmark with {NumConversions} conversions");
        Console.WriteLine("=================================================================");

        // Measure using our array-based approach
        var (memoryArrayBased, timeArrayBased) = MeasureBatchConversions();

        Console.WriteLine("\nSummary:");
        Console.WriteLine("=========");
        Console.WriteLine($"Array-based approach: {memoryArrayBased:N0} bytes, {timeArrayBased.TotalMilliseconds:N2} ms");
        Console.WriteLine($"Memory per conversion: {memoryArrayBased / (double)NumConversions:F2} bytes");
        Console.WriteLine($"Time per conversion: {timeArrayBased.TotalMilliseconds / NumConversions:F6} ms");

        // This assertion just ensures the test completes successfully
        Assert.True(true);
    }

    private (long MemoryUsed, TimeSpan ElapsedTime) MeasureBatchConversions()
    {
        Console.WriteLine("Testing batch conversions (English to Nepali and back)");

        // Force garbage collection before test
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Record initial memory
        long memoryBefore = GC.GetTotalMemory(true);

        // Keep references to prevent garbage collection during measurement
        var nepaliDates = new NepaliDate[NumConversions];
        var englishDates = new DateTime[NumConversions];

        // Start timing
        var stopwatch = Stopwatch.StartNew();

        // Perform the conversions
        for (int i = 0; i < NumConversions; i++)
        {
            // Spread dates across different values to reduce cache hits
            var engDate = new DateTime(2020, 1, 1).AddDays(i % 366);

            // Convert from English to Nepali
            nepaliDates[i] = new NepaliDate(engDate);

            // Convert back to English
            englishDates[i] = nepaliDates[i].EnglishDate;
        }

        // Stop timing
        stopwatch.Stop();

        // Record final memory usage
        long memoryAfter = GC.GetTotalMemory(true);

        // Calculate and return results
        long memoryUsed = memoryAfter - memoryBefore;

        Console.WriteLine($"Memory used: {memoryUsed:N0} bytes");
        Console.WriteLine($"Time elapsed: {stopwatch.Elapsed.TotalMilliseconds:N2} ms");

        return (memoryUsed, stopwatch.Elapsed);
    }

    [Fact]
    public void MeasureSingleConversion()
    {
        Console.WriteLine("Measuring memory usage for a single conversion");
        Console.WriteLine("=============================================");

        // Force garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Measure before
        long memoryBefore = GC.GetTotalMemory(true);

        // Do a single conversion English to Nepali
        var engDate = new DateTime(2023, 9, 1);
        var nepDate = new NepaliDate(engDate);

        // Measure after Eng to Nep
        long memoryAfterToNep = GC.GetTotalMemory(true);

        // Do a single conversion Nepali to English
        var backToEng = nepDate.EnglishDate;

        // Measure after Nep to Eng
        long memoryAfterToEng = GC.GetTotalMemory(true);

        // Print results
        Console.WriteLine($"Memory for Eng to Nep: {memoryAfterToNep - memoryBefore} bytes");
        Console.WriteLine($"Memory for Nep to Eng: {memoryAfterToEng - memoryAfterToNep} bytes");
        Console.WriteLine($"Total memory for both: {memoryAfterToEng - memoryBefore} bytes");
        Console.WriteLine($"NepaliDate size (approx): {sizeof(int) * 3} bytes (3 integers for Year, Month, Day)");

        // Ensure the values are correct
        Assert.Equal(2080, nepDate.Year);
        Assert.Equal(5, nepDate.Month);
        Assert.Equal(15, nepDate.Day);
    }

    [Fact]
    public void CompareRepeatedVsUniqueConversions()
    {
        const int repeats = 1000;
        Console.WriteLine($"Comparing repeated vs unique conversions ({repeats} iterations)");
        Console.WriteLine("========================================================");

        // Measure memory for repeated conversions of the same date
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long memoryBefore = GC.GetTotalMemory(true);

        var engDate = new DateTime(2023, 9, 1);
        for (int i = 0; i < repeats; i++)
        {
            var nepDate = new NepaliDate(engDate);
            var backToEng = nepDate.EnglishDate;
        }

        long memoryAfterRepeated = GC.GetTotalMemory(true);

        // Measure memory for unique conversions
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long memoryBeforeUnique = GC.GetTotalMemory(true);

        for (int i = 0; i < repeats; i++)
        {
            var uniqueEngDate = new DateTime(2023, 1, 1).AddDays(i);
            var nepDate = new NepaliDate(uniqueEngDate);
            var backToEng = nepDate.EnglishDate;
        }

        long memoryAfterUnique = GC.GetTotalMemory(true);

        // Results
        Console.WriteLine($"Memory for {repeats} repeated conversions: {memoryAfterRepeated - memoryBefore:N0} bytes");
        Console.WriteLine($"Memory per repeated conversion: {(memoryAfterRepeated - memoryBefore) / (double)repeats:F2} bytes");

        Console.WriteLine($"Memory for {repeats} unique conversions: {memoryAfterUnique - memoryBeforeUnique:N0} bytes");
        Console.WriteLine($"Memory per unique conversion: {(memoryAfterUnique - memoryBeforeUnique) / (double)repeats:F2} bytes");

        Console.WriteLine($"Memory saved by caching: {(memoryAfterUnique - memoryBeforeUnique) - (memoryAfterRepeated - memoryBefore):N0} bytes");
    }
}