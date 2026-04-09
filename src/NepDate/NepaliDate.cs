using NepDate.Core.Dictionaries;
using NepDate.Extensions;
using System;
using static NepDate.Exceptions.NepDateException;

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
    public readonly partial struct NepaliDate
    {
        #region Ctor
        /// <summary>
        /// Validates the Nepali date components and throws an exception if they are invalid.
        /// </summary>
        /// <remarks>
        /// This checks that:
        /// - Day is between 1 and the maximum day for the month
        /// - Month is between 1 and 12
        /// - Year is within the supported range (1901-2199)
        /// </remarks>
        /// <exception cref="InvalidNepaliDateFormatException">Thrown when any date component is outside the valid range.</exception>
        private void ValidateAndThrow()
        {
            // Check if date is before minimum supported date (1901-04-13)
            if (Year < _minYear)
            {
                throw new ArgumentOutOfRangeException(nameof(Year),
                    $"The date is before the minimum supported date ({MinValue}).");
            }

            // Check if date is after maximum supported date (2143-04-12)
            if (Year > _maxYear)
            {
                throw new ArgumentOutOfRangeException(nameof(Year),
                    $"The date is after the maximum supported date ({MaxValue}).");
            }

            if (Day < 1 || Day > MonthEndDay || Month < 1 || Month > 12)
            {
                throw new InvalidNepaliDateFormatException();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NepaliDate"/> struct with the specified Nepali date components.
        /// </summary>
        /// <param name="yearBs">The year component (Bikram Sambat) of the Nepali date.</param>
        /// <param name="monthBs">The month component of the Nepali date (1-12).</param>
        /// <param name="dayBs">The day component of the Nepali date (1-32, depending on month).</param>
        /// <exception cref="InvalidNepaliDateFormatException">Thrown when any date component is outside the valid range.</exception>
        /// <remarks>
        /// This is the most direct way to create a NepaliDate when the Nepali date components are already known.
        /// The constructor validates that the provided values represent a valid Nepali date.
        /// </remarks>
        public NepaliDate(int yearBs, int monthBs, int dayBs)
        {
            (Year, Month, Day) = (yearBs, monthBs, dayBs);
            _englishDate = null;
            ValidateAndThrow();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NepaliDate"/> struct from a Nepali date string.
        /// </summary>
        /// <param name="rawNepaliDate">The Nepali date string in various formats like "YYYY/MM/DD", "YYYY-MM-DD", etc.</param>
        /// <exception cref="InvalidNepaliDateFormatException">Thrown when the input string is null or empty.</exception>
        /// <exception cref="InvalidNepaliDateFormatException">Thrown when the string cannot be parsed as a valid Nepali date.</exception>
        /// <remarks>
        /// Supports multiple separator characters including slash, dash, dot, underscore, backslash, and space.
        /// The date components must be in the order: year, month, day.
        /// </remarks>
        public NepaliDate(string rawNepaliDate)
        {
            (Year, Month, Day) = SplitNepaliDate(rawNepaliDate);
            _englishDate = null;
            ValidateAndThrow();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NepaliDate"/> struct from a Nepali date string with auto-adjustment options.
        /// </summary>
        /// <param name="rawNepaliDate">The Nepali date string to parse.</param>
        /// <param name="autoAdjust">Whether to automatically adjust the date components to form a valid date.</param>
        /// <param name="monthInMiddle">Whether the month component is in the middle position (true) or not (false).</param>
        /// <exception cref="InvalidNepaliDateFormatException">Thrown when the input string is null or empty.</exception>
        /// <exception cref="InvalidNepaliDateFormatException">Thrown when the string cannot be parsed as a valid Nepali date.</exception>
        /// <remarks>
        /// When autoAdjust is true, this method applies several heuristics to fix common date format issues:
        /// - If day component is unusually large (>32 or 3+ digits), it swaps day and year
        /// - If month is >12 and day is &lt;12, it swaps month and day
        /// - If the year has 3 or fewer digits, it assumes the current millennium (2000+)
        /// 
        /// The monthInMiddle parameter affects how components are interpreted when autoAdjust is true.
        /// </remarks>
        public NepaliDate(string rawNepaliDate, bool autoAdjust, bool monthInMiddle = true)
        {
            const int currentMillennium = 2;

            (Year, Month, Day) = SplitNepaliDate(rawNepaliDate);

            if (autoAdjust)
            {
                if (Day.ToString().Length >= 3 || Day > 32)
                {
                    (Year, Day) = (Day, Year);
                }

                if (!monthInMiddle)
                {
                    (Month, Day) = (Day, Month);
                }

                if (Month > 12 && Day < 12)
                {
                    (Month, Day) = (Day, Month);
                }

                if (Year.ToString().Length <= 3)
                {
                    Year = int.Parse(string.Concat(currentMillennium.ToString(), Year.ToString("D3")));
                }
            }
            _englishDate = null;
            ValidateAndThrow();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NepaliDate"/> struct by converting from a Gregorian (English) date.
        /// </summary>
        /// <param name="engDate">The Gregorian DateTime to convert to Nepali date.</param>
        /// <exception cref="InvalidNepaliDateFormatException">Thrown when the converted date is outside the supported range.</exception>
        /// <remarks>
        /// This constructor converts from the Gregorian calendar to the Nepali calendar.
        /// Only the date components (year, month, day) are used from the input DateTime; time components are ignored.
        /// The conversion is performed using predefined mapping data in the dictionaries.
        /// </remarks>
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
            _englishDate = engDate;
            ValidateAndThrow();
        }
        #endregion

        /// <summary>
        /// Creates a new NepaliDate representing the last day of the current month.
        /// </summary>
        /// <returns>A new NepaliDate set to the month-end date of the current month and year.</returns>
        /// <remarks>
        /// This is useful for finding the last day of a month, which varies in the Nepali calendar.
        /// </remarks>
        public NepaliDate MonthEndDate()
        {
            return new NepaliDate(Year, Month, MonthEndDay);
        }

        /// <summary>
        /// Adds the specified number of months to this NepaliDate and returns a new NepaliDate.
        /// </summary>
        /// <param name="months">The number of months to add. Can be positive, negative, or fractional.</param>
        /// <param name="awayFromMonthEnd">
        /// Determines behavior when adding months would put the day beyond the end of the resulting month.
        /// If false (default), the day is capped at the last day of the resulting month.
        /// If true, the excess days are added to the first day of the following month.
        /// </param>
        /// <returns>A new NepaliDate that is the specified number of months away from this instance.</returns>
        /// <remarks>
        /// For fractional months, the value is rounded to the nearest whole number of days (approx. 30.42 days per month).
        /// If months is negative, this method calls SubtractMonths with the absolute value.
        /// </remarks>
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

            var returnFirstDay = false;//added for future

            var nextYear = Year;
            var nextMonth = Month + roundedMonths;
            var nextMonthDay = 1;


            while (nextMonth > 12)
            {
                nextYear++;
                nextMonth -= 12;
            }

            var nextMonthNepDate = new NepaliDate(nextYear, nextMonth, nextMonthDay);

            if (returnFirstDay)
            {
                return nextMonthNepDate;
            }

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

            var returnFirstDay = false;//added for future

            var previousYear = Year;
            var previousMonth = Month - roundedMonths;
            var previousMonthDay = 1;

            while (previousMonth < 1)
            {
                previousYear--;
                previousMonth = previousMonth + 12;
            }

            var previousMonthNepDate = new NepaliDate(previousYear, previousMonth, previousMonthDay);

            if (returnFirstDay)
            {
                return previousMonthNepDate;
            }

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
        /// Adds the specified number of days to this NepaliDate and returns a new NepaliDate.
        /// </summary>
        /// <param name="days">The number of days to add. Can be positive, negative, or fractional.</param>
        /// <returns>A new NepaliDate that is the specified number of days away from this instance.</returns>
        /// <remarks>
        /// This operation works by:
        /// 1. Converting the Nepali date to its equivalent English (Gregorian) date
        /// 2. Adding the specified number of days to that English date 
        /// 3. Converting the result back to a Nepali date
        /// 
        /// This approach ensures accurate date arithmetic across month and year boundaries.
        /// For fractional days, the fractional part affects the time component of the underlying DateTime,
        /// but this time component is not preserved in the final NepaliDate.
        /// </remarks>
        public NepaliDate AddDays(double days)
        {
            return EnglishDate.AddDays(days).ToNepaliDate();
        }

        /// <summary>
        /// Calculates the time difference between this NepaliDate and another NepaliDate.
        /// </summary>
        /// <param name="nepDateTo">The NepaliDate to subtract from this instance.</param>
        /// <returns>A TimeSpan representing the interval between the two dates.</returns>
        /// <remarks>
        /// The result is positive if this date is later than the specified date,
        /// and negative if this date is earlier than the specified date.
        /// This is equivalent to using the subtraction operator.
        /// </remarks>
        public TimeSpan Subtract(NepaliDate nepDateTo)
        {
            return this - nepDateTo;
        }

        /// <summary>
        /// Determines whether the year of this NepaliDate is a leap year in the Gregorian calendar.
        /// </summary>
        /// <returns>true if the year is a leap year; otherwise, false.</returns>
        /// <remarks>
        /// This method applies the standard Gregorian calendar leap year rules:
        /// - Years divisible by 4 are leap years
        /// - Except years divisible by 100 are not leap years
        /// - Unless they are also divisible by 400, in which case they are leap years
        /// 
        /// Note that this evaluates the equivalent Gregorian year, not the Nepali year itself,
        /// as the Nepali calendar doesn't have the concept of leap years in the same way.
        /// </remarks>
        public bool IsLeapYear()
        {
            var engYear = EnglishDate.Year;
            return engYear % 4 == 0 && (engYear % 100 != 0 || engYear % 400 == 0);
        }

        /// <summary>
        /// Determines whether this NepaliDate represents today's date.
        /// </summary>
        /// <returns>true if this date is today; otherwise, false.</returns>
        /// <remarks>
        /// This compares the current date (from the system clock) converted to a NepaliDate
        /// with this instance.
        /// </remarks>
        public bool IsToday()
        {
            return DateTime.Now.ToNepaliDate() == this;
        }

        /// <summary>
        /// Determines whether this NepaliDate represents yesterday's date.
        /// </summary>
        /// <returns>true if this date is yesterday; otherwise, false.</returns>
        /// <remarks>
        /// This compares yesterday's date (from the system clock) converted to a NepaliDate
        /// with this instance.
        /// </remarks>
        public bool IsYesterday()
        {
            return DateTime.Now.AddDays(-1).ToNepaliDate() == this;
        }

        /// <summary>
        /// Determines whether this NepaliDate represents tomorrow's date.
        /// </summary>
        /// <returns>true if this date is tomorrow; otherwise, false.</returns>
        /// <remarks>
        /// This compares tomorrow's date (from the system clock) converted to a NepaliDate
        /// with this instance.
        /// </remarks>
        public bool IsTomorrow()
        {
            return DateTime.Now.AddDays(1).ToNepaliDate() == this;
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

            // Optimized splitting to avoid multiple string replacements
            // Pre-allocate array for better performance
            var splitDate = new int[3];
            var currentIndex = 0;
            var numberStart = 0;
            var length = rawNepaliDate.Length;

            for (int i = 0; i < length; i++)
            {
                char c = rawNepaliDate[i];
                if (c == '-' || c == '/' || c == '.' || c == '_' || c == '\\' || c == ' ' || c == '।' || c == '|')
                {
                    if (i > numberStart) // There is a number before this separator
                    {
                        if (currentIndex >= 3) // Too many parts
                        {
                            throw new InvalidNepaliDateFormatException();
                        }

                        // Regular string parsing instead of Span
                        if (!int.TryParse(rawNepaliDate.Substring(numberStart, i - numberStart), out splitDate[currentIndex++]))
                        {
                            throw new InvalidNepaliDateFormatException();
                        }
                    }
                    numberStart = i + 1; // Start of next number is after this separator
                }
            }

            // Handle the last number part
            if (numberStart < length)
            {
                if (currentIndex >= 3) // Too many parts
                {
                    throw new InvalidNepaliDateFormatException();
                }

                // Regular string parsing instead of Span
                if (!int.TryParse(rawNepaliDate.Substring(numberStart, length - numberStart), out splitDate[currentIndex++]))
                {
                    throw new InvalidNepaliDateFormatException();
                }
            }

            if (currentIndex != 3)
            {
                throw new InvalidNepaliDateFormatException();
            }

            return (splitDate[0], splitDate[1], splitDate[2]);
        }
    }
}
