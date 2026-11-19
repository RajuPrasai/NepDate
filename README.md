![NepDate: fast, allocation-free Bikram Sambat dates for .NET](https://user-images.githubusercontent.com/37014558/231635618-bf6599e3-554e-4b02-93df-019e7b8aecc3.png)

**Fast, allocation-free Bikram Sambat (Nepali) dates for .NET**

[![NuGet Version](https://img.shields.io/nuget/v/NepDate.svg?logo=nuget&label=NuGet)](https://www.nuget.org/packages/NepDate/) [![NuGet Downloads](https://img.shields.io/nuget/dt/NepDate?logo=nuget&label=Downloads)](https://www.nuget.org/packages/NepDate/) [![Targets](https://img.shields.io/badge/.NET-Standard%202.0%20%7C%208.0-512BD4?logo=dotnet)](https://www.nuget.org/packages/NepDate/) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/RajuPrasai/NepDate/blob/main/LICENSE) [![GitHub Stars](https://img.shields.io/github/stars/RajuPrasai/NepDate?logo=github)](https://github.com/RajuPrasai/NepDate)

[Website](https://nepdate.rajuprasai.com.np/) · [Documentation](https://nepdate.rajuprasai.com.np/docs.html) · [Changelog](https://nepdate.rajuprasai.com.np/changelog.html) · [NuGet](https://www.nuget.org/packages/NepDate/)

---

NepDate gives .NET a `NepaliDate` type that behaves like `DateTime` for the Bikram Sambat calendar. It converts BS to AD in under a nanosecond and AD to BS in under 3 nanoseconds, does date arithmetic, formats and parses Nepali and English text, handles Nepal's fiscal year, works with date ranges, and ships tithi, holiday and event data. It is an 8-byte `readonly struct` with no NuGet dependencies.

| ⚡ Speed | 🧠 Memory | 📅 Range | 📦 Footprint |
| :---: | :---: | :---: | :---: |
| BS → AD in **~0.8 ns** | **0 B** allocated per conversion | **1901–2199 BS** (1844–2143 AD) | **No dependencies**, netstandard2.0 + net8.0 |

## Contents

- [What's new in 2.1.0](#whats-new-in-210)
- [Installation](#installation)
- [Quick start](#quick-start)
- [Features](#features)
- [Usage guide](#usage-guide): [Creating dates](#creating-dates) · [Properties](#properties) · [Arithmetic and comparison](#arithmetic-and-comparison) · [Conversion](#conversion) · [Formatting](#formatting) · [Parsing](#parsing) · [Fiscal year](#fiscal-year) · [Date ranges](#date-ranges) · [Calendar data](#calendar-data) · [Serialization](#serialization) · [Type integration](#type-integration)
- [Performance](#performance)
- [Contributing](#contributing)
- [License](#license)

## What's new in 2.1.0

- **Much faster core.** Conversion, arithmetic and parsing are 3.6 to 10 times faster than 2.0.7, and a `NepaliDate` is now 8 bytes instead of 32. See [Performance](#performance).
- **.NET 8 fix.** System.Text.Json serialization no longer fails with `FileNotFoundException` on .NET 8.
- **Fiscal quarter fix.** `GetFiscalYearQuarter…(fiscalYear, month)` now treats months 1–3 as part of the given fiscal year.
- **Better tooling.** XML docs for IntelliSense and a symbol package with Source Link, so you can step into NepDate's code.
- **Behaviour changes to know:** `EnglishDate` always has `DateTimeKind.Unspecified`, and `BulkConvert` converts the whole collection when you call it, on the calling thread.

Full notes: [changelog](https://nepdate.rajuprasai.com.np/changelog.html) · [GitHub releases](https://github.com/RajuPrasai/NepDate/releases)

## Installation

```bash
dotnet add package NepDate
```

```xml
<PackageReference Include="NepDate" Version="2.1.0" />
```

| Your app targets | NepDate build used | Notes |
| --- | --- | --- |
| .NET 8 and later | `net8.0` | Adds `ISpanFormattable`, `IParsable<T>` and automatic System.Text.Json support |
| .NET Framework 4.6.1+, .NET Core 2.0+, .NET 5–7, Xamarin, MAUI, Unity | `netstandard2.0` | Full date API; call `ConfigureForNepaliDate()` for JSON |

## Quick start

```csharp
using NepDate;

var date = new NepaliDate(2080, 6, 15);

DateTime ad   = date.EnglishDate;                              // 2023-10-02
NepaliDate bs = new DateTime(2023, 10, 2).ToNepaliDate();      // 2080/06/15

string text   = date.ToLongDateUnicodeString(displayDayName: true); // "सोमवार, असोज १५, २०८०"
NepaliDate p  = SmartDateParser.Parse("15 Ashoj 2080");        // 2080/06/15
NepaliDate fy = date.FiscalYearStartDate();                    // 2080/04/01

var dashami = new NepaliDate(2081, 6, 26);
bool holiday  = dashami.IsPublicHoliday;                       // true
string[] evts = dashami.EventsEn;                              // ["Vijaya Dashami", "Devi Visarjan"]
```

## Features

| | Feature | What you get |
| :---: | --- | --- |
| 🔄 | **Conversion** | BS ↔ AD both ways, plus `BulkConvert` for whole collections |
| ✅ | **Validation** | Exact month lengths (29–32 days) for every year from 1901 to 2199 BS |
| 🧮 | **Arithmetic** | Add days, months (whole or fractional) and years, with month-end control |
| 🖋️ | **Formatting** | Format strings, Nepali digits, Nepali month and weekday names, long dates |
| 🔍 | **Parsing** | Strict parsing, plus `SmartDateParser` for 100+ month spellings, Nepali text and day-first input |
| 💼 | **Fiscal year** | Nepal FY (Shrawan–Ashad) and quarter boundaries, instance and static |
| 📆 | **Date ranges** | Contains, overlap, union, intersect, except, split by month or quarter, working days |
| 🗓️ | **Calendar data** | Tithi, public holidays and events in Nepali and English |
| 💾 | **Serialization** | System.Text.Json, Newtonsoft.Json and XML, as a string or an object |
| 🔗 | **.NET integration** | `IComparable`, `IEquatable`, `IFormattable`, `IParsable`, `TypeConverter` for ASP.NET binding |

## Usage guide

### Creating dates

```csharp
using NepDate;

var a = new NepaliDate(2079, 12, 16);           // year, month, day
var b = new NepaliDate("2079/12/16");           // from text
var c = NepaliDate.Parse("2079-12-16");
var d = new NepaliDate(DateTime.Today);         // from a DateTime
var e = DateTime.Today.ToNepaliDate();          // extension method
var today = NepaliDate.Today;
```

Invalid dates throw `ArgumentOutOfRangeException` (for example `new NepaliDate(2080, 6, 31)`, because Ashoj 2080 has 30 days). Use `NepaliDate.TryParse` when you don't want exceptions.

### Properties

```csharp
var date = new NepaliDate(2079, 12, 16);

date.Year;            // 2079
date.Month;           // 12
date.Day;             // 16
date.EnglishDate;     // 2023-03-30
date.DayOfWeek;       // Thursday
date.DayOfYear;       // 351
date.MonthEndDay;     // 30
date.MonthEndDate();  // 2079/12/30
date.MonthName;       // NepaliMonths.Chaitra
date.IsDefault;       // false, true only for default(NepaliDate)
```

### Arithmetic and comparison

```csharp
var date = new NepaliDate(2081, 4, 32);

date.AddDays(5);                               // 2081/05/05
date.AddDays(-5);                              // 2081/04/27
date.AddMonths(2);                             // 2081/06/30  (clamped to month end)
date.AddMonths(2, awayFromMonthEnd: true);     // 2081/07/02  (overflow carries forward)
date.AddMonths(2.5);                           // 2081/07/15  (fractional: about 30.42 days a month)
date.AddMonths(-2);                            // 2081/02/32
new NepaliDate(2081, 4, 15).AddYears(1);       // 2082/04/15

var start = new NepaliDate(2080, 1, 1);
var end   = new NepaliDate(2080, 4, 1);
TimeSpan gap = end - start;                    // 94 days, same as end.Subtract(start)
bool before  = start < end;                    // true; ==, !=, <, <=, >, >= all work
```

### Conversion

```csharp
DateTime ad   = new NepaliDate(2080, 6, 15).EnglishDate;   // 2023-10-02
NepaliDate bs = new NepaliDate(new DateTime(2023, 10, 2)); // 2080/06/15

// Whole collections, in input order
IEnumerable<NepaliDate> bsDates = NepaliDate.BulkConvert.ToNepaliDates(adDates);
IEnumerable<DateTime>   adList  = NepaliDate.BulkConvert.ToEnglishDates(bsDates);
IEnumerable<DateTime>   adText  = NepaliDate.BulkConvert.ToEnglishDates(new[] { "2080/04/15", "2080-01-01" });
```

`BulkConvert` converts the whole collection when you call it, on the calling thread. An invalid date throws from the call itself.

### Formatting

```csharp
var date = new NepaliDate(2079, 2, 6);

date.ToString();                                                       // "2079/02/06"
date.ToString(DateFormats.DayMonthYear, Separators.Dash, leadingZeros: false); // "6-2-2079"
date.ToUnicodeString(DateFormats.DayMonthYear, Separators.Dot);        // "०६.०२.२०७९"
date.ToLongDateString(leadingZeros: false, displayDayName: true, displayYear: false);        // "Friday, Jestha 6"
date.ToLongDateUnicodeString(leadingZeros: false, displayDayName: true, displayYear: false); // "शुक्रवार, जेठ ६"
```

`NepaliDate` implements `IFormattable`, so format strings work in `ToString`, string interpolation and `string.Format`:

| Format | Result | Notes |
| --- | --- | --- |
| `"d"`, `"G"` or none | `2079/02/06` | Same as `ToString()` |
| `"D"` | `Jestha 06, 2079` | Long date |
| `"s"` | `2079-02-06` | Sortable |
| `"dd-MM-yyyy"` | `06-02-2079` | Custom |
| `"MMMM dd, yyyy"` | `Jestha 06, 2079` | Month name |
| `"MMM yyyy"` | `Jes 2079` | Short month name |
| `"dd 'of' MMMM"` | `06 of Jestha` | Quoted literal text |

Custom tokens: `yyyy`, `yy`, `MMMM`, `MMM`, `MM`, `M`, `dd`, `d`. Quote literal text with `'...'` or escape one character with `\`.

`DateFormats`: `YearMonthDay`, `YearDayMonth`, `MonthYearDay`, `MonthDayYear`, `DayYearMonth`, `DayMonthYear`.
`Separators`: `ForwardSlash`, `BackwardSlash`, `Dot`, `Underscore`, `Dash`, `Space`.

### Parsing

**Strict parsing** reads year, month and day in that order, separated by `/ - . _ \ |`, `।` or a space:

```csharp
NepaliDate.Parse("2080-06-15");                       // 2080/06/15
NepaliDate.Parse("15-06-2080", autoAdjust: true);     // 2080/06/15  (fixes day-first input)
NepaliDate.Parse("25-05-077", autoAdjust: true);      // 2077/05/25  (expands a 3-digit year)
NepaliDate.TryParse("2079/13/16", out var bad);       // false, there is no month 13
```

**Smart parsing** understands month names, Nepali text and looser input:

```csharp
SmartDateParser.Parse("15 Shrawan 2080");     // 2080/04/15
SmartDateParser.Parse("Shrawan 15, 2080");    // 2080/04/15
SmartDateParser.Parse("15 Saun 2080");        // 2080/04/15  (alternate spelling)
SmartDateParser.Parse("२०८० साउन १५");         // 2080/04/15  (Nepali digits and month name)
SmartDateParser.Parse("15th Shrawan 2080");   // 2080/04/15  (ordinal day)
SmartDateParser.Parse("2080/04/15 B.S.");     // 2080/04/15  (era suffix ignored)
SmartDateParser.Parse("80/4/15");             // 2080/04/15  (2-digit year)
"15 Shrawan 2080".ToNepaliDate();             // extension method
"15 Shrawan 2080".TryToNepaliDate(out var r); // no exceptions
```

### Fiscal year

Nepal's fiscal year runs from 1 Shrawan (month 4) to the last day of Ashad (month 3) of the next year. Quarters: Q1 Shrawan–Ashoj, Q2 Kartik–Poush, Q3 Magh–Chaitra, Q4 Baishakh–Ashad.

```csharp
var date = new NepaliDate(2081, 4, 15);

date.FiscalYearStartDate();                    // 2081/04/01
date.FiscalYearEndDate();                      // 2082/03/32
date.FiscalYearQuarterStartAndEndDate();       // (2081/04/01, 2081/06/30)
date.FiscalYearStartDate(yearOffset: 1);       // 2082/04/01  (next fiscal year)

NepaliDate.GetFiscalYearStartAndEndDate(2080);           // (2080/04/01, 2081/03/31)
NepaliDate.GetFiscalYearQuarterStartAndEndDate(2080, 4); // (2080/04/01, 2080/06/30)
NepaliDate.GetFiscalYearQuarterStartAndEndDate(2080, 1); // (2081/01/01, 2081/03/31)  Q4 of FY 2080
```

### Date ranges

```csharp
var ashoj = NepaliDateRange.ForMonth(2080, 6);

ashoj.Length;                                            // 30
ashoj.Contains(new NepaliDate(2080, 6, 15));             // true
ashoj.IsAdjacentTo(NepaliDateRange.ForMonth(2080, 7));   // true
ashoj.Except(NepaliDateRange.FromDayCount(new NepaliDate(2080, 6, 10), 5));
// [2080/06/01 - 2080/06/09, 2080/06/15 - 2080/06/30]

NepaliDateRange.ForMonth(2080, 1).Union(NepaliDateRange.ForMonth(2080, 3));
// 2080/01/01 - 2080/03/31  (fills the gap between the two ranges)

foreach (NepaliDate day in ashoj.WorkingDays()) { }      // skips Saturdays
ashoj.WeekendDays();                                     // Saturdays and Sundays
NepaliDateRange[] quarters = NepaliDateRange.ForFiscalYear(2080).SplitByFiscalQuarter();
```

Also available: `SingleDay`, `FromDayCount`, `ForCalendarYear`, `CurrentMonth`, `CurrentFiscalYear`, `CurrentCalendarYear`, `Overlaps`, `Intersect`, `SplitByMonth` and `DatesWithInterval`.

### Calendar data

```csharp
var teej = new NepaliDate(2081, 5, 21);

teej.TithiNp;          // "तृतिया"
teej.TithiEn;          // "Tritiya"
teej.IsPublicHoliday;  // true
teej.EventsEn;         // ["Haritalika Teej (Women Employees Only)"]
teej.EventsNp;         // ["हरितालिका तीज (महिला कर्मचारीहरूलाई मात्र)"]

var info = teej.GetCalendarInfo();   // all of the above in one lookup
```

| Data | Years covered |
| --- | --- |
| Tithi | 2078–2083 BS |
| Events and public holidays | 2077–2083 BS, plus a few days of 2084 |

Other dates return empty strings, `false` and empty arrays. Nothing throws.

### Serialization

**System.Text.Json.** On .NET 8 and later it works with no setup:

```csharp
JsonSerializer.Serialize(new { Date = new NepaliDate(2080, 6, 15) });  // {"Date":"2080-06-15"}
```

For the object format, or on the netstandard2.0 build (for example .NET Framework), configure it:

```csharp
using NepDate.Serialization;

var options = new JsonSerializerOptions().ConfigureForNepaliDate(useObjectFormat: true);
JsonSerializer.Serialize(new NepaliDate(2080, 6, 15), options);        // {"Year":2080,"Month":6,"Day":15}
```

**Newtonsoft.Json:**

```csharp
using NepDate.Serialization;

var settings = new JsonSerializerSettings().ConfigureForNepaliDate();
JsonConvert.SerializeObject(new { Date = new NepaliDate(2080, 6, 15) }, settings); // {"Date":"2080-06-15"}
```

> **Note:** `ConfigureForNepaliDate()` needs Newtonsoft.Json referenced in your project, because both overloads share the name. On netstandard2.0 targets it also needs System.Text.Json 8.0.5 or later. To use only one library, add its converter directly, for example `options.Converters.Add(new SystemTextJsonConverters.NepaliDateJsonConverter())`.

**XML.** `NepaliDate` has no settable properties, so wrap it in `NepaliDateXmlSerializer`:

```csharp
using NepDate.Serialization;

var xml = new XmlSerializer(typeof(NepaliDateXmlSerializer));
xml.Serialize(writer, new NepaliDateXmlSerializer(new NepaliDate(2080, 6, 15)));
// <NepaliDateXmlSerializer>2080-06-15</NepaliDateXmlSerializer>
```

**ASP.NET model binding.** A `TypeConverter` is registered, so route values, query strings and form fields bind directly. It also converts to and from `int` (`20800615`) and `DateTime`.

```csharp
[HttpGet("{date}")]
public IActionResult Get(NepaliDate date) => Ok(date);   // GET /api/records/2080-06-15
```

### Type integration

- `IComparable<NepaliDate>` and `IEquatable<NepaliDate>` for sorting, LINQ and dictionary keys
- `IFormattable` for format strings everywhere .NET accepts them
- `ISpanFormattable`, `IParsable<NepaliDate>` and `ISpanParsable<NepaliDate>` on .NET 8+
- `Deconstruct`: `var (year, month, day) = date;`
- `MinValue` is 1901/01/01 BS (1844-04-11 AD) and `MaxValue` is 2199/12/30 BS (2143-04-15 AD)
- Immutable and thread-safe; `IsToday()`, `IsYesterday()`, `IsTomorrow()` and `IsLeapYear()` helpers

## Performance

All numbers are measured, not estimated: BenchmarkDotNet 0.15.8, .NET 10.0.12, Intel Core i5-12500H, Windows 11, one pinned CPU core. Each call converts the next date from a pool of 1,024 different dates (2070–2085 BS), so no result can be precomputed. Lower is better.

### Compared with other Nepali date libraries

**BS → AD**

| Library | Mean | vs NepDate | Allocated |
| --- | ---: | ---: | ---: |
| **NepDate** | **0.78 ns** | **1×** | **0 B** |
| NepaliCalendarBS | 14,772 ns | 18,900× slower | 688 B |
| NepaliDateConverter.Net | 31,843 ns | 40,800× slower | 20,016 B |
| NepaliDateConverter.NETCORE | 32,665 ns | 41,900× slower | 20,736 B |

**AD → BS**

| Library | Mean | vs NepDate | Allocated |
| --- | ---: | ---: | ---: |
| **NepDate** | **2.73 ns** | **1×** | **0 B** |
| NepaliCalendarBS | 183 ns | 67× slower | 32 B |
| NepaliDateConverter.Net | 65,305 ns | 23,900× slower | 20,016 B |
| NepaliDateConverter.NETCORE | 67,715 ns | 24,800× slower | 20,736 B |

### NepDate 2.1.0 compared with 2.0.7

Both published packages, same benchmark and machine.

| Operation | 2.0.7 | 2.1.0 | Faster | Allocated (2.0.7 → 2.1.0) |
| --- | ---: | ---: | ---: | ---: |
| BS → AD | 6.05 ns | 0.80 ns | 7.5× | 0 B → 0 B |
| AD → BS | 13.65 ns | 2.72 ns | 5.0× | 0 B → 0 B |
| Add 1,000 days | 14.69 ns | 2.07 ns | 7.1× | 0 B → 0 B |
| Add 13 months | 17.39 ns | 1.75 ns | 9.9× | 0 B → 0 B |
| Strict parse | 19.92 ns | 5.50 ns | 3.6× | 0 B → 0 B |
| `ToString()` | 13.37 ns | 11.34 ns | 1.2× | 96 B → 48 B |
| Long Nepali date with weekday | 145.7 ns | 27.7 ns | 5.3× | 435 B → 69 B |
| Calendar info for a day | 109.4 ns | 13.6 ns | 8.0× | 50 B → 50 B |
| Bulk convert 1,000 dates | 36.6 µs | 4.0 µs | 9.1× | 141 KB → 8 KB |

The library comparison lives in [`benchmarks/NepDate.Benchmarks`](https://github.com/RajuPrasai/NepDate/tree/main/benchmarks/NepDate.Benchmarks). Run it with `dotnet run -c Release --project benchmarks/NepDate.Benchmarks`.

## Contributing

Contributions are welcome. Please read the [contributing guide](https://github.com/RajuPrasai/NepDate/blob/main/CONTRIBUTING.md) before opening a pull request, and open an [issue](https://github.com/RajuPrasai/NepDate/issues) for bugs or ideas.

## License

NepDate is released under the [MIT License](https://github.com/RajuPrasai/NepDate/blob/main/LICENSE).

<div align="center">

Made with ❤️ for the Nepali developer community by [Raju Prasai](https://github.com/RajuPrasai)

</div>
