namespace NepDate.Tests.Core;

public class SmartDateParserTests
{
    [Fact]
    public void Parse_StandardFormat_ReturnsCorrectDate()
    {
        var expectedDate = new NepaliDate(2080, 4, 15);

        Assert.Equal(expectedDate, SmartDateParser.Parse("2080/04/15"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("2080-04-15"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("2080.04.15"));
    }

    [Fact]
    public void Parse_InvertedFormat_ReturnsCorrectDate()
    {
        var expectedDate = new NepaliDate(2080, 4, 15);

        Assert.Equal(expectedDate, SmartDateParser.Parse("15/04/2080"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("15-04-2080"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("15.04.2080"));
    }

    [Fact]
    public void Parse_MonthDayYearFormat_ReturnsCorrectDate()
    {
        var expectedDate = new NepaliDate(2080, 4, 15);

        Assert.Equal(expectedDate, SmartDateParser.Parse("04/15/2080"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("04-15-2080"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("04.15.2080"));
    }

    [Fact]
    public void Parse_WithMonthNames_ReturnsCorrectDate()
    {
        var expectedDate = new NepaliDate(2080, 4, 15);

        Assert.Equal(expectedDate, SmartDateParser.Parse("15 Shrawan 2080"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 Sawan 2080"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 Saun 2080"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("Shrawan 15, 2080"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("Shrawan 15 2080"));
    }

    [Fact]
    public void Parse_WithNepaliUnicode_ReturnsCorrectDate()
    {
        var expectedDate = new NepaliDate(2080, 4, 15);

        Assert.Equal(expectedDate, SmartDateParser.Parse("२०८०/०४/१५"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("१५/०४/२०८०"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("१५ श्रावण २०८०"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("श्रावण १५, २०८०"));
    }

    [Fact]
    public void Parse_MixedFormats_ReturnsCorrectDate()
    {
        var expectedDate = new NepaliDate(2080, 4, 15);

        Assert.Equal(expectedDate, SmartDateParser.Parse("15 साउन 2080"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("साउन 15, २०८०"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 Shrawan २०८०"));
    }

    [Fact]
    public void Parse_WithSuffixes_ReturnsCorrectDate()
    {
        var expectedDate = new NepaliDate(2080, 4, 15);

        Assert.Equal(expectedDate, SmartDateParser.Parse("15 Shrawan 2080 B.S."));
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 साउन 2080 BS"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 Shrawan 2080 V.S."));
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 साउन, 2080 मिति"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 साउन, 2080 गते"));
    }

    [Fact]
    public void Parse_WithTypos_ReturnsCorrectDate()
    {
        var expectedDate = new NepaliDate(2080, 4, 15);

        Assert.Equal(expectedDate, SmartDateParser.Parse("15 Srawan 2080")); // Typo in month name
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 Shraawan 2080")); // Extra 'a'
    }

    [Fact]
    public void Parse_ShorterYearFormats_ReturnsCorrectDate()
    {
        var expectedDate = new NepaliDate(2080, 4, 15);

        Assert.Equal(expectedDate, SmartDateParser.Parse("15/04/80")); // 2-digit year
        Assert.Equal(expectedDate, SmartDateParser.Parse("15/04/080")); // 3-digit year with leading zero
    }

    [Fact]
    public void Parse_InvalidFormat_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => SmartDateParser.Parse("not a date"));
        Assert.Throws<FormatException>(() => SmartDateParser.Parse("15/13/2080")); // Invalid month
        Assert.Throws<FormatException>(() => SmartDateParser.Parse("32/03/2080")); // Invalid day
    }

    [Fact]
    public void TryParse_ValidFormat_ReturnsTrue()
    {
        var expectedDate = new NepaliDate(2080, 4, 15);

        bool success = SmartDateParser.TryParse("15 Shrawan 2080", out var result);

        Assert.True(success);
        Assert.Equal(expectedDate, result);
    }

    [Fact]
    public void TryParse_InvalidFormat_ReturnsFalse()
    {
        bool success = SmartDateParser.TryParse("not a date", out var result);

        Assert.False(success);
        Assert.Equal(default, result);
    }

    [Fact]
    public void ExtensionMethod_ToNepaliDate_ParsesCorrectly()
    {
        var expectedDate = new NepaliDate(2080, 4, 15);

        Assert.Equal(expectedDate, "2080/04/15".ToNepaliDate());
        Assert.Equal(expectedDate, "२०८०/०४/१५".ToNepaliDate());
        Assert.Equal(expectedDate, "15 Shrawan 2080".ToNepaliDate());
    }

    [Fact]
    public void ExtensionMethod_TryToNepaliDate_ParsesCorrectly()
    {
        var expectedDate = new NepaliDate(2080, 4, 15);

        bool success = "15 Shrawan 2080".TryToNepaliDate(out var result);

        Assert.True(success);
        Assert.Equal(expectedDate, result);
    }
}