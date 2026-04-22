#if NET7_0_OR_GREATER
using System;

namespace NepDate
{
    public readonly partial struct NepaliDate : IParsable<NepaliDate>, ISpanParsable<NepaliDate>
    {
        /// <summary>
        /// Parses a string into a <see cref="NepaliDate"/>, implementing <see cref="IParsable{T}"/> (.NET 7+).
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="provider">Format provider. Currently ignored.</param>
        /// <returns>A <see cref="NepaliDate"/> parsed from <paramref name="s"/>.</returns>
        /// <exception cref="InvalidNepaliDateFormatException">Thrown when <paramref name="s"/> is not a valid Nepali date string.</exception>
        static NepaliDate IParsable<NepaliDate>.Parse(string s, IFormatProvider provider)
            => Parse(s);

        /// <summary>
        /// Attempts to parse a string into a <see cref="NepaliDate"/>, implementing <see cref="IParsable{T}"/> (.NET 7+).
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="provider">Format provider. Currently ignored.</param>
        /// <param name="result">When this method returns <see langword="true"/>, contains the parsed date; otherwise the default value.</param>
        /// <returns><see langword="true"/> if parsing succeeded; otherwise <see langword="false"/>.</returns>
        static bool IParsable<NepaliDate>.TryParse(string s, IFormatProvider provider, out NepaliDate result)
            => TryParse(s, out result);

        /// <summary>
        /// Parses a span of characters into a <see cref="NepaliDate"/>, implementing <see cref="ISpanParsable{T}"/> (.NET 7+).
        /// </summary>
        /// <param name="s">The span of characters to parse.</param>
        /// <param name="provider">Format provider. Currently ignored.</param>
        /// <returns>A <see cref="NepaliDate"/> parsed from <paramref name="s"/>.</returns>
        /// <exception cref="InvalidNepaliDateFormatException">Thrown when <paramref name="s"/> is not a valid Nepali date string.</exception>
        static NepaliDate ISpanParsable<NepaliDate>.Parse(ReadOnlySpan<char> s, IFormatProvider provider)
            => Parse(new string(s));

        /// <summary>
        /// Attempts to parse a span of characters into a <see cref="NepaliDate"/>, implementing <see cref="ISpanParsable{T}"/> (.NET 7+).
        /// </summary>
        /// <param name="s">The span of characters to parse.</param>
        /// <param name="provider">Format provider. Currently ignored.</param>
        /// <param name="result">When this method returns <see langword="true"/>, contains the parsed date; otherwise the default value.</param>
        /// <returns><see langword="true"/> if parsing succeeded; otherwise <see langword="false"/>.</returns>
        static bool ISpanParsable<NepaliDate>.TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out NepaliDate result)
            => TryParse(new string(s), out result);
    }
}
#endif
