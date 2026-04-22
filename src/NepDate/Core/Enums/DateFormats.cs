namespace NepDate
{
    /// <summary>
    /// Specifies the order in which the year, month, and day components are written
    /// when formatting a <see cref="NepaliDate"/> with
    /// <see cref="NepaliDate.ToString(DateFormats, Separators, bool)"/>.
    /// </summary>
    public enum DateFormats
    {
        /// <summary>Year, then month, then day. Example: <c>2081/01/15</c></summary>
        YearMonthDay = 0,

        /// <summary>Year, then day, then month. Example: <c>2081/15/01</c></summary>
        YearDayMonth = 1,

        /// <summary>Month, then year, then day. Example: <c>01/2081/15</c></summary>
        MonthYearDay = 2,

        /// <summary>Month, then day, then year. Example: <c>01/15/2081</c></summary>
        MonthDayYear = 3,

        /// <summary>Day, then year, then month. Example: <c>15/2081/01</c></summary>
        DayYearMonth = 4,

        /// <summary>Day, then month, then year. Example: <c>15/01/2081</c></summary>
        DayMonthYear = 5,
    }
}
