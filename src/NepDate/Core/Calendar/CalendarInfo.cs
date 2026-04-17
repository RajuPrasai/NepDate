namespace NepDate.Core.Calendar
{
    /// <summary>
    /// Calendar metadata for a specific Nepali date, populated from scraped HamroPatro data (2001-2089 BS).
    /// All string properties return empty string when data is not available.
    /// Events arrays are empty (not null) when there are no events for the day.
    /// </summary>
    public readonly struct CalendarInfo
    {
        /// <summary>Tithi name in Nepali. Empty when not available.</summary>
        public string TithiNp { get; }

        /// <summary>Tithi name in English. Empty when not available.</summary>
        public string TithiEn { get; }

        /// <summary>Whether this day is a public holiday.</summary>
        public bool IsPublicHoliday { get; }

        /// <summary>Event names in Nepali. Empty array when there are no events.</summary>
        public string[] EventsNp { get; }

        /// <summary>Event names in English. Empty array when there are no events.</summary>
        public string[] EventsEn { get; }

        internal CalendarInfo(
            string tithiNp,
            string tithiEn,
            bool isPublicHoliday,
            string[] eventsNp,
            string[] eventsEn)
        {
            TithiNp = tithiNp;
            TithiEn = tithiEn;
            IsPublicHoliday = isPublicHoliday;
            EventsNp = eventsNp;
            EventsEn = eventsEn;
        }
    }
}
