namespace NepDate.Tests.Core;

public class BulkConvertTests
{
    [Fact]
    public void ToNepaliDates_ValidDateCollection_ReturnsCorrectNepaliDates()
    {
        var englishDates = new List<DateTime>
        {
            new(2023, 1, 1),
            new(2023, 1, 2),
            new(2023, 1, 10)
        };

        var nepaliDates = NepaliDate.BulkConvert.ToNepaliDates(englishDates).ToList();

        Assert.Equal(3, nepaliDates.Count);
        Assert.Equal(new NepaliDate(2079, 9, 17), nepaliDates[0]); // 2023-01-01 -> 2079-09-17
        Assert.Equal(new NepaliDate(2079, 9, 18), nepaliDates[1]); // 2023-01-02 -> 2079-09-18
        Assert.Equal(new NepaliDate(2079, 9, 26), nepaliDates[2]); // 2023-01-10 -> 2079-09-26
    }

    [Fact]
    public void ToNepaliDates_EmptyCollection_ReturnsEmptyCollection()
    {
        var englishDates = new List<DateTime>();

        var nepaliDates = NepaliDate.BulkConvert.ToNepaliDates(englishDates).ToList();

        Assert.Empty(nepaliDates);
    }

    [Fact]
    public void ToEnglishDates_NepaliDateCollection_ReturnsCorrectEnglishDates()
    {
        var nepaliDates = new List<NepaliDate>
        {
            new(2079, 9, 17),
            new(2079, 9, 18),
            new(2079, 9, 26)
        };

        var englishDates = NepaliDate.BulkConvert.ToEnglishDates(nepaliDates).ToList();

        Assert.Equal(3, englishDates.Count);
        Assert.Equal(new DateTime(2023, 1, 1).Date, englishDates[0].Date); // 2079-09-17 -> 2023-01-01
        Assert.Equal(new DateTime(2023, 1, 2).Date, englishDates[1].Date); // 2079-09-18 -> 2023-01-02
        Assert.Equal(new DateTime(2023, 1, 10).Date, englishDates[2].Date); // 2079-09-26 -> 2023-01-10
    }

    [Fact]
    public void ToEnglishDates_EmptyNepaliDateCollection_ReturnsEmptyCollection()
    {
        var nepaliDates = new List<NepaliDate>();

        var englishDates = NepaliDate.BulkConvert.ToEnglishDates(nepaliDates).ToList();

        Assert.Empty(englishDates);
    }

    [Fact]
    public void ToEnglishDates_StringCollection_ReturnsCorrectEnglishDates()
    {
        var nepaliDatesAsStrings = new List<string>
        {
            "2079/09/17",
            "2079/09/18",
            "2079/09/26"
        };

        var englishDates = NepaliDate.BulkConvert.ToEnglishDates(nepaliDatesAsStrings).ToList();

        Assert.Equal(3, englishDates.Count);
        Assert.Equal(new DateTime(2023, 1, 1).Date, englishDates[0].Date); // 2079-09-17 -> 2023-01-01
        Assert.Equal(new DateTime(2023, 1, 2).Date, englishDates[1].Date); // 2079-09-18 -> 2023-01-02
        Assert.Equal(new DateTime(2023, 1, 10).Date, englishDates[2].Date); // 2079-09-26 -> 2023-01-10
    }

    [Fact]
    public void BatchProcessToNepaliDates_ValidDateCollection_ReturnsCorrectNepaliDates()
    {
        var englishDates = new List<DateTime>();
        for (int i = 1; i <= 30; i++)
        {
            englishDates.Add(new DateTime(2023, 1, i));
        }

        var nepaliDates = NepaliDate.BulkConvert.BatchProcessToNepaliDates(englishDates, 10).ToList();

        Assert.Equal(30, nepaliDates.Count);
        Assert.Equal(new NepaliDate(2079, 9, 17), nepaliDates[0]); // 2023-01-01 -> 2079-09-17
        Assert.Equal(new NepaliDate(2079, 10, 16), nepaliDates[29]); // 2023-01-30 -> 2079-10-17
    }

    [Fact]
    public void BatchProcessToEnglishDates_ValidNepaliDateCollection_ReturnsCorrectEnglishDates()
    {
        var nepaliDates = new List<NepaliDate>();
        for (int i = 17; i <= 47; i++)
        {
            nepaliDates.Add(new NepaliDate(2079, 9, i > 30 ? i - 30 : i));
        }

        var englishDates = NepaliDate.BulkConvert.BatchProcessToEnglishDates(nepaliDates, 10).ToList();

        Assert.Equal(31, englishDates.Count);
        Assert.Equal(new DateTime(2023, 1, 1).Date, englishDates[0].Date); // 2079-09-17 -> 2023-01-01
        Assert.Equal(new DateTime(2023, 1, 1).Date, englishDates[30].Date); // 2079-10-17 -> 2023-01-31
    }
}