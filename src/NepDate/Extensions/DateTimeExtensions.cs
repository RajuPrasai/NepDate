using System;

namespace NepDate
{
    /// <summary>
    /// Provides extension methods for common date conversion operations between calendar systems.
    /// </summary>
    /// <remarks>
    /// These extension methods make it easy to convert between DateTime (Gregorian calendar)
    /// and NepaliDate (Bikram Sambat calendar) objects using a fluent, natural syntax.
    /// </remarks>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a DateTime object (Gregorian calendar) to its equivalent NepaliDate (Bikram Sambat calendar).
        /// </summary>
        /// <param name="englishDate">The DateTime object to convert.</param>
        /// <returns>A NepaliDate object representing the equivalent date in the Bikram Sambat calendar.</returns>
        /// <remarks>
        /// This extension method allows for natural and readable syntax when converting dates:
        /// <code>
        /// DateTime today = DateTime.Now;
        /// NepaliDate nepaliToday = today.ToNepaliDate();
        /// </code>
        /// 
        /// Only the date portion (year, month, day) of the DateTime is used in the conversion.
        /// The time portion is ignored, as NepaliDate only represents date values, not time.
        /// 
        /// For converting multiple dates efficiently, consider using the BulkConvert class instead.
        /// </remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown if the input date is outside the supported range (equivalent to 1901-04-13 to 2143-04-12).
        /// This includes DateTime.MinValue, DateTime.MaxValue, and dates before 1901 or after 2143.
        /// </exception>
        public static NepaliDate ToNepaliDate(this DateTime englishDate)
        {
            return new NepaliDate(englishDate);
        }
    }
}
