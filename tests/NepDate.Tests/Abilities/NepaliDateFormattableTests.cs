namespace NepDate.Tests.Abilities;

public class NepaliDateFormattableTests
{
    private readonly NepaliDate _date = new(2081, 4, 15);

    // --- Predefined specifiers ---

    [Fact]
    public void ToString_NullFormat_ReturnsDefault()
    {
        string result = ((IFormattable)_date).ToString(null, null);
        Assert.Equal("2081/04/15", result);
    }

    [Fact]
    public void ToString_G_SameAsDefault()
    {
        Assert.Equal(_date.ToString(), ((IFormattable)_date).ToString("G", null));
    }

    [Fact]
    public void ToString_g_SameAsDefault()
    {
        Assert.Equal(_date.ToString(), ((IFormattable)_date).ToString("g", null));
    }

    [Fact]
    public void ToString_d_SameAsDefault()
    {
        Assert.Equal("2081/04/15", ((IFormattable)_date).ToString("d", null));
    }

    [Fact]
    public void ToString_D_ReturnsLongDateString()
    {
        var result = ((IFormattable)_date).ToString("D", null);
        Assert.Contains("2081", result);
        Assert.Contains("Shrawan", result);
    }

    [Fact]
    public void ToString_s_ReturnsSortableIsoFormat()
    {
        Assert.Equal("2081-04-15", ((IFormattable)_date).ToString("s", null));
    }

    // --- Custom format tokens ---

    [Fact]
    public void CustomFormat_yyyy_MM_dd_WithDash()
    {
        Assert.Equal("2081-04-15", _date.ToString("yyyy-MM-dd", null));
    }

    [Fact]
    public void CustomFormat_dd_MM_yyyy_WithDot()
    {
        Assert.Equal("15.04.2081", _date.ToString("dd.MM.yyyy", null));
    }

    [Fact]
    public void CustomFormat_MM_slash_dd_slash_yyyy()
    {
        Assert.Equal("04/15/2081", _date.ToString("MM/dd/yyyy", null));
    }

    [Fact]
    public void CustomFormat_MMMxxx_ReturnsThreeCharMonthAbbreviation()
    {
        var result = _date.ToString("MMM yyyy", null);
        Assert.StartsWith("Shr", result);
        Assert.EndsWith("2081", result);
    }

    [Fact]
    public void CustomFormat_MMMM_ReturnsFullMonthName()
    {
        var result = _date.ToString("MMMM dd, yyyy", null);
        Assert.StartsWith("Shrawan", result);
    }

    [Fact]
    public void CustomFormat_yy_ReturnsTwoDigitYear()
    {
        var result = _date.ToString("yy/MM/dd", null);
        Assert.StartsWith("81/", result);
    }

    [Fact]
    public void CustomFormat_M_WithoutLeadingZero_ReturnsUnpadded()
    {
        // Month 4 — single M should give "4" not "04"
        var result = _date.ToString("M/d/yyyy", null);
        Assert.Equal("4/15/2081", result);
    }

    [Fact]
    public void CustomFormat_SingleDigitMonth_WithoutLeadingZero()
    {
        var jan = new NepaliDate(2081, 1, 5);
        var result = jan.ToString("M/d/yyyy", null);
        Assert.Equal("1/5/2081", result);
    }

    [Fact]
    public void CustomFormat_LiteralInSingleQuotes()
    {
        var result = _date.ToString("yyyy'BS'", null);
        Assert.Equal("2081BS", result);
    }

    [Fact]
    public void CustomFormat_BackslashEscapedTokens_ProduceLiterals()
    {
        // \M → literal 'M', \d → literal 'd'
        var result = _date.ToString(@"yyyy\M\M-\d\d", null);
        Assert.Equal("2081MM-dd", result);
    }

    [Fact]
    public void CustomFormat_BackslashBeforeLiteralChar_IsPassedThrough()
    {
        // \- just produces '-' (same as bare '-', since '-' is not a format token)
        var result = _date.ToString(@"yyyy\-MM\-dd", null);
        Assert.Equal("2081-04-15", result);
    }

    // --- IFormattable contract in string interpolation ---

    [Fact]
    public void StringInterpolation_WithFormatSpecifier_UsesIFormattable()
    {
        string result = $"{_date:s}";
        Assert.Equal("2081-04-15", result);
    }

    [Fact]
    public void StringFormatMethod_WithSpecifier()
    {
        string result = string.Format("{0:yyyy-MM-dd}", _date);
        Assert.Equal("2081-04-15", result);
    }

    // --- Sortability guarantee of "s" format ---

    [Fact]
    public void SortableFormat_LexicographicOrderEqualsChronologicalOrder()
    {
        var earlier = new NepaliDate(2080, 12, 30);
        var later = new NepaliDate(2081, 1, 1);

        string s1 = earlier.ToString("s", null);
        string s2 = later.ToString("s", null);

        Assert.True(string.CompareOrdinal(s1, s2) < 0, "Lexicographic order must equal chronological order for 's' format.");
    }
}
