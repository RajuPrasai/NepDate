using System;
using System.Collections;
using System.Collections.Generic;

namespace NepDate
{
    /// <summary>
    /// Represents a range of Nepali dates from a start date to an end date (inclusive).
    /// Provides functionality for working with date ranges including intersection, union,
    /// containment checks, and iteration through dates.
    /// </summary>
    /// <remarks>
    /// This class implements IEnumerable to allow iterating through all dates in the range.
    /// Both the start and end dates are inclusive in the range.
    /// </remarks>
    public readonly struct NepaliDateRange : IEnumerable<NepaliDate>, IEquatable<NepaliDateRange>
    {
        /// <summary>
        /// Gets the start date of the range (inclusive).
        /// </summary>
        public NepaliDate Start { get; }

        /// <summary>
        /// Gets the end date of the range (inclusive).
        /// </summary>
        public NepaliDate End { get; }

        /// <summary>
        /// Gets a value indicating whether the range is empty (Start date is later than End date).
        /// </summary>
        public bool IsEmpty => Start > End;

        /// <summary>
        /// Gets the length of the range in days.
        /// </summary>
        /// <remarks>
        /// If the range is empty, the length is 0.
        /// Otherwise, the length is the number of days between the start and end dates (inclusive).
        /// </remarks>
        public int Length => IsEmpty ? 0 : (End.EnglishDate.Date - Start.EnglishDate.Date).Days + 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="NepaliDateRange"/> struct with the specified start and end dates.
        /// </summary>
        /// <param name="start">The start date of the range (inclusive).</param>
        /// <param name="end">The end date of the range (inclusive).</param>
        /// <remarks>
        /// Both start and end dates are included in the range.
        /// If the end date is earlier than the start date, an empty range is created.
        /// </remarks>
        public NepaliDateRange(NepaliDate start, NepaliDate end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Creates a range that spans a single date.
        /// </summary>
        /// <param name="date">The date to include in the range.</param>
        /// <returns>A new NepaliDateRange that includes only the specified date.</returns>
        public static NepaliDateRange SingleDay(NepaliDate date)
        {
            return new NepaliDateRange(date, date);
        }

        /// <summary>
        /// Creates a range that spans a specified number of days from a start date.
        /// </summary>
        /// <param name="start">The start date of the range.</param>
        /// <param name="days">The number of days in the range (including the start date).</param>
        /// <returns>A new NepaliDateRange that spans the specified number of days.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when days is less than 1.</exception>
        public static NepaliDateRange FromDayCount(NepaliDate start, int days)
        {
            if (days < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(days), "Days must be at least 1.");
            }

            return new NepaliDateRange(start, start.AddDays(days - 1));
        }

        /// <summary>
        /// Creates a range for a complete Nepali month.
        /// </summary>
        /// <param name="year">The Nepali year.</param>
        /// <param name="month">The Nepali month (1-12).</param>
        /// <returns>A new NepaliDateRange that spans the entire specified month.</returns>
        public static NepaliDateRange ForMonth(int year, int month)
        {
            var firstDay = new NepaliDate(year, month, 1);
            var lastDay = new NepaliDate(year, month, firstDay.MonthEndDay);
            return new NepaliDateRange(firstDay, lastDay);
        }

        /// <summary>
        /// Creates a range for a complete Nepali fiscal year.
        /// </summary>
        /// <param name="fiscalYear">The Nepali fiscal year.</param>
        /// <returns>A new NepaliDateRange that spans the entire specified fiscal year.</returns>
        /// <remarks>
        /// In Nepal, the fiscal year starts on 1 Shrawan (month 4) and ends on the last day of Ashadh (month 3) of the next year.
        /// </remarks>
        public static NepaliDateRange ForFiscalYear(int fiscalYear)
        {
            var firstDay = new NepaliDate(fiscalYear, 4, 1); // 1 Shrawan
            var lastDay = new NepaliDate(fiscalYear + 1, 3, 1).MonthEndDate(); // Last day of Ashadh next year
            return new NepaliDateRange(firstDay, lastDay);
        }

        /// <summary>
        /// Creates a range for a complete Nepali calendar year.
        /// </summary>
        /// <param name="year">The Nepali calendar year.</param>
        /// <returns>A new NepaliDateRange that spans the entire specified calendar year.</returns>
        public static NepaliDateRange ForCalendarYear(int year)
        {
            var firstDay = new NepaliDate(year, 1, 1); // 1 Baisakh
            var lastDay = new NepaliDate(year, 12, 1).MonthEndDate(); // Last day of Chaitra
            return new NepaliDateRange(firstDay, lastDay);
        }

        /// <summary>
        /// Creates a range for the current Nepali month.
        /// </summary>
        /// <returns>A new NepaliDateRange that spans the current Nepali month.</returns>
        public static NepaliDateRange CurrentMonth()
        {
            var today = new NepaliDate(DateTime.Today);
            return ForMonth(today.Year, today.Month);
        }

        /// <summary>
        /// Creates a range for the current Nepali fiscal year.
        /// </summary>
        /// <returns>A new NepaliDateRange that spans the current Nepali fiscal year.</returns>
        public static NepaliDateRange CurrentFiscalYear()
        {
            var today = new NepaliDate(DateTime.Today);
            int fiscalYear = today.Month >= 4 ? today.Year : today.Year - 1;
            return ForFiscalYear(fiscalYear);
        }

        /// <summary>
        /// Creates a range for the current Nepali calendar year.
        /// </summary>
        /// <returns>A new NepaliDateRange that spans the current Nepali calendar year.</returns>
        public static NepaliDateRange CurrentCalendarYear()
        {
            var today = new NepaliDate(DateTime.Today);
            return ForCalendarYear(today.Year);
        }

        /// <summary>
        /// Determines whether the range contains the specified date.
        /// </summary>
        /// <param name="date">The date to check.</param>
        /// <returns>true if the range contains the specified date; otherwise, false.</returns>
        public bool Contains(NepaliDate date)
        {
            return !IsEmpty && date >= Start && date <= End;
        }

        /// <summary>
        /// Determines whether this range fully contains another range.
        /// </summary>
        /// <param name="other">The range to check.</param>
        /// <returns>true if this range fully contains the other range; otherwise, false.</returns>
        /// <remarks>
        /// An empty range never contains another range.
        /// A range always contains another empty range.
        /// </remarks>
        public bool Contains(NepaliDateRange other)
        {
            if (IsEmpty)
            {
                return false;
            }

            if (other.IsEmpty)
            {
                return true;
            }

            return other.Start >= Start && other.End <= End;
        }

        /// <summary>
        /// Determines whether this range overlaps with another range.
        /// </summary>
        /// <param name="other">The range to check for overlap.</param>
        /// <returns>true if the ranges overlap; otherwise, false.</returns>
        /// <remarks>
        /// Two ranges overlap if they share at least one date.
        /// Empty ranges never overlap with any other range.
        /// </remarks>
        public bool Overlaps(NepaliDateRange other)
        {
            if (IsEmpty || other.IsEmpty)
            {
                return false;
            }

            return Start <= other.End && End >= other.Start;
        }

        /// <summary>
        /// Determines whether this range is adjacent to another range.
        /// </summary>
        /// <param name="other">The range to check for adjacency.</param>
        /// <returns>true if the ranges are adjacent; otherwise, false.</returns>
        /// <remarks>
        /// Two ranges are adjacent if they don't overlap but one range's end date is exactly one day before
        /// the other range's start date, or one range's start date is exactly one day after the other range's end date.
        /// Empty ranges are never adjacent to any other range.
        /// </remarks>
        public bool IsAdjacentTo(NepaliDateRange other)
        {
            if (IsEmpty || other.IsEmpty)
            {
                return false;
            }

            // This end + 1 day = other start OR other end + 1 day = this start
            return End.AddDays(1) == other.Start || other.End.AddDays(1) == Start;
        }

        /// <summary>
        /// Gets the intersection of this range and another range.
        /// </summary>
        /// <param name="other">The range to intersect with.</param>
        /// <returns>A new NepaliDateRange representing the intersection of the two ranges.</returns>
        /// <remarks>
        /// The intersection is the range of dates that are in both ranges.
        /// If the ranges don't overlap, an empty range is returned.
        /// </remarks>
        public NepaliDateRange Intersect(NepaliDateRange other)
        {
            if (IsEmpty || other.IsEmpty || !Overlaps(other))
            {
                return new NepaliDateRange(Start, Start.AddDays(-1)); // Empty range
            }

            var intersectStart = Start > other.Start ? Start : other.Start;
            var intersectEnd = End < other.End ? End : other.End;
            return new NepaliDateRange(intersectStart, intersectEnd);
        }

        /// <summary>
        /// Gets the union of this range and another range.
        /// </summary>
        /// <param name="other">The range to union with.</param>
        /// <returns>A new NepaliDateRange representing the union of the two ranges.</returns>
        /// <remarks>
        /// The union is the range that includes all dates from both ranges.
        /// If the ranges don't overlap and aren't adjacent, a gap will exist between them, and the union
        /// will include all dates from the start of the earlier range to the end of the later range.
        /// If either range is empty, the other range is returned unchanged.
        /// </remarks>
        public NepaliDateRange Union(NepaliDateRange other)
        {
            if (IsEmpty)
            {
                return other;
            }

            if (other.IsEmpty)
            {
                return this;
            }

            var unionStart = Start < other.Start ? Start : other.Start;
            var unionEnd = End > other.End ? End : other.End;
            return new NepaliDateRange(unionStart, unionEnd);
        }

        /// <summary>
        /// Gets a range that represents this range with the specified other range excluded.
        /// </summary>
        /// <param name="other">The range to exclude.</param>
        /// <returns>
        /// An array of 0, 1, or 2 ranges that represent this range with the other range excluded.
        /// Returns an empty array if this range is fully contained by the other range.
        /// Returns this range unchanged if the ranges don't overlap.
        /// Returns one range if the excluded range overlaps with one end of this range.
        /// Returns two ranges if the excluded range is in the middle of this range.
        /// </returns>
        public NepaliDateRange[] Except(NepaliDateRange other)
        {
            // If this range is empty or doesn't overlap with the other range, return this range unchanged
            if (IsEmpty || !Overlaps(other))
            {
                return new[] { this };
            }

            // If this range is completely contained by the other range, return empty array
            if (other.Contains(this))
            {
                return Array.Empty<NepaliDateRange>();
            }

            var result = new List<NepaliDateRange>(2);

            // Handle the left part (if any)
            if (Start < other.Start)
            {
                result.Add(new NepaliDateRange(Start, other.Start.AddDays(-1)));
            }

            // Handle the right part (if any)
            if (End > other.End)
            {
                result.Add(new NepaliDateRange(other.End.AddDays(1), End));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Splits this range into months.
        /// </summary>
        /// <returns>An array of NepaliDateRange objects, each representing a complete or partial month within the range.</returns>
        /// <remarks>
        /// This is useful for reporting by month or for performing operations that need to be grouped by months.
        /// Empty ranges return an empty array.
        /// </remarks>
        public NepaliDateRange[] SplitByMonth()
        {
            if (IsEmpty)
            {
                return Array.Empty<NepaliDateRange>();
            }

            var result = new List<NepaliDateRange>();
            var current = Start;

            while (current <= End)
            {
                // Calculate the end of the current month
                var monthEnd = new NepaliDate(current.Year, current.Month, 1).MonthEndDate();
                // If month end is beyond our range end, use the range end
                if (monthEnd > End)
                {
                    monthEnd = End;
                }

                // Add this month's range
                result.Add(new NepaliDateRange(current, monthEnd));

                // Move to the first day of the next month
                if (current.Month == 12)
                {
                    current = new NepaliDate(current.Year + 1, 1, 1);
                }
                else
                {
                    current = new NepaliDate(current.Year, current.Month + 1, 1);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Splits this range into fiscal quarters.
        /// </summary>
        /// <returns>An array of NepaliDateRange objects, each representing a complete or partial fiscal quarter within the range.</returns>
        /// <remarks>
        /// The Nepali fiscal quarters are:
        /// - First Quarter: Shrawan, Bhadra, Ashwin (Month 4, 5, 6)
        /// - Second Quarter: Kartik, Mangsir, Poush (Month 7, 8, 9)
        /// - Third Quarter: Magh, Falgun, Chaitra (Month 10, 11, 12)
        /// - Fourth Quarter: Baisakh, Jestha, Ashadh (Month 1, 2, 3)
        /// Empty ranges return an empty array.
        /// </remarks>
        public NepaliDateRange[] SplitByFiscalQuarter()
        {
            if (IsEmpty)
            {
                return Array.Empty<NepaliDateRange>();
            }

            var result = new List<NepaliDateRange>();
            var current = Start;

            while (current <= End)
            {
                // Determine the end month of the current quarter
                int endQuarterMonth;
                if (current.Month >= 1 && current.Month <= 3)
                {
                    endQuarterMonth = 3; // Fourth quarter ends in month 3 (Ashadh)
                }
                else if (current.Month >= 4 && current.Month <= 6)
                {
                    endQuarterMonth = 6; // First quarter ends in month 6 (Ashwin)
                }
                else if (current.Month >= 7 && current.Month <= 9)
                {
                    endQuarterMonth = 9; // Second quarter ends in month 9 (Poush)
                }
                else
                {
                    endQuarterMonth = 12; // Third quarter ends in month 12 (Chaitra)
                }

                // Calculate the end of the current quarter
                var quarterEnd = new NepaliDate(current.Year, endQuarterMonth, 1).MonthEndDate();
                // If quarter end is beyond our range end, use the range end
                if (quarterEnd > End)
                {
                    quarterEnd = End;
                }

                // Add this quarter's range
                result.Add(new NepaliDateRange(current, quarterEnd));

                // Move to the first day of the next quarter
                if (endQuarterMonth == 12)
                {
                    current = new NepaliDate(current.Year + 1, 1, 1);
                }
                else
                {
                    current = new NepaliDate(current.Year, endQuarterMonth + 1, 1);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Returns a collection of dates within the range, with a specified interval between each date.
        /// </summary>
        /// <param name="interval">The interval (in days) between each date in the collection.</param>
        /// <returns>A collection of dates within the range, separated by the specified interval.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when interval is less than 1.</exception>
        /// <remarks>
        /// This is useful for generating dates at regular intervals, such as every week or every 10 days.
        /// Empty ranges return an empty collection.
        /// </remarks>
        public IEnumerable<NepaliDate> DatesWithInterval(int interval)
        {
            if (interval < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(interval), "Interval must be at least 1.");
            }

            if (IsEmpty)
            {
                yield break;
            }

            var current = Start;
            while (current <= End)
            {
                yield return current;
                current = current.AddDays(interval);
            }
        }

        /// <summary>
        /// Returns a collection of working days within the range (excluding Saturdays).
        /// </summary>
        /// <param name="excludeSunday">Whether to exclude Sundays as well. Default is false.</param>
        /// <returns>A collection of working days within the range.</returns>
        /// <remarks>
        /// In Nepal, Saturday is traditionally the weekly holiday.
        /// Some businesses also close on Sunday, which can be excluded by setting the parameter.
        /// Empty ranges return an empty collection.
        /// </remarks>
        public IEnumerable<NepaliDate> WorkingDays(bool excludeSunday = false)
        {
            if (IsEmpty)
            {
                yield break;
            }

            var current = Start;
            while (current <= End)
            {
                var dayOfWeek = current.DayOfWeek;
                if (dayOfWeek != DayOfWeek.Saturday && (!excludeSunday || dayOfWeek != DayOfWeek.Sunday))
                {
                    yield return current;
                }
                current = current.AddDays(1);
            }
        }

        /// <summary>
        /// Returns a collection of weekend days within the range (Saturdays and optionally Sundays).
        /// </summary>
        /// <param name="includeSunday">Whether to include Sundays as weekend days. Default is true.</param>
        /// <returns>A collection of weekend days within the range.</returns>
        /// <remarks>
        /// In Nepal, Saturday is traditionally the weekly holiday.
        /// Some businesses also consider Sunday as part of the weekend.
        /// Empty ranges return an empty collection.
        /// </remarks>
        public IEnumerable<NepaliDate> WeekendDays(bool includeSunday = true)
        {
            if (IsEmpty)
            {
                yield break;
            }

            var current = Start;
            while (current <= End)
            {
                var dayOfWeek = current.DayOfWeek;
                if (dayOfWeek == DayOfWeek.Saturday || (includeSunday && dayOfWeek == DayOfWeek.Sunday))
                {
                    yield return current;
                }
                current = current.AddDays(1);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through all the dates in the range.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        /// <remarks>
        /// This enables foreach iteration over all dates in the range.
        /// If the range is empty, no dates are enumerated.
        /// Be cautious when using this with very large ranges, as it will enumerate every single date.
        /// </remarks>
        public IEnumerator<NepaliDate> GetEnumerator()
        {
            if (IsEmpty)
            {
                yield break;
            }

            var current = Start;
            while (current <= End)
            {
                yield return current;
                current = current.AddDays(1);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through all the dates in the range.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns a string representation of the date range.
        /// </summary>
        /// <returns>A string in the format "Start - End", or "Empty Range" if the range is empty.</returns>
        public override string ToString()
        {
            if (IsEmpty)
            {
                return "Empty Range";
            }

            return $"{Start} - {End}";
        }

        /// <summary>
        /// Gets a formatted string representation of the date range.
        /// </summary>
        /// <param name="format">The date format to use for the start and end dates.</param>
        /// <param name="separator">The separator to use between the dates.</param>
        /// <returns>A formatted string representing the date range.</returns>
        /// <remarks>
        /// If the range is empty, "Empty Range" is returned.
        /// The format is applied to both start and end dates.
        /// </remarks>
        public string ToString(DateFormats format, Separators separator = Separators.ForwardSlash)
        {
            if (IsEmpty)
            {
                return "Empty Range";
            }

            return $"{Start.ToString(format, separator)} - {End.ToString(format, separator)}";
        }

        /// <summary>
        /// Determines whether this range is equal to another range.
        /// </summary>
        /// <param name="other">The range to compare with.</param>
        /// <returns>true if the ranges are equal; otherwise, false.</returns>
        /// <remarks>
        /// Two ranges are equal if they have the same start and end dates,
        /// or if both are empty.
        /// </remarks>
        public bool Equals(NepaliDateRange other)
        {
            if (IsEmpty && other.IsEmpty)
            {
                return true;
            }

            return Start == other.Start && End == other.End;
        }

        /// <summary>
        /// Determines whether this range is equal to another object.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>true if the object is a NepaliDateRange and is equal to this range; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is NepaliDateRange other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code for this range.
        /// </summary>
        /// <returns>A hash code based on the start and end dates.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Start.GetHashCode() * 397) ^ End.GetHashCode();
            }
        }

        /// <summary>
        /// Determines whether two ranges are equal.
        /// </summary>
        /// <param name="left">The first range.</param>
        /// <param name="right">The second range.</param>
        /// <returns>true if the ranges are equal; otherwise, false.</returns>
        public static bool operator ==(NepaliDateRange left, NepaliDateRange right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two ranges are not equal.
        /// </summary>
        /// <param name="left">The first range.</param>
        /// <param name="right">The second range.</param>
        /// <returns>true if the ranges are not equal; otherwise, false.</returns>
        public static bool operator !=(NepaliDateRange left, NepaliDateRange right)
        {
            return !(left == right);
        }
    }
}