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
        private const ushort _minYear = 1901;

        /// <summary>
        /// The maximum supported year value for Nepali dates (2199 BS).
        /// </summary>
        private const ushort _maxYear = 2199;

        private const double _approxDaysPerMonth = 30.41666666666667;

        /// <summary>
        /// Converts the date to an integer representation in the format YYYYMMDD.
        /// This is optimized to use direct arithmetic instead of string operations.
        /// </summary>
        /// <example>
        /// For the date 2080/05/15, the result would be 20800515.
        /// </example>
        internal int AsInteger => Year * 10000 + Month * 100 + Day;

        /// <summary>
        /// Gets a value indicating whether this instance is the default (uninitialized) value.
        /// A default NepaliDate has Year=0, Month=0, Day=0 and does not represent a valid date.
        /// </summary>
        public bool IsDefault => Year == 0 && Month == 0 && Day == 0;

        /// <summary>
        /// Gets the year component of the Nepali date (in BS - Bikram Sambat).
        /// Valid range is from 1901 to 2199.
        /// </summary>
        public int Year { get; }

        /// <summary>
        /// Gets the month component of the Nepali date.
        /// Valid range is from 1 to 12, where 1 represents Baisakh and 12 represents Chaitra.
        /// </summary>
        public int Month { get; }

        /// <summary>
        /// Gets the day component of the Nepali date.
        /// Valid range depends on the month and year, typically 1-32 for Nepali months.
        /// </summary>
        public int Day { get; }

        /// <summary>
        /// The cached English date equivalent, if available.
        /// This is populated during conversion operations to prevent repeated calculations.
        /// </summary>
        private readonly DateTime? _englishDate;

        public DateTime EnglishDate => _englishDate ?? DictionaryBridge.NepToEng.GetEnglishDate(Year, Month, Day) + DateTime.Now.TimeOfDay;



        /// <summary>
        /// Gets the day of the week represented by this Nepali date.
        /// This is calculated by converting to the equivalent English date and getting its day of week.
        /// </summary>
        /// <remarks>
        /// The returned value follows the .NET DayOfWeek enumeration where Sunday = 0, Monday = 1, etc.
        /// </remarks>
        public DayOfWeek DayOfWeek => EnglishDate.DayOfWeek;

        /// <summary>
        /// Gets the day of the year represented by this Nepali date.
        /// This is calculated by converting to the equivalent English date and getting its day of year.
        /// </summary>
        /// <remarks>
        /// The value represents the day position within the English calendar year, not the Nepali calendar year.
        /// To get the actual Nepali day of year, additional calculation would be needed.
        /// </remarks>
        public int DayOfYear => EnglishDate.DayOfYear;

        /// <summary>
        /// Gets the last day of the month represented by this Nepali date.
        /// Nepali months have varying lengths (29-32 days) depending on the month and year.
        /// </summary>
        /// <remarks>
        /// This is used for date validation and calculating month-end dates.
        /// This information is retrieved from the calendar data dictionary.
        /// </remarks>
        public int MonthEndDay
            => DictionaryBridge.NepToEng.GetNepaliMonthEndDay(Year, Month);

        /// <summary>
        /// Gets the Nepali month name as an enumeration value based on the current month.
        /// </summary>
        /// <remarks>
        /// The enumeration provides the traditional Nepali month names (Baisakh, Jestha, etc.)
        /// This is a simple cast from the month number to the corresponding enum value.
        /// </remarks>
        public NepaliMonths MonthName => (NepaliMonths)Month;

        /// <summary>
        /// Gets the current Nepali date (today) according to the system clock.
        /// </summary>
        /// <remarks>
        /// This creates a new NepaliDate object by converting the current system time (DateTime.Now).
        /// The time component is not preserved in the NepaliDate structure.
        /// </remarks>
        public static NepaliDate Now => DateTime.Now.ToNepaliDate();

        /// <summary>
        /// Represents the smallest possible value of a Nepali date in the supported range.
        /// This is equivalent to 1 Baisakh 1901 BS.
        /// </summary>
        /// <remarks>
        /// This is useful as a boundary value for date comparisons and validations.
        /// </remarks>
        public static readonly NepaliDate MinValue = new NepaliDate(_minYear, 1, 1);

        /// <summary>
        /// Represents the largest possible value of a Nepali date in the supported range.
        /// This is equivalent to the last day of Chaitra 2199 BS.
        /// </summary>
        /// <remarks>
        /// This is useful as a boundary value for date comparisons and validations.
        /// </remarks>
        public static readonly NepaliDate MaxValue = new NepaliDate(_maxYear, 12, 1).MonthEndDate();

        /// <summary>
        /// Gets the Tithi (lunar day) name in Nepali for this date.
        /// Returns empty string when data is not available (outside 2001-2089 BS or no Tithi recorded).
        /// </summary>
        public string TithiNp => CalendarBridge.GetTithiNp(Year, Month, Day);

        /// <summary>
        /// Gets the Tithi (lunar day) name in English for this date.
        /// Returns empty string when data is not available (outside 2001-2089 BS or no Tithi recorded).
        /// </summary>
        public string TithiEn => CalendarBridge.GetTithiEn(Year, Month, Day);

        /// <summary>
        /// Gets whether this date is a public holiday (based on HamroPatro data, 2001-2089 BS).
        /// Returns false when outside the supported range.
        /// </summary>
        public bool IsPublicHoliday => CalendarBridge.IsHoliday(Year, Month, Day);

        /// <summary>
        /// Gets all calendar metadata for this date including Tithi, events, and holiday status.
        /// Data is available for 2001-2089 BS. Returns empty values outside this range.
        /// </summary>
        public CalendarInfo GetCalendarInfo() => CalendarBridge.GetCalendarInfo(Year, Month, Day);
    }
}
