# NepDate API Reference

Version: 2.1.0 | https://nepdate.rajuprasai.com.np/docs.html
Full plain-text: https://nepdate.rajuprasai.com.np/llms-full.txt

---

## Getting Started

```
dotnet add package NepDate
```

```csharp
using NepDate;

var today = NepaliDate.Today;
var bs = new NepaliDate(2080, 6, 15);
DateTime ad = bs.EnglishDate;            // 2023-10-02
var bs2 = new NepaliDate(DateTime.Today);
var bs3 = DateTime.Today.ToNepaliDate(); // extension method
```

---

## NepaliDate struct

`namespace NepDate`
`readonly partial struct NepaliDate`
Implements: `IComparable`, `IComparable<NepaliDate>`, `IEquatable<NepaliDate>`, `IFormattable`
net8.0 build also: `ISpanFormattable`, `IParsable<NepaliDate>`, `ISpanParsable<NepaliDate>`

### Constructors and static members

```csharp
NepaliDate(int yearBs, int monthBs, int dayBs)
NepaliDate(DateTime engDate)                     // time and DateTimeKind are ignored
NepaliDate(string rawNepaliDate)                 // "2080/06/15", "2080-6-15", ...
NepaliDate(string rawNepaliDate, bool autoAdjust, bool monthInMiddle = true)

static NepaliDate Today { get; }
static NepaliDate Now { get; }
static readonly NepaliDate MinValue              // 1901/01/01 (1844-04-11)
static readonly NepaliDate MaxValue              // 2199/12/30 (2143-04-15)

static NepaliDate Parse(string rawNepaliDate)
static NepaliDate Parse(string rawNepaliDate, bool autoAdjust, bool monthInMiddle = true)
static bool TryParse(string rawNepDate, out NepaliDate result)
static bool TryParse(string rawNepDate, out NepaliDate result, bool autoAdjust, bool monthInMiddle = true)
```

### Properties

```csharp
int Year                  // BS year (1901-2199)
int Month                 // BS month (1-12)
int Day                   // BS day (1-32)
DateTime EnglishDate      // Gregorian date, midnight, DateTimeKind.Unspecified
DayOfWeek DayOfWeek
NepaliMonths MonthName    // NepaliMonths.Ashoj for month 6
int MonthEndDay           // Days in this month (29-32)
int DayOfYear             // 1 on 1 Baisakh
bool IsDefault            // true for default(NepaliDate)

// Calendar data (see "Calendar Data" for the years covered)
string TithiNp            // "दशमी*" or empty
string TithiEn            // "Dashami (K)" or empty
bool IsPublicHoliday
string[] EventsNp         // empty array when none
string[] EventsEn
```

### Methods

```csharp
NepaliDate AddDays(double days)                                   // negative ok
NepaliDate AddMonths(double months, bool awayFromMonthEnd = false) // clamps to month end by default
NepaliDate AddYears(int years, bool awayFromMonthEnd = false)
NepaliDate MonthEndDate()
TimeSpan Subtract(NepaliDate nepDateTo)                           // same as d1 - d2
bool IsLeapYear()                                                  // Gregorian year of EnglishDate
bool IsToday(); bool IsYesterday(); bool IsTomorrow();
CalendarInfo GetCalendarInfo()                                     // all calendar data in one call
void Deconstruct(out int year, out int month, out int day)         // var (y, m, d) = date;
int CompareTo(NepaliDate other)
bool Equals(NepaliDate other)
// Operators: ==, !=, <, >, <=, >=, and d1 - d2 (TimeSpan)
```

### Fiscal Year

Nepal's fiscal year runs from 1 Shrawan (month 4) to the last day of Ashad (month 3) of the next year.

```csharp
NepaliDate FiscalYearStartDate(int yearOffset = 0)
NepaliDate FiscalYearEndDate(int yearOffset = 0)
(NepaliDate startDate, NepaliDate endDate) FiscalYearStartAndEndDate(int yearOffset = 0)
NepaliDate FiscalYearQuarterStartDate(FiscalYearQuarters fiscalYearQuarters = FiscalYearQuarters.Current, int yearOffset = 0)
NepaliDate FiscalYearQuarterEndDate(FiscalYearQuarters fiscalYearQuarters = FiscalYearQuarters.Current, int yearOffset = 0)
(NepaliDate startDate, NepaliDate endDate) FiscalYearQuarterStartAndEndDate(FiscalYearQuarters fiscalYearQuarters = FiscalYearQuarters.Current, int yearOffset = 0)

static NepaliDate GetFiscalYearStartDate(int fiscalYear)
static NepaliDate GetFiscalYearEndDate(int fiscalYear)
static (NepaliDate startDate, NepaliDate endDate) GetFiscalYearStartAndEndDate(int fiscalYear)
static NepaliDate GetFiscalYearQuarterStartDate(int fiscalYear, int month)
static NepaliDate GetFiscalYearQuarterEndDate(int fiscalYear, int month)
static (NepaliDate startDate, NepaliDate endDate) GetFiscalYearQuarterStartAndEndDate(int fiscalYear, int month)
```

```csharp
var d = new NepaliDate(2080, 6, 15);
d.FiscalYearStartDate();                  // 2080/04/01
d.FiscalYearEndDate();                    // 2081/03/31
d.FiscalYearQuarterStartAndEndDate();     // (2080/04/01, 2080/06/30)
```

---

## Formatting

```csharp
var d = new NepaliDate(2080, 6, 15);

d.ToString();                                              // "2080/06/15"
d.ToString(DateFormats.DayMonthYear, Separators.Dash);     // "15-06-2080"
d.ToString(DateFormats.MonthDayYear, Separators.Dot, false); // "6.15.2080"
d.ToUnicodeString();                                       // "२०८०/०६/१५"
d.ToLongDateString();                                      // "Ashoj 15, 2080"
d.ToLongDateString(displayDayName: true);                  // "Monday, Ashoj 15, 2080"
d.ToLongDateUnicodeString(displayDayName: true);           // "सोमवार, असोज १५, २०८०"
```

### Signatures

```csharp
string ToString()                                   // "yyyy/MM/dd"
string ToString(DateFormats dateFormat, Separators separator = Separators.ForwardSlash, bool leadingZeros = true)
string ToUnicodeString(DateFormats dateFormat = DateFormats.YearMonthDay, Separators separator = Separators.ForwardSlash, bool leadingZeros = true)
string ToLongDateString(bool leadingZeros = true, bool displayDayName = false, bool displayYear = true)
string ToLongDateUnicodeString(bool leadingZeros = true, bool displayDayName = false, bool displayYear = true)
string ToString(string format, IFormatProvider formatProvider = null) // IFormattable
```

### Format strings (IFormattable)

| Format | Result for 2080/06/15 |
|---|---|
| `null`, `""`, `"G"`, `"g"`, `"d"` | 2080/06/15 |
| `"D"` | Ashoj 15, 2080 |
| `"s"` | 2080-06-15 |
| `"MMMM dd, yyyy"` | Ashoj 15, 2080 |
| `"MMM d yy"` | Ash 15 80 |

Custom tokens: `yyyy`, `yy`, `MMMM` (month name), `MMM` (first 3 letters), `MM`, `M`, `dd`, `d`. Quote literal text with `'...'` or escape one character with `\`. String interpolation works: `$"{d:yyyy-MM-dd}"`.

On the net8.0 build, `ISpanFormattable` is implemented explicitly, so call it through the interface: `((ISpanFormattable)d).TryFormat(buffer, out int written, "s", null)`.

### DateFormats enum

`YearMonthDay`, `YearDayMonth`, `MonthYearDay`, `MonthDayYear`, `DayYearMonth`, `DayMonthYear`

### Separators enum

`ForwardSlash` (/), `BackwardSlash` (\\), `Dot` (.), `Underscore` (_), `Dash` (-), `Space` ( )

---

## Parsing

### Strict parsing: NepaliDate.Parse / TryParse

Year, month, day in that order, ASCII digits, separated by `/`, `-`, `.`, `_`, `\`, space, `|` or `।`.

```csharp
NepaliDate.Parse("2080-06-15");                    // 2080/06/15
NepaliDate.Parse("2080/6/15");                     // 2080/06/15
NepaliDate.Parse("15-06-2080", autoAdjust: true);  // 2080/06/15 (swaps parts that cannot be a year)
NepaliDate.TryParse("2080/13/01", out var bad);    // false
```

### Smart parsing: SmartDateParser

Accepts month names (100+ spellings), Nepali digits and Devanagari month names, and day-first input.

```csharp
SmartDateParser.Parse("15 Ashoj 2080");     // 2080/06/15
SmartDateParser.Parse("Asoj 15, 2080");     // 2080/06/15
SmartDateParser.Parse("२०८० असोज १५");       // 2080/06/15
SmartDateParser.Parse("२०८०/०६/१५");         // 2080/06/15
SmartDateParser.Parse("080/6/15");          // 2080/06/15
SmartDateParser.TryParse(input, out NepaliDate result);
"15 Ashoj 2080".ToNepaliDate();             // extension method
"15 Ashoj 2080".TryToNepaliDate(out var r); // extension method
```

| Month | Accepted spellings (sample) |
|---|---|
| Baishakh | Baisakh, Baishakh, Vaishakh, Baisak |
| Jestha | Jestha, Jeshtha, Jesth, Jeth |
| Ashad | Ashadh, Ashad, Asad, Asar, Ashar |
| Shrawan | Shrawan, Shravan, Sawan, Saun |
| Bhadra | Bhadra, Bhadau, Bhado, Bhadon |
| Ashoj | Ashoj, Ashwin, Aswin, Asoj |
| Kartik | Kartik, Kartick, Karthik |
| Mangsir | Mangsir, Mangshir, Margashirsha |
| Poush | Poush, Push, Paush |
| Magh | Magh, Maagh |
| Falgun | Falgun, Phalgun, Phagun |
| Chaitra | Chaitra, Chait |

---

## NepaliDateRange

```csharp
readonly struct NepaliDateRange : IEnumerable<NepaliDate>, IEquatable<NepaliDateRange>

NepaliDateRange(NepaliDate start, NepaliDate end)
static NepaliDateRange SingleDay(NepaliDate date)
static NepaliDateRange FromDayCount(NepaliDate start, int days)
static NepaliDateRange ForMonth(int year, int month)
static NepaliDateRange ForFiscalYear(int fiscalYear)
static NepaliDateRange ForCalendarYear(int year)
static NepaliDateRange CurrentMonth()
static NepaliDateRange CurrentFiscalYear()
static NepaliDateRange CurrentCalendarYear()

NepaliDate Start { get; }
NepaliDate End   { get; }
bool IsEmpty     { get; }     // Start > End
int Length       { get; }     // inclusive day count

bool Contains(NepaliDate date)
bool Contains(NepaliDateRange other)
bool Overlaps(NepaliDateRange other)
bool IsAdjacentTo(NepaliDateRange other)
NepaliDateRange Intersect(NepaliDateRange other)
NepaliDateRange Union(NepaliDateRange other)
NepaliDateRange[] Except(NepaliDateRange other)
NepaliDateRange[] SplitByMonth()
NepaliDateRange[] SplitByFiscalQuarter()
IEnumerable<NepaliDate> DatesWithInterval(int interval)
IEnumerable<NepaliDate> WorkingDays(bool excludeSunday = false)  // excludes Saturdays
IEnumerable<NepaliDate> WeekendDays(bool includeSunday = true)
string ToString()
string ToString(DateFormats format, Separators separator = Separators.ForwardSlash)
```

```csharp
var ashoj = NepaliDateRange.ForMonth(2080, 6);
ashoj.Length;                                                // 30
ashoj.IsAdjacentTo(NepaliDateRange.ForMonth(2080, 7));       // true
ashoj.Except(NepaliDateRange.FromDayCount(new NepaliDate(2080, 6, 10), 5));
// [2080/06/01 - 2080/06/09, 2080/06/15 - 2080/06/30]
foreach (var day in ashoj) Console.WriteLine(day);
```

---

## Bulk Conversion

`BulkConvert` is a class nested in `NepaliDate`.

```csharp
IEnumerable<NepaliDate> NepaliDate.BulkConvert.ToNepaliDates(IEnumerable<DateTime> engDates, bool useParallel = true)
IEnumerable<DateTime>   NepaliDate.BulkConvert.ToEnglishDates(IEnumerable<NepaliDate> nepDates, bool useParallel = true)
IEnumerable<DateTime>   NepaliDate.BulkConvert.ToEnglishDates(IEnumerable<string> nepDates, bool useParallel = true)
IEnumerable<NepaliDate> NepaliDate.BulkConvert.BatchProcessToNepaliDates(IEnumerable<DateTime> engDates, int batchSize = 1000)
IEnumerable<DateTime>   NepaliDate.BulkConvert.BatchProcessToEnglishDates(IEnumerable<NepaliDate> nepDates, int batchSize = 1000)
```

Each call converts the whole collection in one pass on the calling thread and keeps the input order. An invalid date throws from the call itself. `useParallel` and `batchSize` are kept for source compatibility and do not change the result.

---

## Calendar Data

```csharp
var d = new NepaliDate(2081, 6, 26);
d.TithiNp;          // "दशमी*"
d.TithiEn;          // "Dashami (K)"
d.IsPublicHoliday;  // true
d.EventsEn;         // ["Vijaya Dashami", "Devi Visarjan"]
d.EventsNp;         // ["विजया दशमी", "देवी विसर्जन"]

CalendarInfo info = d.GetCalendarInfo();  // namespace NepDate.Core.Calendar
// info.TithiNp, info.TithiEn, info.IsPublicHoliday, info.EventsNp, info.EventsEn
```

Data bundled with the library:

| Data | Years covered |
|---|---|
| Tithi | 2078-2083 BS |
| Events and public holidays | 2077-2083 BS, plus a few days of 2084 BS |

For any other date the properties return empty strings, `false` and empty arrays. Nothing throws.

---

## Serialization

### System.Text.Json

Automatic on the net8.0 build through the `[JsonConverter]` attribute.

```csharp
JsonSerializer.Serialize(new { D = new NepaliDate(2080, 6, 15) });
// {"D":"2080-06-15"}

using NepDate.Serialization;
var options = new JsonSerializerOptions().ConfigureForNepaliDate(useObjectFormat: true);
JsonSerializer.Serialize(new NepaliDate(2080, 6, 15), options);
// {"Year":2080,"Month":6,"Day":15}
```

On the netstandard2.0 build (for example .NET Framework), call `ConfigureForNepaliDate()` and reference System.Text.Json 8.0.5 or later yourself.

### Newtonsoft.Json

```csharp
using NepDate.Serialization;
var settings = new JsonSerializerSettings().ConfigureForNepaliDate();
JsonConvert.SerializeObject(new { D = new NepaliDate(2080, 6, 15) }, settings);
// {"D":"2080-06-15"}
// Converters: NewtonsoftJsonConverters.NepaliDateJsonConverter, NewtonsoftJsonConverters.NepaliDateObjectJsonConverter
```

Your project must reference Newtonsoft.Json to call either `ConfigureForNepaliDate` overload.

### XML

Wrap the date in `NepDate.Serialization.NepaliDateXmlSerializer`. It writes `2080-06-15` as the element text.

```csharp
var xml = new XmlSerializer(typeof(NepaliDateXmlSerializer));
xml.Serialize(writer, new NepaliDateXmlSerializer(new NepaliDate(2080, 6, 15)));
// <NepaliDateXmlSerializer>2080-06-15</NepaliDateXmlSerializer>
```

### ASP.NET Model Binding

`NepaliDateTypeConverter` is registered with `[TypeConverter]`. It converts from and to `string`, `int` (YYYYMMDD, for example `20800615`) and `DateTime`, so route values, query strings and form fields bind directly.

```csharp
[HttpGet("{date}")]
public IActionResult Get(NepaliDate date) { ... }
```

---

## Enums

### NepaliMonths

`Baishakh=1, Jestha=2, Ashad=3, Shrawan=4, Bhadra=5, Ashoj=6, Kartik=7, Mangsir=8, Poush=9, Magh=10, Falgun=11, Chaitra=12`

### FiscalYearQuarters

| Member | Value | Months |
|---|---|---|
| `Current` | 0 | The quarter of the date |
| `First` | 4 | Shrawan-Ashoj (4-6) |
| `Second` | 7 | Kartik-Poush (7-9) |
| `Third` | 10 | Magh-Chaitra (10-12) |
| `Fourth` | 1 | Baishakh-Ashad (1-3) |

---

## Exceptions

| Type | Thrown when |
|---|---|
| `InvalidNepaliDateFormatException` (a `FormatException`) | Text cannot be parsed, or a property needing a date is read on `default(NepaliDate)` |
| `ArgumentOutOfRangeException` | Year, month or day out of range, or a result outside 1901-2199 BS |
| `ArgumentNullException` | A `BulkConvert` input collection is null, or `SmartDateParser.Parse` gets null or empty text |
| `FormatException` | `SmartDateParser.Parse` cannot read the input |

---

## Supported Range

| BS | AD |
|---|---|
| 1901/01/01 | 1844-04-11 |
| 2199/12/30 | 2143-04-15 |

Calendar data covers fewer years, see "Calendar Data".
