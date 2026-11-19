using NepDate.Core.Dictionaries;
using NepDate.TypeConversion;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
#if NETSTANDARD2_0
using CharSequence = System.String;
#else
using CharSequence = System.ReadOnlySpan<char>;
#endif

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
        /// Stores components that the caller has already validated.
        /// </summary>
        /// <param name="year">The Bikram Sambat year (1901-2199).</param>
        /// <param name="month">The month (1-12).</param>
        /// <param name="day">The day within the month.</param>
        /// <param name="serial">Days from 1 Baisakh 1901 BS to this date.</param>
        private NepaliDate(int year, int month, int day, int serial)
        {
            _dayNumber = serial + 1;
            _year = (ushort)year;
            _month = (byte)month;
            _day = (byte)day;
        }

        /// <summary>
        /// Validates the components and builds the date with one table lookup.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the year is outside 1901-2199, the month is outside 1-12,
        /// or the day exceeds the number of days in the given month.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NepaliDate Create(int year, int month, int day)
        {
            if ((uint)(year - _minYear) > (uint)(_maxYear - _minYear) || (uint)(month - 1) > 11u)
            {
                ThrowYearOrMonthOutOfRange(year, month);
            }

            var starts = MonthTable.Start;
            var monthIndex = (year - _minYear) * 12 + (month - 1);
            var start = starts[monthIndex];
            var monthEndDay = starts[monthIndex + 1] - start;

            if ((uint)(day - 1) >= (uint)monthEndDay)
            {
                ThrowDayOutOfRange(year, month, day, monthEndDay);
            }

            return new NepaliDate(year, month, day, start + day - 1);
        }

        /// <summary>
        /// Builds the date from valid-looking components without throwing.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the combination is a real date within the supported
        /// 1901-2199 BS range; otherwise <see langword="false"/>.
        /// </returns>
        private static bool TryCreate(int year, int month, int day, out NepaliDate result)
        {
            if ((uint)(year - _minYear) > (uint)(_maxYear - _minYear) || (uint)(month - 1) > 11u)
            {
                result = default;
                return false;
            }

            var starts = MonthTable.Start;
            var monthIndex = (year - _minYear) * 12 + (month - 1);
            var start = starts[monthIndex];

            if ((uint)(day - 1) >= (uint)(starts[monthIndex + 1] - start))
            {
                result = default;
                return false;
            }

            result = new NepaliDate(year, month, day, start + day - 1);
            return true;
        }

        /// <summary>
        /// Builds the date that lies <paramref name="serial"/> days after 1 Baisakh 1901 BS.
        /// The caller guarantees that the serial is inside the supported range.
        /// </summary>
        /// <remarks>
        /// A Bikram Sambat month is 30.44 days long on average, so the month index is first
        /// estimated by proportion and then corrected by at most a step or two.
        /// </remarks>
        private static NepaliDate FromSerial(int serial)
        {
            var starts = MonthTable.Start;
            var monthIndex = (int)((uint)serial * (uint)MonthTable.MonthCount / (uint)MonthTable.TotalDays);

            if (serial < starts[monthIndex])
            {
                do
                {
                    monthIndex--;
                }
                while (serial < starts[monthIndex]);
            }
            else
            {
                while (serial >= starts[monthIndex + 1])
                {
                    monthIndex++;
                }
            }

            var yearIndex = monthIndex / 12;
            return new NepaliDate(
                _minYear + yearIndex,
                monthIndex - yearIndex * 12 + 1,
                serial - starts[monthIndex] + 1,
                serial);
        }

        /// <summary>
        /// Builds the date for a serial that may lie outside the supported range.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the serial is outside the supported range.</exception>
        private static NepaliDate FromSerialChecked(int serial)
        {
            if ((uint)serial >= (uint)MonthTable.TotalDays)
            {
                ThrowEnglishDateOutOfRange(serial < 0);
            }

            return FromSerial(serial);
        }

        private static void ThrowYearOrMonthOutOfRange(int year, int month)
        {
            if (year < _minYear || year > _maxYear)
            {
                throw new ArgumentOutOfRangeException(nameof(Year),
                    $"Year must be between {_minYear} and {_maxYear}. Got: {year}.");
            }

            throw new ArgumentOutOfRangeException(nameof(Month),
                $"Month must be between 1 and 12. Got: {month}.");
        }

        private static void ThrowDayOutOfRange(int year, int month, int day, int monthEndDay)
        {
            throw new ArgumentOutOfRangeException(nameof(Day),
                $"Day must be between 1 and {monthEndDay} for {year}/{month:D2}. Got: {day}.");
        }

        private static void ThrowEnglishDateOutOfRange(bool beforeMinimum)
        {
            if (beforeMinimum)
            {
                throw new ArgumentOutOfRangeException("engDate",
                    $"The date is before the minimum supported date ({MinValue.EnglishDate.Date}).");
            }

            throw new ArgumentOutOfRangeException("engDate",
                $"The date is after the maximum supported date ({MaxValue.EnglishDate.Date}).");
        }

        private static void ThrowDefaultInstance()
        {
            throw new InvalidNepaliDateFormatException();
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
            this = Create(yearBs, monthBs, dayBs);
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
            if (!TrySplitNepaliDate(rawNepaliDate, out var year, out var month, out var day))
            {
                throw new InvalidNepaliDateFormatException();
            }

            this = Create(year, month, day);
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

            if (!TrySplitNepaliDate(rawNepaliDate, out var year, out var month, out var day))
            {
                throw new InvalidNepaliDateFormatException();
            }

            if (autoAdjust)
            {
                if (day > 32)
                {
                    (year, day) = (day, year);
                }

                if (!monthInMiddle)
                {
                    (month, day) = (day, month);
                }

                if (month > 12 && day < 13)
                {
                    (month, day) = (day, month);
                }

                if (year < 1000)
                {
                    year = currentMillennium * 1000 + year;
                }
            }

            this = Create(year, month, day);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NepaliDate"/> struct by converting a Gregorian
        /// <see cref="DateTime"/> to its Bikram Sambat equivalent.
        /// </summary>
        /// <param name="engDate">
        /// The Gregorian date to convert. Only the date portion is used; the time component and the
        /// <see cref="DateTime.Kind"/> are ignored.
        /// The date must fall on or after 1844-04-11 and on or before 2143-04-15 (the Gregorian equivalents
        /// of <see cref="MinValue"/> and <see cref="MaxValue"/>).
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="engDate"/> falls before the Gregorian equivalent of
        /// <see cref="MinValue"/> (1844-04-11) or after that of <see cref="MaxValue"/> (2143-04-15).
        /// </exception>
        /// <remarks>
        /// The conversion uses one precomputed table of month start days, with no heap allocation.
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
            this = FromSerialChecked((int)(engDate.Ticks / _ticksPerDay) - _epochDayNumber);
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
            if (_dayNumber == 0)
            {
                ThrowDefaultInstance();
            }

            var monthEndDay = MonthEndDay;
            return new NepaliDate(_year, _month, monthEndDay, _dayNumber - 1 + monthEndDay - _day);
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
            // The usual call passes a small whole number: skip the rounding and go straight to
            // month arithmetic. Everything else takes the general path below.
            if (months > -_wholeMonthsFastLimit && months < _wholeMonthsFastLimit)
            {
                var wholeMonths = (int)months;

                if (wholeMonths == months)
                {
                    return AddWholeMonths(wholeMonths, awayFromMonthEnd);
                }
            }

            return AddMonthsGeneral(months, awayFromMonthEnd);
        }

        /// <summary>
        /// Whole-number month counts below this magnitude take the fast path of
        /// <see cref="AddMonths(double, bool)"/>. The supported range spans 3,588 months.
        /// </summary>
        private const double _wholeMonthsFastLimit = 100000;

        /// <summary>
        /// Whole-number day counts up to this magnitude take the fast path of
        /// <see cref="AddDays(double)"/>. The supported range spans 109,212 days.
        /// </summary>
        private const double _wholeDaysFastLimit = 200000;

        /// <summary>
        /// Handles fractional, very large and non-finite month counts.
        /// </summary>
        /// <remarks>
        /// A fractional value is converted to days (approx. 30.42 days per month) and added with
        /// <see cref="AddDays(double)"/>. A whole value is added in month space.
        /// </remarks>
        private NepaliDate AddMonthsGeneral(double months, bool awayFromMonthEnd)
        {
            var magnitude = Math.Abs(months);
            var roundedMonths = (int)Math.Round(magnitude, 0, MidpointRounding.AwayFromZero);

            if (magnitude != roundedMonths)
            {
                var days = Math.Round(magnitude * _approxDaysPerMonth, 0, MidpointRounding.AwayFromZero);
                return AddDays(months < 0 ? -days : days);
            }

            return AddWholeMonths(months < 0 ? -roundedMonths : roundedMonths, awayFromMonthEnd);
        }

        /// <summary>
        /// Moves this date by a whole number of months, forward or backward.
        /// </summary>
        /// <param name="months">The number of months to move. Negative values move backward.</param>
        /// <param name="awayFromMonthEnd">
        /// When the target month is shorter than the current day: <see langword="false"/> clamps the day
        /// to the last day of the target month, <see langword="true"/> carries the extra days into the
        /// following month.
        /// </param>
        private NepaliDate AddWholeMonths(int months, bool awayFromMonthEnd)
        {
            var target = (long)(_year - _minYear) * 12 + (_month - 1) + months;

            if ((ulong)target >= (ulong)MonthTable.MonthCount)
            {
                // Floor division, so that the reported year is right for negative targets too.
                var yearOffset = target >= 0 ? target / 12 : -((11 - target) / 12);
                ThrowYearOrMonthOutOfRange((int)(_minYear + yearOffset), 1);
            }

            var starts = MonthTable.Start;
            var monthIndex = (int)target;
            var monthEndDay = starts[monthIndex + 1] - starts[monthIndex];
            int day = _day;

            if (day > monthEndDay)
            {
                if (awayFromMonthEnd)
                {
                    day -= monthEndDay;
                    monthIndex++;

                    if (monthIndex >= MonthTable.MonthCount)
                    {
                        ThrowYearOrMonthOutOfRange(_maxYear + 1, 1);
                    }

                    monthEndDay = starts[monthIndex + 1] - starts[monthIndex];
                }
                else
                {
                    day = monthEndDay;
                }
            }

            var yearIndex = monthIndex / 12;
            var year = _minYear + yearIndex;
            var month = monthIndex - yearIndex * 12 + 1;

            if ((uint)(day - 1) >= (uint)monthEndDay)
            {
                ThrowDayOutOfRange(year, month, day, monthEndDay);
            }

            return new NepaliDate(year, month, day, starts[monthIndex] + day - 1);
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
        /// A whole number of days is added to the internal day count directly, which handles every
        /// month and year boundary. A fractional value is added in the Gregorian domain
        /// (<see cref="DateTime.AddDays(double)"/>) and the result is converted back.
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
            if (days >= -_wholeDaysFastLimit && days <= _wholeDaysFastLimit)
            {
                var wholeDays = (int)days;

                if (wholeDays == days)
                {
                    if (_dayNumber == 0)
                    {
                        ThrowDefaultInstance();
                    }

                    return FromSerialChecked(_dayNumber - 1 + wholeDays);
                }
            }

            return new NepaliDate(EnglishDate.AddDays(days));
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
        /// Console.WriteLine(gap.Days);  // 94
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
        private static bool TrySplitNepaliDate(CharSequence rawNepaliDate, out int year, out int month, out int day)
        {
            year = month = day = 0;

#if NETSTANDARD2_0
            if (rawNepaliDate is null)
                return false;
#endif

            // Fast path for the canonical shape, four digits, two digits, two digits with single
            // separators ("2080/04/15"). The general loop below gives the same answer for it.
            if (rawNepaliDate.Length == 10 && IsSeparator(rawNepaliDate[4]) && IsSeparator(rawNepaliDate[7]))
            {
                uint y0 = (uint)(rawNepaliDate[0] - '0'), y1 = (uint)(rawNepaliDate[1] - '0');
                uint y2 = (uint)(rawNepaliDate[2] - '0'), y3 = (uint)(rawNepaliDate[3] - '0');
                uint m0 = (uint)(rawNepaliDate[5] - '0'), m1 = (uint)(rawNepaliDate[6] - '0');
                uint d0 = (uint)(rawNepaliDate[8] - '0'), d1 = (uint)(rawNepaliDate[9] - '0');

                // A non-digit gives a value above 9 (it wraps when the character is below '0').
                if (y0 <= 9u && y1 <= 9u && y2 <= 9u && y3 <= 9u && m0 <= 9u && m1 <= 9u && d0 <= 9u && d1 <= 9u)
                {
                    year = (int)(y0 * 1000 + y1 * 100 + y2 * 10 + y3);
                    month = (int)(m0 * 10 + m1);
                    day = (int)(d0 * 10 + d1);
                    return true;
                }
            }

            int part0 = 0, part1 = 0, part2 = 0;
            var currentIndex = 0;
            var hasDigits = false;

            for (int i = 0; i < rawNepaliDate.Length; i++)
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
                else if (IsSeparator(c))
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

        /// <summary>
        /// Returns whether <paramref name="c"/> is one of the accepted component separators.
        /// </summary>
        private static bool IsSeparator(char c)
        {
            return c == '-' || c == '/' || c == '.' || c == '_' || c == '\\' || c == ' ' || c == '।' || c == '|';
        }
    }
}
