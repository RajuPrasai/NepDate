using System;

namespace NepDate
{
    public readonly partial struct NepaliDate
    {
        /// <summary>
        /// Tries to parse the specified string representation of a Nepali date and returns a value indicating whether the parsing succeeded.
        /// </summary>
        /// <param name="rawNepDate">The raw Nepali date string in the format "YYYY/MM/DD".</param>
        /// <param name="result">When this method returns, contains the NepaliDate value equivalent to the Nepali date contained in rawNepDate, if the parsing succeeded, or default if the parsing failed.</param>
        /// <returns>true if the parsing succeeded; otherwise, false.</returns>
        public static bool TryParse(string rawNepDate, out NepaliDate result)
        {
            try
            {
                result = Parse(rawNepDate);
                return true;
            }
            catch (Exception)
            {
                result = default;
                return false;
            }
        }

        public static bool TryParse(string rawNepDate, out NepaliDate result, bool autoAdjust, bool monthInMiddle = true)
        {
            try
            {
                result = Parse(rawNepDate, autoAdjust, monthInMiddle);
                return true;
            }
            catch (Exception)
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// Parses the specified string representation of a Nepali date and returns a NepaliDate object.
        /// </summary>
        /// <param name="rawNepDate">The raw Nepali date string in the format "YYYY/MM/DD".</param>
        /// <returns>A NepaliDate object that is equivalent to the Nepali date contained in rawNepDate.</returns>
        public static NepaliDate Parse(string rawNepaliDate)
        {
            return new NepaliDate(rawNepaliDate);
        }

        public static NepaliDate Parse(string rawNepaliDate, bool autoAdjust, bool monthInMiddle = true)
        {
            return new NepaliDate(rawNepaliDate, autoAdjust, monthInMiddle);
        }
    }
}
