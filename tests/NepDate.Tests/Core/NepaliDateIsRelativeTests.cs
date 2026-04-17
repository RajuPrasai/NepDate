namespace NepDate.Tests.Core;

/// <summary>
/// Tests for the three relative-day predicates: IsToday, IsYesterday, IsTomorrow.
/// These are derived purely from the concept of "today = DateTime.Today converted to Nepali",
/// without reusing any of the three methods under test to compute expectations.
/// </summary>
public class NepaliDateIsRelativeTests
{
    // Anchor: today's Nepali date, constructed independently from DateTime.Today.
    private static NepaliDate TodayNepali => new NepaliDate(DateTime.Today);
    private static NepaliDate YesterdayNepali => new NepaliDate(DateTime.Today.AddDays(-1));
    private static NepaliDate TomorrowNepali => new NepaliDate(DateTime.Today.AddDays(1));

    // ---- IsToday ----

    [Fact]
    public void IsToday_TodaysDate_ReturnsTrue()
    {
        Assert.True(TodayNepali.IsToday());
    }

    [Fact]
    public void IsToday_YesterdaysDate_ReturnsFalse()
    {
        Assert.False(YesterdayNepali.IsToday());
    }

    [Fact]
    public void IsToday_TomorrowsDate_ReturnsFalse()
    {
        Assert.False(TomorrowNepali.IsToday());
    }

    [Fact]
    public void IsToday_ArbitraryPastDate_ReturnsFalse()
    {
        // 2050/01/01 is safely in the past and will never be "today" again.
        var pastDate = new NepaliDate(2050, 1, 1);
        Assert.False(pastDate.IsToday());
    }

    // ---- IsYesterday ----

    [Fact]
    public void IsYesterday_YesterdaysDate_ReturnsTrue()
    {
        Assert.True(YesterdayNepali.IsYesterday());
    }

    [Fact]
    public void IsYesterday_TodaysDate_ReturnsFalse()
    {
        Assert.False(TodayNepali.IsYesterday());
    }

    [Fact]
    public void IsYesterday_TomorrowsDate_ReturnsFalse()
    {
        Assert.False(TomorrowNepali.IsYesterday());
    }

    [Fact]
    public void IsYesterday_ArbitraryPastDate_ReturnsFalse()
    {
        var pastDate = new NepaliDate(2050, 1, 1);
        Assert.False(pastDate.IsYesterday());
    }

    // ---- IsTomorrow ----

    [Fact]
    public void IsTomorrow_TomorrowsDate_ReturnsTrue()
    {
        Assert.True(TomorrowNepali.IsTomorrow());
    }

    [Fact]
    public void IsTomorrow_TodaysDate_ReturnsFalse()
    {
        Assert.False(TodayNepali.IsTomorrow());
    }

    [Fact]
    public void IsTomorrow_YesterdaysDate_ReturnsFalse()
    {
        Assert.False(YesterdayNepali.IsTomorrow());
    }

    [Fact]
    public void IsTomorrow_ArbitraryPastDate_ReturnsFalse()
    {
        var pastDate = new NepaliDate(2050, 1, 1);
        Assert.False(pastDate.IsTomorrow());
    }

    // ---- Mutual exclusion: exactly one of the three can be true for any date ----

    [Fact]
    public void Today_OnlyIsToday_IsTrue()
    {
        var today = TodayNepali;
        Assert.True(today.IsToday());
        Assert.False(today.IsYesterday());
        Assert.False(today.IsTomorrow());
    }

    [Fact]
    public void Yesterday_OnlyIsYesterday_IsTrue()
    {
        var yesterday = YesterdayNepali;
        Assert.False(yesterday.IsToday());
        Assert.True(yesterday.IsYesterday());
        Assert.False(yesterday.IsTomorrow());
    }

    [Fact]
    public void Tomorrow_OnlyIsTomorrow_IsTrue()
    {
        var tomorrow = TomorrowNepali;
        Assert.False(tomorrow.IsToday());
        Assert.False(tomorrow.IsYesterday());
        Assert.True(tomorrow.IsTomorrow());
    }

    [Fact]
    public void TwoDaysAhead_NoneAreTrue()
    {
        var twoDaysAhead = new NepaliDate(DateTime.Today.AddDays(2));
        Assert.False(twoDaysAhead.IsToday());
        Assert.False(twoDaysAhead.IsYesterday());
        Assert.False(twoDaysAhead.IsTomorrow());
    }

    [Fact]
    public void TwoDaysBehind_NoneAreTrue()
    {
        var twoDaysBehind = new NepaliDate(DateTime.Today.AddDays(-2));
        Assert.False(twoDaysBehind.IsToday());
        Assert.False(twoDaysBehind.IsYesterday());
        Assert.False(twoDaysBehind.IsTomorrow());
    }
}
