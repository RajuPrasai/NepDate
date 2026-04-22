using System.Runtime.CompilerServices;

namespace NepDate.Tests.Integration;

public class MemoryUsageTests
{
    [Fact]
    public void MeasureMemoryUsage_SingleConversion()
    {
        // Force garbage collection before test
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Measure baseline memory
        long memoryBefore = GC.GetTotalMemory(true);

        // Perform a single English to Nepali conversion
        var englishDate = new DateTime(2023, 9, 1);
        var nepaliDate = new NepaliDate(englishDate);

        // Measure memory after conversion
        long memoryAfterNepConversion = GC.GetTotalMemory(true);

        // Perform a single Nepali to English conversion
        var backToEnglish = nepaliDate.EnglishDate;

        // Measure final memory
        long memoryAfterEngConversion = GC.GetTotalMemory(true);

        // Output memory usage for different stages
        Console.WriteLine($"Memory for a single Eng to Nep conversion: {memoryAfterNepConversion - memoryBefore} bytes");
        Console.WriteLine($"Memory for a single Nep to Eng conversion: {memoryAfterEngConversion - memoryAfterNepConversion} bytes");
        Console.WriteLine($"Total memory used: {memoryAfterEngConversion - memoryBefore} bytes");

        // Make sure conversions are correct
        Assert.Equal(2080, nepaliDate.Year);
        Assert.Equal(5, nepaliDate.Month);
        Assert.Equal(15, nepaliDate.Day);
        Assert.Equal(englishDate.Date, backToEnglish.Date);
    }

    [Fact]
    public void MeasureMemoryUsage_MultipleConversions()
    {
        const int numConversions = 1000;

        // Force garbage collection before test
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Measure baseline memory
        long memoryBefore = GC.GetTotalMemory(true);

        // Define arrays to hold test results (prevents JIT from optimizing away the operations)
        var nepaliDates = new NepaliDate[numConversions];
        var englishDates = new DateTime[numConversions];

        // Perform multiple English to Nepali conversions
        for (int i = 0; i < numConversions; i++)
        {
            var engDate = new DateTime(2023, 1, 1).AddDays(i);
            nepaliDates[i] = new NepaliDate(engDate);
        }

        // Measure memory after Eng to Nep conversions
        long memoryAfterNepConversions = GC.GetTotalMemory(true);

        // Perform multiple Nepali to English conversions
        for (int i = 0; i < numConversions; i++)
        {
            englishDates[i] = nepaliDates[i].EnglishDate;
        }

        // Measure final memory
        long memoryAfterEngConversions = GC.GetTotalMemory(true);

        // Output memory usage for different stages
        Console.WriteLine($"Memory for {numConversions} Eng to Nep conversions: {memoryAfterNepConversions - memoryBefore} bytes");
        Console.WriteLine($"Memory per Eng to Nep conversion: {(memoryAfterNepConversions - memoryBefore) / (double)numConversions:F2} bytes");
        Console.WriteLine($"Memory for {numConversions} Nep to Eng conversions: {memoryAfterEngConversions - memoryAfterNepConversions} bytes");
        Console.WriteLine($"Memory per Nep to Eng conversion: {(memoryAfterEngConversions - memoryAfterNepConversions) / (double)numConversions:F2} bytes");
        Console.WriteLine($"Total memory used: {memoryAfterEngConversions - memoryBefore} bytes");

        // Verify the values are being used
        Assert.Equal(numConversions, nepaliDates.Length);
        Assert.Equal(numConversions, englishDates.Length);
    }

    [Fact]
    public void MeasureMemoryUsage_WithoutCaching()
    {
        const int numConversions = 100;

        // Force garbage collection before test
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Measure baseline memory
        long memoryBefore = GC.GetTotalMemory(true);

        // Perform repeated conversions with the same date (tests caching)
        var engDate = new DateTime(2023, 9, 1);
        for (int i = 0; i < numConversions; i++)
        {
            // These are different instances but same date values
            var nepDate = new NepaliDate(engDate);
            var backToEnglish = nepDate.EnglishDate;

            // Ensure values are used to prevent optimization
            PreventOptimization(nepDate);
            PreventOptimization(backToEnglish);
        }

        // Measure memory after repeated conversions of the same date
        long memoryAfterRepeatedConversions = GC.GetTotalMemory(true);

        // Perform conversions with different dates (less cache hits)
        for (int i = 0; i < numConversions; i++)
        {
            var engDate2 = new DateTime(2023, 1, 1).AddDays(i);
            var nepDate2 = new NepaliDate(engDate2);
            var backToEnglish2 = nepDate2.EnglishDate;

            // Ensure values are used to prevent optimization
            PreventOptimization(nepDate2);
            PreventOptimization(backToEnglish2);
        }

        // Measure memory after different conversions
        long memoryAfterDifferentConversions = GC.GetTotalMemory(true);

        // Output memory usage
        Console.WriteLine($"Memory for {numConversions} conversions of the same date: {memoryAfterRepeatedConversions - memoryBefore} bytes");
        Console.WriteLine($"Memory per conversion (same date): {(memoryAfterRepeatedConversions - memoryBefore) / (double)numConversions:F2} bytes");
        Console.WriteLine($"Memory for {numConversions} conversions of different dates: {memoryAfterDifferentConversions - memoryAfterRepeatedConversions} bytes");
        Console.WriteLine($"Memory per conversion (different dates): {(memoryAfterDifferentConversions - memoryAfterRepeatedConversions) / (double)numConversions:F2} bytes");
        Console.WriteLine($"Additional memory for different dates: {memoryAfterDifferentConversions - memoryAfterRepeatedConversions} bytes");
    }

    [Fact]
    public void MeasureArrayOptimization_MemoryImpact()
    {
        const int numConversions = 1000;

        // Force garbage collection before test
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Measure baseline memory
        long memoryBefore = GC.GetTotalMemory(true);

        // Create multiple dates and access their properties
        // This will use the array-based optimization
        var nepaliDates = new List<NepaliDate>();
        for (int i = 0; i < numConversions; i++)
        {
            // Create dates across different years and months
            int year = 1901 + (i % 298); // Spread across years from 1901 to 2199
            int month = 1 + (i % 12);
            int day = 1 + (i % 28);

            var nepDate = new NepaliDate(year, month, day);
            nepaliDates.Add(nepDate);

            // Access EnglishDate which triggers the array lookup
            var engDate = nepDate.EnglishDate;
            PreventOptimization(engDate);
        }

        // Measure memory after creating and converting dates
        long memoryAfter = GC.GetTotalMemory(true);

        // Output memory usage
        Console.WriteLine($"Total memory for {numConversions} conversions with array optimization: {memoryAfter - memoryBefore} bytes");
        Console.WriteLine($"Average memory per conversion: {(memoryAfter - memoryBefore) / (double)numConversions:F2} bytes");
    }

    /// <summary>
    /// Prevents the JIT compiler from optimizing away variables that aren't directly used.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void PreventOptimization<T>(T value)
    {
        // This empty method with the NoInlining attribute ensures the compiler won't optimize
        // away the variable, as it can't determine if the variable is used inside this method.
        if (DateTime.Now.Ticks == 0) // This condition is always false
        {
            // This code never executes but prevents optimization
            Console.WriteLine(value?.ToString());
        }
    }
}