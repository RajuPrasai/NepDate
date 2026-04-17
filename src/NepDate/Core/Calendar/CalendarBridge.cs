using NepDate.Core.Dictionaries;

namespace NepDate.Core.Calendar
{
    internal static class CalendarBridge
    {
        private static readonly string[] _emptyStrings = new string[0];

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

        internal static string GetTithiNp(int year, int month, int day)
        {
            int offset = CalendarOffsets.GetOffset(year, month, day);
            if (offset < 0) return string.Empty;
            int ti = TithiData.FindIndex(offset);
            if (ti < 0) return string.Empty;
            return CalendarStrings.Pool[TithiData.PoolIds[ti]].Np;
        }

        internal static string GetTithiEn(int year, int month, int day)
        {
            int offset = CalendarOffsets.GetOffset(year, month, day);
            if (offset < 0) return string.Empty;
            int ti = TithiData.FindIndex(offset);
            if (ti < 0) return string.Empty;
            return CalendarStrings.Pool[TithiData.PoolIds[ti]].En;
        }

        internal static bool IsHoliday(int year, int month, int day)
        {
            int offset = CalendarOffsets.GetOffset(year, month, day);
            if (offset < 0) return false;
            return HolidayData.Contains(offset);
        }
    }
}
