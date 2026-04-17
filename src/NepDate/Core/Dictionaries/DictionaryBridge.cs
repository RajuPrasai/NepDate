using System;

namespace NepDate.Core.Dictionaries
{
    /// <summary>
    /// Provides a bridge between calendar dictionaries and application code for efficient date conversions.
    /// This class manages the complex mapping relationships between Nepali and English calendar systems
    /// and implements caching mechanisms to optimize repeated conversions.
    /// </summary>
    /// <remarks>
    /// The DictionaryBridge serves as an abstraction layer over the raw calendar data dictionaries,
    /// handling the details of calendar system conversion and providing a clean API for date conversions.
    /// It implements caching to improve performance for repeated date conversions.
    /// 
    /// The Nepali calendar (Bikram Sambat) has irregular month lengths that don't follow simple rules,
    /// requiring comprehensive lookup tables to perform accurate conversions.
    /// </remarks>
    internal static class DictionaryBridge
    {
        internal static class NepToEng
        {
            /// <summary>
            /// Converts a Nepali date to its equivalent English (Gregorian) date.
            /// </summary>
            /// <param name="nepYear">The Nepali year (in BS).</param>
            /// <param name="nepMonth">The Nepali month (1-12).</param>
            /// <param name="nepDay">The Nepali day (1-32, depending on month).</param>
            /// <returns>The equivalent English (Gregorian) DateTime.</returns>
            /// <exception cref="InvalidNepaliDateFormatException">
            /// Thrown when the provided Nepali date is outside the supported range or invalid.
            /// </exception>
            /// <remarks>
            /// This method first checks the cache for previously converted dates.
            /// If not found, it uses the conversion dictionaries to calculate the equivalent English date.
            /// 
            /// The conversion works by:
            /// 1. Finding the English date equivalent to the 1st day of the specified Nepali month
            /// 2. Adding the appropriate number of days to reach the specified Nepali day
            /// 
            /// Results are cached to improve performance for repeated conversions.
            /// </remarks>
            internal static DateTime GetEnglishDate(int nepYear, int nepMonth, int nepDay)
            {
                var index = (nepYear - NepaliToEnglish.MinYear) * 12 + (nepMonth - 1);
                if ((uint)index >= (uint)NepaliToEnglish.data.Length)
                {
                    throw new InvalidNepaliDateFormatException();
                }

                var dictVal = NepaliToEnglish.data[index];
                return new DateTime(dictVal.EngYear, dictVal.EngMonth, dictVal.EngDay).AddDays(nepDay - dictVal.NepMonthEndDay);
            }

            /// <summary>
            /// Gets the number of days in a specific Nepali month.
            /// </summary>
            /// <param name="nepYear">The Nepali year (in BS).</param>
            /// <param name="nepMonth">The Nepali month (1-12).</param>
            /// <returns>The number of days in the specified Nepali month (typically 29-32).</returns>
            /// <exception cref="InvalidNepaliDateFormatException">
            /// Thrown when the provided Nepali date is outside the supported range or invalid.
            /// </exception>
            /// <remarks>
            /// Nepali months have varying lengths (29-32 days) that don't follow a simple pattern.
            /// This method retrieves the correct length from the conversion dictionaries.
            /// 
            /// Results are cached to improve performance for repeated queries.
            /// </remarks>
            internal static int GetNepaliMonthEndDay(int nepYear, int nepMonth)
            {
                var index = (nepYear - NepaliToEnglish.MinYear) * 12 + (nepMonth - 1);
                if ((uint)index >= (uint)NepaliToEnglish.data.Length)
                {
                    throw new InvalidNepaliDateFormatException();
                }

                return NepaliToEnglish.data[index].NepMonthEndDay;
            }
        }

        /// <summary>
        /// Provides conversion methods from English (Gregorian) dates to Nepali dates.
        /// </summary>
        internal static class EngToNep
        {
            /// <summary>
            /// Converts an English (Gregorian) date to its equivalent Nepali date.
            /// </summary>
            /// <param name="engYear">The English year.</param>
            /// <param name="engMonth">The English month (1-12).</param>
            /// <param name="engDay">The English day (1-31, depending on month).</param>
            /// <returns>A tuple containing the year, month, and day components of the equivalent Nepali date.</returns>
            /// <remarks>
            /// This method first checks the cache for previously converted dates.
            /// If not found, it uses the conversion dictionaries to calculate the equivalent Nepali date.
            /// 
            /// The conversion works by:
            /// 1. Finding the Nepali date equivalent to the last day of the specified English month
            /// 2. Calculating the difference in days between the last day and the specified day
            /// 3. Subtracting that number of days from the equivalent Nepali date
            /// 
            /// Results are cached to improve performance for repeated conversions.
            /// </remarks>
            internal static (int, int, int) GetNepaliDate(int engYear, int engMonth, int engDay)
            {
                var index = (engYear - EnglishToNepali.MinYear) * 12 + (engMonth - EnglishToNepali.MinMonth);
                var dictVal = EnglishToNepali.data[index];
                return SubtractNepaliDays(dictVal.NepYear, dictVal.NepMonth, dictVal.NepDay, (dictVal.EngMonthEndDay - engDay));
            }

            /// <summary>
            /// Subtracts a number of days from a Nepali date, handling month and year boundaries correctly.
            /// </summary>
            /// <param name="yearBs">The starting Nepali year.</param>
            /// <param name="monthBs">The starting Nepali month.</param>
            /// <param name="dayBs">The starting Nepali day.</param>
            /// <param name="daysToSubtract">The number of days to subtract.</param>
            /// <returns>A tuple containing the resulting Nepali date components after subtraction.</returns>
            /// <remarks>
            /// This method handles the complexities of subtracting days from a Nepali date, 
            /// including crossing month and year boundaries.
            /// 
            /// Since Nepali months have irregular lengths, the calculation needs to account for 
            /// the correct number of days in each month when crossing month boundaries.
            private static (int yearBs, int monthBs, int dayBs) SubtractNepaliDays(int yearBs, int monthBs, int dayBs, int daysToSubtract)
            {
                var newDayBs = dayBs - daysToSubtract;

                while (newDayBs <= 0)
                {
                    if (monthBs == 1)
                    {
                        yearBs--;
                        monthBs = 12;
                    }
                    else
                    {
                        monthBs--;
                    }
                    newDayBs += NepToEng.GetNepaliMonthEndDay(yearBs, monthBs);
                }
                return (yearBs, monthBs, newDayBs);
            }
        }
    }
}
