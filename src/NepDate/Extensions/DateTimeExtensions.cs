using System;

namespace NepDate
{
    /// <summary>
    /// Extension methods for converting <see cref="DateTime"/> values to <see cref="NepaliDate"/>.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a Gregorian <see cref="DateTime"/> to its Bikram Sambat equivalent.
        /// </summary>
        /// <param name="englishDate">The Gregorian date to convert. Only the date portion is used; the time component is ignored.</param>
        /// <returns>A <see cref="NepaliDate"/> representing the same calendar day in the Bikram Sambat system.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="englishDate"/> falls before 1901-04-13 or after 2143-04-12
        /// (the Gregorian bounds of the supported BS range).
        /// </exception>
        /// <example>
        /// <code>
        /// NepaliDate today = DateTime.Today.ToNepaliDate();
        /// NepaliDate specific = new DateTime(2023, 7, 31).ToNepaliDate();  // 2080/04/15
        /// </code>
        /// </example>
        public static NepaliDate ToNepaliDate(this DateTime englishDate)
        {
            return new NepaliDate(englishDate);
        }
    }
}
