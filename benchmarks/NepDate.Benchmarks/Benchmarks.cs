using BenchmarkDotNet.Attributes;

namespace NepDate.Benchmarks;

/// <summary>
/// Compares NepDate with other Nepali date libraries on the same inputs.
/// </summary>
/// <remarks>
/// Every call converts the next date from a pool of 1,024 different dates (2070-2085 BS), so the
/// JIT cannot fold a constant date into a constant result. A fixed input made the NepDate methods
/// look sub-nanosecond, which measured an empty method rather than a conversion.
/// </remarks>
[MemoryDiagnoser]
[RankColumn]
[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class Benchmarks
{
    private const int PoolSize = 1024; // power of two, so "& Mask" picks the next index
    private const int Mask = PoolSize - 1;

    private readonly int[] _bsYear = new int[PoolSize];
    private readonly int[] _bsMonth = new int[PoolSize];
    private readonly int[] _bsDay = new int[PoolSize];
    private readonly string[] _bsText = new string[PoolSize];
    private readonly DateTime[] _adDate = new DateTime[PoolSize];
    private int _next;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        var first = new NepaliDate(2070, 1, 1);
        var span = (new NepaliDate(2085, 12, 1).MonthEndDate() - first).Days;

        for (int i = 0; i < PoolSize; i++)
        {
            var date = first.AddDays(random.Next(span + 1));
            _bsYear[i] = date.Year;
            _bsMonth[i] = date.Month;
            _bsDay[i] = date.Day;
            _bsText[i] = date.ToString();
            _adDate[i] = date.EnglishDate;
        }
    }

    private int Next() => _next++ & Mask;

    // ── Nepali → English ──────────────────────────────────

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("NepToEng")]
    public DateTime GetEngDate_NepDate()
    {
        var i = Next();
        return new NepaliDate(_bsYear[i], _bsMonth[i], _bsDay[i]).EnglishDate;
    }

    [Benchmark]
    [BenchmarkCategory("NepToEng")]
    public DateTime GetEngDate_NepaliDateConverter_NETCORE()
    {
        var i = Next();
        var date = NepaliDateConverter.DateConverter.ConvertToEnglish(_bsYear[i], _bsMonth[i], _bsDay[i]);
        return new DateTime(date.Year, date.Month, date.Day);
    }

    [Benchmark]
    [BenchmarkCategory("NepToEng")]
    public DateTime GetEngDate_NepaliCalendarBS()
    {
        return NepaliCalendarBS.NepaliCalendar.Convert_BS2AD(_bsText[Next()]);
    }

    [Benchmark]
    [BenchmarkCategory("NepToEng")]
    public DateTime GetEngDate_NepaliDateConverter_Net()
    {
        var i = Next();
        var date = NepaliDateConverter.Net.DateConverter.ConvertToEnglish(_bsYear[i], _bsMonth[i], _bsDay[i]);
        return new DateTime(date.Year, date.Month, date.Day);
    }

    // ── English → Nepali ──────────────────────────────────

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("EngToNep")]
    public NepaliDate GetNepDate_NepDate()
    {
        return new NepaliDate(_adDate[Next()]);
    }

    [Benchmark]
    [BenchmarkCategory("EngToNep")]
    public (int Year, int Month, int Day) GetNepDate_NepaliDateConverter_NETCORE()
    {
        var ad = _adDate[Next()];
        var date = NepaliDateConverter.DateConverter.ConvertToNepali(ad.Year, ad.Month, ad.Day);
        return (date.Year, date.Month, date.Day);
    }

    [Benchmark]
    [BenchmarkCategory("EngToNep")]
    public (int Year, int Month, int Day) GetNepDate_NepaliCalendarBS()
    {
        var date = NepaliCalendarBS.NepaliCalendar.Convert_AD2BS(_adDate[Next()]);
        return (date.Year, date.Month, date.Day);
    }

    [Benchmark]
    [BenchmarkCategory("EngToNep")]
    public (int Year, int Month, int Day) GetNepDate_NepaliDateConverter_Net()
    {
        var ad = _adDate[Next()];
        var date = NepaliDateConverter.Net.DateConverter.ConvertToNepali(ad.Year, ad.Month, ad.Day);
        return (date.Year, date.Month, date.Day);
    }
}
