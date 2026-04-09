using NepDate.Core.Dictionaries;
using NepDate.Exceptions;
using System;
using System.Text;

namespace NepDate
{
    public readonly partial struct NepaliDate
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
            string[] nepaliDigits = { "०", "१", "२", "३", "४", "५", "६", "७", "८", "९" };
            var nepaliUnicode = new StringBuilder(date.Length);

            foreach (var digit in date)
            {
                if (char.IsDigit(digit))
                {
                    var digitValue = int.Parse(digit.ToString());
                    nepaliUnicode.Append(nepaliDigits[digitValue]);
                }
                else
                {
                    nepaliUnicode.Append(digit);
                }
            }

            return nepaliUnicode.ToString();
        }

        private string ConvertWordsToNepaliUnicode(string value)
        {
            var converted = Unicode.data.TryGetValue(value, out var nepaliUnicode);

            return converted ? nepaliUnicode : value;
        }
        #endregion


        /// <summary>
        /// Returns a string that represents the current NepaliDate object in the format "yyyy/MM/dd".
        /// </summary>
        /// <returns>A string that represents the current NepaliDate object in the format "yyyy/MM/dd".</returns>
        public override string ToString()
        {
            return $"{Year:D4}/{Month:D2}/{Day:D2}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateFormat"></param>
        /// <param name="separator"></param>
        /// <param name="leadingZeros"></param>
        /// <returns></returns>
        /// <exception cref="NepDateException.InvalidNepaliDateFormatException"></exception>
        /// <exception cref="NepDateException.InvalidNepaliDateFormatException"></exception>
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
                    throw new NepDateException.InvalidNepaliDateFormatException();
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
                        throw new NepDateException.InvalidNepaliDateFormatException("Invalid separator value");
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
        /// Determines whether this NepaliDate instance is equal to another NepaliDate instance.
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
