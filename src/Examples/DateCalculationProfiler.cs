using System.Diagnostics;

namespace NepDate.Examples;

public static class DateCalculationProfiler
{
    public static void RunProfiler()
    {
        Console.WriteLine("=== Date Calculation Performance Profiler ===\n");

        // Profile individual date conversion operations
        ProfileSingleDateConversion();

        // Profile core dictionary operations
        ProfileDictionaryOperations();

        // Profile date calculations involving multiple operations
        ProfileDateCalculations();

        // Profile batch operations
        ProfileBatchOperations();
    }

    private static void ProfileSingleDateConversion()
    {
        Console.WriteLine("=== Single Date Conversion Performance ===");

        // English to Nepali conversion
        {
            var sw = Stopwatch.StartNew();
            int iterations = 50000;
            for (int i = 0; i < iterations; i++)
            {
                var engDate = new DateTime(2000, 1, 1).AddDays(i % 365);
                var nepDate = new NepaliDate(engDate);
            }
            sw.Stop();
            Console.WriteLine($"English to Nepali conversion ({iterations} iterations): {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / (float)iterations:F6}ms per conversion)");
        }

        // Nepali to English conversion
        {
            var sw = Stopwatch.StartNew();
            int iterations = 50000;
            for (int i = 0; i < iterations; i++)
            {
                var nepDate = new NepaliDate(2080, (i % 12) + 1, (i % 28) + 1);
                var engDate = nepDate.EnglishDate;
            }
            sw.Stop();
            Console.WriteLine($"Nepali to English conversion ({iterations} iterations): {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / (float)iterations:F6}ms per conversion)");
        }

        Console.WriteLine();
    }

    private static void ProfileDictionaryOperations()
    {
        Console.WriteLine("=== Dictionary Lookup Performance ===");

        // Dictionary lookups for NepaliToEnglish
        {
            var sw = Stopwatch.StartNew();
            int iterations = 50000;

            for (int i = 0; i < iterations; i++)
            {
                // Create random but valid year and month
                int year = 1950 + (i % 250);
                int month = 1 + (i % 12);

                try
                {
                    var nepDate = new NepaliDate(year, month, 15);
                    var engDate = nepDate.EnglishDate;
                }
                catch
                {
                    // Ignore date validation exceptions
                }
            }

            sw.Stop();
            Console.WriteLine($"NepaliToEnglish dictionary lookups ({iterations} iterations): {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / (float)iterations:F6}ms per lookup)");
        }

        // Dictionary lookups for EnglishToNepali
        {
            var sw = Stopwatch.StartNew();
            int iterations = 50000;

            var startDate = new DateTime(1900, 1, 1);
            for (int i = 0; i < iterations; i++)
            {
                try
                {
                    var date = startDate.AddDays(i * 30); // Sample different months/years
                    var nepDate = date.ToNepaliDate();
                }
                catch
                {
                    // Ignore date validation exceptions
                }
            }

            sw.Stop();
            Console.WriteLine($"EnglishToNepali dictionary lookups ({iterations} iterations): {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / (float)iterations:F6}ms per lookup)");
        }

        // Test effect of repeated lookups (caching effect)
        {
            var sw = Stopwatch.StartNew();
            int iterations = 100000;

            // Use the same date repeatedly to test caching
            var nepDate = new NepaliDate(2080, 5, 15);

            for (int i = 0; i < iterations; i++)
            {
                var engDate = nepDate.EnglishDate;
            }

            sw.Stop();
            Console.WriteLine($"Repeated Nepali to English conversion ({iterations}x same date): {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / (float)iterations:F6}ms per conversion)");
        }

        Console.WriteLine();
    }

    private static void ProfileDateCalculations()
    {
        Console.WriteLine("=== Date Calculation Performance ===");

        // AddDays operation with smaller increments to avoid going out of valid range
        {
            var sw = Stopwatch.StartNew();
            int iterations = 10000;
            var nepDate = new NepaliDate(2080, 5, 15);

            // Add and subtract days to stay within valid range
            for (int i = 0; i < iterations; i++)
            {
                // Alternate between adding and subtracting to stay in valid range
                int dayChange = (i % 2 == 0) ? 1 : -1;
                nepDate = nepDate.AddDays(dayChange);
            }
            sw.Stop();
            Console.WriteLine($"AddDays ({iterations} days, alternating +1/-1): {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / (float)iterations:F6}ms per operation)");
        }

        // AddDays with larger increments
        {
            var sw = Stopwatch.StartNew();
            int iterations = 10000;
            var nepDate = new NepaliDate(2080, 5, 15);

            // Add and subtract days to stay within valid range
            for (int i = 0; i < iterations; i++)
            {
                // Alternate between adding and subtracting to stay in valid range
                int dayChange = (i % 2 == 0) ? 10 : -10;
                nepDate = nepDate.AddDays(dayChange);
            }
            sw.Stop();
            Console.WriteLine($"AddDays ({iterations} iterations, alternating +10/-10 days): {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / (float)iterations:F6}ms per operation)");
        }

        // Crossing month boundaries with AddDays
        {
            var sw = Stopwatch.StartNew();
            int iterations = 5000;
            for (int i = 0; i < iterations; i++)
            {
                // Create date at end of month to force crossing month boundary
                int month = 1 + (i % 12);
                var nepDate = new NepaliDate(2080, month, 28); // Likely end of month for most Nepali months
                nepDate = nepDate.AddDays(i % 2 == 0 ? 5 : -5); // Cross month boundary forward or backward
            }
            sw.Stop();
            Console.WriteLine($"AddDays crossing month boundaries ({iterations} iterations): {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / (float)iterations:F6}ms per operation)");
        }

        // Crossing year boundaries with AddDays
        {
            var sw = Stopwatch.StartNew();
            int iterations = 2000;
            for (int i = 0; i < iterations; i++)
            {
                // Create date at end of year or start of year
                var nepDate = i % 2 == 0 ?
                    new NepaliDate(2080, 12, 30) :
                    new NepaliDate(2080, 1, 5);

                // Cross year boundary forward or backward
                nepDate = nepDate.AddDays(i % 2 == 0 ? 5 : -5);
            }
            sw.Stop();
            Console.WriteLine($"AddDays crossing year boundaries ({iterations} iterations): {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / (float)iterations:F6}ms per operation)");
        }

        // AddMonths operation
        {
            var sw = Stopwatch.StartNew();
            int iterations = 100;
            var nepDate = new NepaliDate(2080, 5, 15);
            for (int i = 0; i < iterations; i++)
            {
                nepDate = nepDate.AddMonths(i % 2 == 0 ? 1 : -1);
            }
            sw.Stop();
            Console.WriteLine($"AddMonths ({iterations} alternating +1/-1 month): {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / (float)iterations:F6}ms per operation)");
        }

        // Date difference calculation
        {
            var sw = Stopwatch.StartNew();
            int iterations = 1000;
            var startDate = new NepaliDate(2080, 1, 1);
            for (int i = 0; i < iterations; i++)
            {
                var endDate = new NepaliDate(2080 + (i % 5), 1, 1);
                var diff = endDate.Subtract(startDate);
            }
            sw.Stop();
            Console.WriteLine($"Date difference ({iterations} calculations): {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / (float)iterations:F6}ms per operation)");
        }

        Console.WriteLine();
    }

    private static void ProfileBatchOperations()
    {
        Console.WriteLine("=== Batch Operation Performance ===");

        // Prepare test data - use valid date range (after 1901-04-13)
        int count = 20000;
        var englishDates = new List<DateTime>(count);
        var startDate = new DateTime(1950, 1, 1);
        for (int i = 0; i < count; i++)
        {
            englishDates.Add(startDate.AddDays(i % 10000)); // Cycle through a range of 10,000 days to stay within valid range
        }

        // Individual conversions
        {
            var sw = Stopwatch.StartNew();
            var result = new List<NepaliDate>(count);
            foreach (var date in englishDates)
            {
                result.Add(new NepaliDate(date));
            }
            sw.Stop();
            Console.WriteLine($"Individual conversions ({count:N0} dates): {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / (float)count:F6}ms per date)");
        }

        // Sequential bulk conversion
        {
            var sw = Stopwatch.StartNew();
            var result = NepaliDate.BulkConvert.ToNepaliDates(englishDates, useParallel: false).ToList();
            sw.Stop();
            Console.WriteLine($"Sequential bulk conversion ({count:N0} dates): {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / (float)count:F6}ms per date)");
        }

        // Parallel bulk conversion
        {
            var sw = Stopwatch.StartNew();
            var result = NepaliDate.BulkConvert.ToNepaliDates(englishDates, useParallel: true).ToList();
            sw.Stop();
            Console.WriteLine($"Parallel bulk conversion ({count:N0} dates): {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / (float)count:F6}ms per date)");
        }

        // Batched conversion (1000 per batch)
        {
            var sw = Stopwatch.StartNew();
            var result = NepaliDate.BulkConvert.BatchProcessToNepaliDates(englishDates, 1000).ToList();
            sw.Stop();
            Console.WriteLine($"Batched conversion ({count:N0} dates, 1000 per batch): {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / (float)count:F6}ms per date)");
        }

        Console.WriteLine();
    }
}
