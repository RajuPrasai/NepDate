namespace NepDate.Tests.Abilities;

public class NepaliDateComparableTests
{
    private readonly NepaliDate _d1 = new(2081, 1, 1);
    private readonly NepaliDate _d2 = new(2081, 6, 15);
    private readonly NepaliDate _d3 = new(2082, 1, 1);

    // --- IComparable (non-generic) ---

    [Fact]
    public void CompareTo_Object_Earlier_ReturnsNegative()
    {
        Assert.True(((IComparable)_d1).CompareTo(_d2) < 0);
    }

    [Fact]
    public void CompareTo_Object_Same_ReturnsZero()
    {
        Assert.Equal(0, ((IComparable)_d1).CompareTo(new NepaliDate(2081, 1, 1)));
    }

    [Fact]
    public void CompareTo_Object_Later_ReturnsPositive()
    {
        Assert.True(((IComparable)_d3).CompareTo(_d1) > 0);
    }

    [Fact]
    public void CompareTo_Object_Null_ReturnsPositive()
    {
        Assert.True(((IComparable)_d1).CompareTo(null) > 0);
    }

    [Fact]
    public void CompareTo_WrongType_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => ((IComparable)_d1).CompareTo("2081/01/01"));
    }

    // --- IComparable<NepaliDate> ---

    [Fact]
    public void CompareTo_Generic_Earlier_ReturnsNegative()
    {
        Assert.True(_d1.CompareTo(_d2) < 0);
    }

    [Fact]
    public void CompareTo_Generic_Same_ReturnsZero()
    {
        Assert.Equal(0, _d1.CompareTo(new NepaliDate(2081, 1, 1)));
    }

    [Fact]
    public void CompareTo_Generic_Later_ReturnsPositive()
    {
        Assert.True(_d3.CompareTo(_d1) > 0);
    }

    // --- SortedSet / Array.Sort via IComparable ---

    [Fact]
    public void SortedSet_OrdersCorrectly()
    {
        var set = new SortedSet<NepaliDate> { _d3, _d1, _d2 };
        var ordered = new List<NepaliDate>(set);
        Assert.Equal(_d1, ordered[0]);
        Assert.Equal(_d2, ordered[1]);
        Assert.Equal(_d3, ordered[2]);
    }

    [Fact]
    public void ArraySort_ViaIComparable_OrdersCorrectly()
    {
        object[] dates = { _d3, _d1, _d2 };
        Array.Sort(dates);
        Assert.Equal(_d1, dates[0]);
        Assert.Equal(_d2, dates[1]);
        Assert.Equal(_d3, dates[2]);
    }

    [Fact]
    public void List_Sort_OrdersCorrectly()
    {
        var list = new List<NepaliDate> { _d3, _d1, _d2 };
        list.Sort();
        Assert.Equal(_d1, list[0]);
        Assert.Equal(_d2, list[1]);
        Assert.Equal(_d3, list[2]);
    }
}
