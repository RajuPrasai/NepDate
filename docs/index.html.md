# NepDate - Bikram Sambat .NET Library

> The fastest zero-dependency .NET library for Bikram Sambat (BS) date conversion, smart parsing, fiscal year operations, calendar metadata, and more.

**Install:** `dotnet add package NepDate`
**Version:** 2.1.0 | **License:** MIT
**NuGet:** https://www.nuget.org/packages/NepDate/
**GitHub:** https://github.com/RajuPrasai/NepDate

---

## Performance

| Operation | NepDate 2.1.0 | Allocated | Next fastest library |
|---|---|---|---|
| BS to AD conversion | 0.78 ns | 0 bytes | NepaliCalendarBS 14,772 ns |
| AD to BS conversion | 2.73 ns | 0 bytes | NepaliCalendarBS 183 ns |
| `ToString()` | 11.3 ns | 48 bytes (the result string) | |
| Strict parse | 5.5 ns | 0 bytes | |

BenchmarkDotNet 0.15.8, .NET 10.0.12, Intel Core i5-12500H, Windows 11, one pinned core. Each call converts a different date from a pool of 1,024 (2070-2085 BS), so nothing can be precomputed. `NepaliDate` is an 8-byte `readonly partial struct` with O(1) calendar table lookups.

---

## What's new in 2.1.0

- 3.6 to 10 times faster than 2.0.7 for conversion, arithmetic and parsing; `NepaliDate` is 8 bytes instead of 32
- System.Text.Json serialization works on .NET 8 (2.0.7 could fail with FileNotFoundException)
- `GetFiscalYearQuarter...(fiscalYear, month)` treats months 1-3 as part of the given fiscal year
- XML docs and a symbol package with Source Link
- `EnglishDate` always has `DateTimeKind.Unspecified`; `BulkConvert` converts the whole collection on the calling thread

Full notes: https://nepdate.rajuprasai.com.np/changelog.html

---

## Installation

**dotnet CLI:**
```
dotnet add package NepDate
```

**Package Manager Console:**
```
Install-Package NepDate
```

**PackageReference:**
```xml
<PackageReference Include="NepDate" Version="2.1.0" />
```

---

## Quick Start

```csharp
using NepDate;

// Today in BS
var today = NepaliDate.Now;
Console.WriteLine($"Today in BS: {today}");

// BS to AD
var bs = new NepaliDate(2080, 6, 15);
DateTime ad = bs.EnglishDate;
Console.WriteLine($"{bs} = {ad:yyyy-MM-dd}");

// AD to BS
var bs2 = new NepaliDate(new DateTime(2024, 1, 15));
Console.WriteLine($"2024-01-15 = {bs2}");

// Parse from string
var d = NepaliDate.Parse("2080-06-15");            // strict: year, month, day
var s = SmartDateParser.Parse("2080 Ashwin 15");   // month names, Nepali text
Console.WriteLine(s); // "2080/06/15"
```

---

## Features

- **BS/AD conversion** - bidirectional, under 1 ns BS to AD and under 3 ns AD to BS, zero allocations
- **Smart parsing** - 100+ month-name spellings, Nepali Unicode digits, any separator
- **Rich formatting** - 6 format patterns, 6 separators, Nepali numeral output
- **Fiscal year** - Nepali fiscal year start/end dates, quarter detection
- **Date ranges** - `NepaliDateRange` with union, intersect, except, contains
- **Calendar metadata** - tithi for 2078-2083 BS, public holidays and festivals for 2077-2083 BS (plus a few days of 2084)
- **Bulk conversion** - `BulkConvert` converts whole collections in one pass
- **Serialization** - System.Text.Json (automatic on .NET 8+), Newtonsoft.Json, XML
- **ASP.NET binding** - `TypeConverter` for route params, query strings, form fields
- **No NuGet dependencies** - Newtonsoft.Json and System.Text.Json are referenced with PrivateAssets=all; to call `ConfigureForNepaliDate()` reference Newtonsoft.Json, plus System.Text.Json 8.0.5+ outside .NET 8
- **Broad compatibility** - .NET Standard 2.0, .NET 5-10, Framework 4.6.1+, Xamarin, MAUI, Unity

---

## FAQ

**What .NET versions does NepDate support?**
.NET Standard 2.0 covers .NET Framework 4.6.1+, .NET Core 2.0+, .NET 5 and later, Xamarin, Unity, MAUI. A net8.0 target adds `IParsable` and `ISpanFormattable`.

**What is the supported date range?**
1901/01/01 BS through 2199/12/last-day BS. Calendar data covers fewer years: tithi for 2078-2083 BS, events and holidays for 2077-2083 BS (plus a few days of 2084).

**Does NepDate have external dependencies?**
No. Newtonsoft.Json and System.Text.Json are referenced with `PrivateAssets=all`, so they are not added to your project. `ConfigureForNepaliDate()` needs Newtonsoft.Json referenced in your project, because both overloads share the name, and on netstandard2.0 targets such as .NET Framework also System.Text.Json 8.0.5 or later. Adding a converter from `SystemTextJsonConverters` or `NewtonsoftJsonConverters` directly needs only that library.

**Is NepDate thread-safe?**
Yes. `NepaliDate` is an immutable readonly struct. `BulkConvert` is safe to call from several threads.

**How does NepDate handle invalid dates?**
Constructors and `Parse` throw typed exceptions. Use `TryParse` for non-throwing validation.

**Can I use NepDate with ASP.NET model binding?**
Yes. The registered `TypeConverter` enables automatic binding from route params, query strings, and form fields.

**How is calendar data sourced?**
Compiled from authoritative BS calendar references: tithi for 2078-2083 BS, events and holidays for 2077-2083 BS (plus a few days of 2084). Other dates return empty values without throwing.

**What is the advantage of the struct design?**
Stack allocation eliminates GC pressure, making it ideal for bulk date processing, large LINQ pipelines, and server-side batch jobs.

---

## Links

- [API Reference](https://nepdate.rajuprasai.com.np/docs.html) - every method, property, enum, exception
- [Changelog](https://nepdate.rajuprasai.com.np/changelog.html) - version history
- [NuGet](https://www.nuget.org/packages/NepDate/)
- [GitHub](https://github.com/RajuPrasai/NepDate)
- [NepDate Widget](https://nepdatewidget.rajuprasai.com.np/) - Windows desktop app using this library
