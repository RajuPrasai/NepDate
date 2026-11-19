# NepDate - Bikram Sambat .NET Library

**Skill type:** library-documentation
**Scope:** All public APIs of the NepDate NuGet package v2.1.0
**Canonical docs:** https://nepdate.rajuprasai.com.np/docs.html
**Full reference (plain text):** https://nepdate.rajuprasai.com.np/llms-full.txt
**NuGet:** https://www.nuget.org/packages/NepDate/
**GitHub:** https://github.com/RajuPrasai/NepDate
**Related:** NepDate Widget (Windows app) at https://nepdatewidget.rajuprasai.com.np/

## What NepDate is

NepDate is a zero-dependency, MIT-licensed .NET library for Bikram Sambat (BS) dates. Its main type is `NepaliDate`, an 8-byte `readonly partial struct` that stores a BS date and converts to and from `System.DateTime` in a few nanoseconds without heap allocations.

Supported range: 1901/01/01 BS (1844-04-11 AD) through 2199/12/30 BS (2143-04-15 AD).

Calendar data is bundled for fewer years: tithi for 2078-2083 BS, events and public holidays for 2077-2083 BS (plus a few days of 2084). Other dates return empty values without throwing.

Targets: .NET Standard 2.0 and net8.0, so it runs on .NET Framework 4.6.1+, .NET Core 2.0+, .NET 5 and later, Xamarin, MAUI, Unity and Mono.

Only the members listed here exist. Do not use names like `FromInt`, `DayName`, `YearStartDate`, `Tithi`, `PublicHoliday`, `DayCount`, `IsContiguousWith` or `CalendarInfo.GetForMonth`; they are not part of the API.

## Installation

```
dotnet add package NepDate
```

Or in a `.csproj`:
```xml
<PackageReference Include="NepDate" Version="2.1.0" />
```

## Core type: NepaliDate

`NepaliDate` is in the `NepDate` namespace. It implements `IComparable`, `IComparable<NepaliDate>`, `IEquatable<NepaliDate>` and `IFormattable`; the net8.0 build also implements `ISpanFormattable`, `IParsable<NepaliDate>` and `ISpanParsable<NepaliDate>`.

### Creating instances

```csharp
using NepDate;

var d  = new NepaliDate(2080, 6, 15);           // from BS year, month, day
var d2 = new NepaliDate(DateTime.Today);        // AD to BS
var d3 = DateTime.Today.ToNepaliDate();         // extension method
var today = NepaliDate.Today;                   // also NepaliDate.Now

var d4 = NepaliDate.Parse("2080-6-15");         // strict: year, month, day with a separator
var d5 = new NepaliDate("2080/06/15");
if (NepaliDate.TryParse("2080-6-15", out var result)) { }

var d6 = SmartDateParser.Parse("15 Ashoj 2080");   // month names
var d7 = SmartDateParser.Parse("२०८० असोज १५");     // Nepali digits and month names
var d8 = "Asoj 15, 2080".ToNepaliDate();          // extension method
```

### Properties

| Property | Type | Description |
|---|---|---|
| `Year` | int | BS year (1901-2199) |
| `Month` | int | BS month (1-12) |
| `Day` | int | BS day (1-32 depending on month) |
| `EnglishDate` | DateTime | Gregorian date, midnight, `DateTimeKind.Unspecified` |
| `DayOfWeek` | DayOfWeek | Day of the week |
| `MonthName` | NepaliMonths | Enum value for the month |
| `MonthEndDay` | int | Days in this month (29-32) |
| `DayOfYear` | int | 1 on 1 Baisakh |
| `IsDefault` | bool | true for `default(NepaliDate)` |
| `TithiNp` | string | Tithi in Devanagari, or empty |
| `TithiEn` | string | Tithi in English, or empty |
| `IsPublicHoliday` | bool | Nepal gazetted public holiday |
| `EventsNp` | string[] | Event names in Devanagari, empty array when none |
| `EventsEn` | string[] | Event names in English, empty array when none |

### Methods

| Method | Returns |
|---|---|
| `AddDays(double days)` | NepaliDate |
| `AddMonths(double months, bool awayFromMonthEnd = false)` | NepaliDate (clamps to month end by default) |
| `AddYears(int years, bool awayFromMonthEnd = false)` | NepaliDate |
| `MonthEndDate()` | NepaliDate, last day of the month |
| `Subtract(NepaliDate other)` | TimeSpan, same as `d1 - d2` |
| `IsLeapYear()` | bool, Gregorian leap year of `EnglishDate` |
| `IsToday()`, `IsYesterday()`, `IsTomorrow()` | bool |
| `GetCalendarInfo()` | CalendarInfo with all calendar data |
| `Deconstruct(out year, out month, out day)` | `var (y, m, d) = date;` |

### Arithmetic and comparison

```csharp
var d = new NepaliDate(2080, 6, 15);

NepaliDate later   = d.AddDays(20);    // 2080/07/05
NepaliDate earlier = d.AddMonths(-2);  // 2080/04/15

bool isEqual = d1 == d2;
bool isLater = d1 > d2;
int order    = d1.CompareTo(d2);
TimeSpan gap = d1 - d2;                // whole days
```

### Formatting

```csharp
var d = new NepaliDate(2080, 6, 15);

d.ToString();                                                // "2080/06/15"
d.ToString(DateFormats.DayMonthYear, Separators.Dash);       // "15-06-2080"
d.ToString(DateFormats.MonthDayYear, Separators.Dot, false); // "6.15.2080"
d.ToUnicodeString();                                         // "२०८०/०६/१५"
d.ToLongDateString();                                        // "Ashoj 15, 2080"
d.ToLongDateString(displayDayName: true);                    // "Monday, Ashoj 15, 2080"
d.ToLongDateUnicodeString(displayDayName: true);             // "सोमवार, असोज १५, २०८०"

// IFormattable
d.ToString("D");               // "Ashoj 15, 2080"
d.ToString("s");               // "2080-06-15"
d.ToString("MMMM dd, yyyy");   // "Ashoj 15, 2080"
$"{d:yyyy-MM-dd}";             // "2080-06-15"
```

Custom format tokens: `yyyy`, `yy`, `MMMM`, `MMM`, `MM`, `M`, `dd`, `d`. Literal text goes in `'...'`.

#### DateFormats enum values

`YearMonthDay`, `YearDayMonth`, `MonthYearDay`, `MonthDayYear`, `DayYearMonth`, `DayMonthYear`

#### Separators enum values

`ForwardSlash`, `BackwardSlash`, `Dot`, `Underscore`, `Dash`, `Space`

### Parsing

- `NepaliDate.Parse`, `TryParse` and the string constructor are strict: year, month and day in that order, ASCII digits, separated by `/ - . _ \ | ।` or a space. Pass `autoAdjust: true` to accept day-first input such as `"15-06-2080"`.
- `SmartDateParser.Parse` and `TryParse` accept 100+ month spellings (Baisakh/Baishakh/Vaishakh, Jestha/Jeth, Ashad/Asar, Shrawan/Saun, Bhadra/Bhadau, Ashoj/Ashwin/Asoj, Kartik, Mangsir/Mangshir, Poush/Push, Magh, Falgun/Phagun, Chaitra/Chait), Devanagari month names, Nepali digits (०-९), two- or three-digit years ("080/6/15") and ordinals ("15th Shrawan 2080").

### Fiscal Year

Nepal's fiscal year runs from 1 Shrawan (month 4) to the last day of Ashad (month 3) of the next year.

```csharp
var d = new NepaliDate(2080, 6, 15);

d.FiscalYearStartDate();                  // 2080/04/01
d.FiscalYearEndDate();                    // 2081/03/31
d.FiscalYearStartAndEndDate(yearOffset: 1);
d.FiscalYearQuarterStartAndEndDate();     // (2080/04/01, 2080/06/30)
d.FiscalYearQuarterStartDate(FiscalYearQuarters.Second);

NepaliDate.GetFiscalYearStartAndEndDate(2080);          // (2080/04/01, 2081/03/31)
NepaliDate.GetFiscalYearQuarterStartAndEndDate(2080, 7);
```

#### FiscalYearQuarters enum

| Value | Int | Months |
|---|---|---|
| `Current` | 0 | The quarter of the date |
| `First` | 4 | Shrawan-Ashoj (4-6) |
| `Second` | 7 | Kartik-Poush (7-9) |
| `Third` | 10 | Magh-Chaitra (10-12) |
| `Fourth` | 1 | Baishakh-Ashad (1-3) |

### NepaliDateRange

```csharp
var range = new NepaliDateRange(new NepaliDate(2080, 1, 1), new NepaliDate(2080, 12, 30));
var ashoj = NepaliDateRange.ForMonth(2080, 6);
var fy    = NepaliDateRange.ForFiscalYear(2080);
var year  = NepaliDateRange.ForCalendarYear(2080);
var week  = NepaliDateRange.FromDayCount(new NepaliDate(2080, 6, 1), 7);

foreach (NepaliDate day in ashoj) { }

int days          = ashoj.Length;          // 30
bool empty        = ashoj.IsEmpty;
bool contains     = ashoj.Contains(new NepaliDate(2080, 6, 15));
bool overlaps     = range1.Overlaps(range2);
bool adjacent     = range1.IsAdjacentTo(range2);
NepaliDateRange union        = range1.Union(range2);
NepaliDateRange intersection = range1.Intersect(range2);
NepaliDateRange[] difference = range1.Except(range2);   // array: may split into two ranges
NepaliDateRange[] months     = fy.SplitByMonth();
NepaliDateRange[] quarters   = fy.SplitByFiscalQuarter();
IEnumerable<NepaliDate> workingDays = ashoj.WorkingDays();   // excludes Saturdays
IEnumerable<NepaliDate> weekend     = ashoj.WeekendDays();   // Saturdays and Sundays
IEnumerable<NepaliDate> everyThird  = ashoj.DatesWithInterval(3);
```

### Bulk Conversion

`BulkConvert` is a class nested in `NepaliDate`.

```csharp
IEnumerable<NepaliDate> bsDates = NepaliDate.BulkConvert.ToNepaliDates(adDates);
IEnumerable<DateTime>   adDates2 = NepaliDate.BulkConvert.ToEnglishDates(bsDates);
IEnumerable<DateTime>   adDates3 = NepaliDate.BulkConvert.ToEnglishDates(new[] { "2080/04/15", "2080-01-01" });

// Converts the whole collection in one pass on the calling thread and keeps input order.
// An invalid date throws from the call. useParallel and batchSize have no effect.
```

### Calendar Data

```csharp
var d = new NepaliDate(2081, 6, 26);

d.TithiNp;          // "दशमी*"
d.TithiEn;          // "Dashami (K)"
d.IsPublicHoliday;  // true
d.EventsEn;         // ["Vijaya Dashami", "Devi Visarjan"]

var info = d.GetCalendarInfo();   // NepDate.Core.Calendar.CalendarInfo
// info.TithiNp, info.TithiEn, info.IsPublicHoliday, info.EventsNp, info.EventsEn
```

Covered years: tithi 2078-2083 BS; events and holidays 2077-2083 BS. Other dates return empty values.

### Serialization

**System.Text.Json:**
```csharp
// net8.0 build: automatic through [JsonConverter] on NepaliDate
string json = JsonSerializer.Serialize(new { Date = new NepaliDate(2080, 6, 15) });
// {"Date":"2080-06-15"}

// Object format, or the netstandard2.0 build (needs System.Text.Json 8.0.5+ in your project)
using NepDate.Serialization;
var options = new JsonSerializerOptions().ConfigureForNepaliDate(useObjectFormat: true);
// {"Year":2080,"Month":6,"Day":15}
```

**Newtonsoft.Json:**
```csharp
using NepDate.Serialization;
var settings = new JsonSerializerSettings().ConfigureForNepaliDate();
string json = JsonConvert.SerializeObject(value, settings);
// Converters: NewtonsoftJsonConverters.NepaliDateJsonConverter, NewtonsoftJsonConverters.NepaliDateObjectJsonConverter
```

Calling either `ConfigureForNepaliDate` overload needs a Newtonsoft.Json reference in your project.

**XML:**
```csharp
using NepDate.Serialization;
var serializer = new XmlSerializer(typeof(NepaliDateXmlSerializer));
serializer.Serialize(writer, new NepaliDateXmlSerializer(new NepaliDate(2080, 6, 15)));
// <NepaliDateXmlSerializer>2080-06-15</NepaliDateXmlSerializer>
```

**ASP.NET Model Binding:**
```csharp
// NepaliDateTypeConverter is registered with [TypeConverter].
// It converts from and to string, int (YYYYMMDD) and DateTime.
[HttpGet("{date}")]
public IActionResult Get(NepaliDate date) { ... }
```

### Enums

#### NepaliMonths

`Baishakh=1, Jestha=2, Ashad=3, Shrawan=4, Bhadra=5, Ashoj=6, Kartik=7, Mangsir=8, Poush=9, Magh=10, Falgun=11, Chaitra=12`

#### Exceptions

| Type | Thrown when |
|---|---|
| `InvalidNepaliDateFormatException` | Text cannot be parsed, or a date-dependent member is used on `default(NepaliDate)` |
| `ArgumentOutOfRangeException` | Year, month or day out of range, or a result outside 1901-2199 BS |
| `ArgumentNullException` | Null `BulkConvert` input, or null or empty text to `SmartDateParser.Parse` |
| `FormatException` | `SmartDateParser.Parse` cannot read the input |

## Pages on this site

| URL | Content |
|---|---|
| https://nepdate.rajuprasai.com.np/ | Homepage, benchmarks, install, FAQ |
| https://nepdate.rajuprasai.com.np/docs.html | Full API reference |
| https://nepdate.rajuprasai.com.np/changelog.html | Version history |
| https://nepdate.rajuprasai.com.np/llms.txt | LLM entry point |
| https://nepdate.rajuprasai.com.np/llms-full.txt | Full documentation in plain text |
| https://nepdate.rajuprasai.com.np/docs.html.md | API reference in Markdown |
| https://nepdate.rajuprasai.com.np/index.html.md | Homepage in Markdown |
| https://nepdate.rajuprasai.com.np/changelog.html.md | Changelog in Markdown |

## External links

- NuGet: https://www.nuget.org/packages/NepDate/
- GitHub: https://github.com/RajuPrasai/NepDate
- License: MIT (https://opensource.org/licenses/MIT)
- NepDate Widget (Windows desktop app): https://nepdatewidget.rajuprasai.com.np/
