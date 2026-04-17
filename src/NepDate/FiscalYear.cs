namespace NepDate
{
    public readonly partial struct NepaliDate
    {
        /// <summary>
        /// Determines the fiscal year quarter for the current Nepali date.
        /// </summary>
        /// <returns>The fiscal year quarter as an enumeration value.</returns>
        /// <remarks>
        /// In the Nepali calendar, the fiscal year quarters are defined as:
        /// - First Quarter: Shrawan, Bhadra, Ashwin (Month 4, 5, 6)
        /// - Second Quarter: Kartik, Mangsir, Poush (Month 7, 8, 9)
        /// - Third Quarter: Magh, Falgun, Chaitra (Month 10, 11, 12)
        /// - Fourth Quarter: Baisakh, Jestha, Ashadh (Month 1, 2, 3)
        /// 
        /// This method determines which quarter the current date falls into.
        /// </remarks>
        /// <exception cref="InvalidNepaliDateFormatException">
        /// Thrown if the month is outside the valid range of 1-12.
        /// </exception>
        private FiscalYearQuarters FiscalYearQuarter()
        {
            switch (Month)
            {
                case 4:
                case 5:
                case 6:
                    return FiscalYearQuarters.First;
                case 7:
                case 8:
                case 9:
                    return FiscalYearQuarters.Second;
                case 10:
                case 11:
                case 12:
                    return FiscalYearQuarters.Third;
                case 1:
                case 2:
                case 3:
                    return FiscalYearQuarters.Fourth;
                default:
                    throw new InvalidNepaliDateFormatException();
            }
        }


        /// <summary>
        /// Calculates the start date of the fiscal year for this date, with an optional year offset.
        /// </summary>
        /// <param name="yearOffset">Number of fiscal years to offset from the current fiscal year. Default is 0 (current fiscal year).</param>
        /// <returns>A NepaliDate representing the first day of the specified fiscal year (1 Shrawan).</returns>
        /// <remarks>
        /// In Nepal, the fiscal year starts on 1 Shrawan (month 4) and ends on 31 Ashadh (month 3) of the next year.
        /// 
        /// If the current date is in months 1-3 (Baisakh, Jestha, Ashadh), it's in the last quarter of the previous fiscal year,
        /// so we need to adjust the year calculation accordingly.
        /// 
        /// The yearOffset parameter allows calculating the start date for past or future fiscal years.
        /// For example, -1 would give the previous fiscal year's start date, and 1 would give the next fiscal year's start date.
        /// </remarks>
        public NepaliDate FiscalYearStartDate(int yearOffset = 0)
        {
            if (Month <= 3)
            {
                yearOffset--;
            }
            return new NepaliDate(Year + yearOffset, 4, 1);
        }


        /// <summary>
        /// Calculates the end date of the fiscal year for this date, with an optional year offset.
        /// </summary>
        /// <param name="yearOffset">Number of fiscal years to offset from the current fiscal year. Default is 0 (current fiscal year).</param>
        /// <returns>A NepaliDate representing the last day of the specified fiscal year (last day of Ashadh, month 3).</returns>
        /// <remarks>
        /// In Nepal, the fiscal year ends on the last day of Ashadh (month 3, typically day 31 or 32).
        /// 
        /// If the current date is in months 4-12 (Shrawan through Chaitra), it's in the first three quarters of the fiscal year,
        /// so the end date will be in the next calendar year.
        /// 
        /// The yearOffset parameter allows calculating the end date for past or future fiscal years.
        /// </remarks>
        public NepaliDate FiscalYearEndDate(int yearOffset = 0)
        {
            if (Month >= 4)
            {
                yearOffset++;
            }

            return new NepaliDate(Year + yearOffset, 3, 1).MonthEndDate();
        }


        /// <summary>
        /// Gets both the start and end dates of the fiscal year for this date, with an optional year offset.
        /// </summary>
        /// <param name="yearOffset">Number of fiscal years to offset from the current fiscal year. Default is 0 (current fiscal year).</param>
        /// <returns>A tuple containing the start date (1 Shrawan) and end date (last day of Ashadh) of the fiscal year.</returns>
        /// <remarks>
        /// This is a convenience method that combines FiscalYearStartDate and FiscalYearEndDate.
        /// See those methods for detailed explanations of how the dates are calculated.
        /// 
        /// The result tuple contains:
        /// - startDate: The first day of the fiscal year (1 Shrawan)
        /// - endDate: The last day of the fiscal year (last day of Ashadh)
        /// </remarks>
        public (NepaliDate startDate, NepaliDate endDate) FiscalYearStartAndEndDate(int yearOffset = 0)
        {
            return (FiscalYearStartDate(yearOffset), FiscalYearEndDate(yearOffset));
        }


        /// <summary>
        /// Calculates the start date of a specific fiscal year quarter, with an optional year offset.
        /// </summary>
        /// <param name="fiscalYearQuarters">The fiscal year quarter to calculate. Use FiscalYearQuarters.Current for the current quarter.</param>
        /// <param name="yearOffset">Number of fiscal years to offset from the current fiscal year. Default is 0 (current fiscal year).</param>
        /// <returns>A NepaliDate representing the first day of the specified fiscal year quarter.</returns>
        /// <remarks>
        /// The fiscal year quarters correspond to specific months:
        /// - First Quarter (Shrawan, Bhadra, Ashwin): Months 4, 5, 6
        /// - Second Quarter (Kartik, Mangsir, Poush): Months 7, 8, 9
        /// - Third Quarter (Magh, Falgun, Chaitra): Months 10, 11, 12
        /// - Fourth Quarter (Baisakh, Jestha, Ashadh): Months 1, 2, 3
        /// 
        /// If FiscalYearQuarters.Current is specified, the method first determines the current quarter
        /// based on the current date's month.
        /// 
        /// If the requested quarter is the fourth quarter (months 1-3), it requires a year adjustment
        /// since that quarter spans into the next calendar year.
        /// </remarks>
        public NepaliDate FiscalYearQuarterStartDate(FiscalYearQuarters fiscalYearQuarters = FiscalYearQuarters.Current, int yearOffset = 0)
        {
            if (fiscalYearQuarters == FiscalYearQuarters.Current)
            {
                fiscalYearQuarters = FiscalYearQuarter();
            }

            if (fiscalYearQuarters == FiscalYearQuarters.Fourth)
            {
                yearOffset++;
            }

            return new NepaliDate(Year + yearOffset, (int)fiscalYearQuarters, 1);
        }


        /// <summary>
        /// Calculates the end date of a specific fiscal year quarter, with an optional year offset.
        /// </summary>
        /// <param name="fiscalYearQuarters">The fiscal year quarter to calculate. Use FiscalYearQuarters.Current for the current quarter.</param>
        /// <param name="yearOffset">Number of fiscal years to offset from the current fiscal year. Default is 0 (current fiscal year).</param>
        /// <returns>A NepaliDate representing the last day of the specified fiscal year quarter.</returns>
        /// <remarks>
        /// The fiscal year quarters correspond to specific months:
        /// - First Quarter (Shrawan, Bhadra, Ashwin): Months 4, 5, 6
        /// - Second Quarter (Kartik, Mangsir, Poush): Months 7, 8, 9
        /// - Third Quarter (Magh, Falgun, Chaitra): Months 10, 11, 12
        /// - Fourth Quarter (Baisakh, Jestha, Ashadh): Months 1, 2, 3
        /// 
        /// If FiscalYearQuarters.Current is specified, the method first determines the current quarter
        /// based on the current date's month.
        /// 
        /// If the requested quarter is the fourth quarter (months 1-3), it requires a year adjustment
        /// since that quarter spans into the next calendar year.
        /// 
        /// The end date is calculated as the last day of the third month in the quarter.
        /// </remarks>
        public NepaliDate FiscalYearQuarterEndDate(FiscalYearQuarters fiscalYearQuarters = FiscalYearQuarters.Current, int yearOffset = 0)
        {
            if (fiscalYearQuarters == FiscalYearQuarters.Current)
            {
                fiscalYearQuarters = FiscalYearQuarter();
            }

            if (fiscalYearQuarters == FiscalYearQuarters.Fourth)
            {
                yearOffset++;
            }
            return new NepaliDate(Year + yearOffset, (int)fiscalYearQuarters + 2, 1).MonthEndDate();
        }


        /// <summary>
        /// Gets both the start and end dates of a specific fiscal year quarter, with an optional year offset.
        /// </summary>
        /// <param name="fiscalYearQuarters">The fiscal year quarter to calculate. Use FiscalYearQuarters.Current for the current quarter.</param>
        /// <param name="yearOffset">Number of fiscal years to offset from the current fiscal year. Default is 0 (current fiscal year).</param>
        /// <returns>A tuple containing the start and end dates of the specified fiscal year quarter.</returns>
        /// <remarks>
        /// This is a convenience method that combines FiscalYearQuarterStartDate and FiscalYearQuarterEndDate.
        /// See those methods for detailed explanations of how the dates are calculated.
        /// 
        /// The result tuple contains:
        /// - startDate: The first day of the specified quarter
        /// - endDate: The last day of the specified quarter
        /// </remarks>
        public (NepaliDate startDate, NepaliDate endDate) FiscalYearQuarterStartAndEndDate(FiscalYearQuarters fiscalYearQuarters = FiscalYearQuarters.Current, int yearOffset = 0)
        {
            return (FiscalYearQuarterStartDate(fiscalYearQuarters, yearOffset), FiscalYearQuarterEndDate(fiscalYearQuarters, yearOffset));
        }


        /// <summary>
        /// Gets both the start and end dates of a specified fiscal year.
        /// </summary>
        /// <param name="fiscalYear">The Nepali fiscal year to calculate dates for (e.g., 2080).</param>
        /// <returns>A tuple containing the start date (1 Shrawan) and end date (last day of Ashadh) of the specified fiscal year.</returns>
        /// <remarks>
        /// This is a static method that calculates fiscal year dates based on a specified fiscal year number,
        /// without requiring a NepaliDate instance.
        /// 
        /// In Nepal, the fiscal year starts on 1 Shrawan (month 4) of the specified year
        /// and ends on the last day of Ashadh (month 3) of the next year.
        /// 
        /// For example, fiscal year 2080 starts on 1 Shrawan 2080 and ends on the last day of Ashadh 2081.
        /// </remarks>
        public static (NepaliDate startDate, NepaliDate endDate) GetFiscalYearStartAndEndDate(int fiscalYear)
        {
            return (GetFiscalYearStartDate(fiscalYear), GetFiscalYearEndDate(fiscalYear));
        }


        /// <summary>
        /// Gets the start date of a specified fiscal year.
        /// </summary>
        /// <param name="fiscalYear">The Nepali fiscal year to calculate the start date for (e.g., 2080).</param>
        /// <returns>A NepaliDate representing the first day of the specified fiscal year (1 Shrawan).</returns>
        /// <remarks>
        /// This static method creates a NepaliDate for the first day of Shrawan (month 4) of the specified fiscal year.
        /// In Nepal, the fiscal year begins on 1 Shrawan.
        /// </remarks>
        public static NepaliDate GetFiscalYearStartDate(int fiscalYear)
        {
            return new NepaliDate(fiscalYear, 4, 1);
        }


        /// <summary>
        /// Gets the end date of a specified fiscal year.
        /// </summary>
        /// <param name="fiscalYear">The Nepali fiscal year to calculate the end date for (e.g., 2080).</param>
        /// <returns>A NepaliDate representing the last day of the specified fiscal year (last day of Ashadh).</returns>
        /// <remarks>
        /// This static method calculates the last day of Ashadh (month 3) in the year following the specified fiscal year.
        /// In Nepal, the fiscal year ends on the last day of Ashadh, which is typically the 31st or 32nd day.
        /// 
        /// For example, for fiscal year 2080, the end date would be the last day of Ashadh 2081.
        /// </remarks>
        public static NepaliDate GetFiscalYearEndDate(int fiscalYear)
        {
            return new NepaliDate(fiscalYear + 1, 3, 1).MonthEndDate();
        }


        /// <summary>
        /// Gets the start date of a fiscal year quarter based on the provided fiscal year and month.
        /// </summary>
        /// <param name="fiscalYear">The fiscal year for the calculation.</param>
        /// <param name="month">The Nepali month (1-12) within the fiscal year.</param>
        /// <returns>A NepaliDate representing the start date of the fiscal year quarter containing the specified month.</returns>
        /// <remarks>
        /// This static method first creates a NepaliDate for the first day of the specified month and year,
        /// then determines which fiscal quarter that date falls in, and returns the start date of that quarter.
        /// 
        /// The month parameter determines which quarter will be calculated:
        /// - Months 4-6 (Shrawan, Bhadra, Ashwin): First Quarter
        /// - Months 7-9 (Kartik, Mangsir, Poush): Second Quarter
        /// - Months 10-12 (Magh, Falgun, Chaitra): Third Quarter
        /// - Months 1-3 (Baisakh, Jestha, Ashadh): Fourth Quarter
        /// </remarks>
        public static NepaliDate GetFiscalYearQuarterStartDate(int fiscalYear, int month)
        {
            return new NepaliDate(fiscalYear, month, 1).FiscalYearQuarterStartDate();
        }


        /// <summary>
        /// Gets the end date of a fiscal year quarter based on the provided fiscal year and month.
        /// </summary>
        /// <param name="fiscalYear">The fiscal year for the calculation.</param>
        /// <param name="month">The Nepali month (1-12) within the fiscal year.</param>
        /// <returns>A NepaliDate representing the end date of the fiscal year quarter containing the specified month.</returns>
        /// <remarks>
        /// This static method first creates a NepaliDate for the first day of the specified month and year,
        /// then determines which fiscal quarter that date falls in, and returns the end date of that quarter.
        /// 
        /// The month parameter determines which quarter will be calculated:
        /// - Months 4-6 (Shrawan, Bhadra, Ashwin): First Quarter
        /// - Months 7-9 (Kartik, Mangsir, Poush): Second Quarter
        /// - Months 10-12 (Magh, Falgun, Chaitra): Third Quarter
        /// - Months 1-3 (Baisakh, Jestha, Ashadh): Fourth Quarter
        /// 
        /// The end date is the last day of the last month in the quarter.
        /// </remarks>
        public static NepaliDate GetFiscalYearQuarterEndDate(int fiscalYear, int month)
        {
            return new NepaliDate(fiscalYear, month, 1).FiscalYearQuarterEndDate();
        }


        /// <summary>
        /// Gets both the start and end dates of a fiscal year quarter based on the provided fiscal year and month.
        /// </summary>
        /// <param name="fiscalYear">The fiscal year for the calculation.</param>
        /// <param name="month">The Nepali month (1-12) within the fiscal year.</param>
        /// <returns>A tuple containing the start and end dates of the fiscal year quarter containing the specified month.</returns>
        /// <remarks>
        /// This is a convenience method that combines GetFiscalYearQuarterStartDate and GetFiscalYearQuarterEndDate.
        /// It determines which quarter the specified month belongs to and returns both boundary dates.
        /// 
        /// The result tuple contains:
        /// - startDate: The first day of the quarter containing the specified month
        /// - endDate: The last day of the quarter containing the specified month
        /// </remarks>
        public static (NepaliDate startDate, NepaliDate endDate) GetFiscalYearQuarterStartAndEndDate(int fiscalYear, int month)
        {
            return (GetFiscalYearQuarterStartDate(fiscalYear, month), GetFiscalYearQuarterEndDate(fiscalYear, month));
        }
    }
}
