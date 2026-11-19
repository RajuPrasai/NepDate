namespace NepDate.Core.Calendar
{
    /// <summary>
    /// Immutable snapshot of calendar metadata for a specific Nepali date, compiled from
    /// authoritative Bikram Sambat calendar references. Tithi is available for 2078–2083 BS, events
    /// and public holidays for 2077–2083 BS (plus a few days of 2084). All members return empty or default values for other dates.
    /// </summary>
    public readonly struct CalendarInfo
    {
        /// <summary>
        /// Tithi (lunar day) name in Nepali Devanagari script.
        /// <see cref="string.Empty"/> when no Tithi data is recorded for this date.
        /// </summary>
        public string TithiNp { get; }

        /// <summary>
        /// Tithi (lunar day) name transliterated to English.
        /// <see cref="string.Empty"/> when no Tithi data is recorded for this date.
        /// </summary>
        public string TithiEn { get; }

        /// <summary>
        /// <see langword="true"/> when this day is a gazetted public holiday in Nepal; otherwise <see langword="false"/>.
        /// </summary>
        public bool IsPublicHoliday { get; }

        /// <summary>
        /// Names of events and observances for this day in Nepali Devanagari.
        /// An empty (non-null) array when no events are recorded.
        /// </summary>
        public string[] EventsNp { get; }

        /// <summary>
        /// Names of events and observances for this day transliterated to English.
        /// An empty (non-null) array when no events are recorded.
        /// </summary>
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
