using System;

namespace NepDate.Core.Text
{
    /// <summary>
    /// A small fixed-capacity character buffer used to build date strings without intermediate
    /// string allocations. It writes to the stack on modern targets and to one array on
    /// .NET Standard 2.0, where <c>Span&lt;char&gt;</c> is not available.
    /// </summary>
    internal ref struct CharBuffer
    {
        /// <summary>The first Devanagari digit, ० (U+0966).</summary>
        internal const char NepaliZero = '०';

#if NETSTANDARD2_0
        private readonly char[] _chars;

        internal CharBuffer(char[] chars)
        {
            _chars = chars;
            _length = 0;
        }
#else
        private readonly Span<char> _chars;

        internal CharBuffer(Span<char> chars)
        {
            _chars = chars;
            _length = 0;
        }
#endif

        private int _length;

        internal void Append(char value)
        {
            _chars[_length++] = value;
        }

        internal void Append(string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                _chars[_length + i] = value[i];
            }

            _length += value.Length;
        }

        /// <summary>
        /// Appends a non-negative number, left-padded to <paramref name="minimumWidth"/> digits.
        /// </summary>
        /// <param name="value">The number to write.</param>
        /// <param name="minimumWidth">The smallest number of digits to write.</param>
        /// <param name="zero">The character of digit zero: <c>'0'</c>, or <see cref="NepaliZero"/> for Devanagari digits.</param>
        internal void AppendNumber(int value, int minimumWidth, char zero)
        {
            var width = 1;

            for (var rest = value / 10; rest != 0; rest /= 10)
            {
                width++;
            }

            if (width < minimumWidth)
            {
                width = minimumWidth;
            }

            for (int i = _length + width - 1; i >= _length; i--)
            {
                _chars[i] = (char)(zero + value % 10);
                value /= 10;
            }

            _length += width;
        }

        public override string ToString()
        {
#if NETSTANDARD2_0
            return new string(_chars, 0, _length);
#else
            return new string(_chars.Slice(0, _length));
#endif
        }
    }
}
