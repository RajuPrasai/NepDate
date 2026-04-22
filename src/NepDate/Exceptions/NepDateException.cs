using System;

namespace NepDate
{
    /// <summary>
    /// The exception that is thrown when a string or set of date components cannot be interpreted
    /// as a valid Nepali (Bikram Sambat) date, or when a formatting operation receives an
    /// unrecognised format specifier or separator value.
    /// </summary>
    /// <remarks>
    /// Inherits from <see cref="FormatException"/> so callers can catch it generically alongside
    /// other format-related exceptions from the .NET base class library.
    /// </remarks>
    public sealed class InvalidNepaliDateFormatException : FormatException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="InvalidNepaliDateFormatException"/> with an optional
        /// message describing the invalid input.
        /// </summary>
        /// <param name="message">
        /// A human-readable description of what was invalid. Defaults to
        /// <c>"Invalid Nepali date format"</c> when omitted.
        /// </param>
        public InvalidNepaliDateFormatException(string message = "Invalid Nepali date format") : base(message) { }
    }
}
