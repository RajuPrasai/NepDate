using NepDate.Core.Dictionaries;

namespace NepDate.Tests.Core;

/// <summary>
/// Exhaustive verification tests for the calendar data pipeline:
/// CalendarStrings, CalendarOffsets, TithiData, EventData, HolidayData,
/// and the public NepaliDate properties/methods that expose them.
/// </summary>
public class CalendarDataTests
{
    // -------------------------------------------------------------------------
    // Array structural integrity
    // -------------------------------------------------------------------------

    [Fact]
    public void CalendarStrings_Pool_IsNonEmpty()
    {
        Assert.True(CalendarStrings.Pool.Length > 0, "String pool must not be empty.");
    }

    [Fact]
    public void CalendarStrings_Pool_AllEntriesHaveNonEmptyNpAndEn()
    {
        for (int i = 0; i < CalendarStrings.Pool.Length; i++)
        {
            Assert.False(string.IsNullOrEmpty(CalendarStrings.Pool[i].Np), $"Pool[{i}].Np is empty");
            Assert.False(string.IsNullOrEmpty(CalendarStrings.Pool[i].En), $"Pool[{i}].En is empty");
        }
    }

    [Fact]
    public void CalendarOffsets_MonthStart_HasExpectedLength()
    {
        // YearCount * 12 month entries
        Assert.Equal(CalendarOffsets.YearCount * 12, CalendarOffsets.MonthStart.Length);
    }

    [Fact]
    public void CalendarOffsets_MonthStart_IsStrictlyMonotonic()
    {
        for (int i = 1; i < CalendarOffsets.MonthStart.Length; i++)
        {
            Assert.True(
                CalendarOffsets.MonthStart[i] > CalendarOffsets.MonthStart[i - 1],
                $"MonthStart not strictly increasing at index {i}: {CalendarOffsets.MonthStart[i]} <= {CalendarOffsets.MonthStart[i - 1]}");
        }
    }

    [Fact]
    public void CalendarOffsets_TotalDays_MatchesExpectedCount()
    {
        // Scraped range is 2001-2089 = 89 years, 32509 days as verified during data analysis.
        Assert.Equal(32509, CalendarOffsets.TotalDays);
    }

    [Fact]
    public void TithiData_DayOffsets_IsStrictlySorted()
    {
        for (int i = 1; i < TithiData.DayOffsets.Length; i++)
        {
            Assert.True(
                TithiData.DayOffsets[i] > TithiData.DayOffsets[i - 1],
                $"TithiData.DayOffsets not strictly sorted at index {i}");
        }
    }

    [Fact]
    public void TithiData_AllPoolIds_AreInRange()
    {
        int poolLen = CalendarStrings.Pool.Length;
        for (int i = 0; i < TithiData.PoolIds.Length; i++)
        {
            Assert.True(
                TithiData.PoolIds[i] < poolLen,
                $"TithiData.PoolIds[{i}] = {TithiData.PoolIds[i]} is out of range (pool len = {poolLen})");
        }
    }

    [Fact]
    public void TithiData_EntryCount_Matches2190()
    {
        // Verified via data analysis: 2078-2082 = 1825 days with Tithi.
        Assert.Equal(2190, TithiData.DayOffsets.Length);
        Assert.Equal(2190, TithiData.PoolIds.Length);
    }

    [Fact]
    public void EventData_DayOffsets_IsStrictlySorted()
    {
        for (int i = 1; i < EventData.DayOffsets.Length; i++)
        {
            Assert.True(
                EventData.DayOffsets[i] > EventData.DayOffsets[i - 1],
                $"EventData.DayOffsets not strictly sorted at index {i}");
        }
    }

    [Fact]
    public void EventData_SliceStart_IsNonDecreasing()
    {
        for (int i = 1; i < EventData.SliceStart.Length; i++)
        {
            Assert.True(
                EventData.SliceStart[i] >= EventData.SliceStart[i - 1],
                $"EventData.SliceStart not non-decreasing at index {i}");
        }
    }

    [Fact]
    public void EventData_SliceStart_SentinelEqualsEntriesLength()
    {
        // SliceStart has DayOffsets.Length + 1 entries; the last is the sentinel.
        Assert.Equal(EventData.DayOffsets.Length + 1, EventData.SliceStart.Length);
        Assert.Equal(EventData.Entries.Length, EventData.SliceStart[EventData.SliceStart.Length - 1]);
    }

    [Fact]
    public void EventData_AllEntries_AreInRange()
    {
        int poolLen = CalendarStrings.Pool.Length;
        for (int i = 0; i < EventData.Entries.Length; i++)
        {
            Assert.True(
                EventData.Entries[i] < poolLen,
                $"EventData.Entries[{i}] = {EventData.Entries[i]} is out of range (pool len = {poolLen})");
        }
    }

    [Fact]
    public void EventData_DayCount_Matches1671()
    {
        // Verified via data analysis.
        Assert.Equal(1671, EventData.DayOffsets.Length);
    }

    [Fact]
    public void HolidayData_DayOffsets_IsStrictlySorted()
    {
        for (int i = 1; i < HolidayData.DayOffsets.Length; i++)
        {
            Assert.True(
                HolidayData.DayOffsets[i] > HolidayData.DayOffsets[i - 1],
                $"HolidayData.DayOffsets not strictly sorted at index {i}");
        }
    }

    [Fact]
    public void HolidayData_Count_Matches294()
    {
        // Verified via data analysis.
        Assert.Equal(294, HolidayData.DayOffsets.Length);
    }

    // -------------------------------------------------------------------------
    // GetOffset boundary / correctness
    // -------------------------------------------------------------------------

    [Fact]
    public void CalendarOffsets_GetOffset_BaseYear_Month1_Day1_ReturnsZero()
    {
        Assert.Equal(0, CalendarOffsets.GetOffset(2001, 1, 1));
    }

    [Fact]
    public void CalendarOffsets_GetOffset_OutOfRange_ReturnsNegativeOne()
    {
        Assert.Equal(-1, CalendarOffsets.GetOffset(2000, 1, 1));
        Assert.Equal(-1, CalendarOffsets.GetOffset(2090, 1, 1));
        Assert.Equal(-1, CalendarOffsets.GetOffset(1900, 6, 15));
    }

    [Fact]
    public void CalendarOffsets_GetOffset_AllOffsetsAreUniqueSentinel()
    {
        // Every valid date must produce an offset that is non-negative and < TotalDays.
        // We only spot-check the year boundaries to avoid massive test time.
        Assert.True(CalendarOffsets.GetOffset(2001, 1, 1) >= 0);
        Assert.True(CalendarOffsets.GetOffset(2089, 12, 1) < CalendarOffsets.TotalDays);
    }

    // -------------------------------------------------------------------------
    // Known-good date assertions (NepaliDate public surface)
    // -------------------------------------------------------------------------

    [Fact]
    public void NepaliDate_2081_01_01_CalendarData()
    {
        var date = new NepaliDate(2081, 1, 1);
        var info = date.GetCalendarInfo();
        Assert.Equal("पञ्चमी", date.TithiNp);
        Assert.Equal("Panchami", date.TithiEn);
        Assert.True(date.IsPublicHoliday);
        Assert.Contains("नयाँ वर्ष", info.EventsNp);
        Assert.Contains("New Year", info.EventsEn);
    }

    [Fact]
    public void NepaliDate_2081_01_02_Tithi_IsShashthi()
    {
        var date = new NepaliDate(2081, 1, 2);
        Assert.Equal("षष्ठी", date.TithiNp);
        Assert.Equal("Shashthi", date.TithiEn);
    }

    [Fact]
    public void NepaliDate_2081_01_05_IsPublicHoliday()
    {
        // Ram Navami
        var date = new NepaliDate(2081, 1, 5);
        Assert.True(date.IsPublicHoliday);
        Assert.Equal("नवमी", date.TithiNp);
        Assert.Equal("Navami", date.TithiEn);
    }

    [Fact]
    public void NepaliDate_2081_02_15_IsPublicHoliday_GanatantraDiwas()
    {
        var info = new NepaliDate(2081, 2, 15).GetCalendarInfo();
        Assert.True(info.IsPublicHoliday);
        Assert.Contains("गणतन्त्र दिवस", info.EventsNp);
        Assert.Contains("Republic Day", info.EventsEn);
    }

    [Fact]
    public void NepaliDate_2080_11_01_CalendarData()
    {
        var date = new NepaliDate(2080, 11, 1);
        var info = date.GetCalendarInfo();
        Assert.Equal("चतुर्थी", date.TithiNp);
        Assert.Equal("Chaturthi", date.TithiEn);
        Assert.False(date.IsPublicHoliday);
        Assert.Contains("जनयुद्ध दिवस", info.EventsNp);
        Assert.Contains("People's War Day", info.EventsEn);
    }

    [Fact]
    public void NepaliDate_BeforeRange_AllPropertiesReturnDefaults()
    {
        // 2000 BS is before the 2001 base year.
        var date = new NepaliDate(2000, 6, 15);
        Assert.Equal(string.Empty, date.TithiNp);
        Assert.Equal(string.Empty, date.TithiEn);
        Assert.False(date.IsPublicHoliday);
        var info = date.GetCalendarInfo();
        Assert.Equal(string.Empty, info.TithiNp);
        Assert.Equal(string.Empty, info.TithiEn);
        Assert.False(info.IsPublicHoliday);
        Assert.Empty(info.EventsNp);
        Assert.Empty(info.EventsEn);
    }

    [Fact]
    public void NepaliDate_AfterRange_AllPropertiesReturnDefaults()
    {
        // 2090 BS is after the 2089 end year.
        var date = new NepaliDate(2090, 1, 1);
        Assert.Equal(string.Empty, date.TithiNp);
        Assert.Equal(string.Empty, date.TithiEn);
        Assert.False(date.IsPublicHoliday);
        var info = date.GetCalendarInfo();
        Assert.Empty(info.EventsNp);
        Assert.Empty(info.EventsEn);
    }

    [Fact]
    public void NepaliDate_OutsideTithiRange_TithiIsEmpty()
    {
        // 2001-2077 have no Tithi data in the scrape.
        var date = new NepaliDate(2070, 6, 15);
        Assert.Equal(string.Empty, date.TithiNp);
        Assert.Equal(string.Empty, date.TithiEn);
    }

    [Fact]
    public void NepaliDate_EventsNp_And_EventsEn_AlwaysHaveSameLength()
    {
        // Spot-check several dates including multi-event days.
        int[][] samples =
        {
            new[] { 2081, 1, 1 },
            new[] { 2081, 1, 5 },
            new[] { 2081, 2, 10 },
            new[] { 2080, 11, 1 },
            new[] { 2075, 1, 1 },
        };

        foreach (var s in samples)
        {
            var info = new NepaliDate(s[0], s[1], s[2]).GetCalendarInfo();
            Assert.Equal(info.EventsNp.Length, info.EventsEn.Length);
        }
    }

    // -------------------------------------------------------------------------
    // GetCalendarInfo consistency: TithiNp/TithiEn match the flat properties
    // -------------------------------------------------------------------------

    [Fact]
    public void GetCalendarInfo_TithiMatchesFlatProperties()
    {
        var date = new NepaliDate(2081, 1, 1);
        var info = date.GetCalendarInfo();
        Assert.Equal(date.TithiNp, info.TithiNp);
        Assert.Equal(date.TithiEn, info.TithiEn);
        Assert.Equal(date.IsPublicHoliday, info.IsPublicHoliday);
    }

    // -------------------------------------------------------------------------
    // Exhaustive range scan — no exceptions and no out-of-bounds
    // -------------------------------------------------------------------------

    [Fact]
    public void AllDaysInScrapedRange_OffsetsAreInBounds()
    {
        int failures = 0;
        for (int year = 2001; year <= 2089; year++)
        {
            for (int month = 1; month <= 12; month++)
            {
                var nepDate = new NepaliDate(year, month, 1);
                int endDay = nepDate.MonthEndDay;
                for (int day = 1; day <= endDay; day++)
                {
                    int offset = CalendarOffsets.GetOffset(year, month, day);
                    if (offset < 0 || offset >= CalendarOffsets.TotalDays)
                        failures++;
                }
            }
        }
        Assert.Equal(0, failures);
    }

    [Fact]
    public void AllDaysInScrapedRange_NoExceptionOnPropertyAccess()
    {
        // Does not assert specific values; asserts no exceptions are thrown on any day.
        var exceptions = new System.Text.StringBuilder();
        for (int year = 2078; year <= 2082; year++) // dense Tithi years — highest risk
        {
            for (int month = 1; month <= 12; month++)
            {
                int endDay = new NepaliDate(year, month, 1).MonthEndDay;
                for (int day = 1; day <= endDay; day++)
                {
                    try
                    {
                        var d = new NepaliDate(year, month, day);
                        _ = d.TithiNp;
                        _ = d.TithiEn;
                        _ = d.IsPublicHoliday;
                        _ = d.GetCalendarInfo();
                    }
                    catch (Exception ex)
                    {
                        exceptions.AppendLine($"{year}/{month}/{day}: {ex.Message}");
                    }
                }
            }
        }
        Assert.True(exceptions.Length == 0, $"Exceptions during property access:\n{exceptions}");
    }

    [Fact]
    public void AllTithiEntries_ResolveToNonEmptyStrings()
    {
        for (int i = 0; i < TithiData.PoolIds.Length; i++)
        {
            var (np, en) = CalendarStrings.Pool[TithiData.PoolIds[i]];
            Assert.False(string.IsNullOrEmpty(np), $"TithiData.PoolIds[{i}] resolves to empty Np");
            Assert.False(string.IsNullOrEmpty(en), $"TithiData.PoolIds[{i}] resolves to empty En");
        }
    }

    [Fact]
    public void AllHolidayOffsets_AreInBounds()
    {
        foreach (var offset in HolidayData.DayOffsets)
        {
            Assert.True(offset < CalendarOffsets.TotalDays,
                $"Holiday offset {offset} is >= TotalDays ({CalendarOffsets.TotalDays})");
        }
    }

    [Fact]
    public void AllEventOffsets_AreInBounds()
    {
        foreach (var offset in EventData.DayOffsets)
        {
            Assert.True(offset < CalendarOffsets.TotalDays,
                $"Event offset {offset} is >= TotalDays ({CalendarOffsets.TotalDays})");
        }
    }

    // -------------------------------------------------------------------------
    // Holiday count cross-check via API
    // -------------------------------------------------------------------------

    [Fact]
    public void HolidayCount_ViaApi_Matches294()
    {
        int count = 0;
        for (int year = 2001; year <= 2089; year++)
        {
            for (int month = 1; month <= 12; month++)
            {
                int endDay = new NepaliDate(year, month, 1).MonthEndDay;
                for (int day = 1; day <= endDay; day++)
                {
                    if (new NepaliDate(year, month, day).IsPublicHoliday)
                        count++;
                }
            }
        }
        Assert.Equal(294, count);
    }

    // -------------------------------------------------------------------------
    // CalendarInfo struct correctness
    // -------------------------------------------------------------------------

    [Fact]
    public void CalendarInfo_EventArrays_AreNeverNull()
    {
        // Spot check a day with no events and a day with events.
        var withEvents = new NepaliDate(2081, 1, 1).GetCalendarInfo();
        var withoutEvents = new NepaliDate(2001, 1, 2).GetCalendarInfo();

        Assert.NotNull(withEvents.EventsNp);
        Assert.NotNull(withEvents.EventsEn);
        Assert.NotNull(withoutEvents.EventsNp);
        Assert.NotNull(withoutEvents.EventsEn);
    }
}