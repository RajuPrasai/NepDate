using BenchmarkDotNet.Attributes;

namespace NepDate.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class Benchmarks
{
    private static readonly DateTime FixedEngDate = new(2023, 8, 28);

    // ── Nepali → English ──────────────────────────────────

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("NepToEng")]
    public DateTime GetEngDate_NepDate()
    {
        return new NepaliDate(2079, 12, 12).EnglishDate;
    }

    [Benchmark]
    [BenchmarkCategory("NepToEng")]
    public DateTime GetEngDate_NepaliDateConverter_NETCORE()
    {
        var date = NepaliDateConverter.DateConverter.ConvertToEnglish(2079, 12, 12);
        return new DateTime(date.Year, date.Month, date.Day);
    }

    [Benchmark]
    [BenchmarkCategory("NepToEng")]
    public DateTime GetEngDate_NepaliCalendarBS()
    {
        return NepaliCalendarBS.NepaliCalendar.Convert_BS2AD("2079/12/12");
    }

    [Benchmark]
    [BenchmarkCategory("NepToEng")]
    public DateTime GetEngDate_NepaliDateConverter_Net()
    {
        var date = NepaliDateConverter.Net.DateConverter.ConvertToEnglish(2079, 12, 12);
        return new DateTime(date.Year, date.Month, date.Day);
    }

    // ── English → Nepali ──────────────────────────────────

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("EngToNep")]
    public NepaliDate GetNepDate_NepDate()
    {
        return new NepaliDate(FixedEngDate);
    }

    [Benchmark]
    [BenchmarkCategory("EngToNep")]
    public (int Year, int Month, int Day) GetNepDate_NepaliDateConverter_NETCORE()
    {
        var date = NepaliDateConverter.DateConverter.ConvertToNepali(2023, 8, 28);
        return (date.Year, date.Month, date.Day);
    }

    [Benchmark]
    [BenchmarkCategory("EngToNep")]
    public (int Year, int Month, int Day) GetNepDate_NepaliCalendarBS()
    {
        var date = NepaliCalendarBS.NepaliCalendar.Convert_AD2BS(FixedEngDate);
        return (date.Year, date.Month, date.Day);
    }

    [Benchmark]
    [BenchmarkCategory("EngToNep")]
    public (int Year, int Month, int Day) GetNepDate_NepaliDateConverter_Net()
    {
        var date = NepaliDateConverter.Net.DateConverter.ConvertToNepali(2023, 8, 28);
        return (date.Year, date.Month, date.Day);
    }
}
