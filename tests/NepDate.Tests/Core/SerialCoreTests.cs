using System.Runtime.CompilerServices;
using NepDate.Core.Calendar;
using NepDate.Core.Dictionaries;

namespace NepDate.Tests.Core;

/// <summary>
/// Tests for the day-count core: the month start table, the conversion in both directions over
/// every supported day, and the constant-time calendar data index.
/// </summary>
public class SerialCoreTests
{
    private static readonly DateTime Epoch = new(1844, 4, 11);

    [Fact]
    public void MonthTable_HasOneEntryPerMonthPlusTheEnd()
    {
        Assert.Equal((MonthTable.MaxYear - MonthTable.MinYear + 1) * 12, MonthTable.MonthCount);
        Assert.Equal(MonthTable.MonthCount + 1, MonthTable.Start.Length);
        Assert.Equal(0, MonthTable.Start[0]);
        Assert.Equal(MonthTable.TotalDays, MonthTable.Start[MonthTable.MonthCount]);
    }

    [Fact]
    public void MonthTable_EveryMonthHas29To32Days()
    {
        for (int i = 0; i < MonthTable.MonthCount; i++)
        {
            int length = MonthTable.Start[i + 1] - MonthTable.Start[i];
            Assert.InRange(length, 29, 32);
        }
    }

    [Fact]
    public void MonthTable_AgreesWithTheCalendarDataOffsets()
    {
        int first = (CalendarOffsets.BaseYear - MonthTable.MinYear) * 12;
        int baseSerial = MonthTable.Start[first];

        for (int i = 0; i < CalendarOffsets.MonthStart.Length; i++)
        {
            Assert.Equal(CalendarOffsets.MonthStart[i], MonthTable.Start[first + i] - baseSerial);
        }

        Assert.Equal(CalendarOffsets.TotalDays, MonthTable.Start[first + CalendarOffsets.MonthStart.Length] - baseSerial);
    }

    [Fact]
    public void EveryDay_ConvertsBothWaysAndKeepsItsOrder()
    {
        var expected = NepaliDate.MinValue;
        int year = 1901, month = 1, day = 1;

        for (int serial = 0; serial < MonthTable.TotalDays; serial++)
        {
            var english = Epoch.AddDays(serial);
            var fromEnglish = new NepaliDate(english);
            var fromParts = new NepaliDate(year, month, day);

            Assert.Equal(fromParts, fromEnglish);
            Assert.Equal(year, fromEnglish.Year);
            Assert.Equal(month, fromEnglish.Month);
            Assert.Equal(day, fromEnglish.Day);
            Assert.Equal(english, fromParts.EnglishDate);
            Assert.Equal(english.DayOfWeek, fromParts.DayOfWeek);
            Assert.Equal(serial, (fromParts - NepaliDate.MinValue).Days);
            Assert.Equal(fromParts, NepaliDate.MinValue.AddDays(serial));
            Assert.True(serial == 0 || fromParts > expected);
            expected = fromParts;

            if (day == fromParts.MonthEndDay)
            {
                day = 1;
                if (month == 12) { month = 1; year++; } else { month++; }
            }
            else
            {
                day++;
            }
        }

        Assert.Equal(NepaliDate.MaxValue, expected);
        Assert.Equal(2200, year);
    }

    [Fact]
    public void DayOfYear_CountsFromBaisakhFirst()
    {
        for (int year = 1901; year <= 2199; year++)
        {
            Assert.Equal(1, new NepaliDate(year, 1, 1).DayOfYear);
            var last = new NepaliDate(year, 12, 1).MonthEndDate();
            Assert.Equal((last - new NepaliDate(year, 1, 1)).Days + 1, last.DayOfYear);
        }
    }

    [Fact]
    public void EnglishDate_IsMidnightWithUnspecifiedKind()
    {
        var date = new NepaliDate(new DateTime(2024, 2, 29, 12, 30, 0, DateTimeKind.Utc));

        Assert.Equal(new DateTime(2024, 2, 29), date.EnglishDate);
        Assert.Equal(DateTimeKind.Unspecified, date.EnglishDate.Kind);
    }

    [Fact]
    public void NepaliDate_IsEightBytes()
    {
        Assert.Equal(8, Unsafe.SizeOf<NepaliDate>());
    }

    [Fact]
    public void Default_SortsBeforeEveryDateAndEqualsOnlyItself()
    {
        NepaliDate value = default;

        Assert.True(value < NepaliDate.MinValue);
        Assert.NotEqual(NepaliDate.MinValue, value);
        Assert.Equal(default(NepaliDate), value);
        Assert.Equal(0, value.GetHashCode());
    }

    [Theory]
    [InlineData("2080/04/15")]
    [InlineData("2080-04-15")]
    [InlineData("2080.04.15")]
    [InlineData("2080_04_15")]
    [InlineData("2080 04 15")]
    [InlineData("2080|04|15")]
    [InlineData("2080\\04\\15")]
    [InlineData("2080।04।15")]
    [InlineData("2080/4/15")]
    [InlineData("2080//04/15")]
    public void TryParse_CanonicalAndLooseShapesAgree(string text)
    {
        Assert.True(NepaliDate.TryParse(text, out var parsed));
        Assert.Equal(new NepaliDate(2080, 4, 15), parsed);
        Assert.Equal(parsed, NepaliDate.Parse(text));
        Assert.Equal(parsed, ParseSpan<NepaliDate>(text));
    }

    [Theory]
    [InlineData("2080/0a/15")]
    [InlineData("2080/04/1x")]
    [InlineData("208o/04/15")]
    [InlineData("2080/13/01")]
    [InlineData("2080/04/33")]
    [InlineData("")]
    public void TryParse_RejectsBadText(string text)
    {
        Assert.False(NepaliDate.TryParse(text, out var parsed));
        Assert.True(parsed.IsDefault);
        Assert.False(TryParseSpan<NepaliDate>(text, out _));
    }

    [Fact]
    public void OffsetIndex_MatchesBinarySearchForEveryDay()
    {
        foreach (var offsets in new[] { TithiData.DayOffsets, HolidayData.DayOffsets, EventData.DayOffsets })
        {
            var index = new OffsetIndex(offsets, CalendarOffsets.TotalDays);

            for (int offset = 0; offset < CalendarOffsets.TotalDays; offset++)
            {
                int expected = Array.BinarySearch(offsets, (ushort)offset);
                Assert.Equal(expected >= 0, index.Contains(offset));
                Assert.Equal(expected >= 0 ? expected : -1, index.IndexOf(offset));
            }
        }
    }

    private static T ParseSpan<T>(string text) where T : ISpanParsable<T> => T.Parse(text.AsSpan(), null);

    private static bool TryParseSpan<T>(string text, out T result) where T : ISpanParsable<T> => T.TryParse(text.AsSpan(), null, out result!);
}
