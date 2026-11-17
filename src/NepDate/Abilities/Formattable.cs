using NepDate.Core.Dictionaries;
using NepDate.Core.Text;
using System;
using System.Text;

namespace NepDate
{
#if NET6_0_OR_GREATER
    public readonly partial struct NepaliDate : IFormattable, ISpanFormattable
#else
    public readonly partial struct NepaliDate : IFormattable
#endif
    {
        #region Private Methods
        /// <summary>
        /// Room for the longest formatted date, with space to spare:
        /// "Wednesday, Baishakh 05, 2080" is 28 characters.
        /// </summary>
        private const int _formatBufferSize = 48;

        /// <summary>
        /// Core long-date formatter used by both <see cref="ToLongDateString(bool, bool, bool)"/>
        /// and <see cref="ToLongDateUnicodeString(bool, bool, bool)"/>.
        /// </summary>
        /// <param name="leadingZeros">When <see langword="true"/>, pads the day number with a leading zero.</param>
        /// <param name="displayDayName">When <see langword="true"/>, prepends the weekday name.</param>
        /// <param name="displayYear">When <see langword="true"/>, appends the year after the month and day.</param>
        /// <param name="isUnicode">When <see langword="true"/>, returns all text in Devanagari (month name, weekday name, and digits).</param>
        /// <returns>A formatted long-form date string.</returns>
        private string ToLongDateString(bool leadingZeros, bool displayDayName, bool displayYear, bool isUnicode)
        {
#if NETSTANDARD2_0
            var buffer = new CharBuffer(new char[_formatBufferSize]);
#else
            var buffer = new CharBuffer(stackalloc char[_formatBufferSize]);
#endif
            var zero = isUnicode ? CharBuffer.NepaliZero : '0';

            if (displayDayName)
            {
                buffer.Append(CalendarNames.Weekday(DayOfWeek, isUnicode));
                buffer.Append(',');
                buffer.Append(' ');
            }

            var monthName = CalendarNames.Month(_month, isUnicode);
            if (monthName is null)
            {
                // Only a default instance has no month name; its month number is written as is.
                buffer.AppendNumber(_month, 1, zero);
            }
            else
            {
                buffer.Append(monthName);
            }

            buffer.Append(' ');
            buffer.AppendNumber(_day, leadingZeros ? 2 : 1, zero);

            if (displayYear)
            {
                buffer.Append(',');
                buffer.Append(' ');
                // The Devanagari form never pads the year. Real years have four digits either way.
                buffer.AppendNumber(_year, leadingZeros && !isUnicode ? 4 : 1, zero);
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Core numeric formatter used by <see cref="ToString(DateFormats, Separators, bool)"/>
        /// and <see cref="ToUnicodeString(DateFormats, Separators, bool)"/>.
        /// </summary>
        /// <param name="dateFormat">The order of the year, month and day.</param>
        /// <param name="separator">The separator between the components.</param>
        /// <param name="leadingZeros">When <see langword="true"/>, pads the year to four digits and the month and day to two.</param>
        /// <param name="zero">The character of digit zero: <c>'0'</c> or the Devanagari zero.</param>
        private string ToNumericString(DateFormats dateFormat, Separators separator, bool leadingZeros, char zero)
        {
            char separatorChar;
            switch (separator)
            {
                case Separators.ForwardSlash: separatorChar = '/'; break;
                case Separators.BackwardSlash: separatorChar = '\\'; break;
                case Separators.Dash: separatorChar = '-'; break;
                case Separators.Dot: separatorChar = '.'; break;
                case Separators.Underscore: separatorChar = '_'; break;
                case Separators.Space: separatorChar = ' '; break;
                default:
                    throw new InvalidNepaliDateFormatException("Invalid separator value");
            }

            int year = _year, month = _month, day = _day;
            var yearWidth = leadingZeros ? 4 : DigitCount(year);
            var monthWidth = leadingZeros || month > 9 ? 2 : 1;
            var dayWidth = leadingZeros || day > 9 ? 2 : 1;

            // Pack each component as value * 8 + width, in the order they are written.
            int first, second, third;
            switch (dateFormat)
            {
                case DateFormats.YearMonthDay: first = year * 8 + yearWidth; second = month * 8 + monthWidth; third = day * 8 + dayWidth; break;
                case DateFormats.YearDayMonth: first = year * 8 + yearWidth; second = day * 8 + dayWidth; third = month * 8 + monthWidth; break;
                case DateFormats.MonthDayYear: first = month * 8 + monthWidth; second = day * 8 + dayWidth; third = year * 8 + yearWidth; break;
                case DateFormats.MonthYearDay: first = month * 8 + monthWidth; second = year * 8 + yearWidth; third = day * 8 + dayWidth; break;
                case DateFormats.DayMonthYear: first = day * 8 + dayWidth; second = month * 8 + monthWidth; third = year * 8 + yearWidth; break;
                case DateFormats.DayYearMonth: first = day * 8 + dayWidth; second = year * 8 + yearWidth; third = month * 8 + monthWidth; break;
                default:
                    throw new InvalidNepaliDateFormatException();
            }

            // The exact length is known, so the digits go straight into the final string.
            var length = (first & 7) + (second & 7) + (third & 7) + 2;
#if NETSTANDARD2_0
            var chars = new char[length];
            WriteNumeric(chars, first, second, third, separatorChar, zero);
            return new string(chars);
#else
            return string.Create(length, (first, second, third, separatorChar, zero), static (chars, state) =>
                WriteNumeric(chars, state.first, state.second, state.third, state.separatorChar, state.zero));
#endif
        }

        /// <summary>
        /// Writes three packed components (value * 8 + width) with a separator between them.
        /// </summary>
#if NETSTANDARD2_0
        private static void WriteNumeric(char[] chars, int first, int second, int third, char separator, char zero)
#else
        private static void WriteNumeric(Span<char> chars, int first, int second, int third, char separator, char zero)
#endif
        {
            var position = WritePacked(chars, 0, first, zero);
            chars[position++] = separator;
            position = WritePacked(chars, position, second, zero);
            chars[position++] = separator;
            WritePacked(chars, position, third, zero);
        }

        /// <summary>
        /// Writes one packed component at <paramref name="start"/> and returns the position after it.
        /// </summary>
#if NETSTANDARD2_0
        private static int WritePacked(char[] chars, int start, int packed, char zero)
#else
        private static int WritePacked(Span<char> chars, int start, int packed, char zero)
#endif
        {
            var value = packed >> 3;
            var end = start + (packed & 7);

            for (var i = end - 1; i >= start; i--)
            {
                chars[i] = (char)(zero + value % 10);
                value /= 10;
            }

            return end;
        }

        /// <summary>The number of decimal digits in a year from 0 to 9999.</summary>
        private static int DigitCount(int value)
        {
            return value > 999 ? 4 : value > 99 ? 3 : value > 9 ? 2 : 1;
        }

        /// <summary>
        /// Returns the English month name used by custom format strings. A default instance,
        /// which has no month, yields its month number.
        /// </summary>
        private string EnglishMonthName()
        {
            return CalendarNames.Month(_month, nepali: false) ?? ((int)_month).ToString();
        }
        #endregion


        /// <summary>
        /// Returns a string that represents the current NepaliDate object in the format "yyyy/MM/dd".
        /// </summary>
        /// <returns>A string that represents the current NepaliDate object in the format "yyyy/MM/dd".</returns>
        public override string ToString()
        {
            // The most used format gets a fixed layout: no width counting, one allocation.
#if NETSTANDARD2_0
            var chars = new char[10];
            WriteCanonical(chars, _year, _month, _day);
            return new string(chars);
#else
            return string.Create(10, AsInteger, static (chars, value) =>
                WriteCanonical(chars, value / 10000, value / 100 % 100, value % 100));
#endif
        }

        /// <summary>
        /// Writes "yyyy/MM/dd" into the first 10 characters of <paramref name="chars"/>.
        /// </summary>
#if NETSTANDARD2_0
        private static void WriteCanonical(char[] chars, int year, int month, int day)
#else
        private static void WriteCanonical(Span<char> chars, int year, int month, int day)
#endif
        {
            chars[0] = (char)('0' + year / 1000 % 10);
            chars[1] = (char)('0' + year / 100 % 10);
            chars[2] = (char)('0' + year / 10 % 10);
            chars[3] = (char)('0' + year % 10);
            chars[4] = '/';
            chars[5] = (char)('0' + month / 10);
            chars[6] = (char)('0' + month % 10);
            chars[7] = '/';
            chars[8] = (char)('0' + day / 10);
            chars[9] = (char)('0' + day % 10);
        }

        /// <summary>
        /// Returns the date formatted with a specific component order and separator.
        /// </summary>
        /// <param name="dateFormat">The order in which year, month, and day components are written.</param>
        /// <param name="separator">The character used to separate the components. Defaults to forward slash.</param>
        /// <param name="leadingZeros">When true, month and day numbers are zero-padded to two digits.</param>
        /// <returns>A formatted date string according to the specified format and separator.</returns>
        /// <exception cref="InvalidNepaliDateFormatException">Thrown when <paramref name="dateFormat"/> is not a defined enum value.</exception>
        /// <exception cref="InvalidNepaliDateFormatException">Thrown when <paramref name="separator"/> is not a defined enum value.</exception>
        public string ToString(DateFormats dateFormat, Separators separator = Separators.ForwardSlash, bool leadingZeros = true)
        {
            return ToNumericString(dateFormat, separator, leadingZeros, '0');
        }

        /// <summary>
        /// Returns the date as a long-form English string, e.g. "Baishakh 01, 2080" or "Monday, Baishakh 01, 2080".
        /// </summary>
        /// <param name="leadingZeros">When true, month and day numbers are zero-padded to two digits.</param>
        /// <param name="displayDayName">When true, the English weekday name is prepended.</param>
        /// <param name="displayYear">When true, the year is appended after the month and day.</param>
        /// <returns>A formatted long-form date string.</returns>
        public string ToLongDateString(bool leadingZeros = true, bool displayDayName = false, bool displayYear = true)
        {
            return ToLongDateString(leadingZeros, displayDayName, displayYear, false);
        }

        /// <summary>
        /// Returns the date as a numeric string with Nepali (Devanagari) Unicode digits, e.g. "२०८०/०१/१५".
        /// </summary>
        /// <param name="dateFormat">The order of the date components. Defaults to <see cref="DateFormats.YearMonthDay"/>.</param>
        /// <param name="separator">The character used to separate date components. Defaults to forward slash.</param>
        /// <param name="leadingZeros">When true, month and day numbers are zero-padded to two digits.</param>
        /// <returns>A date string with all digits converted to Nepali Unicode.</returns>
        public string ToUnicodeString(DateFormats dateFormat = DateFormats.YearMonthDay, Separators separator = Separators.ForwardSlash, bool leadingZeros = true)
        {
            return ToNumericString(dateFormat, separator, leadingZeros, CharBuffer.NepaliZero);
        }

        /// <summary>
        /// Returns the date as a long-form Nepali Unicode string with the month name and digits both in Devanagari,
        /// e.g. "वैशाख ०१, २०८०".
        /// </summary>
        /// <param name="leadingZeros">When true, month and day numbers are zero-padded to two digits.</param>
        /// <param name="displayDayName">When true, the Nepali weekday name is prepended.</param>
        /// <param name="displayYear">When true, the year is appended after the month and day.</param>
        /// <returns>A long-form date string fully rendered in Nepali Unicode.</returns>
        public string ToLongDateUnicodeString(bool leadingZeros = true, bool displayDayName = false, bool displayYear = true)
        {
            return ToLongDateString(leadingZeros, displayDayName, displayYear, true);
        }

        /// <summary>
        /// Formats this <see cref="NepaliDate"/> using the specified format string and optional format provider,
        /// implementing the <see cref="IFormattable"/> contract.
        /// </summary>
        /// <param name="format">
        /// A format string. Predefined specifiers:
        /// <list type="table">
        ///   <item><term><c>"d"</c> / <c>"G"</c> / <c>"g"</c></term><description>Default short date: <c>YYYY/MM/DD</c></description></item>
        ///   <item><term><c>"D"</c></term><description>Long date: <c>Baishakh 15, 2081</c></description></item>
        ///   <item><term><c>"s"</c></term><description>Sortable ISO: <c>YYYY-MM-DD</c></description></item>
        ///   <item><term>custom</term><description>Tokens: <c>yyyy</c>, <c>yy</c>, <c>MMMM</c>, <c>MMM</c>, <c>MM</c>, <c>M</c>, <c>dd</c>, <c>d</c>. Escape literals with <c>'...'</c> or <c>\</c>.</description></item>
        /// </list>
        /// Pass <see langword="null"/> or an empty string to use the default format (<c>"d"</c>).
        /// </param>
        /// <param name="formatProvider">Reserved for future locale support. Currently ignored.</param>
        /// <returns>A formatted date string.</returns>
        /// <example>
        /// <code>
        /// var date = new NepaliDate(2079, 2, 6);
        /// Console.WriteLine(date.ToString("d"));             // 2079/02/06
        /// Console.WriteLine(date.ToString("D"));             // Jestha 06, 2079
        /// Console.WriteLine(date.ToString("s"));             // 2079-02-06
        /// Console.WriteLine(date.ToString("MMMM dd, yyyy")); // Jestha 06, 2079
        /// Console.WriteLine(date.ToString("dd 'of' MMMM")); // 06 of Jestha
        /// Console.WriteLine($"{date:s}");                    // 2079-02-06
        /// </code>
        /// </example>
        public string ToString(string format, IFormatProvider formatProvider = null)
        {
            if (string.IsNullOrEmpty(format) || format == "G" || format == "g")
                return ToString();

            switch (format)
            {
                case "d": return ToString();
                case "D": return ToLongDateString(leadingZeros: true, displayDayName: false, displayYear: true);
                case "s": return ToString(DateFormats.YearMonthDay, Separators.Dash);
                default: return FormatCustom(format);
            }
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Attempts to format this <see cref="NepaliDate"/> directly into a character span,
        /// implementing <see cref="ISpanFormattable"/> for allocation-free formatting on .NET 6+.
        /// </summary>
        /// <param name="destination">The span to write into.</param>
        /// <param name="charsWritten">
        /// When this method returns <see langword="true"/>, contains the number of characters written;
        /// otherwise 0.
        /// </param>
        /// <param name="format">The format specifier. Same tokens as <see cref="ToString(string, IFormatProvider)"/>.</param>
        /// <param name="provider">Format provider. Currently ignored.</param>
        /// <returns>
        /// <see langword="true"/> if the formatted result fits within <paramref name="destination"/>;
        /// <see langword="false"/> when the destination is too small.
        /// </returns>
        bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider provider)
        {
            var result = ToString(format.Length == 0 ? null : new string(format), provider);
            if (result.Length > destination.Length)
            {
                charsWritten = 0;
                return false;
            }
            result.AsSpan().CopyTo(destination);
            charsWritten = result.Length;
            return true;
        }
#endif

        /// <summary>
        /// Expands a custom format pattern into a date string by replacing recognised token sequences
        /// (<c>yyyy</c>, <c>yy</c>, <c>MMMM</c>, <c>MMM</c>, <c>MM</c>, <c>M</c>, <c>dd</c>, <c>d</c>)
        /// with the corresponding date component values. Literal text may be wrapped in single quotes
        /// or preceded by a backslash.
        /// </summary>
        private string FormatCustom(string format)
        {
            var sb = new StringBuilder(format.Length + 4);
            int i = 0;
            while (i < format.Length)
            {
                char c = format[i];
                if (c == '\\' && i + 1 < format.Length)
                {
                    sb.Append(format[i + 1]);
                    i += 2;
                    continue;
                }
                if (c == '\'')
                {
                    i++;
                    while (i < format.Length && format[i] != '\'')
                        sb.Append(format[i++]);
                    if (i < format.Length) i++;
                    continue;
                }
                if (c == 'y')
                {
                    int run = CountRepeat(format, i, 'y');
                    sb.Append(run >= 4 ? $"{Year:D4}" : $"{Year % 100:D2}");
                    i += run;
                    continue;
                }
                if (c == 'M')
                {
                    int run = CountRepeat(format, i, 'M');
                    if (run >= 4) sb.Append(EnglishMonthName());
                    else if (run == 3) sb.Append(EnglishMonthName(), 0, 3);
                    else if (run == 2) sb.Append($"{Month:D2}");
                    else sb.Append(Month);
                    i += run;
                    continue;
                }
                if (c == 'd')
                {
                    int run = CountRepeat(format, i, 'd');
                    if (run == 2) sb.Append($"{Day:D2}");
                    else sb.Append(Day);
                    i += run;
                    continue;
                }
                sb.Append(c);
                i++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Counts consecutive occurrences of character <paramref name="c"/> in <paramref name="s"/>
        /// starting at <paramref name="start"/>. Used to determine the width of a format token,
        /// e.g. distinguishing <c>MM</c> (2) from <c>MMMM</c> (4).
        /// </summary>
        private static int CountRepeat(string s, int start, char c)
        {
            int count = 0;
            while (start + count < s.Length && s[start + count] == c)
                count++;
            return count;
        }
    }
}
