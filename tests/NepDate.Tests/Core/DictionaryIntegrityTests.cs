namespace NepDate.Tests.Core;

/// <summary>
/// Exhaustive verification tests for the flat array dictionary migration.
/// These tests verify that every single date conversion in the entire supported range
/// produces correct and consistent results.
/// </summary>
public class DictionaryIntegrityTests
{
    /// <summary>
    /// Verifies that every Nepali month from 1901/1 to 2199/12 can be converted to English
    /// and back, producing the original Nepali date.
    /// This is a roundtrip test across ALL 3,588 year-month combinations.
    /// </summary>
    [Fact]
    public void AllNepaliMonths_RoundtripToEnglishAndBack_ProducesOriginalDate()
    {
        int failures = 0;

        for (int year = 1901; year <= 2199; year++)
        {
            for (int month = 1; month <= 12; month++)
            {
                var nepDate = new NepaliDate(year, month, 1);
                var engDate = nepDate.EnglishDate.Date;
                var roundtrip = new NepaliDate(engDate);

                if (roundtrip.Year != year || roundtrip.Month != month || roundtrip.Day != 1)
                {
                    failures++;
                }
            }
        }

        Assert.Equal(0, failures);
    }

    /// <summary>
    /// Verifies that every single day in the entire supported range (1901/1/1 to 2199/12/end)
    /// can be converted to English and back without data loss.
    /// This tests ALL individual days across the full 299 year range.
    /// </summary>
    [Fact]
    public void AllNepaliDays_RoundtripToEnglishAndBack_ProducesOriginalDate()
    {
        int totalDays = 0;
        int failures = 0;

        for (int year = 1901; year <= 2199; year++)
        {
            for (int month = 1; month <= 12; month++)
            {
                var firstOfMonth = new NepaliDate(year, month, 1);
                int daysInMonth = firstOfMonth.MonthEndDay;

                for (int day = 1; day <= daysInMonth; day++)
                {
                    var nepDate = new NepaliDate(year, month, day);
                    var engDate = nepDate.EnglishDate.Date;
                    var roundtrip = new NepaliDate(engDate);

                    if (roundtrip.Year != year || roundtrip.Month != month || roundtrip.Day != day)
                    {
                        failures++;
                    }
                    totalDays++;
                }
            }
        }

        Assert.Equal(0, failures);
        Assert.True(totalDays > 100000, $"Expected 100k+ days tested, got {totalDays}");
    }

    /// <summary>
    /// Verifies that every English date in the supported range can be converted to Nepali and back.
    /// Iterates day by day from MinValue.EnglishDate to MaxValue.EnglishDate.
    /// </summary>
    [Fact]
    public void AllEnglishDays_RoundtripToNepaliAndBack_ProducesOriginalDate()
    {
        var startEng = NepaliDate.MinValue.EnglishDate.Date;
        var endEng = NepaliDate.MaxValue.EnglishDate.Date;
        int totalDays = 0;
        int failures = 0;

        for (var date = startEng; date <= endEng; date = date.AddDays(1))
        {
            var nepDate = new NepaliDate(date);
            var roundtrip = nepDate.EnglishDate.Date;

            if (roundtrip != date)
            {
                failures++;
            }
            totalDays++;
        }

        Assert.Equal(0, failures);
        Assert.True(totalDays > 100000, $"Expected 100k+ days tested, got {totalDays}");
    }

    /// <summary>
    /// Verifies that MonthEndDay values are within valid range (29-32) for every month.
    /// </summary>
    [Fact]
    public void AllMonths_MonthEndDay_IsWithinValidRange()
    {
        for (int year = 1901; year <= 2199; year++)
        {
            for (int month = 1; month <= 12; month++)
            {
                var nepDate = new NepaliDate(year, month, 1);
                int endDay = nepDate.MonthEndDay;
                Assert.InRange(endDay, 29, 32);
            }
        }
    }

    /// <summary>
    /// Verifies that English dates are strictly increasing as Nepali dates increase.
    /// Each day's English equivalent must be exactly one day after the previous day's equivalent.
    /// </summary>
    [Fact]
    public void ConsecutiveNepaliDays_ProduceConsecutiveEnglishDays()
    {
        DateTime? previousEngDate = null;
        int violations = 0;

        for (int year = 1901; year <= 2199; year++)
        {
            for (int month = 1; month <= 12; month++)
            {
                var firstOfMonth = new NepaliDate(year, month, 1);
                int daysInMonth = firstOfMonth.MonthEndDay;

                for (int day = 1; day <= daysInMonth; day++)
                {
                    var nepDate = new NepaliDate(year, month, day);
                    var engDate = nepDate.EnglishDate.Date;

                    if (previousEngDate.HasValue)
                    {
                        var expected = previousEngDate.Value.AddDays(1);
                        if (engDate != expected)
                        {
                            violations++;
                        }
                    }
                    previousEngDate = engDate;
                }
            }
        }

        Assert.Equal(0, violations);
    }

    /// <summary>
    /// Verifies known reference dates that are publicly verifiable.
    /// These are anchor points that should never change.
    /// </summary>
    [Theory]
    [InlineData(2080, 1, 1, 2023, 4, 14)]   // 1 Baisakh 2080 = April 14, 2023
    [InlineData(2000, 1, 1, 1943, 4, 14)]   // 1 Baisakh 2000 = April 14, 1943
    [InlineData(2081, 1, 1, 2024, 4, 13)]   // 1 Baisakh 2081 = April 13, 2024
    [InlineData(1901, 1, 1, 1844, 4, 11)]   // MinValue: 1 Baisakh 1901
    public void KnownReferenceDates_ConvertCorrectly(int nepY, int nepM, int nepD, int engY, int engM, int engD)
    {
        var nepDate = new NepaliDate(nepY, nepM, nepD);
        var engDate = nepDate.EnglishDate.Date;
        Assert.Equal(new DateTime(engY, engM, engD), engDate);

        var reverse = new NepaliDate(new DateTime(engY, engM, engD));
        Assert.Equal(nepY, reverse.Year);
        Assert.Equal(nepM, reverse.Month);
        Assert.Equal(nepD, reverse.Day);
    }

    /// <summary>
    /// Verifies boundary dates at the min and max of the supported range.
    /// </summary>
    [Fact]
    public void BoundaryDates_ConvertCorrectly()
    {
        // MinValue
        var min = NepaliDate.MinValue;
        Assert.Equal(1901, min.Year);
        Assert.Equal(1, min.Month);
        Assert.Equal(1, min.Day);
        var minEng = min.EnglishDate.Date;
        var minRoundtrip = new NepaliDate(minEng);
        Assert.Equal(min.Year, minRoundtrip.Year);
        Assert.Equal(min.Month, minRoundtrip.Month);
        Assert.Equal(min.Day, minRoundtrip.Day);

        // MaxValue
        var max = NepaliDate.MaxValue;
        Assert.Equal(2199, max.Year);
        Assert.Equal(12, max.Month);
        var maxEng = max.EnglishDate.Date;
        var maxRoundtrip = new NepaliDate(maxEng);
        Assert.Equal(max.Year, maxRoundtrip.Year);
        Assert.Equal(max.Month, maxRoundtrip.Month);
        Assert.Equal(max.Day, maxRoundtrip.Day);
    }

    /// <summary>
    /// Verifies that the total number of days across all months sums correctly.
    /// Also verifies the sum matches the English date span.
    /// </summary>
    [Fact]
    public void TotalDaysInRange_MatchesEnglishDateSpan()
    {
        int totalNepaliDays = 0;
        for (int year = 1901; year <= 2199; year++)
        {
            for (int month = 1; month <= 12; month++)
            {
                var nepDate = new NepaliDate(year, month, 1);
                totalNepaliDays += nepDate.MonthEndDay;
            }
        }

        var engSpan = (NepaliDate.MaxValue.EnglishDate.Date - NepaliDate.MinValue.EnglishDate.Date).Days + 1;
        Assert.Equal(engSpan, totalNepaliDays);
    }
}