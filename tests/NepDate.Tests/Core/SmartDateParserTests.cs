using NepDate.Extensions;

namespace NepDate.Tests.Core;

public class SmartDateParserTests
{
    [Fact]
    public void Parse_StandardFormat_ReturnsCorrectDate()
    {
        // Arrange
        var expectedDate = new NepaliDate(2080, 4, 15);

        // Act & Assert
        Assert.Equal(expectedDate, SmartDateParser.Parse("2080/04/15"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("2080-04-15"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("2080.04.15"));
    }

    [Fact]
    public void Parse_InvertedFormat_ReturnsCorrectDate()
    {
        // Arrange
        var expectedDate = new NepaliDate(2080, 4, 15);

        // Act & Assert
        Assert.Equal(expectedDate, SmartDateParser.Parse("15/04/2080"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("15-04-2080"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("15.04.2080"));
    }

    [Fact]
    public void Parse_MonthDayYearFormat_ReturnsCorrectDate()
    {
        // Arrange
        var expectedDate = new NepaliDate(2080, 4, 15);

        // Act & Assert
        Assert.Equal(expectedDate, SmartDateParser.Parse("04/15/2080"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("04-15-2080"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("04.15.2080"));
    }

    [Fact]
    public void Parse_WithMonthNames_ReturnsCorrectDate()
    {
        // Arrange
        var expectedDate = new NepaliDate(2080, 4, 15);

        // Act & Assert
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 Shrawan 2080"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 Sawan 2080"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 Saun 2080"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("Shrawan 15, 2080"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("Shrawan 15 2080"));
    }

    [Fact]
    public void Parse_WithNepaliUnicode_ReturnsCorrectDate()
    {
        // Arrange
        var expectedDate = new NepaliDate(2080, 4, 15);

        // Act & Assert
        Assert.Equal(expectedDate, SmartDateParser.Parse("२०८०/०४/१५"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("१५/०४/२०८०"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("१५ श्रावण २०८०"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("श्रावण १५, २०८०"));
    }

    [Fact]
    public void Parse_MixedFormats_ReturnsCorrectDate()
    {
        // Arrange
        var expectedDate = new NepaliDate(2080, 4, 15);

        // Act & Assert
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 साउन 2080"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("साउन 15, २०८०"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 Shrawan २०८०"));
    }

    [Fact]
    public void Parse_WithSuffixes_ReturnsCorrectDate()
    {
        // Arrange
        var expectedDate = new NepaliDate(2080, 4, 15);

        // Act & Assert
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 Shrawan 2080 B.S."));
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 साउन 2080 BS"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 Shrawan 2080 V.S."));
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 साउन, 2080 मिति"));
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 साउन, 2080 गते"));
    }

    [Fact]
    public void Parse_WithTypos_ReturnsCorrectDate()
    {
        // Arrange
        var expectedDate = new NepaliDate(2080, 4, 15);

        // Act & Assert
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 Srawan 2080")); // Typo in month name
        Assert.Equal(expectedDate, SmartDateParser.Parse("15 Shraawan 2080")); // Extra 'a'
    }

    [Fact]
    public void Parse_ShorterYearFormats_ReturnsCorrectDate()
    {
        // Arrange
        var expectedDate = new NepaliDate(2080, 4, 15);

        // Act & Assert
        Assert.Equal(expectedDate, SmartDateParser.Parse("15/04/80")); // 2-digit year
        Assert.Equal(expectedDate, SmartDateParser.Parse("15/04/080")); // 3-digit year with leading zero
    }

    [Fact]
    public void Parse_InvalidFormat_ThrowsFormatException()
    {
        // Act & Assert
        Assert.Throws<FormatException>(() => SmartDateParser.Parse("not a date"));
        Assert.Throws<FormatException>(() => SmartDateParser.Parse("15/13/2080")); // Invalid month
        Assert.Throws<FormatException>(() => SmartDateParser.Parse("32/03/2080")); // Invalid day
    }

    [Fact]
    public void TryParse_ValidFormat_ReturnsTrue()
    {
        // Arrange
        var expectedDate = new NepaliDate(2080, 4, 15);

        // Act
        bool success = SmartDateParser.TryParse("15 Shrawan 2080", out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(expectedDate, result);
    }

    [Fact]
    public void TryParse_InvalidFormat_ReturnsFalse()
    {
        // Act
        bool success = SmartDateParser.TryParse("not a date", out var result);

        // Assert
        Assert.False(success);
        Assert.Equal(default, result);
    }

    [Fact]
    public void ExtensionMethod_ToNepaliDate_ParsesCorrectly()
    {
        // Arrange
        var expectedDate = new NepaliDate(2080, 4, 15);

        // Act & Assert
        Assert.Equal(expectedDate, "2080/04/15".ToNepaliDate());
        Assert.Equal(expectedDate, "२०८०/०४/१५".ToNepaliDate());
        Assert.Equal(expectedDate, "15 Shrawan 2080".ToNepaliDate());
    }

    [Fact]
    public void ExtensionMethod_TryToNepaliDate_ParsesCorrectly()
    {
        // Arrange
        var expectedDate = new NepaliDate(2080, 4, 15);

        // Act
        bool success = "15 Shrawan 2080".TryToNepaliDate(out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(expectedDate, result);
    }

    [Theory]
    // Baisakh (1)
    [InlineData("baisakh", 1)]
    [InlineData("baishakh", 1)]
    [InlineData("baisak", 1)]
    [InlineData("vaisakh", 1)]
    [InlineData("vaisakha", 1)]
    [InlineData("vaishak", 1)]
    [InlineData("vaisakhi", 1)]
    [InlineData("baishak", 1)]
    [InlineData("baisaga", 1)]
    [InlineData("baishaga", 1)]

    // Jestha (2)
    [InlineData("jestha", 2)]
    [InlineData("jeth", 2)]
    [InlineData("jeshtha", 2)]
    [InlineData("jyeshtha", 2)]
    [InlineData("jyestha", 2)]
    [InlineData("jesth", 2)]
    [InlineData("jeshth", 2)]
    [InlineData("jetha", 2)]
    [InlineData("jeshta", 2)]
    [InlineData("jayshtha", 2)]
    [InlineData("jayestha", 2)]
    [InlineData("jesta", 2)]
    [InlineData("jyesth", 2)]
    [InlineData("jyaistha", 2)]
    [InlineData("jaistha", 2)]

    // Asar (3)
    [InlineData("asar", 3)]
    [InlineData("asadh", 3)]
    [InlineData("ashar", 3)]
    [InlineData("ashad", 3)]
    [InlineData("asad", 3)]
    [InlineData("aasad", 3)]
    [InlineData("asada", 3)]
    [InlineData("ashadh", 3)]
    [InlineData("asadha", 3)]
    [InlineData("ashadha", 3)]
    [InlineData("ashara", 3)]
    [InlineData("asara", 3)]
    [InlineData("ashada", 3)]
    [InlineData("asaad", 3)]
    [InlineData("aashar", 3)]

    // Shrawan (4)
    [InlineData("shrawan", 4)]
    [InlineData("sawan", 4)]
    [InlineData("saun", 4)]
    [InlineData("srawan", 4)]
    [InlineData("shraawan", 4)]
    [InlineData("shravan", 4)]
    [InlineData("shravana", 4)]
    [InlineData("sawun", 4)]
    [InlineData("savan", 4)]
    [InlineData("shrawana", 4)]
    [InlineData("sravana", 4)]
    [InlineData("sawon", 4)]
    [InlineData("sravan", 4)]
    [InlineData("saawan", 4)]
    [InlineData("sharwan", 4)]
    [InlineData("sarwan", 4)]
    [InlineData("sraawan", 4)]
    [InlineData("shaun", 4)]
    [InlineData("shawan", 4)]

    // Bhadra (5)
    [InlineData("bhadra", 5)]
    [InlineData("bhadrapad", 5)]
    [InlineData("bhadrapada", 5)]
    [InlineData("bhadra pad", 5)]
    [InlineData("bhadraw", 5)]
    [InlineData("bhadar", 5)]
    [InlineData("bhadrapaksh", 5)]
    [InlineData("bhdra", 5)]
    [InlineData("bhadarwa", 5)]
    [InlineData("bhadrawa", 5)]

    // Ashoj (6)
    [InlineData("ashoj", 6)]
    [InlineData("asoj", 6)]
    [InlineData("ashwin", 6)]
    [InlineData("ashvina", 6)]
    [InlineData("ashwayuja", 6)]
    [InlineData("asuj", 6)]
    [InlineData("asoja", 6)]
    [InlineData("ashvayuja", 6)]
    [InlineData("ashwinak", 6)]

    // Kartik (7)
    [InlineData("kartik", 7)]
    [InlineData("kartika", 7)]
    [InlineData("karthik", 7)]
    [InlineData("karttika", 7)]
    [InlineData("kaartik", 7)]
    [InlineData("karthika", 7)]
    [InlineData("kaarthik", 7)]
    [InlineData("kartic", 7)]

    // Mangsir (8)
    [InlineData("mangsir", 8)]
    [InlineData("mangsirh", 8)]
    [InlineData("mangsar", 8)]
    [InlineData("mangsira", 8)]
    [InlineData("mangsire", 8)]
    [InlineData("mangsira", 8)]
    [InlineData("margashirsha", 8)]
    [InlineData("margashira", 8)]
    [InlineData("margshirsha", 8)]
    [InlineData("margsheersh", 8)]
    [InlineData("mangsira", 8)]

    // Poush (9)
    [InlineData("poush", 9)]
    [InlineData("paush", 9)]
    [InlineData("pousha", 9)]
    [InlineData("pausha", 9)]
    [InlineData("push", 9)]
    [InlineData("pusha", 9)]
    [InlineData("paus", 9)]
    [InlineData("pous", 9)]

    // Magh (10)
    [InlineData("magh", 10)]
    [InlineData("magha", 10)]
    [InlineData("mag", 10)]
    [InlineData("maagh", 10)]
    [InlineData("maagha", 10)]

    // Falgun (11)
    [InlineData("falgun", 11)]
    [InlineData("phagun", 11)]
    [InlineData("phalgun", 11)]
    [InlineData("phalguna", 11)]
    [InlineData("faagun", 11)]
    [InlineData("phalguni", 11)]
    [InlineData("fagun", 11)]

    // Chaitra (12)
    [InlineData("chaitra", 12)]
    [InlineData("chait", 12)]
    [InlineData("chaitri", 12)]
    [InlineData("chaitram", 12)]
    [InlineData("chaitanya", 12)]
    [InlineData("chaitarah", 12)]

    // Month 1 - Baisakh
    [InlineData("बैशाख", 1)]
    [InlineData("वैशाख", 1)]
    [InlineData("बैसाख", 1)]
    [InlineData("बैशाक", 1)]
    [InlineData("वैसाख", 1)]
    [InlineData("वैशाक", 1)]

// Month 2 - Jestha
    [InlineData("जेष्ठ", 2)]
    [InlineData("जेठ", 2)]
    [InlineData("जेस्थ", 2)]
    [InlineData("ज्येष्ठ", 2)]
    [InlineData("जेस्ठ", 2)]
    [InlineData("जेष्ट", 2)]

// Month 3 - Asar
    [InlineData("आषाढ", 3)]
    [InlineData("असार", 3)]
    [InlineData("अषाढ", 3)]
    [InlineData("आशाढ", 3)]
    [InlineData("आषाढ़", 3)]
    [InlineData("असाढ", 3)]
    [InlineData("अषाड", 3)]

// Month 4 - Shrawan
    [InlineData("श्रावण", 4)]
    [InlineData("सावन", 4)]
    [InlineData("साउन", 4)]
    [InlineData("श्रावन", 4)]
    [InlineData("सावण", 4)]
    [InlineData("श्रवण", 4)]

// Month 5 - Bhadra
    [InlineData("भाद्र", 5)]
    [InlineData("भदौ", 5)]
    [InlineData("भाद्रपद", 5)]
    [InlineData("भदो", 5)]
    [InlineData("भादोै", 5)]
    [InlineData("भाद्रा", 5)]

// Month 6 - Ashwin
    [InlineData("आश्विन", 6)]
    [InlineData("असोज", 6)]
    [InlineData("अश्विन", 6)]
    [InlineData("आसोज", 6)]
    [InlineData("अस्विन", 6)]
    [InlineData("अश्वीन", 6)]
    [InlineData("अश्वीना", 6)]

// Month 7 - Kartik
    [InlineData("कार्तिक", 7)]
    [InlineData("कात्तिक", 7)]
    [InlineData("कार्तीक", 7)]
    [InlineData("कार्तिका", 7)]
    [InlineData("कातिक", 7)]
    [InlineData("कर्तिक", 7)]
    [InlineData("कार्तिक्", 7)]

// Month 8 - Mangsir
    [InlineData("मंसिर", 8)]
    [InlineData("मङ्सिर", 8)]
    [InlineData("मंग्सिर", 8)]
    [InlineData("मंशिर", 8)]
    [InlineData("मागशिर", 8)]
    [InlineData("मार्गशीर्ष", 8)]

// Month 9 - Poush
    [InlineData("पौष", 9)]
    [InlineData("पुष", 9)]
    [InlineData("पुस", 9)]
    [InlineData("पौश", 9)]
    [InlineData("पौष्य", 9)]
    [InlineData("पौस", 9)]

// Month 10 - Magh
    [InlineData("माघ", 10)]
    [InlineData("माग", 10)]
    [InlineData("माह", 10)]
    [InlineData("माघा", 10)]
    [InlineData("माग्ह", 10)]
    [InlineData("मा्घ", 10)]

// Month 11 - Falgun
    [InlineData("फाल्गुन", 11)]
    [InlineData("फागुन", 11)]
    [InlineData("फाल्गुण", 11)]
    [InlineData("फल्गुन", 11)]
    [InlineData("फाल्गुना", 11)]

// Month 12 - Chaitra
    [InlineData("चैत्र", 12)]
    [InlineData("चैत", 12)]
    [InlineData("चैता", 12)]
    [InlineData("चॆत्र", 12)]
    [InlineData("चेत्र", 12)]
    [InlineData("चैत्रा", 12)]
    public void Parse_WithNepaliMonthName_ReturnsCorrectDate(string input, int expectedMonth)
    {
        var expectedDate = new NepaliDate(2080, expectedMonth, 15);
        Assert.Equal(expectedDate, SmartDateParser.Parse($"15 {input} 2080"));
    }
}