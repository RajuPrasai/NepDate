namespace NepDate.Core.Dictionaries
{
    /// <summary>
    /// Month and weekday names in English and in Nepali (Devanagari), indexed by number so that
    /// formatting needs no enum reflection and no dictionary lookup.
    /// </summary>
    internal static class CalendarNames
    {
        // Index 1 = Baishakh ... 12 = Chaitra, matching NepaliMonths. Index 0 is unused.
        private static readonly string[] MonthsEnglish =
        {
            null, "Baishakh", "Jestha", "Ashad", "Shrawan", "Bhadra", "Ashoj",
            "Kartik", "Mangsir", "Poush", "Magh", "Falgun", "Chaitra",
        };

        private static readonly string[] MonthsNepali =
        {
            null, "बैशाख", "जेठ", "असार", "साउन", "भदौ", "असोज",
            "कार्तिक", "मंसिर", "पुष", "माघ", "फागुन", "चैत",
        };

        // Index 0 = Sunday ... 6 = Saturday, matching System.DayOfWeek.
        private static readonly string[] WeekdaysEnglish =
        {
            "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday",
        };

        private static readonly string[] WeekdaysNepali =
        {
            "आइतवार", "सोमवार", "मङ्गलवार", "बुधवार", "बिहिवार", "शुक्रवार", "शनिवार",
        };

        /// <summary>
        /// Returns the month name, or <see langword="null"/> when <paramref name="month"/> is not 1-12
        /// (the month of a default <see cref="NepaliDate"/> is 0).
        /// </summary>
        internal static string Month(int month, bool nepali)
        {
            if ((uint)(month - 1) > 11u)
            {
                return null;
            }

            return nepali ? MonthsNepali[month] : MonthsEnglish[month];
        }

        internal static string Weekday(System.DayOfWeek dayOfWeek, bool nepali)
        {
            return nepali ? WeekdaysNepali[(int)dayOfWeek] : WeekdaysEnglish[(int)dayOfWeek];
        }
    }
}
