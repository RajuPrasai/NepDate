# NepDate Documentation Website — Full AI Generation Prompt

You are building a complete, production ready documentation website for the **NepDate** .NET NuGet package. The site will be hosted on GitHub Pages from the `docs/` folder of the `main` branch at `https://rajuprasai.github.io/NepDate/`.

The output must be a self contained static site using only HTML, CSS, and vanilla JavaScript. No frameworks, no build tools, no npm. Every file must be placed inside the `docs/` folder.

---

## Project Context

NepDate is a high performance, memory efficient `readonly partial struct` targeting `.NET Standard 2.0` (and `net8.0`) that provides complete Bikram Sambat (B.S.) calendar support for .NET applications. It handles conversion, arithmetic, formatting, smart parsing, fiscal year operations, calendar metadata, date ranges, bulk conversion, and serialization in a single lightweight package with zero external dependencies.

**Repository:** https://github.com/rajuprasai/NepDate
**NuGet:** https://www.nuget.org/packages/NepDate/
**Current Version:** 2.0.5
**License:** MIT
**Author:** Raju Prasai
**Sponsor Link:** https://buymemomo.com/rajuprasai

---

## Site Structure

Generate the following files:

```
docs/
  index.html          — Landing page
  docs.html           — Full API documentation page
  changelog.html      — Changelog / releases page
  css/
    style.css         — All styles (dark/light theme, responsive, syntax highlighting)
  js/
    main.js           — Theme toggle, search, sidebar navigation, smooth scroll, mobile menu
  images/
    (reference the existing NepDate logo from GitHub: https://user-images.githubusercontent.com/37014558/231635618-bf6599e3-554e-4b02-93df-019e7b8aecc3.png)
```

---

## Global Design Requirements

### Theme
- Auto dark/light mode toggle that respects `prefers-color-scheme` on first visit.
- A toggle button (sun/moon icon) in the header to manually switch. Store preference in `localStorage`.
- Dark mode: deep navy/charcoal background (`#0d1117` or similar), soft white text, vibrant accent color (use a warm orange or teal, your choice, but be consistent).
- Light mode: clean white/off-white background, dark text, same accent color.
- Transitions between themes should be smooth (0.3s).

### Typography
- Use system font stack: `-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Fira Sans', 'Droid Sans', 'Helvetica Neue', sans-serif`.
- Code blocks use: `'JetBrains Mono', 'Fira Code', 'Cascadia Code', 'Source Code Pro', Consolas, monospace`. Load JetBrains Mono from Google Fonts.
- Base font size 16px, scale up headings proportionally.

### Layout
- Max content width: 1200px centered.
- Responsive: mobile first. Hamburger menu on mobile. Sidebar collapses to off-canvas on screens < 768px.
- Sticky header on all pages.
- Footer on all pages with: copyright, GitHub link, NuGet link, sponsor link, MIT license badge.

### Code Blocks
- Custom syntax highlighting for C# code blocks. Use CSS classes for keywords, strings, comments, types, numbers. Do NOT use any external syntax highlighting library. Write a minimal JS-based highlighter that handles C# keywords (`var`, `int`, `bool`, `string`, `new`, `using`, `public`, `static`, `class`, `struct`, `readonly`, `namespace`, `return`, `if`, `else`, `true`, `false`, `null`, `out`, `this`, `enum`, `foreach`, `in`), types (`NepaliDate`, `NepaliDateRange`, `DateTime`, `TimeSpan`, `DayOfWeek`, `CalendarInfo`, `SmartDateParser`, `BulkConvert`, `FiscalYear`, `NepaliMonths`, `DateFormats`, `Separators`, `FiscalYearQuarters`, `IEnumerable`, `IFormattable`), strings (double-quoted), comments (`//`), and numbers.
- Copy-to-clipboard button on every code block.
- Code blocks should have rounded corners, subtle border, and slight background contrast from page background.

### Navigation Header (all pages)
- Logo (NepDate text or small icon) on the left.
- Navigation links: Home, Documentation, Changelog.
- GitHub star button/link (links to repo).
- Theme toggle button on the right.
- On mobile: hamburger menu that slides in from right.

### Google Analytics
- Include Google Analytics 4 (gtag.js) snippet in the `<head>` of every page.
- Use a placeholder measurement ID: `G-XXXXXXXXXX` with a comment saying "Replace with your GA4 measurement ID".

---

## Page 1: Landing Page (`index.html`)

### Hero Section
- Large centered heading with a strong tagline. Suggest 2 or 3 tagline alternatives that are punchier than "Fast and Efficient Nepali Date Library for .NET". Examples to inspire (do not copy verbatim): "Bikram Sambat for .NET, Done Right", "The Fastest Nepali Calendar Library for .NET", "Nanosecond Nepali Date Conversions for .NET". Pick the best one.
- Subtitle: one sentence summarizing what the library does.
- Animated code demo in the hero: a terminal-style animation that types out a short C# snippet showing NepaliDate creation and conversion, line by line, with a blinking cursor. Use JS for the typing animation. The snippet should be:
```csharp
using NepDate;

var today = NepaliDate.Today;          // 2083/01/07
var english = today.EnglishDate;       // 2026/04/20
var parsed = NepaliDate.Parse("2080/04/15");
var diff = today - parsed;             // TimeSpan
```
- Two CTA buttons below: "Get Started" (scrolls to install section or links to docs.html) and "View on GitHub" (links to repo).
- NuGet download count badge and GitHub stars badge (use shields.io URLs):
  - `https://img.shields.io/nuget/dt/NepDate?style=for-the-badge&label=NuGet%20Downloads`
  - `https://img.shields.io/github/stars/rajuprasai/NepDate?style=for-the-badge`

### Installation Section
- Section title: "Installation"
- Two tab-style code blocks: ".NET CLI" and "Package Manager"
- .NET CLI: `dotnet add package NepDate`
- Package Manager: `Install-Package NepDate`
- Copy button on each.

### Key Features Section
- Grid layout (2 or 3 columns on desktop, 1 on mobile).
- Each feature card has an icon (use simple SVG icons or Unicode symbols, no external icon library), title, and short description.
- Features to highlight (one card each):

1. **Blazing Fast** — Up to 1000x faster than alternatives. Zero heap allocation on BS to AD path. 62ns per conversion.
2. **Bidirectional Conversion** — Convert between Bikram Sambat and Gregorian in nanoseconds. Both directions fully supported.
3. **Smart Parsing** — Accepts 100+ month name spellings, Nepali Unicode digits, alternate separators, typo tolerant, 2 digit year expansion.
4. **Rich Formatting** — Standard and custom format tokens, Unicode Nepali digits, long form Nepali strings, IFormattable integration.
5. **Fiscal Year Operations** — Full Nepal FY (Shrawan to Ashadh) quarter and boundary calculations, both instance and static.
6. **Calendar Data** — Built in Tithi, public holiday flag, and event data for 2001 to 2089 BS compiled from authoritative Bikram Sambat calendar references and cross-verified for accuracy.
7. **Date Range Operations** — Intersection, union, split by month or fiscal quarter, working days, weekend days, and interval iteration.
8. **Serialization** — System.Text.Json, Newtonsoft.Json, and XML. String and object format modes. Zero config on .NET 5+.
9. **First Class .NET Type** — IFormattable, IComparable, IEquatable, IParsable, ISpanFormattable, TypeConverter, JsonConverter. Behaves like DateTime.
10. **Zero Dependencies** — No external runtime dependencies. Lightweight single package.
11. **Bulk Conversion** — Auto parallel processing for 500+ items. Explicit batch size control available.
12. **Struct Design** — readonly partial struct keeps instances on the stack. Eliminates GC pressure in high volume scenarios.

### Performance / Benchmark Section
- Section title: "Performance"
- Brief intro text: "NepDate uses flat array dictionaries for O(1) lookups with zero intermediate object allocations."
- A styled comparison table:

| Library and Method | Mean (ns) | Rank | Allocated (B) |
|---|---:|---:|---:|
| NepDate `BS → AD` | 62.59 | 1 | 0 |
| NepDate `AD → BS` | 276.83 | 2 | 120 |
| NepaliDateConverter.NETCORE `BS → AD` | 63,460 | 3 | 20,176 |
| NepaliDateConverter.Net `BS → AD` | 75,327 | 4 | 20,176 |
| NepaliCalendarBS `BS → AD` | 99,511 | 5 | 159,328 |
| NepaliCalendarBS `AD → BS` | 113,258 | 6 | 158,760 |
| NepaliDateConverter.NETCORE `AD → BS` | 186,610 | 7 | 20,160 |
| NepaliDateConverter.Net `AD → BS` | 212,478 | 8 | 20,160 |

- Visually highlight NepDate rows (accent color background or left border).
- Add a note: "Benchmarked with BenchmarkDotNet on .NET 8."

### Comparison Table Section
- Section title: "How NepDate Compares"
- Feature comparison matrix between NepDate and other known Nepali date libraries:

| Feature | NepDate | NepaliDateConverter | NepaliCalendarBS |
|---|:---:|:---:|:---:|
| BS to AD Conversion | Yes | Yes | Yes |
| AD to BS Conversion | Yes | Yes | Yes |
| Zero Heap Allocation (BS to AD) | Yes | No | No |
| Smart Multi-format Parsing | Yes | No | No |
| Nepali Unicode Support | Yes | No | No |
| Fiscal Year Operations | Yes | No | No |
| Date Range Operations | Yes | No | No |
| Calendar/Tithi/Holiday Data | Yes | No | No |
| Bulk Conversion | Yes | No | No |
| JSON Serialization | Yes | No | No |
| XML Serialization | Yes | No | No |
| IFormattable/IComparable | Yes | No | No |
| TypeConverter | Yes | No | No |
| .NET Standard 2.0 | Yes | Yes | Yes |
| Active Maintenance | Yes | Limited | Limited |

Use checkmark/cross icons. Make NepDate column visually prominent.

### FAQ Section
- Section title: "Frequently Asked Questions"
- Accordion style (click to expand/collapse).
- Include these FAQs:

**Q: What .NET versions does NepDate support?**
A: NepDate targets .NET Standard 2.0, which means it works with .NET Framework 4.6.1+, .NET Core 2.0+, .NET 5/6/7/8/9, Xamarin, Unity, and MAUI. It also has a net8.0 target for newer API features like IParsable and ISpanFormattable.

**Q: What is the supported date range?**
A: NepDate supports Bikram Sambat dates from 1901/01/01 BS (approximately 1844 April 13 AD) to the last day of 2199/12 BS (approximately 2143 April 12 AD). Calendar metadata (Tithi, holidays, events) is available for 2001 to 2089 BS.

**Q: Does NepDate have any external dependencies?**
A: No. NepDate has zero external runtime dependencies. Newtonsoft.Json and System.Text.Json references are internalized using PrivateAssets=all.

**Q: Is NepDate thread safe?**
A: Yes. NepaliDate is a readonly struct, making it inherently immutable and safe for concurrent access. BulkConvert operations use thread safe parallel processing.

**Q: How does NepDate handle invalid dates?**
A: Constructors and Parse throw InvalidNepaliDateFormatException or ArgumentOutOfRangeException for invalid input. Use TryParse for non-throwing validation. Each month's day count is enforced per year (29 to 32 days depending on month and year).

**Q: Can I use NepDate with ASP.NET model binding?**
A: Yes. NepaliDate has a registered TypeConverter that enables automatic model binding in ASP.NET MVC/Web API, WPF, and WinForms property grids.

**Q: How is calendar data (Tithi, holidays) sourced?**
A: Calendar data is compiled from authoritative Bikram Sambat calendar references and cross-verified for accuracy, covering 2001 to 2089 BS. The data is compiled into the library at build time. Properties return empty/default values for dates outside this range without throwing exceptions.

**Q: What is the struct design advantage?**
A: NepaliDate is a readonly partial struct, meaning instances live on the stack rather than the heap. This eliminates garbage collection pressure and makes it extremely efficient for high throughput scenarios like bulk date processing.

### Contributors Section
- Section title: "Contributors"
- Text: "NepDate is open source. Contributions are welcome."
- Link to CONTRIBUTING guide on GitHub.
- Use GitHub's contributor avatar API: `https://contrib.rocks/image?repo=rajuprasai/NepDate` (renders contributor avatars automatically).
- "Become a contributor" button linking to the repo.

### Sponsor Section
- A subtle, tasteful banner or card.
- Text: "Support NepDate Development"
- Short message: "If NepDate saves you time, consider supporting its continued development."
- Button: "Buy Me a Momo" linking to `https://buymemomo.com/rajuprasai`.
- Style it warmly but not aggressively. It should feel appreciative, not desperate.

### Footer
- Copyright: "© 2023-2026 NepDate. MIT License."
- Links: GitHub, NuGet, Documentation, Sponsor.
- "Made with care for the Nepali developer community."

---

## Page 2: Documentation Page (`docs.html`)

This is the core of the site. It must contain the COMPLETE API reference, every method, every property, every enum, every example. Nothing from the source code documentation should be missing.

### Layout
- Left sidebar (fixed on desktop, off-canvas on mobile) with a hierarchical table of contents.
- Right side: main content area.
- Right margin (desktop only, hidden on tablet/mobile): a mini "On This Page" navigation showing H3-level headings of the currently visible section.
- Smooth scroll to anchors. Active section highlighted in sidebar.

### Search
- Search input at the top of the sidebar.
- Client side full text search across all documentation sections.
- As the user types, filter the sidebar items and show matching sections. Highlight matches.
- Pressing Enter or clicking a result scrolls to that section.
- Index all headings, method names, property names, and first ~100 chars of each section's text.

### Sidebar Structure (Table of Contents)

```
Getting Started
  Installation
  Quick Start
  Creating a NepaliDate
  Accessing Properties

Date Operations
  Adding and Subtracting Days
  Adding and Subtracting Months
  Adding and Subtracting Years
  Fractional Month Addition
  Away From Month End
  Comparison Operators
  Date Difference (Subtract)
  Conversion (BS to AD, AD to BS)

Bulk Conversion
  ToNepaliDates
  ToEnglishDates
  BatchProcessToNepaliDates
  BatchProcessToEnglishDates

Date Range Operations
  Creating Ranges
  Factory Methods
  Properties and Containment
  Set Operations (Intersect, Union, Except)
  Splitting (By Month, By Fiscal Quarter)
  Iteration and Filtering
  Working Days and Weekend Days
  Range Formatting

Formatting and Display
  Default Format
  Custom Format and Separator
  Long Date String
  Unicode String
  Long Date Unicode String
  IFormattable Format Specifiers
  Custom Format Tokens
  ISpanFormattable

Smart Date Parsing
  SmartDateParser.Parse
  SmartDateParser.TryParse
  String Extension Methods
  Auto Adjust Parsing
  Supported Formats and Spellings

Fiscal Year Operations
  Instance Methods
  Static Methods
  Quarters
  Year Offset

Calendar Data
  Tithi
  Public Holidays
  Events
  GetCalendarInfo
  Data Coverage

Serialization
  System.Text.Json
  Newtonsoft.Json
  XML Serialization
  String vs Object Format
  Configuration Extensions

Type System Integration
  IComparable
  IEquatable
  IFormattable
  ISpanFormattable
  IParsable and ISpanParsable
  TypeConverter
  JsonConverter Auto Registration

Enums
  NepaliMonths
  DateFormats
  Separators
  FiscalYearQuarters

Exceptions
  InvalidNepaliDateFormatException

Advanced
  Boundary Values (MinValue, MaxValue)
  Default Detection (IsDefault)
  Leap Year Check
  Relative Date Checks (IsToday, IsYesterday, IsTomorrow)
  Deconstruction
  Struct Design Notes
```

### Documentation Content

Below is the COMPLETE content for every section. Render each section with its heading, explanatory text, code examples, and API tables exactly as specified. Every method signature, every parameter, every return type must be present.

---

#### Getting Started > Installation

```bash
# .NET CLI
dotnet add package NepDate

# Package Manager Console
Install-Package NepDate
```

Requires .NET Standard 2.0 compatible runtime. Works with .NET Framework 4.6.1+, .NET Core 2.0+, .NET 5/6/7/8/9, Xamarin, Unity, and MAUI.

#### Getting Started > Quick Start

```csharp
using NepDate;

// Get today's Nepali date
var today = NepaliDate.Today;
Console.WriteLine(today);                    // 2083/01/07

// Convert to English
DateTime english = today.EnglishDate;       // 2026-04-20

// Parse a Nepali date string
var date = NepaliDate.Parse("2080/04/15");

// Smart parse from various formats
var smart = SmartDateParser.Parse("15 Shrawan 2080");

// Date arithmetic
var future = today.AddDays(30);
var nextMonth = today.AddMonths(1);

// Date range
var range = NepaliDateRange.CurrentMonth();
foreach (var d in range.WorkingDays())
    Console.WriteLine(d);
```

#### Getting Started > Creating a NepaliDate

**Constructors:**

| Constructor | Description |
|---|---|
| `NepaliDate(int year, int month, int day)` | From Bikram Sambat components. Year: 1901 to 2199, Month: 1 to 12, Day: 1 to month end day. Throws `ArgumentOutOfRangeException` if out of range. |
| `NepaliDate(string rawNepaliDate)` | Parses a Nepali date string in year-month-day order. Accepted separators: `/`, `-`, `.`, `_`, `\`, space, pipe. Throws `InvalidNepaliDateFormatException`. |
| `NepaliDate(string rawNepaliDate, bool autoAdjust, bool monthInMiddle = true)` | Parses with heuristic correction. Rules: day > 32 swaps year and day; monthInMiddle false swaps month and day; month > 12 and day < 13 swaps them; year < 1000 prepends current millennium. |
| `NepaliDate(DateTime engDate)` | Converts a Gregorian DateTime to Bikram Sambat. Only date portion is used. Must fall within MinValue to MaxValue range. Throws `ArgumentOutOfRangeException`. |

```csharp
// From components
var date1 = new NepaliDate(2079, 12, 16);

// From string
var date2 = new NepaliDate("2079/12/16");

// From Parse
var date3 = NepaliDate.Parse("2079/12/16");

// From English DateTime
var date4 = new NepaliDate(DateTime.Today);

// Extension method
var date5 = DateTime.Today.ToNepaliDate();

// Current date
var today = NepaliDate.Today;
var now = NepaliDate.Now;  // same as Today
```

#### Getting Started > Accessing Properties

| Property | Type | Description |
|---|---|---|
| `Year` | `int` | Year component (1901 to 2199). |
| `Month` | `int` | Month component (1 = Baisakh through 12 = Chaitra). |
| `Day` | `int` | Day component (1 to MonthEndDay, range 29 to 32). |
| `EnglishDate` | `DateTime` | Gregorian equivalent. Time component is always midnight. Throws on default instance. |
| `DayOfWeek` | `DayOfWeek` | Day of the week. Saturday is Nepal's official weekly holiday. |
| `DayOfYear` | `int` | 1-based ordinal within the BS calendar year. Baisakh 1 = day 1. |
| `MonthEndDay` | `int` | Number of days in this month/year (29 to 32). |
| `MonthName` | `NepaliMonths` | Enum for month name (Baisakh through Chaitra). |
| `IsDefault` | `bool` | True when instance is uninitialized (default). |

```csharp
var date = new NepaliDate("2079/12/16");

int year        = date.Year;            // 2079
int month       = date.Month;           // 12
int day         = date.Day;             // 16
DateTime eng    = date.EnglishDate;     // 2023/03/30
DayOfWeek dow   = date.DayOfWeek;      // Thursday
int dayOfYear   = date.DayOfYear;      // 346
int monthEndDay = date.MonthEndDay;    // 30
NepaliDate end  = date.MonthEndDate(); // 2079/12/30
NepaliMonths mn = date.MonthName;      // Chaitra
bool isDefault  = date.IsDefault;      // false
```

**Calendar Properties:**

| Property | Type | Description |
|---|---|---|
| `TithiNp` | `string` | Tithi (lunar day) in Nepali Devanagari. Empty string outside 2001 to 2089 BS. |
| `TithiEn` | `string` | Tithi transliterated to English. Empty string outside 2001 to 2089 BS. |
| `IsPublicHoliday` | `bool` | Whether date is a gazetted public holiday based on cross-verified calendar data. |
| `EventsNp` | `string[]` | Event names in Nepali. Empty array if none. |
| `EventsEn` | `string[]` | Event names in English. Empty array if none. |

```csharp
var date = new NepaliDate(2081, 4, 15);

string tithiNp    = date.TithiNp;          // Nepali tithi name
string tithiEn    = date.TithiEn;          // English tithi name
bool isHoliday    = date.IsPublicHoliday;  // true/false
string[] eventsNp = date.EventsNp;         // events in Nepali
string[] eventsEn = date.EventsEn;         // events in English
```

**Static Properties:**

| Property | Type | Description |
|---|---|---|
| `NepaliDate.Today` | `NepaliDate` | Current date per local system clock. Evaluated fresh on each access. |
| `NepaliDate.Now` | `NepaliDate` | Same as Today. Included for parity with DateTime.Now. |
| `NepaliDate.MinValue` | `NepaliDate` | 1 Baisakh 1901 BS (approx 1844-04-13 AD). |
| `NepaliDate.MaxValue` | `NepaliDate` | Last day of Chaitra 2199 BS (approx 2143-04-12 AD). |

---

#### Date Operations > Adding and Subtracting Days

```csharp
var date = new NepaliDate("2081/04/32");

var later   = date.AddDays(5);     // 2081/05/05
var earlier = date.AddDays(-5);    // 2081/04/27
```

**Signature:** `NepaliDate AddDays(double days)`
Adds the specified number of days. Performed in the Gregorian domain then converted back. Negative values move backward.

#### Date Operations > Adding and Subtracting Months

```csharp
var date = new NepaliDate("2081/04/32");

var plus2  = date.AddMonths(2);    // 2081/06/30
var minus2 = date.AddMonths(-2);   // 2081/02/32
```

**Signature:** `NepaliDate AddMonths(double months, bool awayFromMonthEnd = false)`
Adds whole or fractional months. Fractional months are converted to approximate days (1 month approximately equals 30.42 days). Negative values move backward.

#### Date Operations > Adding and Subtracting Years

```csharp
var date = new NepaliDate("2081/04/15");
var nextYear = date.AddYears(1);   // 2082/04/15
var prevYear = date.AddYears(-1);  // 2080/04/15
```

**Signature:** `NepaliDate AddYears(int years, bool awayFromMonthEnd = false)`
Delegates to AddMonths(years * 12).

#### Date Operations > Fractional Month Addition

```csharp
var date = new NepaliDate("2081/04/32");
var frac = date.AddMonths(2.5);    // 2081/07/15
```

Fractional months are converted to days using the approximation of 30.42 days per month.

#### Date Operations > Away From Month End

When `awayFromMonthEnd` is true, the result is pulled away from the month end boundary into the next month. This is useful when you need a specific day rather than clamping to month end.

```csharp
var date = new NepaliDate("2081/04/32");

// Default: clamps to month end
var clamped = date.AddMonths(2);                          // 2081/06/30

// Away from month end: overflows into next month
var overflow = date.AddMonths(2, awayFromMonthEnd: true); // 2081/07/02
```

#### Date Operations > Comparison Operators

```csharp
var a = NepaliDate.Parse("2079/12/16");
var b = NepaliDate.Parse("2080/01/01");

bool eq = a == b;    // false
bool ne = a != b;    // true
bool lt = a < b;     // true
bool le = a <= b;    // true
bool gt = a > b;     // false
bool ge = a >= b;    // false
```

All standard comparison operators are supported through `IComparable<NepaliDate>` and operator overloads.

#### Date Operations > Date Difference (Subtract)

```csharp
var a = NepaliDate.Parse("2079/12/16");
var b = NepaliDate.Parse("2080/06/01");

TimeSpan diff1 = b - a;           // operator overload
TimeSpan diff2 = b.Subtract(a);   // method equivalent
```

**Signature:** `TimeSpan Subtract(NepaliDate nepDateTo)`
Returns the elapsed time between this date and another. Equivalent to the `-` operator.

#### Date Operations > Conversion (BS to AD, AD to BS)

```csharp
// BS to AD
DateTime eng = NepaliDate.Parse("2079/12/16").EnglishDate;

// AD to BS
NepaliDate nep = new NepaliDate(DateTime.Now);
// or
NepaliDate nep2 = DateTime.Now.ToNepaliDate();  // extension method

string nepStr = nep.ToString();  // "2083/01/07"
```

---

#### Bulk Conversion > ToNepaliDates

```csharp
var engDates = new List<DateTime> { DateTime.Today, DateTime.Today.AddDays(1) };
IEnumerable<NepaliDate> nepDates = NepaliDate.BulkConvert.ToNepaliDates(engDates);
```

**Signature:** `static IEnumerable<NepaliDate> ToNepaliDates(IEnumerable<DateTime> engDates, bool useParallel = true)`
Converts a collection of DateTime values to NepaliDates. Automatically uses parallel processing for collections exceeding 500 items.

#### Bulk Conversion > ToEnglishDates

```csharp
// From NepaliDate collection
IEnumerable<DateTime> eng1 = NepaliDate.BulkConvert.ToEnglishDates(nepDates);

// From string collection
var nepStrings = new List<string> { "2080/01/01", "2080/02/15" };
IEnumerable<DateTime> eng2 = NepaliDate.BulkConvert.ToEnglishDates(nepStrings);
```

**Signatures:**
- `static IEnumerable<DateTime> ToEnglishDates(IEnumerable<NepaliDate> nepDates, bool useParallel = true)`
- `static IEnumerable<DateTime> ToEnglishDates(IEnumerable<string> nepDates, bool useParallel = true)`

#### Bulk Conversion > BatchProcessToNepaliDates

```csharp
var largeDateList = Enumerable.Range(0, 10000).Select(i => DateTime.Today.AddDays(i));
IEnumerable<NepaliDate> results = NepaliDate.BulkConvert.BatchProcessToNepaliDates(largeDateList, batchSize: 2000);
```

**Signature:** `static IEnumerable<NepaliDate> BatchProcessToNepaliDates(IEnumerable<DateTime> engDates, int batchSize = 1000)`
Processes in explicit batches. Throws `ArgumentOutOfRangeException` if batchSize < 1.

#### Bulk Conversion > BatchProcessToEnglishDates

```csharp
IEnumerable<DateTime> results = NepaliDate.BulkConvert.BatchProcessToEnglishDates(nepDates, batchSize: 2000);
```

**Signature:** `static IEnumerable<DateTime> BatchProcessToEnglishDates(IEnumerable<NepaliDate> nepDates, int batchSize = 1000)`

---

#### Date Range Operations > Creating Ranges

```csharp
var start = new NepaliDate(2080, 1, 1);
var end   = new NepaliDate(2080, 3, 15);
var range = new NepaliDateRange(start, end);
```

**Constructor:** `NepaliDateRange(NepaliDate start, NepaliDate end)`
Both inclusive. If end < start, creates an empty range.

#### Date Range Operations > Factory Methods

| Method | Returns | Description |
|---|---|---|
| `SingleDay(NepaliDate date)` | `NepaliDateRange` | Single day range. |
| `FromDayCount(NepaliDate start, int days)` | `NepaliDateRange` | Range spanning specified days. Throws if days < 1. |
| `ForMonth(int year, int month)` | `NepaliDateRange` | Complete Nepali month. |
| `ForFiscalYear(int fiscalYear)` | `NepaliDateRange` | 1 Shrawan to last day of Ashadh. |
| `ForCalendarYear(int year)` | `NepaliDateRange` | 1 Baisakh to last day of Chaitra. |
| `CurrentMonth()` | `NepaliDateRange` | Current Nepali month. |
| `CurrentFiscalYear()` | `NepaliDateRange` | Current Nepali fiscal year. |
| `CurrentCalendarYear()` | `NepaliDateRange` | Current Nepali calendar year. |

```csharp
var single     = NepaliDateRange.SingleDay(start);
var tenDays    = NepaliDateRange.FromDayCount(start, 10);
var month      = NepaliDateRange.ForMonth(2080, 1);
var fy         = NepaliDateRange.ForFiscalYear(2080);
var calYear    = NepaliDateRange.ForCalendarYear(2080);
var curMonth   = NepaliDateRange.CurrentMonth();
var curFY      = NepaliDateRange.CurrentFiscalYear();
var curYear    = NepaliDateRange.CurrentCalendarYear();
```

#### Date Range Operations > Properties and Containment

| Property/Method | Type | Description |
|---|---|---|
| `Start` | `NepaliDate` | Start date (inclusive). |
| `End` | `NepaliDate` | End date (inclusive). |
| `IsEmpty` | `bool` | True when Start > End. |
| `Length` | `int` | Number of days in range (inclusive). 0 if empty. |
| `Contains(NepaliDate)` | `bool` | Whether range contains the date. |
| `Contains(NepaliDateRange)` | `bool` | Whether this range fully contains another. |
| `Overlaps(NepaliDateRange)` | `bool` | Whether ranges share at least one date. |
| `IsAdjacentTo(NepaliDateRange)` | `bool` | Whether ranges are adjacent (no overlap, 1 day gap). |

```csharp
var range = new NepaliDateRange(start, end);

bool empty    = range.IsEmpty;           // false
int  days     = range.Length;            // total days
bool hasDate  = range.Contains(someDate);
bool hasRange = range.Contains(otherRange);
bool overlaps = range.Overlaps(otherRange);
bool adjacent = range.IsAdjacentTo(otherRange);
```

#### Date Range Operations > Set Operations (Intersect, Union, Except)

```csharp
NepaliDateRange intersection = range.Intersect(otherRange);    // dates in both
NepaliDateRange union        = range.Union(otherRange);         // all dates from both
NepaliDateRange[] remaining  = range.Except(otherRange);        // 0, 1, or 2 ranges
```

| Method | Returns | Description |
|---|---|---|
| `Intersect(NepaliDateRange)` | `NepaliDateRange` | Dates present in both ranges. Empty if no overlap. |
| `Union(NepaliDateRange)` | `NepaliDateRange` | All dates from both ranges (may span the gap). |
| `Except(NepaliDateRange)` | `NepaliDateRange[]` | Remaining range(s) after excluding the other. Returns 0, 1, or 2 segments. |

#### Date Range Operations > Splitting (By Month, By Fiscal Quarter)

```csharp
NepaliDateRange[] byMonth   = range.SplitByMonth();
NepaliDateRange[] byQuarter = range.SplitByFiscalQuarter();
```

| Method | Returns | Description |
|---|---|---|
| `SplitByMonth()` | `NepaliDateRange[]` | Sub ranges split at month boundaries. |
| `SplitByFiscalQuarter()` | `NepaliDateRange[]` | Sub ranges split at fiscal quarter boundaries. |

#### Date Range Operations > Iteration and Filtering

```csharp
// Enumerate every date in the range
foreach (NepaliDate date in range) { }

// Dates at regular intervals
IEnumerable<NepaliDate> weekly = range.DatesWithInterval(7);
```

**Signature:** `IEnumerable<NepaliDate> DatesWithInterval(int interval)`
Returns dates at the specified interval. Throws `ArgumentOutOfRangeException` if interval < 1.

#### Date Range Operations > Working Days and Weekend Days

```csharp
// Working days (excludes Saturdays)
IEnumerable<NepaliDate> working = range.WorkingDays();

// Working days (excludes Saturdays and Sundays)
IEnumerable<NepaliDate> workingNoSun = range.WorkingDays(excludeSunday: true);

// Weekend days (Saturdays only)
IEnumerable<NepaliDate> weekends = range.WeekendDays();

// Weekend days (Saturdays and Sundays)
IEnumerable<NepaliDate> weekendsIncSun = range.WeekendDays(includeSunday: true);
```

| Method | Returns | Description |
|---|---|---|
| `WorkingDays(bool excludeSunday = false)` | `IEnumerable<NepaliDate>` | Days excluding Saturday, optionally Sunday. |
| `WeekendDays(bool includeSunday = true)` | `IEnumerable<NepaliDate>` | Saturday, optionally Sunday. |

#### Date Range Operations > Range Formatting

```csharp
string basic     = range.ToString();
// "2080/01/01 - 2080/03/15"

string formatted = range.ToString(DateFormats.DayMonthYear, Separators.Dash);
// "01-01-2080 - 15-03-2080"
```

---

#### Formatting and Display > Default Format

```csharp
var date = new NepaliDate("2079/02/06");
date.ToString()   // "2079/02/06"
```

Default format is `YYYY/MM/DD` with leading zeros.

#### Formatting and Display > Custom Format and Separator

```csharp
date.ToString(DateFormats.DayMonthYear, Separators.Dash, leadingZeros: false)
// "6-2-2079"
```

**Signature:** `string ToString(DateFormats dateFormat, Separators separator = ForwardSlash, bool leadingZeros = true)`

**Available DateFormats:** `YearMonthDay`, `YearDayMonth`, `MonthYearDay`, `MonthDayYear`, `DayYearMonth`, `DayMonthYear`.
**Available Separators:** `ForwardSlash`, `BackwardSlash`, `Dash`, `Dot`, `Underscore`, `Space`.

#### Formatting and Display > Long Date String

```csharp
date.ToLongDateString()
// "Jestha 06, 2079"

date.ToLongDateString(leadingZeros: false, displayDayName: true, displayYear: false)
// "Friday, Jestha 6"
```

**Signature:** `string ToLongDateString(bool leadingZeros = true, bool displayDayName = false, bool displayYear = true)`

#### Formatting and Display > Unicode String

```csharp
date.ToUnicodeString(DateFormats.DayMonthYear, Separators.Dot, leadingZeros: true)
// "०६.०२.२०७९"
```

**Signature:** `string ToUnicodeString(DateFormats dateFormat = YearMonthDay, Separators separator = ForwardSlash, bool leadingZeros = true)`

#### Formatting and Display > Long Date Unicode String

```csharp
date.ToLongDateUnicodeString(leadingZeros: false, displayDayName: true, displayYear: false)
// "शुक्रबार, जेठ ६"
```

**Signature:** `string ToLongDateUnicodeString(bool leadingZeros = true, bool displayDayName = false, bool displayYear = true)`

#### Formatting and Display > IFormattable Format Specifiers

NepaliDate implements `IFormattable` (and `ISpanFormattable` on .NET 6+). Format strings work in string interpolation, `string.Format`, and any `IFormattable` context.

| Specifier | Output | Notes |
|---|---|---|
| `"d"` or `"G"` | `2079/02/06` | Same as ToString() |
| `"D"` | `Jestha 06, 2079` | Long date with leading zeros |
| `"s"` | `2079-02-06` | Sortable, dash separated |
| custom pattern | varies | See custom tokens below |

```csharp
date.ToString("d")                        // 2079/02/06
date.ToString("D")                        // Jestha 06, 2079
date.ToString("s")                        // 2079-02-06
date.ToString("dd-MM-yyyy")               // 06-02-2079
date.ToString("MMMM dd, yyyy")            // Jestha 06, 2079
date.ToString("MMM yyyy")                 // Jes 2079
date.ToString("dd 'of' MMMM")            // 06 of Jestha
$"{date:s}"                               // 2079-02-06
string.Format("{0:yyyy/MM/dd}", date)     // 2079/02/06
```

#### Formatting and Display > Custom Format Tokens

| Token | Description | Example |
|---|---|---|
| `yyyy` | 4 digit year | 2079 |
| `yy` | 2 digit year | 79 |
| `MMMM` | Full month name | Jestha |
| `MMM` | 3 letter abbreviation | Jes |
| `MM` | Zero padded month | 02 |
| `M` | Month number | 2 |
| `dd` | Zero padded day | 06 |
| `d` | Day number | 6 |
| `'...'` | Literal text | 'of' renders as "of" |
| `\` | Escape next character | |

#### Formatting and Display > ISpanFormattable

On .NET 6+, NepaliDate implements `ISpanFormattable` for allocation free formatting into `Span<char>`.

```csharp
Span<char> buffer = stackalloc char[32];
bool success = date.TryFormat(buffer, out int charsWritten, "yyyy-MM-dd", null);
```

---

#### Smart Date Parsing > SmartDateParser.Parse

```csharp
NepaliDate d1 = SmartDateParser.Parse("15 Shrawan 2080");
NepaliDate d2 = SmartDateParser.Parse("Shrawan 15, 2080");
NepaliDate d3 = SmartDateParser.Parse("15 Saun 2080");        // alternate spelling
NepaliDate d4 = SmartDateParser.Parse("२०८०/०४/१५");          // Nepali digits
NepaliDate d5 = SmartDateParser.Parse("१५ श्रावण २०८०");      // full Nepali
```

**Signature:** `static NepaliDate Parse(string input)`
Parses using all strategies: standard numeric, Nepali Unicode digits, month name detection, ambiguous permutation. Throws `ArgumentNullException` for null input, `FormatException` for unparseable input.

#### Smart Date Parsing > SmartDateParser.TryParse

```csharp
if (SmartDateParser.TryParse("15 Shrawan 2080", out NepaliDate result))
    Console.WriteLine(result);
```

**Signature:** `static bool TryParse(string input, out NepaliDate result)`
Same heuristics without throwing.

#### Smart Date Parsing > String Extension Methods

```csharp
NepaliDate d1 = "15 Shrawan 2080".ToNepaliDate();

if ("15 Shrawan 2080".TryToNepaliDate(out NepaliDate d2))
    Console.WriteLine(d2);
```

| Method | Signature | Description |
|---|---|---|
| `ToNepaliDate()` | `static NepaliDate ToNepaliDate(this string input)` | Smart parses any supported format. Throws FormatException. |
| `TryToNepaliDate()` | `static bool TryToNepaliDate(this string input, out NepaliDate result)` | Non throwing variant. |

#### Smart Date Parsing > Auto Adjust Parsing

```csharp
NepaliDate p1 = NepaliDate.Parse("2077_05_25", autoAdjust: true);
// 2077/05/25

NepaliDate p2 = NepaliDate.Parse("25-05-077", autoAdjust: true);
// 2077/05/25 (year < 1000 expanded, day > 32 swapped)

NepaliDate p3 = NepaliDate.Parse("05/06/2077", autoAdjust: true);
// 2077/06/05 (month in middle)

NepaliDate p4 = NepaliDate.Parse("05/06/2077", autoAdjust: true, monthInMiddle: false);
// 2077/05/06
```

**Auto adjust rules applied in order:**
1. day > 32: swap year and day
2. monthInMiddle false: swap month and day first
3. month > 12 and day < 13: swap month and day
4. year < 1000: prepend current millennium (2000)

**TryParse variant:**
```csharp
bool ok = NepaliDate.TryParse("05/06/2077", out NepaliDate result, autoAdjust: true, monthInMiddle: true);
```

#### Smart Date Parsing > Supported Formats and Spellings

SmartDateParser recognizes:
- **Standard numeric:** `2080/04/15`, `2080-04-15`, `2080.04.15`, etc.
- **Nepali Unicode digits:** `२०८०/०४/१५`
- **Month names (100+ spellings):** Shrawan, Sawan, Saun, Srawan, श्रावण, साउन, etc. for all 12 months.
- **Mixed formats:** `15 Shrawan 2080`, `Shrawan 15, 2080`, `2080 Shrawan 15`
- **Optional suffixes:** B.S., V.S., गते, मिति (stripped before parsing)
- **2 digit year expansion:** 80 becomes 2080
- **Common separators:** `/`, `-`, `.`, `_`, `\`, space, pipe, Nepali pipe

---

#### Fiscal Year Operations > Instance Methods

Nepal's fiscal year runs from 1 Shrawan (month 4) to the last day of Ashadh (month 3) of the following year.

| Method | Returns | Description |
|---|---|---|
| `FiscalYearStartDate(int yearOffset = 0)` | `NepaliDate` | Start date (1 Shrawan) of the containing fiscal year. yearOffset shifts: 0 = current, 1 = next, -1 = previous. |
| `FiscalYearEndDate(int yearOffset = 0)` | `NepaliDate` | End date (last day of Ashadh) of the containing fiscal year. |
| `FiscalYearStartAndEndDate(int yearOffset = 0)` | `(NepaliDate, NepaliDate)` | Both start and end dates. |
| `FiscalYearQuarterStartDate(FiscalYearQuarters q = Current, int yearOffset = 0)` | `NepaliDate` | Start of specified fiscal quarter. |
| `FiscalYearQuarterEndDate(FiscalYearQuarters q = Current, int yearOffset = 0)` | `NepaliDate` | End of specified fiscal quarter. |
| `FiscalYearQuarterStartAndEndDate(FiscalYearQuarters q = Current, int yearOffset = 0)` | `(NepaliDate, NepaliDate)` | Both start and end of quarter. |

```csharp
var date = new NepaliDate("2081/04/15");  // Q1 of FY 2081

NepaliDate fyStart = date.FiscalYearStartDate();                // 2081/04/01
NepaliDate fyEnd   = date.FiscalYearEndDate();                  // 2082/03/last
var (s, e)         = date.FiscalYearStartAndEndDate();

NepaliDate qStart  = date.FiscalYearQuarterStartDate();         // 2081/04/01
NepaliDate qEnd    = date.FiscalYearQuarterEndDate();           // 2081/06/30
var (qs, qe)       = date.FiscalYearQuarterStartAndEndDate();

// Next fiscal year
NepaliDate nextFyStart = date.FiscalYearStartDate(yearOffset: 1); // 2082/04/01
```

#### Fiscal Year Operations > Static Methods

| Method | Returns | Description |
|---|---|---|
| `GetFiscalYearStartDate(int fiscalYear)` | `NepaliDate` | 1 Shrawan of specified FY. |
| `GetFiscalYearEndDate(int fiscalYear)` | `NepaliDate` | Last day of Ashadh of FY+1. |
| `GetFiscalYearStartAndEndDate(int fiscalYear)` | `(NepaliDate, NepaliDate)` | Both dates. |
| `GetFiscalYearQuarterStartDate(int fiscalYear, int month)` | `NepaliDate` | Quarter start. Month determines which quarter. |
| `GetFiscalYearQuarterEndDate(int fiscalYear, int month)` | `NepaliDate` | Quarter end. |
| `GetFiscalYearQuarterStartAndEndDate(int fiscalYear, int month)` | `(NepaliDate, NepaliDate)` | Both quarter dates. |

```csharp
NepaliDate start = NepaliDate.GetFiscalYearStartDate(2080);       // 2080/04/01
NepaliDate end   = NepaliDate.GetFiscalYearEndDate(2080);          // 2081/03/last
var (s, e)       = NepaliDate.GetFiscalYearStartAndEndDate(2080);

NepaliDate qS    = NepaliDate.GetFiscalYearQuarterStartDate(2080, 4);  // 2080/04/01
NepaliDate qE    = NepaliDate.GetFiscalYearQuarterEndDate(2080, 4);    // 2080/06/30

// Q4: month 1 of FY 2080 falls in calendar year 2081
var (q4s, q4e) = NepaliDate.GetFiscalYearQuarterStartAndEndDate(2080, 1);
// (2081/01/01, 2081/03/last)
```

#### Fiscal Year Operations > Quarters

| Quarter | Months | Month Range |
|---|---|---|
| Q1 (First) | Shrawan to Ashoj | 4 to 6 |
| Q2 (Second) | Kartik to Poush | 7 to 9 |
| Q3 (Third) | Magh to Chaitra | 10 to 12 |
| Q4 (Fourth) | Baishakh to Ashadh | 1 to 3 |

The `FiscalYearQuarters` enum:
```csharp
public enum FiscalYearQuarters
{
    Current = 0,    // resolved at runtime from the date's month
    First   = 4,    // Shrawan to Ashoj
    Second  = 7,    // Kartik to Poush
    Third   = 10,   // Magh to Chaitra
    Fourth  = 1,    // Baishakh to Ashadh
}
```

#### Fiscal Year Operations > Year Offset

The `yearOffset` parameter shifts the result relative to the containing fiscal year. `0` means current, `1` means next FY, `-1` means previous FY.

```csharp
var date = new NepaliDate("2081/04/15");

date.FiscalYearStartDate(yearOffset: 0);   // 2081/04/01 (current)
date.FiscalYearStartDate(yearOffset: 1);   // 2082/04/01 (next)
date.FiscalYearStartDate(yearOffset: -1);  // 2080/04/01 (previous)
```

---

#### Calendar Data > Tithi

```csharp
var date = new NepaliDate(2081, 4, 15);
string tithiNp = date.TithiNp;   // Nepali Devanagari tithi name
string tithiEn = date.TithiEn;   // English transliterated tithi name
```

Returns the lunar day (Tithi) for the date. Empty string for dates outside 2001 to 2089 BS.

#### Calendar Data > Public Holidays

```csharp
bool isHoliday = date.IsPublicHoliday;   // true if gazetted holiday
```

Based on cross-verified Bikram Sambat calendar data. Returns false for dates outside the data range.

#### Calendar Data > Events

```csharp
string[] eventsNp = date.EventsNp;   // event names in Nepali
string[] eventsEn = date.EventsEn;   // event names in English
```

Returns an empty (non-null) array when there are no events.

#### Calendar Data > GetCalendarInfo

```csharp
CalendarInfo info = date.GetCalendarInfo();
// info.TithiNp, info.TithiEn, info.IsPublicHoliday, info.EventsNp, info.EventsEn
```

**Signature:** `CalendarInfo GetCalendarInfo()`
Returns all calendar metadata in a single lookup. Prefer this over accessing individual properties when you need multiple pieces of calendar data.

**CalendarInfo struct:**

| Property | Type | Description |
|---|---|---|
| `TithiNp` | `string` | Tithi in Nepali Devanagari. |
| `TithiEn` | `string` | Tithi in English. |
| `IsPublicHoliday` | `bool` | Public holiday flag. |
| `EventsNp` | `string[]` | Event names in Nepali. |
| `EventsEn` | `string[]` | Event names in English. |

#### Calendar Data > Data Coverage

Calendar data (Tithi, holidays, events) is compiled from authoritative Bikram Sambat calendar references and cross-verified for accuracy, covering **2001 to 2089 BS**. All properties gracefully return empty or default values for dates outside this range without throwing exceptions.

---

#### Serialization > System.Text.Json

On .NET 5+, a `[JsonConverter]` is auto registered on NepaliDate, so basic serialization works without setup.

```csharp
using System.Text.Json;
using NepDate.Serialization;

// String format (default): "2080-04-15"
var opts = new JsonSerializerOptions().ConfigureForNepaliDate();

// Object format: {"Year":2080,"Month":4,"Day":15}
var optsObj = new JsonSerializerOptions().ConfigureForNepaliDate(useObjectFormat: true);

var date = new NepaliDate(2080, 4, 15);
string json  = JsonSerializer.Serialize(date, opts);         // "2080-04-15"
var restored = JsonSerializer.Deserialize<NepaliDate>(json, opts);
```

#### Serialization > Newtonsoft.Json

```csharp
using Newtonsoft.Json;
using NepDate.Serialization;

var settings    = new JsonSerializerSettings().ConfigureForNepaliDate();
var settingsObj = new JsonSerializerSettings().ConfigureForNepaliDate(useObjectFormat: true);

var date = new NepaliDate(2080, 4, 15);
string json = JsonConvert.SerializeObject(date, settings);         // "2080-04-15"
var back    = JsonConvert.DeserializeObject<NepaliDate>(json, settings);
```

#### Serialization > XML Serialization

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

var p = new PersonRecord
{
    Name = "Ram Sharma",
    ActualBirthDate = new NepaliDate(2040, 2, 15)
};
var serializer = new XmlSerializer(typeof(PersonRecord));
```

`NepaliDateXmlSerializer` implements `IXmlSerializable`. Required because NepaliDate is a struct without a parameterless constructor.

#### Serialization > String vs Object Format

| Mode | JSON Output | When to Use |
|---|---|---|
| String (default) | `"2080-04-15"` | APIs, compact payloads, human readable |
| Object | `{"Year":2080,"Month":4,"Day":15}` | When consumers need individual components |

Both modes accept either format on deserialization (string or object JSON), regardless of which mode was used for serialization.

#### Serialization > Configuration Extensions

| Method | Target | Description |
|---|---|---|
| `ConfigureForNepaliDate(this JsonSerializerOptions, bool useObjectFormat = false)` | System.Text.Json | Registers converter. Returns the options for chaining. |
| `ConfigureForNepaliDate(this JsonSerializerSettings, bool useObjectFormat = false)` | Newtonsoft.Json | Registers converter. Returns the settings for chaining. |

---

#### Type System Integration > IComparable

```csharp
int cmp = date1.CompareTo(date2);  // negative, zero, or positive
```

Implements `IComparable<NepaliDate>` and `IComparable`. Compares using the internal integer representation (YYYYMMDD).

#### Type System Integration > IEquatable

```csharp
bool eq = date1.Equals(date2);
int hash = date1.GetHashCode();
```

Implements `IEquatable<NepaliDate>`. Hash code is the internal YYYYMMDD integer.

#### Type System Integration > IFormattable

```csharp
string s = string.Format("{0:yyyy-MM-dd}", date);
string t = $"{date:MMMM dd, yyyy}";
```

Enables format string support in interpolation, `string.Format`, and any `IFormattable` consumer.

#### Type System Integration > ISpanFormattable

Available on .NET 6+. Enables allocation free formatting.

```csharp
Span<char> buf = stackalloc char[32];
date.TryFormat(buf, out int written, "yyyy-MM-dd", null);
```

#### Type System Integration > IParsable and ISpanParsable

Available on .NET 7+. Enables generic parsing in contexts that accept `IParsable<T>`.

```csharp
var d = NepaliDate.Parse("2080/04/15", null);  // IParsable<NepaliDate>.Parse
bool ok = NepaliDate.TryParse("2080/04/15", null, out var result);
```

#### Type System Integration > TypeConverter

`NepaliDateTypeConverter` is auto registered via `[TypeConverter]` attribute. Enables:
- ASP.NET MVC/Web API model binding
- WPF and WinForms data binding and property grids
- Generic `TypeDescriptor.GetConverter()` usage

Supports conversion to/from `string`, `int` (YYYYMMDD format), and `DateTime`.

#### Type System Integration > JsonConverter Auto Registration

On .NET 5+, `[JsonConverter(typeof(NepaliDateJsonConverter))]` is applied to the struct. System.Text.Json serialization works without any configuration for the default string format.

---

#### Enums > NepaliMonths

```csharp
public enum NepaliMonths
{
    Baishakh = 1,    // mid April to mid May
    Jestha   = 2,    // mid May to mid June
    Ashad    = 3,    // mid June to mid July
    Shrawan  = 4,    // mid July to mid August
    Bhadra   = 5,    // mid August to mid September
    Ashoj    = 6,    // mid September to mid October
    Kartik   = 7,    // mid October to mid November
    Mangsir  = 8,    // mid November to mid December
    Poush    = 9,    // mid December to mid January
    Magh     = 10,   // mid January to mid February
    Falgun   = 11,   // mid February to mid March
    Chaitra  = 12,   // mid March to mid April
}
```

#### Enums > DateFormats

```csharp
public enum DateFormats
{
    YearMonthDay = 0,   // 2081/01/15
    YearDayMonth = 1,   // 2081/15/01
    MonthYearDay = 2,   // 01/2081/15
    MonthDayYear = 3,   // 01/15/2081
    DayYearMonth = 4,   // 15/2081/01
    DayMonthYear = 5,   // 15/01/2081
}
```

#### Enums > Separators

```csharp
public enum Separators
{
    ForwardSlash  = 0,   // /
    BackwardSlash = 1,   // \
    Dot           = 2,   // .
    Underscore    = 3,   // _
    Dash          = 4,   // -
    Space         = 5,   //
}
```

#### Enums > FiscalYearQuarters

```csharp
public enum FiscalYearQuarters
{
    Current = 0,    // resolved at runtime
    First   = 4,    // Shrawan to Ashoj (months 4 to 6)
    Second  = 7,    // Kartik to Poush (months 7 to 9)
    Third   = 10,   // Magh to Chaitra (months 10 to 12)
    Fourth  = 1,    // Baishakh to Ashadh (months 1 to 3)
}
```

---

#### Exceptions > InvalidNepaliDateFormatException

```csharp
public sealed class InvalidNepaliDateFormatException : FormatException
{
    public InvalidNepaliDateFormatException(string message = "Invalid Nepali date format")
        : base(message) { }
}
```

Thrown when a string or date components cannot be interpreted as a valid Nepali date, or when a formatting operation receives an unrecognized format specifier.

---

#### Advanced > Boundary Values (MinValue, MaxValue)

```csharp
NepaliDate min = NepaliDate.MinValue;   // 1901/01/01 BS (approx 1844-04-13 AD)
NepaliDate max = NepaliDate.MaxValue;   // last day of 2199/12 BS (approx 2143-04-12 AD)
```

#### Advanced > Default Detection (IsDefault)

```csharp
NepaliDate unset = default;
bool isEmpty = unset.IsDefault;          // true

NepaliDate real = new NepaliDate(2080, 1, 1);
bool notEmpty = real.IsDefault;          // false
```

`IsDefault` identifies an uninitialized struct (all components zero). Use this to check for "no value" scenarios instead of nullable.

#### Advanced > Leap Year Check

```csharp
bool isLeap = date.IsLeapYear();   // checks the Gregorian equivalent year
```

Returns whether the Gregorian year of the date's English equivalent is a leap year.

#### Advanced > Relative Date Checks (IsToday, IsYesterday, IsTomorrow)

```csharp
bool today     = date.IsToday();
bool yesterday = date.IsYesterday();
bool tomorrow  = date.IsTomorrow();
```

Compare against the system clock's current date.

#### Advanced > Deconstruction

```csharp
var (year, month, day) = date;   // C# 7+ deconstruction
```

**Signature:** `void Deconstruct(out int year, out int month, out int day)`

#### Advanced > Struct Design Notes

NepaliDate is a `readonly partial struct`, which means:
- Instances live on the stack, not the heap.
- No garbage collection pressure, even at high volume.
- Immutable: all operations return new instances.
- Safe for concurrent access without synchronization.
- Behaves like built in value types (`DateTime`, `int`, `decimal`).

---

## Page 3: Changelog Page (`changelog.html`)

### Layout
- Same header and footer as other pages.
- Single column, centered content (max 800px).
- Title: "Changelog"
- Subtitle: "All notable changes to NepDate are documented here."

### Content
- A timeline style layout showing releases.
- For each release, show: version number, date, and change items grouped by Added/Changed/Fixed/Breaking.
- Fetch the releases from: `https://github.com/rajuprasai/NepDate/releases`
- Since this is a static site, include a prominent link: "View all releases on GitHub" that opens the releases page.
- Manually include these known releases as static content (the generator should include placeholder entries that match this format):

**v2.0.5** (Latest)
- Calendar data pipeline improvements
- Date range operations (NepaliDateRange)
- Smart date parser
- Fiscal year quarter operations
- Bulk conversion with batch processing
- Serialization support (System.Text.Json, Newtonsoft.Json, XML)
- IFormattable and ISpanFormattable
- TypeConverter
- Auto-registered JsonConverter on .NET 5+

**v1.x** (Previous)
- Core BS/AD conversion
- Date arithmetic
- Basic formatting
- Calendar metadata

Add a note: "For the complete release history with detailed changelogs, visit the GitHub Releases page."

---

## CSS Requirements

### General
- CSS custom properties (variables) for all colors, making theme switching trivial.
- `:root` for light theme, `[data-theme="dark"]` for dark theme.
- Smooth transitions on theme switch.
- Box-sizing border-box globally.
- Scrollbar styling for webkit browsers (subtle, thin).

### Code Blocks
- Dark background in both themes (code always looks best on dark).
- Rounded corners (8px).
- Padding 16px 20px.
- Position relative (for copy button).
- Overflow-x auto with styled scrollbar.
- Line numbers are not needed.

### Tables
- Striped rows for readability.
- Sticky header on scroll.
- Responsive: horizontal scroll on mobile.
- Borders consistent with theme.

### Sidebar (docs page)
- Fixed position, height 100vh minus header.
- Scrollable with thin scrollbar.
- 260px width on desktop.
- Nested items indented.
- Active item highlighted with accent color left border.
- Collapsible section groups (click parent to toggle children).

### Cards (features)
- Subtle border or shadow.
- Hover effect (slight lift or glow).
- Consistent padding and spacing.

### Responsive Breakpoints
- Desktop: > 1024px (full layout with sidebar, right nav)
- Tablet: 768px to 1024px (sidebar off-canvas, no right nav)
- Mobile: < 768px (hamburger menu, single column, full width code blocks)

---

## JavaScript Requirements

### Theme Toggle
- Check `localStorage` first, then `prefers-color-scheme`.
- Toggle sets `data-theme` attribute on `<html>`.
- Update icon (sun/moon).
- Save to `localStorage`.

### Mobile Menu
- Hamburger icon toggles a slide-in navigation panel.
- Close on outside click or Escape key.
- Body scroll lock when open.

### Code Syntax Highlighting
- Run on DOMContentLoaded.
- Find all `<code>` blocks with class `language-csharp` or `language-bash`.
- Apply regex-based tokenization for C# keywords, types, strings, comments, numbers.
- Wrap tokens in `<span>` with appropriate CSS classes.

### Copy to Clipboard
- Add a "Copy" button to each `<pre>` block.
- On click, copy raw text content (without HTML tags).
- Show brief "Copied!" feedback that fades after 2 seconds.

### Search (docs page)
- Build an index on page load from all heading elements and their parent section text.
- Filter sidebar items as user types.
- Debounce input (300ms).
- Highlight matching text in sidebar.
- Click result to scroll to section.
- Show "No results" message when nothing matches.
- Keyboard shortcut: Ctrl+K or / to focus search.

### Sidebar Navigation (docs page)
- Intersection Observer to detect which section is in view.
- Update active state in sidebar accordingly.
- Smooth scroll on click.
- Collapsible groups: parent items toggle visibility of children.
- Remember collapsed state in sessionStorage.

### Right Side Navigation (docs page, desktop only)
- Shows H3 headings within the currently active section.
- Updates as user scrolls.
- Clicking scrolls to the heading.

### Typing Animation (landing page hero)
- Type code line by line with realistic speed variation.
- Blinking cursor.
- After typing completes, pause, then optionally loop or stay static.
- Use `requestAnimationFrame` for smooth animation.

### FAQ Accordion (landing page)
- Click question to expand/collapse answer.
- Only one open at a time (optional, but recommended).
- Smooth height animation.
- Chevron icon rotates on open.

---

## Accessibility Requirements
- All interactive elements must be keyboard accessible.
- Proper `aria-` attributes on toggles, accordions, and navigation.
- Skip to content link.
- Sufficient color contrast in both themes (WCAG AA).
- Focus visible indicators.
- Semantic HTML (header, nav, main, aside, footer, section, article).

---

## SEO
- Proper `<title>` tags per page.
- Meta description per page.
- Open Graph meta tags (title, description, image, url).
- `<link rel="canonical">` on each page.
- Structured data (JSON-LD) for software application.

---

## File Size and Performance
- No external CSS or JS frameworks.
- Total site weight should be under 100KB excluding images.
- Inline critical CSS if possible, or keep one CSS file.
- One JS file, or split per page if beneficial.
- Lazy load non-critical animations.
- `defer` attribute on script tags.

---

## Important Notes for the Generator

1. Every code example must be wrapped in `<pre><code class="language-csharp">` or `<code class="language-bash">` for syntax highlighting to work.
2. All internal links must account for the GitHub Pages base path (`/NepDate/`). Use relative links where possible.
3. The site must work correctly when served from `https://rajuprasai.github.io/NepDate/`.
4. Do not use any CDN links or external dependencies. Everything must be self-contained.
5. The documentation page is the most critical page. Every single API method, property, enum value, and code example listed above MUST appear on the documentation page. Do not summarize, truncate, or skip anything.
6. Use the NepDate logo from: `https://user-images.githubusercontent.com/37014558/231635618-bf6599e3-554e-4b02-93df-019e7b8aecc3.png`
7. Test that dark/light toggle works, code copy works, search filters correctly, sidebar navigation scrolls properly, and mobile hamburger menu opens/closes.
8. The sponsor button color and style should use the accent color but in a subtle, non-intrusive way.
9. Google Analytics placeholder `G-XXXXXXXXXX` must be present in all pages with a code comment for replacement.
