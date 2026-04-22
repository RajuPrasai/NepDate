using NepDate.Core.Dictionaries;
using NepDate.TypeConversion;
using System;
using System.ComponentModel;

namespace NepDate
{
    /// <summary>
    /// Represents a date in the Nepali calendar system (Bikram Sambat).
    /// Provides methods for date calculations, conversions between Nepali and Gregorian calendars,
    /// and various date operations specific to the Nepali calendar.
    /// </summary>
    /// <remarks>
    /// The Nepali date structure supports dates from 1901 BS to 2199 BS.
    /// This is an immutable value type that behaves similar to System.DateTime.
    /// </remarks>
    [TypeConverter(typeof(NepaliDateTypeConverter))]
#if NET5_0_OR_GREATER
    [System.Text.Json.Serialization.JsonConverter(typeof(NepDate.Serialization.SystemTextJsonConverters.NepaliDateJsonConverter))]
#endif
    public readonly partial struct NepaliDate
    {
        #region Ctor
        /// <summary>
        /// Validates the date components and throws an exception for any that are out of range.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <see cref="Year"/> is outside 1901–2199, <see cref="Month"/> is outside 1–12,
        /// or <see cref="Day"/> exceeds the number of days in the given month.
        /// </exception>
        private void ValidateAndThrow()
        {
            if (Year < _minYear || Year > _maxYear)
            {
                throw new ArgumentOutOfRangeException(nameof(Year),
                    $"Year must be between {_minYear} and {_maxYear}. Got: {Year}.");
            }

            if (Month < 1 || Month > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(Month),
                    $"Month must be between 1 and 12. Got: {Month}.");
            }

            if (Day < 1 || Day > MonthEndDay)
            {
                throw new ArgumentOutOfRangeException(nameof(Day),
                    $"Day must be between 1 and {MonthEndDay} for {Year}/{Month:D2}. Got: {Day}.");
            }
        }

        /// <summary>
        /// Determines whether the given Nepali date components form a valid, in-range date
        /// without throwing an exception.
        /// </summary>
        /// <param name="year">The Nepali year to validate.</param>
        /// <param name="month">The Nepali month to validate (1–12).</param>
        /// <param name="day">The Nepali day to validate.</param>
        /// <returns>
        /// <see langword="true"/> if the combination is a real date within the supported
        /// 1901–2199 BS range; otherwise <see langword="false"/>.
        /// </returns>
        private static bool IsValidDate(int year, int month, int day)
        {
            if (year < _minYear || year > _maxYear)
                return false;
            if (month < 1 || month > 12)
                return false;
            if (!DictionaryBridge.NepToEng.TryGetNepaliMonthEndDay(year, month, out var monthEndDay))
                return false;
            if (day < 1 || day > monthEndDay)
                return false;
            return true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NepaliDate"/> struct with the specified Bikram Sambat components.
        /// </summary>
        /// <param name="yearBs">
        /// The Bikram Sambat year. Valid range: 1901–2199.
        /// </param>
        /// <param name="monthBs">
        /// The Nepali month: 1 (Baisakh) through 12 (Chaitra).
        /// </param>
        /// <param name="dayBs">
        /// The day within the month. Valid range depends on the specific month and year (typically 29–32).
        /// Use <see cref="MonthEndDay"/> to retrieve the last valid day for a given month.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="yearBs"/> is outside 1901–2199, <paramref name="monthBs"/> is outside 1–12,
        /// or <paramref name="dayBs"/> exceeds the number of days in the given month.
        /// </exception>
        /// <example>
        /// <code>
        /// var date = new NepaliDate(2080, 4, 15);  // 15 Shrawan 2080 BS
        /// Console.WriteLine(date);                  // 2080/04/15
        /// Console.WriteLine(date.EnglishDate);      // 07/31/2023
        /// </code>
        /// </example>
        public NepaliDate(int yearBs, int monthBs, int dayBs)
        {
            (Year, Month, Day) = (yearBs, monthBs, dayBs);
            _englishDate = null;
            ValidateAndThrow();
            _englishDate = DictionaryBridge.NepToEng.GetEnglishDate(yearBs, monthBs, dayBs);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NepaliDate"/> struct by parsing a Nepali date string.
        /// The components must be in year-month-day order.
        /// </summary>
        /// <param name="rawNepaliDate">
        /// A string containing a Nepali date, e.g. <c>"2080/04/15"</c>, <c>"2080-04-15"</c>,
        /// <c>"2080.04.15"</c>, <c>"2080_04_15"</c>, or <c>"2080 04 15"</c>.
        /// Accepted separators: <c>/</c>, <c>-</c>, <c>.</c>, <c>_</c>, <c>\</c>, space, <c>|</c>, and <c>।</c>.
        /// </param>
        /// <exception cref="InvalidNepaliDateFormatException">
        /// Thrown when <paramref name="rawNepaliDate"/> is <see langword="null"/> or empty, contains
        /// non-numeric characters other than the accepted separators, or does not resolve to a valid Nepali date.
        /// </exception>
        /// <remarks>
        /// For flexible format support (month names, Nepali digits, ambiguous component order),
        /// use <see cref="SmartDateParser.Parse(string)"/> or the <c>autoAdjust</c> overload instead.
        /// </remarks>
        /// <example>
        /// <code>
        /// var a = new NepaliDate("2080/04/15");
        /// var b = new NepaliDate("2080-04-15");
        /// var c = new NepaliDate("2080.04.15");
        /// Console.WriteLine(a == b &amp;&amp; b == c);  // True
        /// </code>
        /// </example>
        public NepaliDate(string rawNepaliDate)
        {
            (Year, Month, Day) = SplitNepaliDate(rawNepaliDate);
            _englishDate = null;
            ValidateAndThrow();
            _englishDate = DictionaryBridge.NepToEng.GetEnglishDate(Year, Month, Day);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NepaliDate"/> struct by parsing a Nepali date string
        /// with optional heuristic correction for ambiguous or non-standard component ordering.
        /// </summary>
        /// <param name="rawNepaliDate">The Nepali date string to parse.</param>
        /// <param name="autoAdjust">
        /// When <see langword="true"/>, applies heuristics to recover valid dates from ambiguous input:
        /// <list type="bullet">
        ///   <item><description>If the day component exceeds 32, the day and year are swapped.</description></item>
        ///   <item><description>If the month exceeds 12 and the day is below 13, the month and day are swapped.</description></item>
        ///   <item><description>If the year has fewer than four digits, the current millennium (2000) is prepended.</description></item>
        /// </list>
        /// </param>
        /// <param name="monthInMiddle">
        /// Relevant only when <paramref name="autoAdjust"/> is <see langword="true"/>.
        /// When <see langword="false"/>, the method first swaps the month and day components before
        /// applying other adjustments, allowing DD/YY/MM-style strings to be handled.
        /// Defaults to <see langword="true"/> (month is already in the middle position).
        /// </param>
        /// <exception cref="InvalidNepaliDateFormatException">
        /// Thrown when <paramref name="rawNepaliDate"/> is <see langword="null"/> or empty,
        /// or when the (possibly adjusted) components do not form a valid Nepali date.
        /// </exception>
        /// <example>
        /// <code>
        /// // 3-digit year expands: "077" becomes 2077
        /// var a = new NepaliDate("25-05-077", autoAdjust: true);   // 2077/05/25
        ///
        /// // Month not in middle: "05/06/2077" is treated as day=05, year=06, month=2077
        /// var b = new NepaliDate("05/06/2077", autoAdjust: true, monthInMiddle: false); // 2077/05/06
        /// </code>
        /// </example>
        public NepaliDate(string rawNepaliDate, bool autoAdjust, bool monthInMiddle = true)
        {
            const int currentMillennium = 2;

            (Year, Month, Day) = SplitNepaliDate(rawNepaliDate);

            if (autoAdjust)
            {
                if (Day > 32)
                {
                    (Year, Day) = (Day, Year);
                }

                if (!monthInMiddle)
                {
                    (Month, Day) = (Day, Month);
                }

                if (Month > 12 && Day < 13)
                {
                    (Month, Day) = (Day, Month);
                }

                if (Year < 1000)
                {
                    Year = currentMillennium * 1000 + Year;
                }
            }
            _englishDate = null;
            ValidateAndThrow();
            _englishDate = DictionaryBridge.NepToEng.GetEnglishDate(Year, Month, Day);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NepaliDate"/> struct by converting a Gregorian
        /// <see cref="DateTime"/> to its Bikram Sambat equivalent.
        /// </summary>
        /// <param name="engDate">
        /// The Gregorian date to convert. Only the date portion is used; any time component is ignored.
        /// The date must fall on or after 1901-04-13 and on or before 2143-04-12 (the Gregorian equivalents
        /// of <see cref="MinValue"/> and <see cref="MaxValue"/>).
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="engDate"/> falls before the Gregorian equivalent of
        /// <see cref="MinValue"/> (1901-04-13) or after that of <see cref="MaxValue"/> (2143-04-12).
        /// </exception>
        /// <remarks>
        /// The conversion uses precomputed mapping tables for O(1) performance with zero heap allocation.
        /// For converting many dates at once, prefer <see cref="BulkConvert.ToNepaliDates"/> to amortize
        /// overhead across the collection.
        /// </remarks>
        /// <example>
        /// <code>
        /// var date = new NepaliDate(new DateTime(2023, 7, 31));
        /// Console.WriteLine(date);  // 2080/04/15
        ///
        /// // Extension method equivalent
        /// var date2 = new DateTime(2023, 7, 31).ToNepaliDate();
        /// </code>
        /// </example>
        public NepaliDate(DateTime engDate)
        {
            // Check if date is before minimum supported date (1901-04-13)
            if (engDate.Date < MinValue.EnglishDate.Date)
            {
                throw new ArgumentOutOfRangeException(nameof(engDate),
                    $"The date is before the minimum supported date ({MinValue.EnglishDate.Date}).");
            }

            // Check if date is after maximum supported date (2143-04-12)
            if (engDate.Date > MaxValue.EnglishDate.Date)
            {
                throw new ArgumentOutOfRangeException(nameof(engDate),
                    $"The date is after the maximum supported date ({MaxValue.EnglishDate.Date}).");
            }

            (Year, Month, Day) = DictionaryBridge.EngToNep.GetNepaliDate(engDate.Year, engDate.Month, engDate.Day);
            _englishDate = engDate.Date;
            ValidateAndThrow();
        }
        #endregion

        /// <summary>
        /// Returns a new <see cref="NepaliDate"/> set to the last day of this instance's month.
        /// </summary>
        /// <returns>
        /// A <see cref="NepaliDate"/> with the same year and month as this instance, but with
        /// <see cref="Day"/> set to <see cref="MonthEndDay"/>.
        /// </returns>
        /// <remarks>
        /// Nepali months have irregular lengths (29–32 days) that vary by year. This method
        /// always returns the correct last day for the specific year and month combination.
        /// </remarks>
        /// <example>
        /// <code>
        /// var date = new NepaliDate(2080, 4, 10);
        /// var end  = date.MonthEndDate();  // 2080/04/32
        /// Console.WriteLine(end.Day);      // 32
        /// </code>
        /// </example>
        public NepaliDate MonthEndDate()
        {
            return new NepaliDate(Year, Month, MonthEndDay);
        }

        /// <summary>
        /// Returns a new <see cref="NepaliDate"/> that is the specified number of months from this instance.
        /// Supports positive, negative, and fractional values.
        /// </summary>
        /// <param name="months">
        /// The number of months to add. Negative values move backward. Fractional values are
        /// converted to an equivalent number of days (1 month ≈ 30.42 days) before adding.
        /// </param>
        /// <param name="awayFromMonthEnd">
        /// Controls what happens when the target month is shorter than the current day.
        /// <list type="bullet">
        ///   <item>
        ///     <description><see langword="false"/> (default): the day is clamped to the last day of the target month.</description>
        ///   </item>
        ///   <item>
        ///     <description><see langword="true"/>: the overflow days carry forward into the next month.</description>
        ///   </item>
        /// </list>
        /// </param>
        /// <returns>A new <see cref="NepaliDate"/> offset by the requested number of months.</returns>
        /// <remarks>
        /// Whole-number month arithmetic works in month space with no day drift. Fractional month
        /// arithmetic converts to days first and delegates to <see cref="AddDays(double)"/>.
        /// </remarks>
        /// <example>
        /// <code>
        /// var date = new NepaliDate(2081, 4, 32);  // last day of Shrawan
        ///
        /// // Clamp: if Bhadra 2081 has 31 days, result is 2081/05/31
        /// var clamped  = date.AddMonths(1);
        ///
        /// // Overflow: day 32 in a 31-day month spills 1 day into Ashoj
        /// var overflow = date.AddMonths(1, awayFromMonthEnd: true);  // 2081/06/01
        ///
        /// // Negative: move back 2 months
        /// var earlier  = date.AddMonths(-2);
        /// </code>
        /// </example>
        public NepaliDate AddMonths(double months, bool awayFromMonthEnd = false)
        {
            if (months < 0)
            {
                return SubtractMonths(Math.Abs(months), awayFromMonthEnd);
            }

            var roundedMonths = (int)Math.Round(months, 0, MidpointRounding.AwayFromZero);

            if (months != roundedMonths)
            {
                return AddDays(Math.Round(months * _approxDaysPerMonth, 0, MidpointRounding.AwayFromZero));
            }

            var nextYear = Year;
            var nextMonth = Month + roundedMonths;
            var nextMonthDay = 1;

            while (nextMonth > 12)
            {
                nextYear++;
                nextMonth -= 12;
            }

            var nextMonthNepDate = new NepaliDate(nextYear, nextMonth, nextMonthDay);

            if (awayFromMonthEnd)
            {
                if (Day > nextMonthNepDate.MonthEndDay)
                {
                    nextMonthDay = Day - nextMonthNepDate.MonthEndDay;
                    nextMonth++;

                    if (nextMonth > 12)
                    {
                        nextYear++;
                        nextMonth = 1;
                    }
                }
                else
                {
                    nextMonthDay = Day;
                }
            }
            else
            {
                nextMonthDay = Math.Min(nextMonthNepDate.MonthEndDay, Day);
            }

            return new NepaliDate(nextYear, nextMonth, nextMonthDay);
        }

        /// <summary>
        /// Subtracts the specified number of months from this NepaliDate and returns a new NepaliDate.
        /// </summary>
        /// <param name="months">The number of months to subtract. Can be positive, negative, or fractional.</param>
        /// <param name="awayFromMonthEnd">
        /// Determines behavior when subtracting months would put the day beyond the end of the resulting month.
        /// If false (default), the day is capped at the last day of the resulting month.
        /// If true, the excess days are added to the first day of the following month.
        /// </param>
        /// <returns>A new NepaliDate that is the specified number of months before this instance.</returns>
        /// <remarks>
        /// For fractional months, the value is rounded to the nearest whole number of days (approx. 30.42 days per month).
        /// If months is negative, this method calls AddMonths with the absolute value.
        /// This is an internal method used by AddMonths with a negative value.
        /// </remarks>
        private NepaliDate SubtractMonths(double months, bool awayFromMonthEnd = false)
        {
            if (months < 0)
            {
                return AddMonths(Math.Abs(months), awayFromMonthEnd);
            }

            var roundedMonths = (int)Math.Round(months, 0, MidpointRounding.AwayFromZero);

            if (months != roundedMonths)
            {
                return AddDays(-Math.Round(months * _approxDaysPerMonth, 0, MidpointRounding.AwayFromZero));
            }

            var previousYear = Year;
            var previousMonth = Month - roundedMonths;
            var previousMonthDay = 1;

            while (previousMonth < 1)
            {
                previousYear--;
                previousMonth = previousMonth + 12;
            }

            var previousMonthNepDate = new NepaliDate(previousYear, previousMonth, previousMonthDay);

            if (awayFromMonthEnd)
            {
                if (Day > previousMonthNepDate.MonthEndDay)
                {
                    previousMonthDay = Day - previousMonthNepDate.MonthEndDay;
                    previousMonth++;

                    if (previousMonth > 12)
                    {
                        previousYear++;
                        previousMonth = 1;
                    }
                }
                else
                {
                    previousMonthDay = Day;
                }
            }
            else
            {
                previousMonthDay = Math.Min(previousMonthNepDate.MonthEndDay, Day);
            }

            return new NepaliDate(previousYear, previousMonth, previousMonthDay);
        }

        /// <summary>
        /// Returns a new <see cref="NepaliDate"/> that is the specified number of days from this instance.
        /// </summary>
        /// <param name="days">
        /// The number of days to add. Negative values move to an earlier date.
        /// Fractional values are accepted; only whole-day precision is preserved because
        /// <see cref="NepaliDate"/> has no time component.
        /// </param>
        /// <returns>A new <see cref="NepaliDate"/> offset by the requested number of days.</returns>
        /// <remarks>
        /// The addition is performed in the Gregorian domain: the date is converted to a
        /// <see cref="DateTime"/>, the days are added, and the result is converted back.
        /// This guarantees correct handling of all month and year boundaries.
        /// </remarks>
        /// <example>
        /// <code>
        /// var date  = new NepaliDate(2080, 4, 30);
        /// var later = date.AddDays(5);    // 2080/05/03
        /// var prev  = date.AddDays(-10);  // 2080/04/20
        /// </code>
        /// </example>
        public NepaliDate AddDays(double days)
        {
            return EnglishDate.AddDays(days).ToNepaliDate();
        }

        /// <summary>
        /// Returns a new <see cref="NepaliDate"/> that is the specified number of years from this instance.
        /// </summary>
        /// <param name="years">The number of years to add. Use a negative value to move to an earlier year.</param>
        /// <param name="awayFromMonthEnd">
        /// Controls behavior when the target month is shorter than the current day.
        /// When <see langword="false"/> (default), the day is clamped to the last day of the target month.
        /// When <see langword="true"/>, the overflow days carry forward to the next month.
        /// </param>
        /// <returns>A new <see cref="NepaliDate"/> advanced by the specified number of years.</returns>
        /// <remarks>
        /// Delegates to <see cref="AddMonths(double, bool)"/> with <c>years * 12</c>.
        /// Month-end clamping follows the same rules described on that method.
        /// </remarks>
        /// <example>
        /// <code>
        /// var date = new NepaliDate(2080, 12, 30);
        /// var next = date.AddYears(1);   // 2081/12/30 (or last day of Chaitra if shorter)
        /// var prev = date.AddYears(-1);  // 2079/12/30
        /// </code>
        /// </example>
        public NepaliDate AddYears(int years, bool awayFromMonthEnd = false)
        {
            return AddMonths(years * 12.0, awayFromMonthEnd);
        }

        /// <summary>
        /// Deconstructs this <see cref="NepaliDate"/> into its individual year, month, and day components,
        /// enabling C# tuple-assignment syntax.
        /// </summary>
        /// <param name="year">Receives the Bikram Sambat year (1901–2199).</param>
        /// <param name="month">Receives the month number (1 = Baisakh through 12 = Chaitra).</param>
        /// <param name="day">Receives the day within the month.</param>
        /// <example>
        /// <code>
        /// var date = new NepaliDate(2080, 4, 15);
        /// var (y, m, d) = date;
        /// Console.WriteLine($"{y}/{m:D2}/{d:D2}");  // 2080/04/15
        /// </code>
        /// </example>
        public void Deconstruct(out int year, out int month, out int day)
        {
            year = Year;
            month = Month;
            day = Day;
        }

        /// <summary>
        /// Returns the elapsed time between this <see cref="NepaliDate"/> and another <see cref="NepaliDate"/>.
        /// Equivalent to the <c>-</c> operator.
        /// </summary>
        /// <param name="nepDateTo">The date to subtract from this instance.</param>
        /// <returns>
        /// A <see cref="TimeSpan"/> whose sign is positive when this date is later than
        /// <paramref name="nepDateTo"/>, and negative when this date is earlier.
        /// </returns>
        /// <example>
        /// <code>
        /// var start = new NepaliDate(2080, 1, 1);
        /// var end   = new NepaliDate(2080, 4, 1);
        /// TimeSpan gap = end.Subtract(start);
        /// Console.WriteLine(gap.Days);  // 91
        /// </code>
        /// </example>
        public TimeSpan Subtract(NepaliDate nepDateTo)
        {
            return this - nepDateTo;
        }

        /// <summary>
        /// Determines whether this date falls within a Gregorian leap year.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> when the Gregorian year that contains this Nepali date is a leap year;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// The Bikram Sambat calendar does not follow the Gregorian leap-year cycle. This method
        /// evaluates the <em>Gregorian</em> year of <see cref="EnglishDate"/> using the standard rule:
        /// divisible by 4, except century years unless also divisible by 400.
        /// Because a Nepali year spans parts of two Gregorian years, a date near the end of a Nepali
        /// year (Falgun/Chaitra) may return a different result from one near the beginning (Baisakh/Jestha).
        /// </remarks>
        /// <example>
        /// <code>
        /// // 2080 BS spans 2023 AD (not a leap year) and 2024 AD (a leap year)
        /// Console.WriteLine(new NepaliDate(2080, 1, 1).IsLeapYear());   // False  (Baisakh  → April 2023)
        /// Console.WriteLine(new NepaliDate(2080, 11, 1).IsLeapYear());  // True   (Magh     → January 2024)
        /// </code>
        /// </example>
        public bool IsLeapYear()
        {
            var engYear = EnglishDate.Year;
            return engYear % 4 == 0 && (engYear % 100 != 0 || engYear % 400 == 0);
        }

        /// <summary>
        /// Determines whether this <see cref="NepaliDate"/> represents today's date according to the system clock.
        /// </summary>
        /// <returns><see langword="true"/> if this date equals <see cref="Today"/>; otherwise <see langword="false"/>.</returns>
        /// <remarks>
        /// Compared against <see cref="DateTime.Today"/> in the local system time zone.
        /// </remarks>
        public bool IsToday()
        {
            return DateTime.Today.ToNepaliDate() == this;
        }

        /// <summary>
        /// Determines whether this <see cref="NepaliDate"/> represents yesterday's date according to the system clock.
        /// </summary>
        /// <returns><see langword="true"/> if this date equals <see cref="Today"/> minus one day; otherwise <see langword="false"/>.</returns>
        /// <remarks>
        /// Compared against <see cref="DateTime.Today"/> in the local system time zone.
        /// </remarks>
        public bool IsYesterday()
        {
            return DateTime.Today.AddDays(-1).ToNepaliDate() == this;
        }

        /// <summary>
        /// Determines whether this <see cref="NepaliDate"/> represents tomorrow's date according to the system clock.
        /// </summary>
        /// <returns><see langword="true"/> if this date equals <see cref="Today"/> plus one day; otherwise <see langword="false"/>.</returns>
        /// <remarks>
        /// Compared against <see cref="DateTime.Today"/> in the local system time zone.
        /// </remarks>
        public bool IsTomorrow()
        {
            return DateTime.Today.AddDays(1).ToNepaliDate() == this;
        }

        /// <summary>
        /// Parses a string representation of a Nepali date into year, month, and day components.
        /// </summary>
        /// <param name="rawNepaliDate">The string containing a Nepali date to parse.</param>
        /// <returns>A tuple containing the year, month, and day components.</returns>
        /// <exception cref="InvalidNepaliDateFormatException">Thrown when the input string is null or empty.</exception>
        /// <exception cref="InvalidNepaliDateFormatException">Thrown when the string cannot be parsed as a valid Nepali date.</exception>
        /// <remarks>
        /// This method supports various separator characters including slashes, dashes, dots, underscores, and spaces.
        /// The components must be in the order: year, month, day.
        /// This is an optimized implementation that avoids unnecessary string allocations.
        /// </remarks>
        private static (int year, int month, int day) SplitNepaliDate(string rawNepaliDate)
        {
            if (string.IsNullOrEmpty(rawNepaliDate))
            {
                throw new InvalidNepaliDateFormatException();
            }

            int part0 = 0, part1 = 0, part2 = 0;
            var currentIndex = 0;
            var hasDigits = false;
            var length = rawNepaliDate.Length;

            for (int i = 0; i < length; i++)
            {
                char c = rawNepaliDate[i];

                if (c >= '0' && c <= '9')
                {
                    int digit = c - '0';
                    switch (currentIndex)
                    {
                        case 0: part0 = part0 * 10 + digit; break;
                        case 1: part1 = part1 * 10 + digit; break;
                        case 2: part2 = part2 * 10 + digit; break;
                        default: throw new InvalidNepaliDateFormatException();
                    }
                    hasDigits = true;
                }
                else if (c == '-' || c == '/' || c == '.' || c == '_' || c == '\\' || c == ' ' || c == '।' || c == '|')
                {
                    if (hasDigits)
                    {
                        currentIndex++;
                        hasDigits = false;
                    }
                }
                else
                {
                    throw new InvalidNepaliDateFormatException();
                }
            }

            if (hasDigits)
            {
                currentIndex++;
            }

            if (currentIndex != 3)
            {
                throw new InvalidNepaliDateFormatException();
            }

            return (part0, part1, part2);
        }

        /// <summary>
        /// Attempts to parse a Nepali date string into its numeric year, month, and day components
        /// without throwing an exception on failure.
        /// </summary>
        /// <param name="rawNepaliDate">The raw date string to parse.</param>
        /// <param name="year">When this method returns <see langword="true"/>, contains the parsed year; otherwise 0.</param>
        /// <param name="month">When this method returns <see langword="true"/>, contains the parsed month; otherwise 0.</param>
        /// <param name="day">When this method returns <see langword="true"/>, contains the parsed day; otherwise 0.</param>
        /// <returns>
        /// <see langword="true"/> if exactly three numeric segments separated by a recognised delimiter were found;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// Only extracts raw integer values. Range and calendar validity are checked separately by the caller.
        /// </remarks>
        private static bool TrySplitNepaliDate(string rawNepaliDate, out int year, out int month, out int day)
        {
            year = month = day = 0;

            if (string.IsNullOrEmpty(rawNepaliDate))
                return false;

            int part0 = 0, part1 = 0, part2 = 0;
            var currentIndex = 0;
            var hasDigits = false;
            var length = rawNepaliDate.Length;

            for (int i = 0; i < length; i++)
            {
                char c = rawNepaliDate[i];

                if (c >= '0' && c <= '9')
                {
                    int digit = c - '0';
                    switch (currentIndex)
                    {
                        case 0: part0 = part0 * 10 + digit; break;
                        case 1: part1 = part1 * 10 + digit; break;
                        case 2: part2 = part2 * 10 + digit; break;
                        default: return false;
                    }
                    hasDigits = true;
                }
                else if (c == '-' || c == '/' || c == '.' || c == '_' || c == '\\' || c == ' ' || c == '।' || c == '|')
                {
                    if (hasDigits)
                    {
                        currentIndex++;
                        hasDigits = false;
                    }
                }
                else
                {
                    return false;
                }
            }

            if (hasDigits)
            {
                currentIndex++;
            }

            if (currentIndex != 3)
                return false;

            (year, month, day) = (part0, part1, part2);
            return true;
        }
    }
}
