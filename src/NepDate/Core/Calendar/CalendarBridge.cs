using NepDate.Core.Dictionaries;

namespace NepDate.Core.Calendar
{
    /// <summary>
    /// Reads the bundled calendar data (Tithi, public holidays and events) for a day.
    /// </summary>
    /// <remarks>
    /// Every method takes the day offset counted from 1 Baisakh 2001 BS, the first day of the data.
    /// An offset outside <c>[0, CalendarOffsets.TotalDays)</c> means "no data" and yields empty results.
    /// </remarks>
    internal static class CalendarBridge
    {
        private static readonly string[] _emptyStrings = new string[0];

        private static readonly OffsetIndex _tithiDays = new OffsetIndex(TithiData.DayOffsets, CalendarOffsets.TotalDays);
        private static readonly OffsetIndex _holidays = new OffsetIndex(HolidayData.DayOffsets, CalendarOffsets.TotalDays);
        private static readonly OffsetIndex _eventDays = new OffsetIndex(EventData.DayOffsets, CalendarOffsets.TotalDays);

        /// <summary>
        /// Retrieves all calendar metadata for the day in one pass.
        /// Returns an empty <see cref="CalendarInfo"/> when the day falls outside the 2001-2089 BS data range.
        /// </summary>
        internal static CalendarInfo GetCalendarInfo(int offset)
        {
            if ((uint)offset >= (uint)CalendarOffsets.TotalDays)
                return new CalendarInfo(string.Empty, string.Empty, false, _emptyStrings, _emptyStrings);

            string tithiNp = string.Empty;
            string tithiEn = string.Empty;
            int ti = _tithiDays.IndexOf(offset);
            if (ti >= 0)
            {
                var entry = CalendarStrings.Pool[TithiData.PoolIds[ti]];
                tithiNp = entry.Np;
                tithiEn = entry.En;
            }

            bool isHoliday = _holidays.Contains(offset);

            string[] eventsNp;
            string[] eventsEn;
            int ei = _eventDays.IndexOf(offset);
            if (ei < 0)
            {
                eventsNp = _emptyStrings;
                eventsEn = _emptyStrings;
            }
            else
            {
                int start = EventData.SliceStart[ei];
                int count = EventData.SliceStart[ei + 1] - start;
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
        /// Returns the Tithi (lunar day) name in Nepali for the day,
        /// or <see cref="string.Empty"/> when no data is available.
        /// </summary>
        internal static string GetTithiNp(int offset)
        {
            if ((uint)offset >= (uint)CalendarOffsets.TotalDays) return string.Empty;
            int ti = _tithiDays.IndexOf(offset);
            if (ti < 0) return string.Empty;
            return CalendarStrings.Pool[TithiData.PoolIds[ti]].Np;
        }

        /// <summary>
        /// Returns the Tithi (lunar day) name in English for the day,
        /// or <see cref="string.Empty"/> when no data is available.
        /// </summary>
        internal static string GetTithiEn(int offset)
        {
            if ((uint)offset >= (uint)CalendarOffsets.TotalDays) return string.Empty;
            int ti = _tithiDays.IndexOf(offset);
            if (ti < 0) return string.Empty;
            return CalendarStrings.Pool[TithiData.PoolIds[ti]].En;
        }

        /// <summary>
        /// Returns <see langword="true"/> when the day is a gazetted public holiday;
        /// <see langword="false"/> when it is not, or when the day falls outside the data range.
        /// </summary>
        internal static bool IsHoliday(int offset)
        {
            return (uint)offset < (uint)CalendarOffsets.TotalDays && _holidays.Contains(offset);
        }

        /// <summary>
        /// Returns event names in Nepali for the day as an array.
        /// Returns an empty (non-null) array when no events exist or the day is outside the data range.
        /// </summary>
        internal static string[] GetEventsNp(int offset)
        {
            return GetEvents(offset, nepali: true);
        }

        /// <summary>
        /// Returns event names in English for the day as an array.
        /// Returns an empty (non-null) array when no events exist or the day is outside the data range.
        /// </summary>
        internal static string[] GetEventsEn(int offset)
        {
            return GetEvents(offset, nepali: false);
        }

        private static string[] GetEvents(int offset, bool nepali)
        {
            if ((uint)offset >= (uint)CalendarOffsets.TotalDays) return _emptyStrings;
            int ei = _eventDays.IndexOf(offset);
            if (ei < 0) return _emptyStrings;
            int start = EventData.SliceStart[ei];
            int count = EventData.SliceStart[ei + 1] - start;
            string[] result = new string[count];
            for (int i = 0; i < count; i++)
            {
                var entry = CalendarStrings.Pool[EventData.Entries[start + i]];
                result[i] = nepali ? entry.Np : entry.En;
            }
            return result;
        }
    }
}
