using NepDate.Core.Dictionaries;

namespace NepDate.Core.Calendar
{
    internal static class CalendarBridge
    {
        private static readonly string[] _emptyStrings = new string[0];

        /// <summary>
        /// Retrieves all calendar metadata for the specified date in a single dictionary lookup.
        /// Returns an empty <see cref="CalendarInfo"/> when the date falls outside the 2001\u20132089 BS data range.
        /// </summary>
        internal static CalendarInfo GetCalendarInfo(int year, int month, int day)
        {
            int offset = CalendarOffsets.GetOffset(year, month, day);
            if (offset < 0)
                return new CalendarInfo(string.Empty, string.Empty, false, _emptyStrings, _emptyStrings);

            string tithiNp = string.Empty;
            string tithiEn = string.Empty;
            int ti = TithiData.FindIndex(offset);
            if (ti >= 0)
            {
                var entry = CalendarStrings.Pool[TithiData.PoolIds[ti]];
                tithiNp = entry.Np;
                tithiEn = entry.En;
            }

            bool isHoliday = HolidayData.Contains(offset);

            string[] eventsNp;
            string[] eventsEn;
            int ei = EventData.FindIndex(offset);
            if (ei < 0)
            {
                eventsNp = _emptyStrings;
                eventsEn = _emptyStrings;
            }
            else
            {
                int start = EventData.SliceStart[ei];
                int end = EventData.SliceStart[ei + 1];
                int count = end - start;
                eventsNp = new string[count];
                eventsEn = new string[count];
                for (int i = 0; i < count; i++)
                {
                    var entry = CalendarStrings.Pool[EventData.Entries[start + i]];
                    eventsNp[i] = entry.Np;
                    eventsEn[i] = entry.En;
                }
            }

            return new CalendarInfo(tithiNp, tithiEn, isHoliday, eventsNp, eventsEn);
        }

        /// <summary>
        /// Returns the Tithi (lunar day) name in Nepali for the specified date,
        /// or <see cref="string.Empty"/> when no data is available.
        /// </summary>
        internal static string GetTithiNp(int year, int month, int day)
        {
            int offset = CalendarOffsets.GetOffset(year, month, day);
            if (offset < 0) return string.Empty;
            int ti = TithiData.FindIndex(offset);
            if (ti < 0) return string.Empty;
            return CalendarStrings.Pool[TithiData.PoolIds[ti]].Np;
        }

        /// <summary>
        /// Returns the Tithi (lunar day) name in English for the specified date,
        /// or <see cref="string.Empty"/> when no data is available.
        /// </summary>
        internal static string GetTithiEn(int year, int month, int day)
        {
            int offset = CalendarOffsets.GetOffset(year, month, day);
            if (offset < 0) return string.Empty;
            int ti = TithiData.FindIndex(offset);
            if (ti < 0) return string.Empty;
            return CalendarStrings.Pool[TithiData.PoolIds[ti]].En;
        }

        /// <summary>
        /// Returns <see langword="true"/> when the specified date is a gazetted public holiday;
        /// <see langword="false"/> when it is not, or when the date falls outside the data range.
        /// </summary>
        internal static bool IsHoliday(int year, int month, int day)
        {
            int offset = CalendarOffsets.GetOffset(year, month, day);
            if (offset < 0) return false;
            return HolidayData.Contains(offset);
        }

        /// <summary>
        /// Returns event names in Nepali for the specified date as an array.
        /// Returns an empty (non-null) array when no events exist or the date is outside the data range.
        /// </summary>
        internal static string[] GetEventsNp(int year, int month, int day)
        {
            int offset = CalendarOffsets.GetOffset(year, month, day);
            if (offset < 0) return _emptyStrings;
            int ei = EventData.FindIndex(offset);
            if (ei < 0) return _emptyStrings;
            int start = EventData.SliceStart[ei];
            int end = EventData.SliceStart[ei + 1];
            int count = end - start;
            string[] result = new string[count];
            for (int i = 0; i < count; i++)
                result[i] = CalendarStrings.Pool[EventData.Entries[start + i]].Np;
            return result;
        }

        /// <summary>
        /// Returns event names in English for the specified date as an array.
        /// Returns an empty (non-null) array when no events exist or the date is outside the data range.
        /// </summary>
        internal static string[] GetEventsEn(int year, int month, int day)
        {
            int offset = CalendarOffsets.GetOffset(year, month, day);
            if (offset < 0) return _emptyStrings;
            int ei = EventData.FindIndex(offset);
            if (ei < 0) return _emptyStrings;
            int start = EventData.SliceStart[ei];
            int end = EventData.SliceStart[ei + 1];
            int count = end - start;
            string[] result = new string[count];
            for (int i = 0; i < count; i++)
                result[i] = CalendarStrings.Pool[EventData.Entries[start + i]].En;
            return result;
        }
    }
}
