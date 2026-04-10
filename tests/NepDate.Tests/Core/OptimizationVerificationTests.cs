namespace NepDate.Tests.Core;

/// <summary>
/// Comprehensive tests verifying that all optimized methods produce
/// identical output to the pre-optimization behavior.
/// Covers ToString, parsing, Unicode conversion, SmartDateParser,
/// operators, comparison, and all edge cases.
/// </summary>
public class OptimizationVerificationTests
{
    #region ToString() Format Verification

    [Theory]
    [InlineData(2080, 5, 15, "2080/05/15")]
    [InlineData(2080, 1, 1, "2080/01/01")]
    [InlineData(2080, 12, 30, "2080/12/30")]
    [InlineData(1901, 1, 1, "1901/01/01")]
    [InlineData(2199, 12, 30, "2199/12/30")]
    [InlineData(2000, 6, 9, "2000/06/09")]
    [InlineData(2100, 10, 29, "2100/10/29")]
    public void ToString_ProducesExactFormat(int year, int month, int day, string expected)
    {
        var date = new NepaliDate(year, month, day);
        Assert.Equal(expected, date.ToString());
    }

    [Fact]
    public void ToString_OutputIsExactly10Characters()
    {
        var date = new NepaliDate(2080, 5, 15);
        var str = date.ToString();
        Assert.Equal(10, str.Length);
        Assert.Equal('/', str[4]);
        Assert.Equal('/', str[7]);
    }

    [Fact]
    public void ToString_AllMonthsZeroPadded()
    {
        for (int month = 1; month <= 12; month++)
        {
            var date = new NepaliDate(2080, month, 1);
            var str = date.ToString();
            var monthPart = str.Substring(5, 2);
            Assert.Equal(month.ToString("D2"), monthPart);
        }
    }

    [Fact]
    public void ToString_AllDaysZeroPadded()
    {
        var date2080_01 = new NepaliDate(2080, 1, 1);
        int endDay = date2080_01.MonthEndDay;

        for (int day = 1; day <= endDay; day++)
        {
            var date = new NepaliDate(2080, 1, day);
            var str = date.ToString();
            var dayPart = str.Substring(8, 2);
            Assert.Equal(day.ToString("D2"), dayPart);
        }
    }

    [Fact]
    public void ToString_RoundtripsWithConstructor()
    {
        for (int year = 1901; year <= 2199; year += 50)
        {
            for (int month = 1; month <= 12; month++)
            {
                var date = new NepaliDate(year, month, 1);
                var str = date.ToString();
                var parsed = new NepaliDate(str);
                Assert.Equal(date.Year, parsed.Year);
                Assert.Equal(date.Month, parsed.Month);
                Assert.Equal(date.Day, parsed.Day);
            }
        }
    }

    #endregion

    #region ToString(DateFormats, Separators) Verification

    [Theory]
    [InlineData(DateFormats.YearMonthDay, Separators.ForwardSlash, true, "2080/05/15")]
    [InlineData(DateFormats.YearMonthDay, Separators.Dash, true, "2080-05-15")]
    [InlineData(DateFormats.YearMonthDay, Separators.Dot, true, "2080.05.15")]
    [InlineData(DateFormats.YearMonthDay, Separators.Underscore, true, "2080_05_15")]
    [InlineData(DateFormats.YearMonthDay, Separators.Space, true, "2080 05 15")]
    [InlineData(DateFormats.YearMonthDay, Separators.BackwardSlash, true, "2080\\05\\15")]
    [InlineData(DateFormats.DayMonthYear, Separators.ForwardSlash, true, "15/05/2080")]
    [InlineData(DateFormats.DayMonthYear, Separators.Dash, true, "15-05-2080")]
    [InlineData(DateFormats.MonthDayYear, Separators.ForwardSlash, true, "05/15/2080")]
    [InlineData(DateFormats.YearDayMonth, Separators.ForwardSlash, true, "2080/15/05")]
    [InlineData(DateFormats.MonthYearDay, Separators.ForwardSlash, true, "05/2080/15")]
    [InlineData(DateFormats.DayYearMonth, Separators.ForwardSlash, true, "15/2080/05")]
    public void ToString_WithFormatAndSeparator_ProducesCorrectOutput(
        DateFormats format, Separators separator, bool leadingZeros, string expected)
    {
        var date = new NepaliDate(2080, 5, 15);
        Assert.Equal(expected, date.ToString(format, separator, leadingZeros));
    }

    [Theory]
    [InlineData(DateFormats.YearMonthDay, Separators.ForwardSlash, false, "2080/5/15")]
    [InlineData(DateFormats.DayMonthYear, Separators.Dash, false, "15-5-2080")]
    [InlineData(DateFormats.MonthDayYear, Separators.Dot, false, "5.15.2080")]
    public void ToString_WithoutLeadingZeros_ProducesCorrectOutput(
        DateFormats format, Separators separator, bool leadingZeros, string expected)
    {
        var date = new NepaliDate(2080, 5, 15);
        Assert.Equal(expected, date.ToString(format, separator, leadingZeros));
    }

    #endregion

    #region ToUnicodeString Verification

    [Fact]
    public void ToUnicodeString_DefaultFormat_ConvertsAllDigits()
    {
        var date = new NepaliDate(2080, 5, 15);
        var unicode = date.ToUnicodeString();
        Assert.Equal("\u0968\u0966\u096E\u0966/\u0966\u096B/\u0967\u096B", unicode);
        // 2080/05/15 → २०८०/०५/१५
    }

    [Fact]
    public void ToUnicodeString_EachDigitConvertsCorrectly()
    {
        // Test each digit 0-9 appears correctly in various dates
        // Year 1902 has digits 1,9,0,2
        var date = new NepaliDate(1902, 3, 4);
        var unicode = date.ToUnicodeString();

        // 1902/03/04 → १९०२/०३/०४
        Assert.Contains("\u0967", unicode); // 1 → १
        Assert.Contains("\u096F", unicode); // 9 → ९
        Assert.Contains("\u0966", unicode); // 0 → ०
        Assert.Contains("\u0968", unicode); // 2 → २
        Assert.Contains("\u0969", unicode); // 3 → ३
        Assert.Contains("\u096A", unicode); // 4 → ४
    }

    [Fact]
    public void ToUnicodeString_SeparatorsPreserved()
    {
        var date = new NepaliDate(2080, 5, 15);
        var unicode = date.ToUnicodeString(DateFormats.YearMonthDay, Separators.Dash);
        Assert.Contains("-", unicode);
        Assert.DoesNotContain("/", unicode);
    }

    [Fact]
    public void ToUnicodeString_SameLengthAsToString()
    {
        // Unicode digits are single chars, same as ASCII digits
        for (int year = 1901; year <= 2199; year += 100)
        {
            var date = new NepaliDate(year, 6, 15);
            Assert.Equal(date.ToString().Length, date.ToUnicodeString().Length);
        }
    }

    #endregion

    #region SplitNepaliDate Parsing Edge Cases (via constructor)

    [Theory]
    [InlineData("2080/05/15")]
    [InlineData("2080-05-15")]
    [InlineData("2080.05.15")]
    [InlineData("2080_05_15")]
    [InlineData("2080\\05\\15")]
    [InlineData("2080 05 15")]
    [InlineData("2080।05।15")]
    [InlineData("2080|05|15")]
    public void Constructor_AllSeparators_ParseCorrectly(string input)
    {
        var date = new NepaliDate(input);
        Assert.Equal(2080, date.Year);
        Assert.Equal(5, date.Month);
        Assert.Equal(15, date.Day);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("2080/05")]
    [InlineData("2080")]
    [InlineData("2080/05/15/99")]
    [InlineData("abc/def/ghi")]
    public void Constructor_InvalidStrings_ThrowsException(string input)
    {
        Assert.ThrowsAny<Exception>(() => new NepaliDate(input));
    }

    [Fact]
    public void Constructor_NullString_ThrowsException()
    {
        Assert.ThrowsAny<Exception>(() => new NepaliDate(null!));
    }

    [Theory]
    [InlineData("2080/5/15", 2080, 5, 15)]
    [InlineData("2080/05/5", 2080, 5, 5)]
    [InlineData("2080/1/1", 2080, 1, 1)]
    public void Constructor_WithoutLeadingZeros_ParsesCorrectly(string input, int year, int month, int day)
    {
        var date = new NepaliDate(input);
        Assert.Equal(year, date.Year);
        Assert.Equal(month, date.Month);
        Assert.Equal(day, date.Day);
    }

    #endregion

    #region Nepali Digit Conversion (via SmartDateParser)

    [Theory]
    [InlineData("\u0968\u0966\u096E\u0966/\u0966\u096B/\u0967\u096B", 2080, 5, 15)]  // २०८०/०५/१५
    [InlineData("\u0968\u0966\u096E\u0966-\u0966\u096B-\u0967\u096B", 2080, 5, 15)]  // २०८०-०५-१५
    public void SmartDateParser_NepaliDigits_ParseCorrectly(string input, int year, int month, int day)
    {
        var date = SmartDateParser.Parse(input);
        Assert.Equal(year, date.Year);
        Assert.Equal(month, date.Month);
        Assert.Equal(day, date.Day);
    }

    [Fact]
    public void SmartDateParser_PureEnglishDigits_NoConversionNeeded()
    {
        var date = SmartDateParser.Parse("2080/05/15");
        Assert.Equal(2080, date.Year);
        Assert.Equal(5, date.Month);
        Assert.Equal(15, date.Day);
    }

    [Fact]
    public void SmartDateParser_MixedNepaliEnglishDigits_ParseCorrectly()
    {
        // 2०80/0५/१5 → 2080/05/15
        var date = SmartDateParser.Parse("2\u0966\u096E0/0\u096B/\u0967" + "5");
        Assert.Equal(2080, date.Year);
        Assert.Equal(5, date.Month);
        Assert.Equal(15, date.Day);
    }

    #endregion

    #region SmartDateParser NormalizeInput (via Parse)

    [Theory]
    [InlineData("2080/05/15 B.S.", 2080, 5, 15)]
    [InlineData("2080/05/15 BS", 2080, 5, 15)]
    [InlineData("2080/05/15 V.S.", 2080, 5, 15)]
    [InlineData("2080/05/15 VS", 2080, 5, 15)]
    [InlineData("2080/05/15 bs", 2080, 5, 15)]
    [InlineData("2080/05/15 v.s.", 2080, 5, 15)]
    public void SmartDateParser_WithIndicators_RemovesAndParsesCorrectly(string input, int year, int month, int day)
    {
        var date = SmartDateParser.Parse(input);
        Assert.Equal(year, date.Year);
        Assert.Equal(month, date.Month);
        Assert.Equal(day, date.Day);
    }

    [Fact]
    public void SmartDateParser_WithNepaliKeywords_RemovesAndParses()
    {
        // "15 Shrawan 2080 गते" → should parse after removing गते
        var date = SmartDateParser.Parse("15 Shrawan 2080 \u0917\u0924\u0947");
        Assert.Equal(2080, date.Year);
        Assert.Equal(4, date.Month); // Shrawan = month 4
        Assert.Equal(15, date.Day);
    }

    [Fact]
    public void SmartDateParser_WithExtraSpaces_ParsesCorrectly()
    {
        var date = SmartDateParser.Parse("  2080 / 05 / 15  ");
        Assert.Equal(2080, date.Year);
        Assert.Equal(5, date.Month);
        Assert.Equal(15, date.Day);
    }

    #endregion

    #region SmartDateParser Month Name Formats

    [Theory]
    [InlineData("15 Baisakh 2080", 2080, 1, 15)]
    [InlineData("15 Jestha 2080", 2080, 2, 15)]
    [InlineData("15 Asar 2080", 2080, 3, 15)]
    [InlineData("15 Shrawan 2080", 2080, 4, 15)]
    [InlineData("15 Bhadra 2080", 2080, 5, 15)]
    [InlineData("15 Ashwin 2080", 2080, 6, 15)]
    [InlineData("15 Kartik 2080", 2080, 7, 15)]
    [InlineData("15 Mangsir 2080", 2080, 8, 15)]
    [InlineData("15 Poush 2080", 2080, 9, 15)]
    [InlineData("15 Magh 2080", 2080, 10, 15)]
    [InlineData("15 Falgun 2080", 2080, 11, 15)]
    [InlineData("15 Chaitra 2080", 2080, 12, 15)]
    public void SmartDateParser_AllMonthNames_ParseCorrectly(string input, int year, int month, int day)
    {
        var date = SmartDateParser.Parse(input);
        Assert.Equal(year, date.Year);
        Assert.Equal(month, date.Month);
        Assert.Equal(day, date.Day);
    }

    [Theory]
    [InlineData("Baisakh 15, 2080", 2080, 1, 15)]
    [InlineData("2080 Baisakh 15", 2080, 1, 15)]
    public void SmartDateParser_MonthNameDifferentPositions_ParsesCorrectly(string input, int year, int month, int day)
    {
        var date = SmartDateParser.Parse(input);
        Assert.Equal(year, date.Year);
        Assert.Equal(month, date.Month);
        Assert.Equal(day, date.Day);
    }

    [Theory]
    [InlineData("15 \u0935\u0948\u0936\u093E\u0916 2080", 2080, 1, 15)]  // वैशाख
    [InlineData("15 \u091C\u0947\u0937\u094D\u0920 2080", 2080, 2, 15)]  // जेष्ठ
    [InlineData("15 \u0905\u0938\u093E\u0930 2080", 2080, 3, 15)]  // असार
    [InlineData("15 \u0936\u094D\u0930\u093E\u0935\u0923 2080", 2080, 4, 15)]  // श्रावण
    public void SmartDateParser_NepaliUnicodeMonthNames_ParseCorrectly(string input, int year, int month, int day)
    {
        var date = SmartDateParser.Parse(input);
        Assert.Equal(year, date.Year);
        Assert.Equal(month, date.Month);
        Assert.Equal(day, date.Day);
    }

    #endregion

    #region SmartDateParser Ambiguous Formats

    [Fact]
    public void SmartDateParser_TwoDigitYear_AdjustedTo2000s()
    {
        var date = SmartDateParser.Parse("80/05/15");
        Assert.Equal(2080, date.Year);
    }

    [Fact]
    public void SmartDateParser_InvalidInput_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => SmartDateParser.Parse("not a date"));
    }

    [Fact]
    public void SmartDateParser_NullInput_ThrowsException()
    {
        Assert.ThrowsAny<Exception>(() => SmartDateParser.Parse(null));
    }

    [Fact]
    public void SmartDateParser_EmptyInput_ThrowsException()
    {
        Assert.ThrowsAny<Exception>(() => SmartDateParser.Parse(""));
    }

    [Fact]
    public void SmartDateParser_WhitespaceInput_ThrowsException()
    {
        Assert.ThrowsAny<Exception>(() => SmartDateParser.Parse("   "));
    }

    [Fact]
    public void SmartDateParser_TryParse_ValidInput_ReturnsTrue()
    {
        Assert.True(SmartDateParser.TryParse("2080/05/15", out var result));
        Assert.Equal(2080, result.Year);
    }

    [Fact]
    public void SmartDateParser_TryParse_InvalidInput_ReturnsFalse()
    {
        Assert.False(SmartDateParser.TryParse("garbage", out _));
    }

    [Fact]
    public void SmartDateParser_TryParse_NullInput_ReturnsFalse()
    {
        Assert.False(SmartDateParser.TryParse(null, out _));
    }

    #endregion

    #region Comparison Operators

    [Fact]
    public void Operator_Equality_SameDates_ReturnsTrue()
    {
        var a = new NepaliDate(2080, 5, 15);
        var b = new NepaliDate(2080, 5, 15);
        Assert.True(a == b);
        Assert.False(a != b);
    }

    [Fact]
    public void Operator_Inequality_DifferentDates_ReturnsTrue()
    {
        var a = new NepaliDate(2080, 5, 15);
        var b = new NepaliDate(2080, 5, 16);
        Assert.True(a != b);
        Assert.False(a == b);
    }

    [Fact]
    public void Operator_LessThan_EarlierDate_ReturnsTrue()
    {
        var earlier = new NepaliDate(2080, 5, 15);
        var later = new NepaliDate(2080, 5, 16);
        var sameAsEarlier = earlier;
        Assert.True(earlier < later);
        Assert.False(later < earlier);
        Assert.False(earlier < sameAsEarlier);
    }

    [Fact]
    public void Operator_LessThanOrEqual_SameAndEarlier_ReturnsTrue()
    {
        var earlier = new NepaliDate(2080, 5, 15);
        var later = new NepaliDate(2080, 5, 16);
        var sameAsEarlier = earlier;
        Assert.True(earlier <= later);
        Assert.True(earlier <= sameAsEarlier);
        Assert.False(later <= earlier);
    }

    [Fact]
    public void Operator_GreaterThan_LaterDate_ReturnsTrue()
    {
        var earlier = new NepaliDate(2080, 5, 15);
        var later = new NepaliDate(2080, 5, 16);
        var sameAsLater = later;
        Assert.True(later > earlier);
        Assert.False(earlier > later);
        Assert.False(later > sameAsLater);
    }

    [Fact]
    public void Operator_GreaterThanOrEqual_SameAndLater_ReturnsTrue()
    {
        var earlier = new NepaliDate(2080, 5, 15);
        var later = new NepaliDate(2080, 5, 16);
        var sameAsLater = later;
        Assert.True(later >= earlier);
        Assert.True(later >= sameAsLater);
        Assert.False(earlier >= later);
    }

    [Fact]
    public void Operator_MinMaxBoundary_CorrectOrdering()
    {
        Assert.True(NepaliDate.MinValue < NepaliDate.MaxValue);
        Assert.True(NepaliDate.MaxValue > NepaliDate.MinValue);
        Assert.True(NepaliDate.MinValue <= NepaliDate.MaxValue);
        Assert.True(NepaliDate.MaxValue >= NepaliDate.MinValue);
    }

    #endregion

    #region CompareTo

    [Fact]
    public void CompareTo_EarlierDate_ReturnsNegative()
    {
        var earlier = new NepaliDate(2080, 5, 15);
        var later = new NepaliDate(2080, 5, 16);
        Assert.True(earlier.CompareTo(later) < 0);
    }

    [Fact]
    public void CompareTo_SameDate_ReturnsZero()
    {
        var date = new NepaliDate(2080, 5, 15);
        Assert.Equal(0, date.CompareTo(date));
    }

    [Fact]
    public void CompareTo_LaterDate_ReturnsPositive()
    {
        var earlier = new NepaliDate(2080, 5, 15);
        var later = new NepaliDate(2080, 5, 16);
        Assert.True(later.CompareTo(earlier) > 0);
    }

    [Fact]
    public void CompareTo_DifferentYears_CorrectOrdering()
    {
        var y2080 = new NepaliDate(2080, 1, 1);
        var y2081 = new NepaliDate(2081, 1, 1);
        Assert.True(y2080.CompareTo(y2081) < 0);
    }

    [Fact]
    public void CompareTo_DifferentMonths_CorrectOrdering()
    {
        var m1 = new NepaliDate(2080, 1, 1);
        var m2 = new NepaliDate(2080, 2, 1);
        Assert.True(m1.CompareTo(m2) < 0);
    }

    #endregion

    #region Equals and GetHashCode

    [Fact]
    public void Equals_SameDates_ReturnsTrue()
    {
        var a = new NepaliDate(2080, 5, 15);
        var b = new NepaliDate(2080, 5, 15);
        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
    }

    [Fact]
    public void Equals_DifferentDates_ReturnsFalse()
    {
        var a = new NepaliDate(2080, 5, 15);
        var b = new NepaliDate(2080, 5, 16);
        Assert.False(a.Equals(b));
    }

    [Fact]
    public void GetHashCode_EqualDates_SameHashCode()
    {
        var a = new NepaliDate(2080, 5, 15);
        var b = new NepaliDate(2080, 5, 15);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void GetHashCode_StableAcrossMultipleCalls()
    {
        var date = new NepaliDate(2080, 5, 15);
        var hash1 = date.GetHashCode();
        var hash2 = date.GetHashCode();
        Assert.Equal(hash1, hash2);
    }

    #endregion

    #region Subtraction Operator

    [Fact]
    public void Subtraction_SameDate_ReturnsZeroDays()
    {
        var date = new NepaliDate(2080, 5, 15);
        Assert.Equal(0, (date - date).Days);
    }

    [Fact]
    public void Subtraction_AdjacentDays_ReturnsOneDay()
    {
        var a = new NepaliDate(2080, 5, 15);
        var b = new NepaliDate(2080, 5, 16);
        Assert.Equal(1, (b - a).Days);
        Assert.Equal(-1, (a - b).Days);
    }

    [Fact]
    public void Subtraction_CrossMonth_CorrectDays()
    {
        var firstOfBhadra = new NepaliDate(2080, 5, 1);
        var lastOfShrawan = new NepaliDate(2080, 4, 32);
        Assert.Equal(1, (firstOfBhadra - lastOfShrawan).Days);
    }

    [Fact]
    public void Subtraction_CrossYear_CorrectDays()
    {
        var firstOfYear = new NepaliDate(2081, 1, 1);
        var lastOfPrevYear = new NepaliDate(2080, 12, new NepaliDate(2080, 12, 1).MonthEndDay);
        Assert.Equal(1, (firstOfYear - lastOfPrevYear).Days);
    }

    #endregion

    #region IsLeapYear

    [Theory]
    [InlineData(2080, 1, 1, true)]   // English year 2023 April → 2024 is leap, but Baisakh 2080 starts April 2023
    [InlineData(2081, 1, 1, true)]    // Baisakh 2081 starts April 2024, 2024 IS leap
    [InlineData(2079, 1, 1, false)]   // Baisakh 2079 maps to April 2022, not leap
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "<Pending>")]
    public void IsLeapYear_ChecksEnglishYearCorrectly(int year, int month, int day, bool expected)
    {
        var date = new NepaliDate(year, month, day);
        // Verify via actual English year
        var engYear = date.EnglishDate.Year;
        var expectedFromEng = (engYear % 4 == 0 && (engYear % 100 != 0 || engYear % 400 == 0));
        Assert.Equal(expectedFromEng, date.IsLeapYear());
    }

    #endregion

    #region Constructor Edge Cases

    [Theory]
    [InlineData(2080, 0, 15)]
    [InlineData(2080, 13, 15)]
    [InlineData(2080, 5, 0)]
    [InlineData(2080, 5, 33)]
    [InlineData(1900, 1, 1)]
    [InlineData(2200, 1, 1)]
    public void Constructor_InvalidYearMonthDay_ThrowsException(int year, int month, int day)
    {
        Assert.ThrowsAny<Exception>(() => new NepaliDate(year, month, day));
    }

    [Fact]
    public void Constructor_MinValue_IsValid()
    {
        var min = NepaliDate.MinValue;
        Assert.Equal(1901, min.Year);
        Assert.Equal(1, min.Month);
        Assert.Equal(1, min.Day);
    }

    [Fact]
    public void Constructor_MaxValue_IsValid()
    {
        var max = NepaliDate.MaxValue;
        Assert.Equal(2199, max.Year);
        Assert.Equal(12, max.Month);
        Assert.True(max.Day >= 29 && max.Day <= 32);
    }

    #endregion

    #region Boundary English Date Conversions

    [Fact]
    public void EnglishDate_MinValue_ConvertsAndRoundtrips()
    {
        var min = NepaliDate.MinValue;
        var eng = min.EnglishDate.Date;
        var roundtrip = new NepaliDate(eng);
        Assert.Equal(min.Year, roundtrip.Year);
        Assert.Equal(min.Month, roundtrip.Month);
        Assert.Equal(min.Day, roundtrip.Day);
    }

    [Fact]
    public void EnglishDate_MaxValue_ConvertsAndRoundtrips()
    {
        var max = NepaliDate.MaxValue;
        var eng = max.EnglishDate.Date;
        var roundtrip = new NepaliDate(eng);
        Assert.Equal(max.Year, roundtrip.Year);
        Assert.Equal(max.Month, roundtrip.Month);
        Assert.Equal(max.Day, roundtrip.Day);
    }

    [Fact]
    public void EnglishDate_JustBeforeMin_ThrowsException()
    {
        var minEng = NepaliDate.MinValue.EnglishDate.Date;
        Assert.ThrowsAny<Exception>(() => new NepaliDate(minEng.AddDays(-1)));
    }

    [Fact]
    public void EnglishDate_JustAfterMax_ThrowsException()
    {
        var maxEng = NepaliDate.MaxValue.EnglishDate.Date;
        Assert.ThrowsAny<Exception>(() => new NepaliDate(maxEng.AddDays(1)));
    }

    #endregion

    #region BulkConvert Guards

    [Fact]
    public void BulkConvert_NullCollection_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => NepaliDate.BulkConvert.ToNepaliDates(null as IEnumerable<DateTime>));
        Assert.Throws<ArgumentNullException>(() => NepaliDate.BulkConvert.ToEnglishDates(null as IEnumerable<NepaliDate>));
        Assert.Throws<ArgumentNullException>(() => NepaliDate.BulkConvert.ToEnglishDates(null as IEnumerable<string>));
    }

    [Fact]
    public void BulkConvert_BatchSizeZero_ThrowsException()
    {
        var dates = new List<DateTime> { DateTime.Now };
        Assert.ThrowsAny<Exception>(() => NepaliDate.BulkConvert.BatchProcessToNepaliDates(dates, 0));
    }

    [Fact]
    public void BulkConvert_SingleElement_Works()
    {
        var dates = new List<DateTime> { new DateTime(2023, 8, 28) };
        var result = NepaliDate.BulkConvert.ToNepaliDates(dates);
        Assert.Single(result);
    }

    #endregion

    #region ToString Roundtrip Through All Months (Optimization Correctness)

    [Fact]
    public void ToString_EveryMonthFirstDay_RoundtripsViaConstructor()
    {
        for (int year = 1901; year <= 2199; year++)
        {
            for (int month = 1; month <= 12; month++)
            {
                var original = new NepaliDate(year, month, 1);
                var str = original.ToString();
                var parsed = new NepaliDate(str);

                Assert.Equal(original.Year, parsed.Year);
                Assert.Equal(original.Month, parsed.Month);
                Assert.Equal(original.Day, parsed.Day);
            }
        }
    }

    #endregion

    #region Unicode Conversion Roundtrip

    [Fact]
    public void UnicodeConversion_AllDigits_RoundtripCorrectly()
    {
        // Verify that converting to unicode and parsing back gives the same date
        for (int year = 2070; year <= 2090; year++)
        {
            for (int month = 1; month <= 12; month++)
            {
                var original = new NepaliDate(year, month, 15);
                var unicode = original.ToUnicodeString();

                // Parse the unicode string back via SmartDateParser
                var parsed = SmartDateParser.Parse(unicode);
                Assert.Equal(original.Year, parsed.Year);
                Assert.Equal(original.Month, parsed.Month);
                Assert.Equal(original.Day, parsed.Day);
            }
        }
    }

    #endregion

    #region MonthEndDay Edge Cases

    [Fact]
    public void MonthEndDay_CreatingDateWithExactEndDay_Succeeds()
    {
        for (int year = 2070; year <= 2090; year++)
        {
            for (int month = 1; month <= 12; month++)
            {
                var firstDay = new NepaliDate(year, month, 1);
                int endDay = firstDay.MonthEndDay;

                // Should succeed: create date with exact last day
                var lastDay = new NepaliDate(year, month, endDay);
                Assert.Equal(endDay, lastDay.Day);

                // Should fail: one day past the end
                Assert.ThrowsAny<Exception>(() => new NepaliDate(year, month, endDay + 1));
            }
        }
    }

    #endregion

    #region DayOfWeek Consistency

    [Fact]
    public void DayOfWeek_MatchesEnglishDateDayOfWeek()
    {
        // Sample 100 dates spread across the range
        for (int year = 1910; year <= 2190; year += 30)
        {
            for (int month = 1; month <= 12; month += 3)
            {
                var date = new NepaliDate(year, month, 15);
                var engDate = date.EnglishDate;
                Assert.Equal(engDate.DayOfWeek, date.DayOfWeek);
            }
        }
    }

    #endregion

    #region Properties IsDefault

    [Fact]
    public void IsDefault_DefaultStruct_ReturnsTrue()
    {
        var d = default(NepaliDate);
        Assert.True(d.IsDefault);
    }

    [Fact]
    public void IsDefault_ValidDate_ReturnsFalse()
    {
        var d = new NepaliDate(2080, 5, 15);
        Assert.False(d.IsDefault);
    }

    #endregion

    #region ToLongDateString Verification

    [Fact]
    public void ToLongDateString_Default_ContainsMonthNameAndDay()
    {
        var date = new NepaliDate(2080, 5, 15);
        var longStr = date.ToLongDateString();
        Assert.Contains("15", longStr);
        Assert.Contains("2080", longStr);
        Assert.Contains(",", longStr);
    }

    [Fact]
    public void ToLongDateString_WithDayName_ContainsDayOfWeek()
    {
        var date = new NepaliDate(2080, 5, 15);
        var dayName = date.DayOfWeek.ToString();
        var longStr = date.ToLongDateString(displayDayName: true);
        Assert.Contains(dayName, longStr);
    }

    [Fact]
    public void ToLongDateString_WithoutYear_DoesNotContainYear()
    {
        var date = new NepaliDate(2080, 5, 15);
        var longStr = date.ToLongDateString(displayYear: false);
        Assert.DoesNotContain("2080", longStr);
    }

    #endregion
}
