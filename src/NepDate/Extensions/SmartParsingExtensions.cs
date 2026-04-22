using System;

namespace NepDate
{
    /// <summary>
    /// Provides extension methods for intelligent date parsing in the NepDate library.
    /// </summary>
    public static class SmartParsingExtensions
    {
        /// <summary>
        /// Smartly parses a string representation of a Nepali date in various formats.
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>A NepaliDate representing the parsed date.</returns>
        /// <exception cref="FormatException">Thrown when the input string cannot be parsed as a Nepali date.</exception>
        /// <remarks>
        /// This method can handle a wide variety of input formats including:
        /// - Standard formats (YYYY/MM/DD, DD/MM/YYYY, etc.)
        /// - Formats with month names (15 Jestha 2080, Jestha 15, 2080)
        /// - Nepali unicode digits (२०८०/०४/१५)
        /// - Mixed formats and variations with different separators
        /// </remarks>
        /// <example>
        /// <code>
        /// // All these parse to the same date (2080/04/15):
        /// var date1 = "2080/04/15".ToNepaliDate();
        /// var date2 = "15-04-2080".ToNepaliDate();
        /// var date3 = "15 Shrawan 2080".ToNepaliDate();
        /// var date4 = "Shrawan 15, 2080".ToNepaliDate();
        /// var date5 = "२०८०/०४/१५".ToNepaliDate();
        /// var date6 = "15 साउन 2080".ToNepaliDate();
        /// </code>
        /// </example>
        public static NepaliDate ToNepaliDate(this string input)
        {
            return SmartDateParser.Parse(input);
        }

        /// <summary>
        /// Attempts to smartly parse a string representation of a Nepali date in various formats.
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <param name="result">When this method returns, contains the parsed NepaliDate if successful, or default if not.</param>
        /// <returns>true if the parsing was successful; otherwise, false.</returns>
        /// <remarks>
        /// This method is similar to <see cref="ToNepaliDate(string)"/> but returns a boolean indicating success
        /// instead of throwing an exception when parsing fails.
        /// </remarks>
        /// <example>
        /// <code>
        /// if ("15 Shrawan 2080".TryToNepaliDate(out var date))
        /// {
        ///     Console.WriteLine($"Parsed successfully: {date}");
        /// }
        /// </code>
        /// </example>
        public static bool TryToNepaliDate(this string input, out NepaliDate result)
        {
            return SmartDateParser.TryParse(input, out result);
        }
    }
}