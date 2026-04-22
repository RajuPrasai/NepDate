using System.Diagnostics.CodeAnalysis;

namespace NepDate.Tests.Abilities;

public class NepaliDateParsableTests
{
    private readonly NepaliDate _expected = new(2081, 4, 15);

    // --- IParsable<T> via generic constraint ---

    [Fact]
    public void Parse_Via_GenericConstraint_Returns_CorrectDate()
    {
        NepaliDate result = ParseWithGeneric<NepaliDate>("2081-04-15");
        Assert.Equal(_expected, result);
    }

    [Fact]
    public void TryParse_Via_GenericConstraint_ValidDate_Returns_True()
    {
        bool ok = TryParseWithGeneric<NepaliDate>("2081/04/15", out var result);
        Assert.True(ok);
        Assert.Equal(_expected, result);
    }

    [Fact]
    public void TryParse_Via_GenericConstraint_InvalidDate_Returns_False()
    {
        bool ok = TryParseWithGeneric<NepaliDate>("garbage", out var result);
        Assert.False(ok);
        Assert.Equal(default, result);
    }

    // --- ISpanParsable<T> via generic constraint ---

    [Fact]
    public void SpanParse_Via_GenericConstraint_Returns_CorrectDate()
    {
        NepaliDate result = SpanParseWithGeneric<NepaliDate>("2081-04-15".AsSpan());
        Assert.Equal(_expected, result);
    }

    [Fact]
    public void SpanTryParse_Via_GenericConstraint_ValidDate_Returns_True()
    {
        bool ok = SpanTryParseWithGeneric<NepaliDate>("2081/04/15".AsSpan(), out var result);
        Assert.True(ok);
        Assert.Equal(_expected, result);
    }

    // --- Helpers that exercise the generic interfaces ---

#if NET7_0_OR_GREATER
    private static T ParseWithGeneric<T>(string s) where T : IParsable<T>
        => T.Parse(s, null);

    private static bool TryParseWithGeneric<T>(string s, [MaybeNull] out T result) where T : IParsable<T>
        => T.TryParse(s, null, out result);

    private static T SpanParseWithGeneric<T>(ReadOnlySpan<char> s) where T : ISpanParsable<T>
        => T.Parse(s, null);

    private static bool SpanTryParseWithGeneric<T>(ReadOnlySpan<char> s, [MaybeNull] out T result) where T : ISpanParsable<T>
        => T.TryParse(s, null, out result);
#else
    private static T ParseWithGeneric<T>(string s) where T : struct
    {
        // Fallback: call NepaliDate.Parse directly for pre-net7 TFM
        return (T)(object)NepaliDate.Parse(s);
    }

    private static bool TryParseWithGeneric<T>(string s, out T result) where T : struct
    {
        bool ok = NepaliDate.TryParse(s, out var r);
        result = (T)(object)r;
        return ok;
    }

    private static T SpanParseWithGeneric<T>(ReadOnlySpan<char> s) where T : struct
        => (T)(object)NepaliDate.Parse(new string(s));

    private static bool SpanTryParseWithGeneric<T>(ReadOnlySpan<char> s, out T result) where T : struct
    {
        bool ok = NepaliDate.TryParse(new string(s), out var r);
        result = (T)(object)r;
        return ok;
    }
#endif
}
