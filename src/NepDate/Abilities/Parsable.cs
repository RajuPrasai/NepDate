#if NET7_0_OR_GREATER
using System;

namespace NepDate
{
    public readonly partial struct NepaliDate : IParsable<NepaliDate>, ISpanParsable<NepaliDate>
    {
        static NepaliDate IParsable<NepaliDate>.Parse(string s, IFormatProvider provider)
            => Parse(s);

        static bool IParsable<NepaliDate>.TryParse(string s, IFormatProvider provider, out NepaliDate result)
            => TryParse(s, out result);

        static NepaliDate ISpanParsable<NepaliDate>.Parse(ReadOnlySpan<char> s, IFormatProvider provider)
            => Parse(new string(s));

        static bool ISpanParsable<NepaliDate>.TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out NepaliDate result)
            => TryParse(new string(s), out result);
    }
}
#endif
