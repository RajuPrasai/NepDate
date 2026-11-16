# NepDate - Bikram Sambat .NET Library

**Skill type:** library-documentation
**Scope:** All public APIs of the NepDate NuGet package v2.0.7
**Canonical docs:** https://nepdate.rajuprasai.com.np/docs.html
**NuGet:** https://www.nuget.org/packages/NepDate/
**GitHub:** https://github.com/RajuPrasai/NepDate
**Related:** NepDate Widget (Windows app) at https://nepdatewidget.rajuprasai.com.np/

## What NepDate is

NepDate is a zero-dependency, MIT-licensed .NET library for working with Bikram Sambat (BS) dates. It exposes a single primary type: `NepaliDate`, a `readonly partial struct` that stores a BS date and converts to/from `System.DateTime`.

Performance: 4.55 ns/op for BS-to-AD conversion, zero heap allocations. All calendar table lookups are O(1) flat-array operations.

Supported range: 1901/01/01 BS through 2199/12/last-day BS. Calendar metadata (tithi, public holidays, festivals) for 2001-2089 BS only; dates outside this range return empty/default without throwing.

Targets: .NET Standard 2.0, net8.0. Works on .NET Framework 4.6.1+, .NET Core 2.0+, .NET 5-10, Xamarin, MAUI, Unity, Mono.

## Installation

```
dotnet add package NepDate
```

Or in a `.csproj`:
```xml
<PackageReference Include="NepDate" Version="2.0.7" />
```

## Core type: NepaliDate

`NepaliDate` is in the `NepDate` namespace. It is a `readonly partial struct` implementing `IComparable<NepaliDate>`, `IEquatable<NepaliDate>`, `IFormattable`, `ISpanFormattable` (net8.0), `IParsable<NepaliDate>` (net8.0).

### Creating instances

```csharp
using NepDate;

// From BS year/month/day
var d = new NepaliDate(2080, 6, 15);

// From DateTime (AD to BS conversion)
var d2 = new NepaliDate(DateTime.Today);

// Today in BS
var today = NepaliDate.Now;

// Parse from string (handles 100+ month-name spellings, Nepali digits, separators)
var d3 = NepaliDate.Parse("2080-6-15");
var d4 = NepaliDate.Parse("2080 Ashwin 15");
var d5 = NepaliDate.Parse("२०८०-०६-१५");

// Non-throwing parse
if (NepaliDate.TryParse("2080-6-15", out var result)) { ... }

// From int YYYYMMDD
var d6 = NepaliDate.FromInt(20800615);
```

### Key properties

| Property | Type | Description |
|---|---|---|
| `Year` | int | BS year (1901-2199) |
| `Month` | int | BS month (1-12) |
| `Day` | int | BS day (1-32 depending on year/month) |
| `EnglishDate` | DateTime | Gregorian equivalent |
| `DayOfWeek` | DayOfWeek | Day of week |
| `DayName` | string | English day name |
| `MonthName` | NepaliMonths | Enum value for month |
| `MonthEndDay` | int | Last day of this month (29-32) |
| `IsLeapYear` | bool | Whether BS year is a leap year |
| `YearStartDate` | NepaliDate | 1st of same BS year |
| `YearEndDate` | NepaliDate | Last day of same BS year |
| `MonthStartDate` | NepaliDate | 1st of same BS month |
| `MonthEndDate` | NepaliDate | Last day of same BS month |

### Calendar metadata properties (2001-2089 BS only)

| Property | Type | Description |
|---|---|---|
| `Tithi` | string | Lunar day (तिथि) in Nepali Unicode |
| `PublicHoliday` | string | Government gazetted holiday name or empty |
| `Event` | string | Festival or calendar event name or empty |

### Arithmetic and comparison

```csharp
var d = new NepaliDate(2080, 6, 15);

// Add/subtract days
NepaliDate later = d.AddDays(30);
NepaliDate earlier = d.AddDays(-30);

// Add months
NepaliDate nextMonth = d.AddMonths(2);

// Operators
bool isEqual = d1 == d2;
bool isLater = d1 > d2;
int diff = d1.CompareTo(d2);

// Subtraction returns TimeSpan
TimeSpan gap = d1.EnglishDate - d2.EnglishDate;
```

### Formatting

```csharp
var d = new NepaliDate(2080, 6, 15);

// Default: "2080-06-15"
string s1 = d.ToString();

// Custom format with DateFormats enum
string s2 = d.ToString(DateFormats.dMMMy);     // "15 Ashwin 2080"
string s3 = d.ToString(DateFormats.ddMMMMyyyy); // "15 Aswin 2080"
string s4 = d.ToString(DateFormats.mDy);        // "6/15/2080"

// Custom separator
string s5 = d.ToString(DateFormats.yMd, Separators.Dot);   // "2080.06.15"
string s6 = d.ToString(DateFormats.yMd, Separators.Slash);  // "2080/06/15"

// Nepali Unicode numerals
string s7 = d.ToString(DateFormats.yMd, Separators.Dash, useNepaliDigits: true); // "२०८०-०६-१५"

// IFormattable interface
string s8 = d.ToString("yMd-slash", null); // format string: "<DateFormats>-<Separators>"
```

#### DateFormats enum values

| Value | Pattern | Example |
|---|---|---|
| `yMd` | YYYY-MM-DD | 2080-06-15 |
| `dMy` | D-M-YYYY | 15-6-2080 |
| `mDy` | M/D/YYYY | 6/15/2080 |
| `dMMMy` | D Mon YYYY | 15 Ashwin 2080 |
| `ddMMMMyyyy` | DD Mmmm YYYY | 15 Aswin 2080 |
| `yMMMd` | YYYY Mon DD | 2080 Ashwin 15 |

#### Separators enum values

`Dash`, `Slash`, `Dot`, `Space`, `None`, `Comma`

### Smart parsing

`NepaliDate.Parse` accepts 100+ variant spellings of Nepali month names including romanized forms (Baisakh/Baishakh/Vaishakh, Jestha/Jeshtha/Jesth, Ashadh/Ashad/Asad, Shrawan/Sawan, Bhadra/Bhadau/Bhado, Ashwin/Aswin/Asoj, Kartik/Kartick, Mangsir/Mangshir/Margashirsha, Poush/Push/Paush, Magh, Falgun/Phalgun/Phagun, Chaitra/Chait). Also accepts Nepali Unicode digits (०-९) in any position.

### Fiscal Year

Nepal's fiscal year runs Shrawan 1 (month 4) through Ashadh end (month 3 of next year).

```csharp
var d = new NepaliDate(2080, 6, 15); // Ashwin 2080 = Q2

d.FiscalYearStartDate;     // NepaliDate: Shrawan 1, 2080
d.FiscalYearEndDate;       // NepaliDate: Ashadh last day, 2081
d.FiscalYearQuarter;       // FiscalYearQuarters enum value

// Quarter boundaries within fiscal year
// Q1: Shrawan-Bhadra (months 4-6)
// Q2: Ashwin-Poush (months 7-9)
// Q3: Magh-Chaitra (months 10-12)
// Q4: Baisakh-Ashadh (months 1-3)
```

#### FiscalYearQuarters enum

| Value | Int | Months |
|---|---|---|
| `Current` | 0 | (computed dynamically) |
| `First` | 4 | Shrawan-Bhadra |
| `Second` | 7 | Ashwin-Poush |
| `Third` | 10 | Magh-Chaitra |
| `Fourth` | 1 | Baisakh-Ashadh |

### NepaliDateRange

```csharp
using NepDate;

var start = new NepaliDate(2080, 1, 1);
var end   = new NepaliDate(2080, 12, 30);
var range = new NepaliDateRange(start, end);

// Enumerate
foreach (NepaliDate d in range) { ... }

// Length
int days = range.DayCount;

// Check membership
bool contains = range.Contains(new NepaliDate(2080, 6, 15));

// Set operations
NepaliDateRange union        = range1.Union(range2);
NepaliDateRange intersection = range1.Intersect(range2);
NepaliDateRange difference   = range1.Except(range2);
bool overlaps                = range1.Overlaps(range2);
bool isContiguous            = range1.IsContiguousWith(range2);
```

### Bulk Conversion

```csharp
using NepDate;

// AD to BS
IEnumerable<DateTime> adDates = ...;
IEnumerable<NepaliDate> bsDates = BulkConvert.ToNepaliDates(adDates);

// BS to AD
IEnumerable<NepaliDate> bsDates2 = ...;
IEnumerable<DateTime> adDates2 = BulkConvert.ToEnglishDates(bsDates2);

// Converts in one pass on the calling thread
// Returns results preserving input order
```

### Calendar Data

```csharp
var d = new NepaliDate(2080, 6, 15);

// Via CalendarInfo helper (batch retrieval for a month)
CalendarInfo info = CalendarInfo.GetForMonth(2080, 6);
// info.Tithis    - Dictionary<int, string>: day -> tithi
// info.Holidays  - Dictionary<int, string>: day -> holiday name
// info.Events    - Dictionary<int, string>: day -> event name

// Or directly on NepaliDate instance
string tithi   = d.Tithi;        // "पञ्चमी" or empty outside 2001-2089
string holiday = d.PublicHoliday;
string ev      = d.Event;
```

### Serialization

**System.Text.Json (automatic):**
```csharp
// NepDate registers a JsonConverter automatically via [JsonConverter] attribute
string json = JsonSerializer.Serialize(new { Date = new NepaliDate(2080, 6, 15) });
// {"Date":"2080-06-15"}

var obj = JsonSerializer.Deserialize<MyType>(json);
```

**Newtonsoft.Json:**
```csharp
// Add NepDateJsonConverter to JsonSerializerSettings
var settings = new JsonSerializerSettings();
settings.Converters.Add(new NepDateJsonConverter());
string json = JsonConvert.SerializeObject(value, settings);
```

**XML / DataContract:**
```csharp
// NepaliDate implements IXmlSerializable
var serializer = new XmlSerializer(typeof(MyType));
// Serializes as <Date>2080-06-15</Date>
```

**ASP.NET Model Binding:**
```csharp
// TypeConverter registered automatically
// NepaliDate works as a route parameter, query string param, form field
[HttpGet("{date}")]
public IActionResult Get(NepaliDate date) { ... }
```

### Enums

#### NepaliMonths

`Baisakh=1, Jestha=2, Ashadh=3, Shrawan=4, Bhadra=5, Ashwin=6, Kartik=7, Mangsir=8, Poush=9, Magh=10, Falgun=11, Chaitra=12`

#### Exceptions

| Type | Thrown when |
|---|---|
| `InvalidNepaliDateFormatException` | String input is malformed and cannot be parsed |
| `ArgumentOutOfRangeException` | Year/month/day is out of supported range |
| `NepaliDateOutOfRangeException` | Date falls outside 1901-2199 BS |
| `NepaliMonthEndException` | Day exceeds the month's maximum for that year |

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
