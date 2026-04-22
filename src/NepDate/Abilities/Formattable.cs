using NepDate.Core.Dictionaries;
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
        /// Core long-date formatter used by both <see cref="ToLongDateString(bool, bool, bool)"/>
        /// and <see cref="ToLongDateUnicodeString(bool, bool, bool)"/>.
        /// </summary>
        /// <param name="leadingZeros">When <see langword="true"/>, pads day and month numbers with a leading zero.</param>
        /// <param name="displayDayName">When <see langword="true"/>, prepends the weekday name.</param>
        /// <param name="displayYear">When <see langword="true"/>, appends the year after the month and day.</param>
        /// <param name="isUnicode">When <see langword="true"/>, returns all text in Devanagari (month name, weekday name, and digits).</param>
        /// <returns>A formatted long-form date string.</returns>
        private string ToLongDateString(bool leadingZeros = true, bool displayDayName = false, bool displayYear = true, bool isUnicode = false)
        {
            (var yearStr, var monthStr, var dayStr) = (GetLeadedString(Year, leadingZeros, true, true, isUnicode), GetLeadedString(Month, leadingZeros, true, false, true, isUnicode), GetLeadedString(Day, leadingZeros, true, isUnicode: isUnicode));

            var longDate = $"{monthStr} {dayStr}";

            if (displayYear)
            {
                longDate += $", {yearStr}";
            }

            if (displayDayName)
            {
                var weekDayName = DayOfWeek.ToString();
                if (isUnicode)
                {
                    weekDayName = ConvertWordsToNepaliUnicode(weekDayName);
                }

                longDate = $"{weekDayName}, {longDate}";
            }
            return longDate;
        }
        /// <summary>
        /// Converts a single date component (year, month, or day) to its display string,
        /// applying zero-padding or month-name substitution as configured.
        /// </summary>
        private string GetLeadedString(int datePart, bool leadingZeros, bool displayMonthName = false, bool isYear = false, bool isMonth = false, bool isUnicode = false)
        {
            if (isMonth && displayMonthName)
            {
                if (isUnicode)
                {
                    return ConvertWordsToNepaliUnicode(((NepaliMonths)datePart).ToString());
                }
                else
                {
                    return ((NepaliMonths)datePart).ToString();
                }
            }

            return leadingZeros ? (isYear ? $"{datePart:D4}" : $"{datePart:D2}") : $"{datePart}";
        }
        /// <summary>
        /// Replaces every ASCII digit (0–9) in <paramref name="date"/> with its Devanagari
        /// equivalent (०–९). Non-digit characters such as separators are preserved unchanged.
        /// </summary>
        private string ConvertDigitsToNepaliUnicode(string date)
        {
            var chars = new char[date.Length];
            for (int i = 0; i < date.Length; i++)
            {
                char c = date[i];
                chars[i] = (c >= '0' && c <= '9') ? (char)('\u0966' + (c - '0')) : c;
            }
            return new string(chars);
        }

        /// <summary>
        /// Looks up the Devanagari translation of an English word (month name or weekday name)
        /// in the Unicode mapping table. Returns the original value unchanged if no mapping exists.
        /// </summary>
        private string ConvertWordsToNepaliUnicode(string value)
        {
            var converted = Unicode.data.TryGetValue(value, out var nepaliUnicode);

            return converted ? nepaliUnicode : value;
        }
        /// <summary>
        /// Writes a non-negative integer into a pre-allocated character buffer using a fixed-width,
        /// zero-padded representation. Used by <see cref="ToString()"/> for allocation-free formatting.
        /// </summary>
        private static void WriteDigits(char[] buffer, int offset, int value, int width)
        {
            for (int i = width - 1; i >= 0; i--)
            {
                buffer[offset + i] = (char)('0' + (value % 10));
                value /= 10;
            }
        }
        #endregion


        /// <summary>
        /// Returns a string that represents the current NepaliDate object in the format "yyyy/MM/dd".
        /// </summary>
        /// <returns>A string that represents the current NepaliDate object in the format "yyyy/MM/dd".</returns>
        public override string ToString()
        {
            var chars = new char[10];
            WriteDigits(chars, 0, Year, 4);
            chars[4] = '/';
            WriteDigits(chars, 5, Month, 2);
            chars[7] = '/';
            WriteDigits(chars, 8, Day, 2);
            return new string(chars);
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
            (var yearStr, var monthStr, var dayStr) = (GetLeadedString(Year, leadingZeros, isYear: true), GetLeadedString(Month, leadingZeros, isMonth: true), GetLeadedString(Day, leadingZeros));

            var separatorStr = GetSeparatorStr();


            switch (dateFormat)
            {
                case DateFormats.YearMonthDay: return AddSeparators(yearStr, monthStr, dayStr);
                case DateFormats.YearDayMonth: return AddSeparators(yearStr, dayStr, monthStr);
                case DateFormats.MonthDayYear: return AddSeparators(monthStr, dayStr, yearStr);
                case DateFormats.MonthYearDay: return AddSeparators(monthStr, yearStr, dayStr);
                case DateFormats.DayMonthYear: return AddSeparators(dayStr, monthStr, yearStr);
                case DateFormats.DayYearMonth: return AddSeparators(dayStr, yearStr, monthStr);
                default:
                    throw new InvalidNepaliDateFormatException();
            }


            string GetSeparatorStr()
            {
                switch (separator)
                {
                    case Separators.ForwardSlash: return "/";
                    case Separators.BackwardSlash: return "\\";
                    case Separators.Dash: return "-";
                    case Separators.Dot: return ".";
                    case Separators.Underscore: return "_";
                    case Separators.Space: return " ";
                    default:
                        throw new InvalidNepaliDateFormatException("Invalid separator value");
                }
            }

            string AddSeparators(string firstPart, string secondPart, string thirdPart)
            {
                return $"{firstPart}{separatorStr}{secondPart}{separatorStr}{thirdPart}";
            }
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
            return ConvertDigitsToNepaliUnicode(ToString(dateFormat, separator, leadingZeros));
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
            return ConvertDigitsToNepaliUnicode(ToLongDateString(leadingZeros, displayDayName, displayYear, true));
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
                    if (run >= 4) sb.Append(((NepaliMonths)Month).ToString());
                    else if (run == 3) sb.Append(((NepaliMonths)Month).ToString().Substring(0, 3));
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
