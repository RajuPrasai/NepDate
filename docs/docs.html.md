# NepDate API Reference

Version: 2.0.7 | https://nepdate.rajuprasai.com.np/docs.html
Full plain-text: https://nepdate.rajuprasai.com.np/llms-full.txt

---

## Getting Started

```
dotnet add package NepDate
```

```csharp
using NepDate;

var today = NepaliDate.Now;
var bs = new NepaliDate(2080, 6, 15);
DateTime ad = bs.EnglishDate;
var bs2 = new NepaliDate(DateTime.Today);
```

---

## NepaliDate struct

`namespace NepDate`
`readonly partial struct NepaliDate`
Implements: `IComparable<NepaliDate>`, `IEquatable<NepaliDate>`, `IFormattable`
net8.0 also: `ISpanFormattable`, `IParsable<NepaliDate>`

### Constructors and factory methods

```csharp
NepaliDate(int year, int month, int day)
NepaliDate(DateTime englishDate)
static NepaliDate NepaliDate.Now { get; }
static NepaliDate NepaliDate.Parse(string s)
static bool NepaliDate.TryParse(string s, out NepaliDate result)
static NepaliDate NepaliDate.FromInt(int yyyymmdd)
```

### Properties

```csharp
int Year          // BS year (1901-2199)
int Month         // BS month (1-12)
int Day           // BS day (1-32)
DateTime EnglishDate      // Gregorian equivalent
DayOfWeek DayOfWeek
string DayName            // "Sunday", "Monday", ...
NepaliMonths MonthName    // Enum
int MonthEndDay           // Last day of this month (29-32)
bool IsLeapYear
NepaliDate YearStartDate
NepaliDate YearEndDate
NepaliDate MonthStartDate
NepaliDate MonthEndDate
string Tithi              // Lunar day (2001-2089 BS); empty outside range
string PublicHoliday      // Gazetted holiday name or empty
string Event              // Festival or event name or empty
```

### Arithmetic

```csharp
NepaliDate AddDays(int days)     // negative ok
NepaliDate AddMonths(int months) // negative ok
int CompareTo(NepaliDate other)
bool Equals(NepaliDate other)
// Operators: ==, !=, <, >, <=, >=
```

### Fiscal Year

```csharp
NepaliDate FiscalYearStartDate   // Shrawan 1 of fiscal year
NepaliDate FiscalYearEndDate     // Ashadh last-day of fiscal year
FiscalYearQuarters FiscalYearQuarter
```

Nepal fiscal year: Shrawan 1 (month 4) through Ashadh end (month 3 of next year).

---

## Formatting

### ToString overloads

```csharp
string ToString()                                              // "2080-06-15"
string ToString(DateFormats format)
string ToString(DateFormats format, Separators separator)
string ToString(DateFormats format, Separators separator, bool useNepaliDigits)
string ToString(string? format, IFormatProvider? formatProvider) // IFormattable
```

### DateFormats enum

| Member | Example |
|---|---|
| `yMd` | 2080-06-15 |
| `dMy` | 15-6-2080 |
| `mDy` | 6/15/2080 |
| `dMMMy` | 15 Ashwin 2080 |
| `ddMMMMyyyy` | 15 Aswin 2080 |
| `yMMMd` | 2080 Ashwin 15 |

### Separators enum

`Dash` (-) | `Slash` (/) | `Dot` (.) | `Space` ( ) | `None` | `Comma` (,)

---

## Smart Parsing

`NepaliDate.Parse` accepts:
- Numeric separators: `-`, `/`, `.`, space
- Nepali Unicode digits: `२०८०-०६-१५`
- Int-string: `"20800615"`
- Month names (100+ spellings):

| Month | Accepted spellings (sample) |
|---|---|
| Baisakh | Baisakh, Baishakh, Vaishakh, Baisak |
| Jestha | Jestha, Jeshtha, Jesth, Jeth |
| Ashadh | Ashadh, Ashad, Asad, Asar, Ashar |
| Shrawan | Shrawan, Shravan, Sawan, Saun |
| Bhadra | Bhadra, Bhadau, Bhado, Bhadon |
| Ashwin | Ashwin, Aswin, Asoj, Aaswin |
| Kartik | Kartik, Kartick, Karthik |
| Mangsir | Mangsir, Mangshir, Margashirsha, Aghan |
| Poush | Poush, Push, Paush, Pous |
| Magh | Magh, Maagh |
| Falgun | Falgun, Phalgun, Phagun, Faagun |
| Chaitra | Chaitra, Chait, Chatra |

---

## NepaliDateRange

```csharp
struct NepaliDateRange : IEnumerable<NepaliDate>

NepaliDateRange(NepaliDate start, NepaliDate end)

NepaliDate Start { get; }
NepaliDate End   { get; }
int DayCount     { get; }  // inclusive count

bool Contains(NepaliDate date)
bool Overlaps(NepaliDateRange other)
bool IsContiguousWith(NepaliDateRange other)
NepaliDateRange Union(NepaliDateRange other)
NepaliDateRange Intersect(NepaliDateRange other)
NepaliDateRange Except(NepaliDateRange other)
IEnumerator<NepaliDate> GetEnumerator()
```

```csharp
var range = new NepaliDateRange(new NepaliDate(2080, 4, 1), new NepaliDate(2080, 6, 30));
foreach (var day in range) Console.WriteLine(day);
bool inRange = range.Contains(new NepaliDate(2080, 5, 15)); // true
```

---

## Bulk Conversion

```csharp
static class BulkConvert   // namespace NepDate

static IEnumerable<NepaliDate> ToNepaliDates(IEnumerable<DateTime> englishDates)
static IEnumerable<DateTime>   ToEnglishDates(IEnumerable<NepaliDate> nepaliDates)
```

Converts in one pass on the calling thread. Thread-safe. Results preserve input order.

---

## Calendar Data

```csharp
// Per-date (2001-2089 BS; empty string outside range)
string d.Tithi
string d.PublicHoliday
string d.Event

// Batch for whole month
CalendarInfo info = CalendarInfo.GetForMonth(int year, int month);
// info.Tithis    Dictionary<int, string>  day -> tithi
// info.Holidays  Dictionary<int, string>  day -> holiday name
// info.Events    Dictionary<int, string>  day -> event name
```

---

## Serialization

### System.Text.Json

Automatic via `[JsonConverter]` attribute. No configuration needed.
```csharp
JsonSerializer.Serialize(new { D = new NepaliDate(2080,6,15) })
// {"D":"2080-06-15"}
```

### Newtonsoft.Json

```csharp
settings.Converters.Add(new NepDateNewtonsoftConverter());
```

### XML

`NepaliDate` implements `IXmlSerializable`. Serializes as `<D>2080-06-15</D>`.

### ASP.NET Model Binding

`TypeConverter` registered automatically. Works for route params, query strings, form fields.
```csharp
[HttpGet("{date}")]
public IActionResult Get(NepaliDate date) { ... }
```

---

## Enums

### NepaliMonths

`Baisakh=1, Jestha=2, Ashadh=3, Shrawan=4, Bhadra=5, Ashwin=6, Kartik=7, Mangsir=8, Poush=9, Magh=10, Falgun=11, Chaitra=12`

### FiscalYearQuarters

| Member | Value | Months |
|---|---|---|
| `Current` | 0 | Resolved dynamically |
| `First` | 4 | Shrawan-Bhadra (4-6) |
| `Second` | 7 | Ashwin-Poush (7-9) |
| `Third` | 10 | Magh-Chaitra (10-12) |
| `Fourth` | 1 | Baisakh-Ashadh (1-3) |

---

## Exceptions

| Type | Thrown when |
|---|---|
| `InvalidNepaliDateFormatException` | String input cannot be parsed |
| `ArgumentOutOfRangeException` | Year/month/day out of numeric range |
| `NepaliDateOutOfRangeException` | Date outside 1901-2199 BS |
| `NepaliMonthEndException` | Day exceeds month maximum for year |

---

## Type System

| Interface | Targets | Notes |
|---|---|---|
| `IComparable<NepaliDate>` | both | Sorting, OrderBy |
| `IEquatable<NepaliDate>` | both | ==, .Equals() |
| `IFormattable` | both | ToString(format, provider) |
| `ISpanFormattable` | net8.0 | Span-based formatting |
| `IParsable<NepaliDate>` | net8.0 | NepaliDate.Parse(s, provider) |
| `IXmlSerializable` | both | XML serialization |

`NepaliDateTypeConverter` registered via `[TypeConverter]` - converts between `string`, `int` (YYYYMMDD), `DateTime`, and `NepaliDate`.

---

## Supported Range

| BS | AD (approximate) |
|---|---|
| 1901/01/01 | 1844-04-13 |
| 2199/12/last | 2143-04-12 |

Calendar metadata available: 2001-2089 BS.
