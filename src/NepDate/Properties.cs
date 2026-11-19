using NepDate.Core.Calendar;
using NepDate.Core.Dictionaries;
using System;

namespace NepDate
{
    public readonly partial struct NepaliDate
    {
        /// <summary>
        /// The minimum supported year value for Nepali dates (1901 BS).
        /// </summary>
        private const int _minYear = MonthTable.MinYear;

        /// <summary>
        /// The maximum supported year value for Nepali dates (2199 BS).
        /// </summary>
        private const int _maxYear = MonthTable.MaxYear;

        /// <summary>
        /// Number of 100-nanosecond ticks in one day, as used by <see cref="DateTime.Ticks"/>.
        /// </summary>
        private const long _ticksPerDay = 864000000000L;

        /// <summary>
        /// Days from 0001-01-01 AD to 1844-04-11 AD, the Gregorian date of 1 Baisakh 1901 BS.
        /// </summary>
        private const int _epochDayNumber = 673242;

        /// <summary>
        /// Day of the week of 1844-04-11 AD (a Thursday).
        /// </summary>
        private const int _epochDayOfWeek = (int)System.DayOfWeek.Thursday;

        /// <summary>
        /// Day number of 1 Baisakh 2001 BS, the first day covered by the bundled calendar data.
        /// </summary>
        private const int _calendarDataStart = 36526;

        /// <summary>
        /// One plus the number of days from 1 Baisakh 1901 BS to this date. Zero marks the default
        /// (uninitialized) value, so a default instance sorts before every real date.
        /// </summary>
        private readonly int _dayNumber;

        private readonly ushort _year;
        private readonly byte _month;
        private readonly byte _day;

        /// <summary>
        /// Approximate number of days per Nepali month (365 ÷ 12), used to convert fractional
        /// month values to a day count when calling <see cref="AddMonths(double, bool)"/>.
        /// </summary>
        private const double _approxDaysPerMonth = 30.41666666666667;

        /// <summary>
        /// Converts the date to an integer representation in the format YYYYMMDD.
        /// This is optimized to use direct arithmetic instead of string operations.
        /// </summary>
        /// <example>
        /// For the date 2080/05/15, the result would be 20800515.
        /// </example>
        internal int AsInteger => _year * 10000 + _month * 100 + _day;

        /// <summary>
        /// Gets a value indicating whether this instance is the default (uninitialized) value.
        /// </summary>
        /// <value>
        /// <see langword="true"/> when <see cref="Year"/>, <see cref="Month"/>, and <see cref="Day"/> are all zero,
        /// which occurs when a <see cref="NepaliDate"/> is declared but not assigned.
        /// A default instance does not represent a valid date; accessing <see cref="EnglishDate"/> on it will throw.
        /// </value>
        public bool IsDefault => _dayNumber == 0;

        /// <summary>
        /// Gets the year component of this Nepali date (Bikram Sambat).
        /// </summary>
        /// <value>An integer between 1901 and 2199 inclusive.</value>
        public int Year => _year;

        /// <summary>
        /// Gets the month component of this Nepali date.
        /// </summary>
        /// <value>
        /// An integer from 1 (Baisakh) through 12 (Chaitra).
        /// Use <see cref="MonthName"/> to obtain the corresponding <see cref="NepaliMonths"/> enum value.
        /// </value>
        public int Month => _month;

        /// <summary>
        /// Gets the day component of this Nepali date.
        /// </summary>
        /// <value>
        /// An integer from 1 to <see cref="MonthEndDay"/>. The upper bound varies by month and year,
        /// ranging from 29 to 32.
        /// </value>
        public int Day => _day;

        /// <summary>
        /// Gets the Gregorian <see cref="DateTime"/> that corresponds to this Nepali date.
        /// The time component is always midnight (00:00:00).
        /// </summary>
        /// <exception cref="InvalidNepaliDateFormatException">
        /// Thrown when accessed on a default (uninitialized) <see cref="NepaliDate"/> instance.
        /// Check <see cref="IsDefault"/> before accessing this property if the instance may be uninitialized.
        /// </exception>
        /// <remarks>
        /// The returned value always has <see cref="DateTimeKind.Unspecified"/>, because a Nepali date
        /// carries no time zone.
        /// </remarks>
        public DateTime EnglishDate
        {
            get
            {
                if (_dayNumber == 0)
                {
                    ThrowDefaultInstance();
                }

                return new DateTime((_epochDayNumber - 1L + _dayNumber) * _ticksPerDay);
            }
        }

        /// <summary>
        /// Gets the day of the week for this Nepali date.
        /// </summary>
        /// <value>
        /// A <see cref="System.DayOfWeek"/> value where <see cref="System.DayOfWeek.Sunday"/> = 0
        /// through <see cref="System.DayOfWeek.Saturday"/> = 6.
        /// In Nepal, Saturday is the official weekly holiday.
        /// </value>
        public DayOfWeek DayOfWeek
        {
            get
            {
                if (_dayNumber == 0)
                {
                    ThrowDefaultInstance();
                }

                return (DayOfWeek)((uint)(_dayNumber - 1 + _epochDayOfWeek) % 7);
            }
        }

        /// <summary>
        /// Gets the 1-based ordinal position of this date within the Bikram Sambat calendar year.
        /// Baisakh 1 is day 1; the last day of Chaitra is typically day 365 or 366.
        /// </summary>
        /// <remarks>
        /// This is a Nepali calendar ordinal, not the Gregorian one. To obtain the Gregorian day-of-year,
        /// use <c>date.EnglishDate.DayOfYear</c>.
        /// </remarks>
        public int DayOfYear
        {
            get
            {
                if (_dayNumber == 0)
                {
                    return 0;
                }

                return _dayNumber - MonthTable.Start[(_year - _minYear) * 12];
            }
        }

        /// <summary>
        /// Gets the number of days in this date's month and year (i.e., the last valid day number).
        /// </summary>
        /// <value>
        /// An integer in the range 29–32. Nepali months do not have a fixed length; the value varies per month per year.
        /// </value>
        public int MonthEndDay
        {
            get
            {
                if (_dayNumber == 0)
                {
                    ThrowDefaultInstance();
                }

                var starts = MonthTable.Start;
                var monthIndex = (_year - _minYear) * 12 + (_month - 1);
                return starts[monthIndex + 1] - starts[monthIndex];
            }
        }

        /// <summary>
        /// Gets the <see cref="NepaliMonths"/> enumeration value corresponding to this date's month.
        /// </summary>
        /// <value>
        /// One of the twelve Nepali month names (Baisakh through Chaitra),
        /// cast directly from the numeric <see cref="Month"/> property.
        /// </value>
        public NepaliMonths MonthName => (NepaliMonths)_month;

        /// <summary>
        /// Gets today's Nepali date according to the local system clock.
        /// </summary>
        /// <value>
        /// A <see cref="NepaliDate"/> representing the current date in the Bikram Sambat calendar.
        /// Evaluated fresh on each access; not cached.
        /// </value>
        public static NepaliDate Today => DateTime.Today.ToNepaliDate();

        /// <summary>
        /// Gets the current Nepali date according to the local system clock.
        /// Equivalent to <see cref="Today"/>; included for parity with <see cref="DateTime.Now"/>.
        /// </summary>
        /// <remarks>
        /// Time-of-day information is discarded because <see cref="NepaliDate"/> represents a date only.
        /// Prefer <see cref="Today"/> in new code for clarity.
        /// </remarks>
        public static NepaliDate Now => DateTime.Now.ToNepaliDate();
        /// <summary>
        /// Represents the smallest possible value of a <see cref="NepaliDate"/> in the supported range:
        /// 1 Baisakh 1901 BS (1844-04-11 AD).
        /// </summary>
        public static readonly NepaliDate MinValue = new NepaliDate(_minYear, 1, 1);

        /// <summary>
        /// Represents the largest possible value of a <see cref="NepaliDate"/> in the supported range:
        /// the last day of Chaitra 2199 BS (2143-04-15 AD).
        /// </summary>
        public static readonly NepaliDate MaxValue = FromSerial(MonthTable.TotalDays - 1);

        /// <summary>
        /// Gets the Tithi (lunar day) name in Nepali for this date.
        /// </summary>
        /// <value>
        /// A Devanagari string such as <c>"एकादशी"</c>, or <see cref="string.Empty"/> when no Tithi is
        /// recorded for this day. The bundled data has Tithi for 2078–2083 BS.
        /// </value>
        public string TithiNp => CalendarBridge.GetTithiNp(CalendarOffset);

        /// <summary>
        /// Gets the Tithi (lunar day) name in English for this date.
        /// </summary>
        /// <value>
        /// A transliterated string such as <c>"Ekadashi"</c>, or <see cref="string.Empty"/> when no Tithi
        /// is recorded for this day. The bundled data has Tithi for 2078–2083 BS.
        /// </value>
        public string TithiEn => CalendarBridge.GetTithiEn(CalendarOffset);

        /// <summary>
        /// Gets a value indicating whether this date is a public holiday in Nepal.
        /// </summary>
        /// <value>
        /// <see langword="true"/> when the date is a gazetted public holiday according to the bundled
        /// Bikram Sambat calendar data;
        /// <see langword="false"/> otherwise. The bundled data has holidays for 2077–2083 BS.
        /// </value>
        public bool IsPublicHoliday => CalendarBridge.IsHoliday(CalendarOffset);

        /// <summary>
        /// Gets the names of events and observances recorded for this date, in Nepali.
        /// </summary>
        /// <value>
        /// An array of Devanagari event-name strings. Returns an empty (non-null) array when no events
        /// are recorded. The bundled data has events for 2077–2083 BS.
        /// </value>
        public string[] EventsNp => CalendarBridge.GetEventsNp(CalendarOffset);

        /// <summary>
        /// Gets the names of events and observances recorded for this date, in English.
        /// </summary>
        /// <value>
        /// An array of transliterated event-name strings. Returns an empty (non-null) array when no events
        /// are recorded. The bundled data has events for 2077–2083 BS.
        /// </value>
        public string[] EventsEn => CalendarBridge.GetEventsEn(CalendarOffset);

        /// <summary>
        /// Retrieves all calendar metadata for this date in a single lookup, including Tithi,
        /// public holiday status, and event names in both Nepali and English.
        /// </summary>
        /// <returns>
        /// A <see cref="Core.Calendar.CalendarInfo"/> struct populated from the bundled Bikram Sambat calendar data.
        /// All string members return <see cref="string.Empty"/> and event arrays return empty arrays
        /// when nothing is recorded for this date (tithi data covers 2078–2083 BS, events and holidays 2077–2083 BS).
        /// </returns>
        /// <remarks>
        /// Prefer this method over accessing <see cref="TithiNp"/>, <see cref="TithiEn"/>,
        /// <see cref="IsPublicHoliday"/>, <see cref="EventsNp"/>, and <see cref="EventsEn"/>
        /// individually when you need more than one of these values, as it performs a single lookup.
        /// </remarks>
        /// <example>
        /// <code>
        /// var date = new NepaliDate(2081, 4, 15);
        /// var info = date.GetCalendarInfo();
        ///
        /// Console.WriteLine(info.TithiEn);         // "Ekadashi"
        /// Console.WriteLine(info.IsPublicHoliday); // True or False
        /// foreach (var ev in info.EventsEn)
        ///     Console.WriteLine(ev);
        /// </code>
        /// </example>
        public CalendarInfo GetCalendarInfo() => CalendarBridge.GetCalendarInfo(CalendarOffset);

        /// <summary>
        /// Day offset of this date inside the bundled calendar data, counted from 1 Baisakh 2001 BS.
        /// Negative for earlier dates and for the default value; the caller checks the upper bound.
        /// </summary>
        private int CalendarOffset => _dayNumber - 1 - _calendarDataStart;
    }
}
