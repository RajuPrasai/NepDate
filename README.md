![NepDate Logo](https://user-images.githubusercontent.com/37014558/231635618-bf6599e3-554e-4b02-93df-019e7b8aecc3.png)

# NepDate: Fast and Efficient Nepali Date Library for .NET

### Modern, High Performance Bikram Sambat Date Operations for .NET Applications

[![GitHub Stars](https://img.shields.io/github/stars/rajuprasai/NepDate)](https://github.com/rajuprasai/NepDate)
[![NuGet Version](https://img.shields.io/nuget/v/NepDate.svg)](https://www.nuget.org/packages/NepDate/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/NepDate)](https://www.nuget.org/packages/NepDate/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

[![Open NepDate Website](https://img.shields.io/badge/NepDate-Website-2ea44f?style=for-the-badge)](https://rajuprasai.github.io/NepDate/)

---

NepDate is a high performance, memory efficient `struct` targeting `.NET Standard 2.0` that provides complete Bikram Sambat (B.S.) calendar support for .NET applications. It handles conversion, arithmetic, formatting, smart parsing, fiscal year operations, and calendar metadata in a single lightweight package with zero external dependencies.

## 📋 Table of Contents

| Section | Description |
| ------- | ----------- |
| 📦 [Installation](#-installation) | How to add NepDate to your project |
| ✨ [Key Features](#-key-features) | What makes NepDate special |
| 🚀 [Getting Started](#-getting-started) | Create and access NepaliDate |
| 📅 [Date Operations](#-date-operations) | Arithmetic, comparison, and conversion |
| 🔄 [Date Range Operations](#-date-range-operations) | Working with date ranges |
| 🖨️ [Formatting & Display](#-formatting--display) | Control how dates appear |
| 🔍 [Smart Date Parsing](#-smart-date-parsing) | Flexible input parsing |
| 💼 [Fiscal Year Operations](#-fiscal-year-operations) | Business date calculations |
| 🗓️ [Calendar Data](#️-calendar-data) | Tithi, events, and public holidays |
| 💾 [Serialization](#-serialization) | JSON and XML support |
| 🔧 [Advanced Features](#-advanced-features) | Boundaries, defaults, and type system |
| ⚡ [Performance](#-performance) | Benchmark results |
| 👥 [Contributions](#-contributions) | How to help improve NepDate |
| 📝 [Change Log](#-change-log) | Recent updates |

## 📦 Installation

```bash
# .NET CLI
dotnet add package NepDate

# Package Manager Console
Install-Package NepDate
```

## ✨ Key Features

- ⚡ **Performance:** Benchmarked up to 8,600× faster than comparable libraries; zero heap allocations on both BS→AD and AD→BS paths
- 🔄 **Bidirectional Conversion:** Convert between Bikram Sambat and Gregorian calendar in nanoseconds
- ✅ **Robust Validation:** Correct per month, per year day limit enforcement across 1901–2199 BS
- 🧮 **Date Arithmetic:** Add/subtract days and months (including fractional months) with boundary safe options
- 🗓️ **Calendar Metadata:** Built in Tithi, public holiday flag, and event data in Nepali and English for 2001–2089 BS
- 📊 **Fiscal Year Support:** Full Nepal FY (Shrawan–Ashadh) quarter and boundary calculations, both instance and static
- 🔍 **Smart Parsing:** Accepts 100+ month name spellings, Nepali Unicode digits, alternate separators, typo tolerant formats, and 2 digit year expansion
- 🖋️ **Rich Formatting:** Standard and custom format tokens, Unicode Nepali digits, long form Nepali strings, and `IFormattable` integration
- 🔄 **Date Range Operations:** Intersection, union, split by month or fiscal quarter, working days, and interval iteration
- 💾 **Serialization:** System.Text.Json, Newtonsoft.Json, and XML support with string and object output modes
- 📦 **Struct Design:** `readonly partial struct` keeps instances on the stack and eliminates GC pressure in high volume scenarios
- 🔗 **First Class Type:** Implements `IFormattable`, `IComparable<T>`, `IEquatable<T>`, `IParsable<T>` (.NET 7+), `ISpanFormattable` (.NET 6+), `TypeConverter`, and auto registered `[JsonConverter]` (.NET 5+)

## 🚀 Getting Started

### Creating a NepaliDate

```csharp
using NepDate;

// From year, month, and day
var date = new NepaliDate(2079, 12, 16);

// From a string
var date = new NepaliDate("2079/12/16");
var date = NepaliDate.Parse("2079/12/16");

// From an English DateTime
var date = new NepaliDate(DateTime.Today);
var date = DateTime.Today.ToNepaliDate();  // extension method

// Current Nepali date
var today = NepaliDate.Today;
```

### Accessing Properties

```csharp
var date = new NepaliDate("2079/12/16");

// Core date components
int year        = date.Year;            // 2079
int month       = date.Month;           // 12
int day         = date.Day;             // 16
DateTime eng    = date.EnglishDate;     // 2023/03/30
DayOfWeek dow   = date.DayOfWeek;      // Thursday
int dayOfYear   = date.DayOfYear;      // 346
int monthEndDay = date.MonthEndDay;    // 30
NepaliDate end  = date.MonthEndDate(); // 2079/12/30
NepaliMonths mn = date.MonthName;      // Chaitra
bool isDefault  = date.IsDefault;      // false (true only for uninitialized default())

// Calendar metadata
string   tithiNp  = date.TithiNp;          // "फाल्गुण कृष्ण चतुर्दशी"
string   tithiEn  = date.TithiEn;          // "Falgun Krishna Chaturdashi"
bool     holiday  = date.IsPublicHoliday;  // false
string[] eventsNp = date.EventsNp;         // events in Nepali (empty array if none)
string[] eventsEn = date.EventsEn;         // events in English (empty array if none)
```

## 📅 Date Operations

### Adding and Subtracting

```csharp
var date = new NepaliDate("2081/04/32");

// Days
var later   = date.AddDays(5);        // 2081/05/05
var earlier = date.AddDays(-5);       // 2081/04/27

// Whole months
var plus2   = date.AddMonths(2);      // 2081/06/30
var minus2  = date.AddMonths(-2);     // 2081/02/32

// Fractional months (converted to approximate days: 1 month ≈ 30.42 days)
var frac    = date.AddMonths(2.5);    // 2081/07/15

// Pull the result away from month-end when you need a specific day
var adj     = date.AddMonths(2, awayFromMonthEnd: true); // 2081/07/02
```

### Comparison and Difference

```csharp
var a = NepaliDate.Parse("2079/12/16");
var b = DateTime.Now.ToNepaliDate();

bool eq   = a == b;
bool lt   = a < b;
bool gt   = a > b;
int cmp   = a.CompareTo(b);          // IComparable<NepaliDate>

TimeSpan diff = b - a;               // operator overload
TimeSpan diff = b.Subtract(a);       // method equivalent
```

### Conversion

```csharp
// BS to AD
DateTime eng = NepaliDate.Parse("2079/12/16").EnglishDate;

// AD to BS
string nep = DateTime.Now.ToNepaliDate().ToString();
```

### Bulk Conversion

```csharp
var engDates      = new List<DateTime>();
var nepStrings    = new List<string>();

// English → Nepali
IEnumerable<NepaliDate> nepDates   = NepaliDate.BulkConvert.ToNepaliDates(engDates);

// Nepali → English
IEnumerable<DateTime>  engResult1  = NepaliDate.BulkConvert.ToEnglishDates(nepDates);
IEnumerable<DateTime>  engResult2  = NepaliDate.BulkConvert.ToEnglishDates(nepStrings);
```

Collections above 500 items are processed in parallel automatically. Use `BatchProcessToNepaliDates` or `BatchProcessToEnglishDates` for explicit batch size control.

## 🔄 Date Range Operations

### Creating Ranges

```csharp
var start = new NepaliDate(2080, 1, 1);
var end   = new NepaliDate(2080, 3, 15);

// Direct construction
var range = new NepaliDateRange(start, end);

// Factory methods
var single      = NepaliDateRange.SingleDay(start);
var tenDays     = NepaliDateRange.FromDayCount(start, 10);
var month       = NepaliDateRange.ForMonth(2080, 1);
var fiscalYear  = NepaliDateRange.ForFiscalYear(2080);
var calYear     = NepaliDateRange.ForCalendarYear(2080);

// Current-period shortcuts
var curMonth    = NepaliDateRange.CurrentMonth();
var curFY       = NepaliDateRange.CurrentFiscalYear();
var curYear     = NepaliDateRange.CurrentCalendarYear();
```

### Properties and Containment

```csharp
var range = new NepaliDateRange(start, end);

bool empty    = range.IsEmpty;
int  days     = range.Length;
var  s        = range.Start;
var  e        = range.End;

bool hasDate  = range.Contains(someDate);
bool hasRange = range.Contains(otherRange);
bool overlaps = range.Overlaps(otherRange);
bool adjacent = range.IsAdjacentTo(otherRange);
```

### Set Operations

```csharp
NepaliDateRange  intersection = range.Intersect(otherRange);
NepaliDateRange  union        = range.Union(otherRange);
NepaliDateRange[] segments    = range.Except(otherRange);   // returns array; may have 0–2 items
```

### Splitting and Iterating

```csharp
// Split into sub-ranges
IEnumerable<NepaliDateRange> byMonth   = range.SplitByMonth();
IEnumerable<NepaliDateRange> byQuarter = range.SplitByFiscalQuarter();

// Enumerate every date
foreach (NepaliDate date in range) { }

// Filtered collections
IEnumerable<NepaliDate> working       = range.WorkingDays();                   // excludes Saturdays
IEnumerable<NepaliDate> workingNoSun  = range.WorkingDays(excludeSunday: true);
IEnumerable<NepaliDate> weekends      = range.WeekendDays();                   // Saturdays only by default
IEnumerable<NepaliDate> weekendsIncSun = range.WeekendDays(includeSunday: true);
IEnumerable<NepaliDate> everyWeek     = range.DatesWithInterval(7);
```

### Range Formatting

```csharp
string basic     = range.ToString();  // "2080/01/01 - 2080/03/15"
string formatted = range.ToString(DateFormats.DayMonthYear, Separators.Dash); // "01-01-2080 - 15-03-2080"
```

## 🖨️ Formatting & Display

### Output Formats at a Glance

| Style | Example output |
| ----- | -------------- |
| Default | `2079/02/06` |
| Custom | `6-2-2079` |
| Long | `Friday, Jestha 6` |
| Unicode digits | `०६.०२.२०७९` |
| Nepali long | `शुक्रबार, जेठ ६` |

### Formatting Methods

```csharp
var date = new NepaliDate("2079/02/06");

// Default YYYY/MM/DD
date.ToString()

// Custom format and separator (leadingZeros optional, default true)
date.ToString(DateFormats.DayMonthYear, Separators.Dash, leadingZeros: false)  // "6-2-2079"

// Long date in English
date.ToLongDateString(leadingZeros: false, displayDayName: true, displayYear: false)  // "Friday, Jestha 6"

// Unicode Nepali digits
date.ToUnicodeString(DateFormats.DayMonthYear, Separators.Dot, leadingZeros: true)   // "०६.०२.२०७९"

// Long date in Nepali
date.ToLongDateUnicodeString(leadingZeros: false, displayDayName: true, displayYear: false) // "शुक्रबार, जेठ ६"
```

Available `DateFormats`: `YearMonthDay`, `YearDayMonth`, `MonthYearDay`, `MonthDayYear`, `DayYearMonth`, `DayMonthYear`.
Available `Separators`: `ForwardSlash`, `BackwardSlash`, `Dash`, `Dot`, `Underscore`, `Space`.

### IFormattable Format Specifiers

`NepaliDate` implements `IFormattable` (and `ISpanFormattable` on .NET 6+), so format strings work in string interpolation, `string.Format`, and any context that accepts `IFormattable`.

| Specifier | Output | Notes |
| --------- | ------ | ----- |
| `"d"` or `"G"` | `2079/02/06` | Same as `ToString()` |
| `"D"` | `Jestha 06, 2079` | Long date, leading zeros |
| `"s"` | `2079-02-06` | Sortable, dash separated |
| custom pattern | varies | Tokens below |

**Custom tokens:** `yyyy` (4-digit year), `yy` (2-digit year), `MMMM` (full month name), `MMM` (3-letter abbreviation), `MM` (2-digit month), `M` (month number), `dd` (2-digit day), `d` (day number). Wrap literal text in single quotes.

```csharp
date.ToString("d")                           // 2079/02/06
date.ToString("D")                           // Jestha 06, 2079
date.ToString("s")                           // 2079-02-06
date.ToString("dd-MM-yyyy")                  // 06-02-2079
date.ToString("MMMM dd, yyyy")               // Jestha 06, 2079
date.ToString("MMM yyyy")                    // Jes 2079
date.ToString("dd 'of' MMMM")                // 06 of Jestha
$"{date:s}"                                  // 2079-02-06
string.Format("{0:yyyy/MM/dd}", date)         // 2079/02/06
```

## 🔍 Smart Date Parsing

`SmartDateParser` handles ambiguous, informal, and multi locale date strings. It recognizes over 100 month name spellings (English and Nepali Unicode), Nepali digits, common separators, optional suffixes (`B.S.`, `V.S.`, `गते`, `मिति`), and 2 digit year expansion.

```csharp
// SmartDateParser.Parse — throws on failure
NepaliDate d1 = SmartDateParser.Parse("15 Shrawan 2080");    // DD Month YYYY
NepaliDate d2 = SmartDateParser.Parse("Shrawan 15, 2080");   // Month DD, YYYY
NepaliDate d3 = SmartDateParser.Parse("15 Saun 2080");       // alternate month spelling
NepaliDate d4 = SmartDateParser.Parse("२०८०/०४/१५");         // Nepali digits
NepaliDate d5 = SmartDateParser.Parse("१५ श्रावण २०८०");     // full Nepali

// String extension methods
NepaliDate d6 = "15 Shrawan 2080".ToNepaliDate();

if ("15 Shrawan 2080".TryToNepaliDate(out NepaliDate d7))
    Console.WriteLine(d7);

// NepaliDate.Parse with autoAdjust — heuristic correction for ambiguous or non-standard input
// Rules applied in order:
//   1. day > 32 → swap year and day
//   2. monthInMiddle: false → swap month and day first
//   3. month > 12 and day < 13 → swap month and day
//   4. year < 1000 → prepend current millennium (2000)
NepaliDate p1 = NepaliDate.Parse("2077_05_25", autoAdjust: true);              // 2077/05/25
NepaliDate p2 = NepaliDate.Parse("25-05-077",  autoAdjust: true);              // 2077/05/25
NepaliDate p3 = NepaliDate.Parse("05/06/2077", autoAdjust: true);              // 2077/06/05 (month in middle)
NepaliDate p4 = NepaliDate.Parse("05/06/2077", autoAdjust: true, monthInMiddle: false); // 2077/05/06

// TryParse variants
bool ok1 = NepaliDate.TryParse("2079/12/16", out NepaliDate r1);
bool ok2 = NepaliDate.TryParse("05/06/2077", out NepaliDate r2, autoAdjust: true, monthInMiddle: true);
```

## 💼 Fiscal Year Operations

Nepal's fiscal year runs from 1 Shrawan (month 4) to the last day of Ashadh (month 3) of the following year. Quarters are: Q1 Shrawan–Ashoj (4–6), Q2 Kartik–Poush (7–9), Q3 Magh–Chaitra (10–12), Q4 Baishakh–Ashadh (1–3).

### Instance Methods

```csharp
var date = new NepaliDate("2081/04/15");  // Q1 of FY 2081

// Fiscal year boundaries for the FY containing this date
NepaliDate fyStart          = date.FiscalYearStartDate();               // 2081/04/01
NepaliDate fyEnd            = date.FiscalYearEndDate();                 // 2082/03/last-day-of-Ashadh
(NepaliDate s, NepaliDate e) = date.FiscalYearStartAndEndDate();        // (2081/04/01, 2082/03/last-day-of-Ashadh)

// Quarter boundaries for the quarter containing this date
NepaliDate qStart           = date.FiscalYearQuarterStartDate();        // 2081/04/01
NepaliDate qEnd             = date.FiscalYearQuarterEndDate();          // 2081/06/30
(NepaliDate qs, NepaliDate qe) = date.FiscalYearQuarterStartAndEndDate(); // (2081/04/01, 2081/06/30)

// yearOffset shifts the result relative to the containing FY (0 = current, 1 = next, -1 = previous)
NepaliDate nextFyStart      = date.FiscalYearStartDate(yearOffset: 1);  // 2082/04/01
```

### Static Methods

```csharp
// Fiscal year boundaries by FY number
NepaliDate start  = NepaliDate.GetFiscalYearStartDate(2080);      // 2080/04/01
NepaliDate end    = NepaliDate.GetFiscalYearEndDate(2080);         // 2081/03/last-day-of-Ashadh
(var s, var e)    = NepaliDate.GetFiscalYearStartAndEndDate(2080); // (2080/04/01, 2081/03/last-day-of-Ashadh)

// Quarter boundaries — second parameter is the month (1–12) used to identify the quarter
// Month 4 → Q1, month 7 → Q2, month 10 → Q3, month 1 → Q4
NepaliDate qS  = NepaliDate.GetFiscalYearQuarterStartDate(2080, 4);         // 2080/04/01
NepaliDate qE  = NepaliDate.GetFiscalYearQuarterEndDate(2080, 4);           // 2080/06/30
(var qs, var qe) = NepaliDate.GetFiscalYearQuarterStartAndEndDate(2080, 4); // (2080/04/01, 2080/06/30)

// Q4 example: month 1 of FY 2080 falls in the calendar year 2081
(var q4s, var q4e) = NepaliDate.GetFiscalYearQuarterStartAndEndDate(2080, 1); // (2081/01/01, 2081/03/last-day-of-Ashadh)
```

## 🗓️ Calendar Data

Built in Tithi (lunar day), public holiday flag, and event data compiled from authoritative Bikram Sambat calendar references covers **2001–2089 BS**. All properties return empty/default values outside this range without throwing exceptions.

```csharp
var date = new NepaliDate(2081, 4, 15);

string   tithiNp   = date.TithiNp;          // "एकादशी"
string   tithiEn   = date.TithiEn;          // "Ekadashi"
bool     isHoliday = date.IsPublicHoliday;  // true
string[] eventsNp  = date.EventsNp;         // ["हरितालिका तीज", "गौरा पर्व", ...]
string[] eventsEn  = date.EventsEn;         // ["Haritalika Teej", "Gaura Parva", ...]

// Retrieve all calendar info in a single call
CalendarInfo info = date.GetCalendarInfo();
// info.TithiNp, info.TithiEn, info.IsPublicHoliday, info.EventsNp, info.EventsEn
```

| Member | Type | Description |
| ------ | ---- | ----------- |
| `TithiNp` | `string` | Lunar day name in Nepali. Empty string if outside data range. |
| `TithiEn` | `string` | Lunar day name in English. Empty string if outside data range. |
| `IsPublicHoliday` | `bool` | True if the date is a public holiday in Nepal. |
| `EventsNp` | `string[]` | Event names in Nepali. Empty array if no events or outside data range. |
| `EventsEn` | `string[]` | Event names in English. Empty array if no events or outside data range. |
| `GetCalendarInfo()` | `CalendarInfo` | All properties above in a single lookup. |

## 💾 Serialization

### System.Text.Json

On .NET 5+, a `[JsonConverter]` is registered on `NepaliDate` automatically, so basic serialization works without any setup. Call `ConfigureForNepaliDate()` when you need explicit control over the output format.

```csharp
using System.Text.Json;
using NepDate.Serialization;

// String format: "2080-04-15"  (default, sortable)
var opts = new JsonSerializerOptions().ConfigureForNepaliDate();

// Object format: {"Year":2080,"Month":4,"Day":15}
var optsObj = new JsonSerializerOptions().ConfigureForNepaliDate(useObjectFormat: true);

var date = new NepaliDate(2080, 4, 15);
string json  = JsonSerializer.Serialize(date, opts);        // "2080-04-15"
var restored = JsonSerializer.Deserialize<NepaliDate>(json, opts);
```

### Newtonsoft.Json

```csharp
using Newtonsoft.Json;
using NepDate.Serialization;

var settings    = new JsonSerializerSettings().ConfigureForNepaliDate();
var settingsObj = new JsonSerializerSettings().ConfigureForNepaliDate(useObjectFormat: true);

var date  = new NepaliDate(2080, 4, 15);
string j  = JsonConvert.SerializeObject(date, settings);        // "2080-04-15"
var back  = JsonConvert.DeserializeObject<NepaliDate>(j, settings);
```

### XML

XML serialization requires a thin wrapper because `NepaliDate` is a struct without a parameterless constructor.

```csharp
using System.Xml.Serialization;
using NepDate.Serialization;

public class PersonRecord
{
    public string Name { get; set; }
    public NepaliDateXmlSerializer BirthDate { get; set; }

    [XmlIgnore]
    public NepaliDate ActualBirthDate
    {
        get => BirthDate?.Value ?? default;
        set => BirthDate = new NepaliDateXmlSerializer(value);
    }
}

var p = new PersonRecord { Name = "Ram Sharma", ActualBirthDate = new NepaliDate(2040, 2, 15) };
var serializer = new XmlSerializer(typeof(PersonRecord));
```

## 🔧 Advanced Features

### Boundary Values and Default Detection

```csharp
NepaliDate min = NepaliDate.MinValue;  // 1901/01/01 BS
NepaliDate max = NepaliDate.MaxValue;  // last day of 2199/12 BS

// IsDefault identifies an uninitialized struct (default(NepaliDate))
NepaliDate unset = default;
bool isEmpty = unset.IsDefault;  // true

NepaliDate real = new NepaliDate(2080, 1, 1);
bool notEmpty = real.IsDefault;  // false
```

### Leap Year and Relative Date Checks

```csharp
var date = new NepaliDate("2079/12/16");

bool isLeap     = date.IsLeapYear();   // false
bool isToday    = date.IsToday();
bool isYesterday = date.IsYesterday();
bool isTomorrow  = date.IsTomorrow();
```

### Safe Parsing

```csharp
// Returns false instead of throwing on invalid input
bool ok = NepaliDate.TryParse("2079/13/16", out NepaliDate result);  // false, month 13 is invalid

// With autoAdjust and monthInMiddle control
bool ok2 = NepaliDate.TryParse("05/06/2077", out NepaliDate result2, autoAdjust: true, monthInMiddle: true);
```

### Type System Integration

`NepaliDate` is a fully integrated .NET type:

- `IComparable<NepaliDate>` and `IEquatable<NepaliDate>` for sorting and equality
- `IFormattable` for format-string support in interpolation and `string.Format`
- `ISpanFormattable` (.NET 6+) for allocation free formatting into spans
- `IParsable<NepaliDate>` and `ISpanParsable<NepaliDate>` (.NET 7+) for generic parsing
- `TypeConverter` registration for automatic binding in model binders and UI frameworks
- Auto registered `[JsonConverter]` (.NET 5+) for zero config JSON support

## ⚡ Performance

NepDate uses flat array dictionaries for O(1) date lookups with zero heap allocations on **both** conversion paths.

> Measured with BenchmarkDotNet 0.15.8 on .NET 10.0.5, 12th Gen Intel Core i5-12500H, Windows 11. Smaller is better.

### BS → AD (Nepali to English)

| Library                        |          Mean | Ratio    | Allocated |
| ------------------------------ | ------------: | -------: | --------: |
| **NepDate**                    |   **4.55 ns** | **1.00** |   **0 B** |
| NepaliCalendarBS               |   14,453.0 ns |    3,176 |     688 B |
| NepaliDateConverter.Net        |   37,150.0 ns |    8,164 |  19,968 B |
| NepaliDateConverter.NETCORE    |   39,144.3 ns |    8,603 |  20,688 B |

### AD → BS (English to Nepali)

| Library                        |          Mean | Ratio    | Allocated |
| ------------------------------ | ------------: | -------: | --------: |
| **NepDate**                    |  **13.60 ns** | **1.00** |   **0 B** |
| NepaliCalendarBS               |      162.5 ns |       12 |      32 B |
| NepaliDateConverter.Net        |   66,468.8 ns |    4,889 |  19,968 B |
| NepaliDateConverter.NETCORE    |   66,487.7 ns |    4,890 |  20,688 B |

NepDate is up to **8,600× faster** than comparable libraries while producing zero heap allocations on every conversion.

## 👥 Contributions

Contributions are welcome. Please read the [CONTRIBUTING](https://github.com/rajuprasai/NepDate/blob/main/CONTRIBUTING.md) guide before submitting a pull request.

## 📝 Change Log

For a detailed list of changes in each release, visit the [releases page](https://github.com/rajuprasai/NepDate/releases).

---

Made with ❤️ by [rajuprasai](https://github.com/rajuprasai)
