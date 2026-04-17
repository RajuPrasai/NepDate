using System;

namespace NepDate
{
    public sealed class InvalidNepaliDateFormatException : FormatException
    {
        public InvalidNepaliDateFormatException(string message = "Invalid Nepali date format") : base(message) { }
    }
}
