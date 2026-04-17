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
        /// Gets the integer representation of this Nepali date in the format "YYYYMMDD".
        /// </summary>

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

        private string ConvertWordsToNepaliUnicode(string value)
        {
            var converted = Unicode.data.TryGetValue(value, out var nepaliUnicode);

            return converted ? nepaliUnicode : value;
        }
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
        /// 
        /// </summary>
        /// <param name="dateFormat"></param>
        /// <param name="separator"></param>
        /// <param name="leadingZeros"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNepaliDateFormatException"></exception>
        /// <exception cref="InvalidNepaliDateFormatException"></exception>
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
        /// 
        /// </summary>
        /// <param name="leadingZeros"></param>
        /// <param name="displayDayName"></param>
        /// <param name="displayYear"></param>
        /// <returns></returns>
        public string ToLongDateString(bool leadingZeros = true, bool displayDayName = false, bool displayYear = true)
        {
            return ToLongDateString(leadingZeros, displayDayName, displayYear, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateFormat"></param>
        /// <param name="separator"></param>
        /// <param name="leadingZeros"></param>
        /// <returns></returns>
        public string ToUnicodeString(DateFormats dateFormat = DateFormats.YearMonthDay, Separators separator = Separators.ForwardSlash, bool leadingZeros = true)
        {
            return ConvertDigitsToNepaliUnicode(ToString(dateFormat, separator, leadingZeros));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leadingZeros"></param>
        /// <param name="displayDayName"></param>
        /// <param name="displayYear"></param>
        /// <returns></returns>
        public string ToLongDateUnicodeString(bool leadingZeros = true, bool displayDayName = false, bool displayYear = true)
        {
            return ConvertDigitsToNepaliUnicode(ToLongDateString(leadingZeros, displayDayName, displayYear, true));
        }

        /// <summary>
        /// Formats the value of the current NepaliDate instance using the specified format string.
        /// Supported predefined specifiers:
        ///   "d"  → YYYY/MM/DD (same as default ToString)
        ///   "D"  → Long date: "Baishakh 15, 2081"
        ///   "s"  → Sortable ISO-style: "YYYY-MM-DD" (canonical for EF/serialization)
        ///   "G"/"g" → same as "d"
        ///   custom → tokens: yyyy, yy, MMMM, MM, M, dd, d (as literal 'd' in custom position is treated as day token)
        /// Escape characters with backslash or single-quotes.
        /// </summary>
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

        private static int CountRepeat(string s, int start, char c)
        {
            int count = 0;
            while (start + count < s.Length && s[start + count] == c)
                count++;
            return count;
        }
        /// </summary>
        /// <param name="other">The NepaliDate to compare with the current instance.</param>
        /// <returns>true if the specified NepaliDate has the same value as the current instance; otherwise, false.</returns>
        public bool Equals(NepaliDate other)
        {
            return AsInteger == other.AsInteger;
        }

        /// <summary>
        /// Returns the hash code for this NepaliDate instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code based on the value of this NepaliDate instance.</returns>
        public override int GetHashCode()
        {
            return AsInteger;
        }
    }
}
