using System;

namespace NepDate
{
    public readonly partial struct NepaliDate
    {
        /// <summary>
        /// Tries to parse the specified string representation of a Nepali date and returns a value indicating whether the parsing succeeded.
        /// </summary>
        /// <param name="rawNepDate">The raw Nepali date string in the format <c>"YYYY/MM/DD"</c> or any supported separator variant.</param>
        /// <param name="result">When this method returns <see langword="true"/>, contains the parsed <see cref="NepaliDate"/>; otherwise the default value.</param>
        /// <returns><see langword="true"/> if parsing succeeded; otherwise <see langword="false"/>.</returns>
        public static bool TryParse(string rawNepDate, out NepaliDate result)
        {
            result = default;

            if (!TrySplitNepaliDate(rawNepDate, out var year, out var month, out var day))
                return false;

            if (!IsValidDate(year, month, day))
                return false;

            result = new NepaliDate(year, month, day);
            return true;
        }

        /// <summary>
        /// Tries to parse the specified string with optional heuristic adjustment and returns a value indicating whether the parsing succeeded.
        /// </summary>
        /// <param name="rawNepDate">The raw Nepali date string to parse.</param>
        /// <param name="result">When this method returns <see langword="true"/>, contains the parsed <see cref="NepaliDate"/>; otherwise the default value.</param>
        /// <param name="autoAdjust">
        /// When <see langword="true"/>, applies heuristics to recover valid dates from ambiguous input
        /// (swaps oversized components, expands short year values). See
        /// <see cref="NepaliDate(string, bool, bool)"/> for the full rules.
        /// </param>
        /// <param name="monthInMiddle">
        /// Relevant only when <paramref name="autoAdjust"/> is <see langword="true"/>.
        /// When <see langword="false"/>, month and day are swapped before other adjustments.
        /// </param>
        /// <returns><see langword="true"/> if parsing succeeded; otherwise <see langword="false"/>.</returns>
        public static bool TryParse(string rawNepDate, out NepaliDate result, bool autoAdjust, bool monthInMiddle = true)
        {
            result = default;

            if (!TrySplitNepaliDate(rawNepDate, out var year, out var month, out var day))
                return false;

            if (autoAdjust)
            {
                const int currentMillennium = 2;

                if (day > 32)
                    (year, day) = (day, year);

                if (!monthInMiddle)
                    (month, day) = (day, month);

                if (month > 12 && day < 13)
                    (month, day) = (day, month);

                if (year < 1000)
                    year = currentMillennium * 1000 + year;
            }

            if (!IsValidDate(year, month, day))
                return false;

            result = new NepaliDate(year, month, day);
            return true;
        }

        /// <summary>
        /// Parses the specified string representation of a Nepali date and returns a <see cref="NepaliDate"/>.
        /// </summary>
        /// <param name="rawNepDate">The raw Nepali date string in the format <c>"YYYY/MM/DD"</c> or any supported separator variant.</param>
        /// <returns>A <see cref="NepaliDate"/> equivalent to the date in <paramref name="rawNepDate"/>.</returns>
        /// <exception cref="InvalidNepaliDateFormatException">Thrown when <paramref name="rawNepDate"/> cannot be parsed as a valid Nepali date.</exception>
        public static NepaliDate Parse(string rawNepaliDate)
        {
            return new NepaliDate(rawNepaliDate);
        }

        /// <summary>
        /// Parses the specified string with optional heuristic adjustment and returns a <see cref="NepaliDate"/>.
        /// </summary>
        /// <param name="rawNepaliDate">The raw Nepali date string to parse.</param>
        /// <param name="autoAdjust">
        /// When <see langword="true"/>, applies heuristics to recover valid dates from ambiguous input.
        /// See <see cref="NepaliDate(string, bool, bool)"/> for the full rules.
        /// </param>
        /// <param name="monthInMiddle">
        /// Relevant only when <paramref name="autoAdjust"/> is <see langword="true"/>.
        /// When <see langword="false"/>, month and day are swapped before other adjustments.
        /// </param>
        /// <returns>A <see cref="NepaliDate"/> parsed from <paramref name="rawNepaliDate"/>.</returns>
        /// <exception cref="InvalidNepaliDateFormatException">Thrown when the (possibly adjusted) components do not form a valid Nepali date.</exception>
        public static NepaliDate Parse(string rawNepaliDate, bool autoAdjust, bool monthInMiddle = true)
        {
            return new NepaliDate(rawNepaliDate, autoAdjust, monthInMiddle);
        }
    }
}
