# NepDate Library Examples

This project contains sample code demonstrating the functionality of the NepDate library, a .NET library for working with Nepali (Bikram Sambat) dates.

## Structure

The example code is organized into separate modules for each main component of the library:

- **NepaliDateExamples**: Basic functionality of the NepaliDate struct, including creation, conversion, formatting, and manipulation
- **FiscalYearExamples**: Working with fiscal years in the Nepali calendar system
- **NepaliDateRangeExamples**: Using date ranges to work with spans of dates
- **SmartDateParserExamples**: Parsing dates from various formats and handling ambiguity
- **BulkConvertExamples**: Efficiently converting collections of dates between calendar systems

## Running the Examples

Build and run the project to start an interactive console application. You can:

1. Choose a specific category of examples to run
2. Run all examples in sequence
3. Exit the application

## Key Examples

### NepaliDate Basics

```csharp
// Create a NepaliDate
NepaliDate date = new NepaliDate(2080, 1, 15);

// Convert from English date
NepaliDate todayNepali = new NepaliDate(DateTime.Now);

// Convert to English date
DateTime englishDate = date.EnglishDate;
```

### Fiscal Year Operations

```csharp
// Get the start and end of the fiscal year the date falls in
var (fyStart, fyEnd) = date.FiscalYearStartAndEndDate();   // (2079/04/01, 2080/03/31) for 2080/01/15

// Get fiscal quarter dates
var (qStart, qEnd) = date.FiscalYearQuarterStartAndEndDate();
```

### Date Ranges

```csharp
// Create a date range
NepaliDateRange range = new NepaliDateRange(startDate, endDate);

// Create a range for a specific month
NepaliDateRange monthRange = NepaliDateRange.ForMonth(2080, 1);

// Check if a date is in the range
bool isInRange = range.Contains(someDate);

// Iterate through dates in the range
foreach (NepaliDate date in range)
{
    // Process each date
}
```

### Smart Date Parsing

```csharp
// Parse from string
NepaliDate date = SmartDateParser.Parse("15 Baisakh 2080");

// Try parse without exceptions; day-first input is recognised
if (SmartDateParser.TryParse("1/2/2080", out NepaliDate result))
{
    // result is 2080/02/01
}
```

### Bulk Conversion

```csharp
// Convert a collection of English dates to Nepali
IEnumerable<NepaliDate> nepaliDates = NepaliDate.BulkConvert.ToNepaliDates(englishDates);

// Convert a collection of Nepali dates to English
IEnumerable<DateTime> backToEnglish = NepaliDate.BulkConvert.ToEnglishDates(nepaliDates);
```

Each call converts the whole collection in one pass, in input order, on the calling thread.

## Notes

- These examples demonstrate the key functionality but don't cover every method and property
- Check the XML documentation in the library code for comprehensive API documentation
- The examples use dates from the 2080s BS (2023-2024 AD) as representative samples
