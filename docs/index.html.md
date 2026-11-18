# NepDate - Bikram Sambat .NET Library

> The fastest zero-dependency .NET library for Bikram Sambat (BS) date conversion, smart parsing, fiscal year operations, calendar metadata, and more.

**Install:** `dotnet add package NepDate`
**Version:** 2.0.7 | **License:** MIT
**NuGet:** https://www.nuget.org/packages/NepDate/
**GitHub:** https://github.com/RajuPrasai/NepDate

---

## Performance

| Operation | Time | Allocations |
|---|---|---|
| BS to AD conversion | 4.55 ns/op | 0 bytes |
| AD to BS conversion | 4.55 ns/op | 0 bytes |
| String formatting | ~12 ns/op | 0 bytes |
| Smart parse | ~18 ns/op | 0 bytes |

Benchmarked with BenchmarkDotNet on .NET 8 (Apple M1). `NepaliDate` is a `readonly partial struct` with O(1) calendar table lookups.

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
<PackageReference Include="NepDate" Version="2.0.7" />
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
var d = NepaliDate.Parse("2080 Ashwin 15");
Console.WriteLine(d); // "2080-06-15"
```

---

## Features

- **BS/AD conversion** - bidirectional, 4.55 ns/op, zero allocations
- **Smart parsing** - 100+ month-name spellings, Nepali Unicode digits, any separator
- **Rich formatting** - 6 format patterns, 6 separators, Nepali numeral output
- **Fiscal year** - Nepali fiscal year start/end dates, quarter detection
- **Date ranges** - `NepaliDateRange` with union, intersect, except, contains
- **Calendar metadata** - tithi, public holidays, festivals for 2001-2089 BS
- **Bulk conversion** - `BulkConvert` converts whole collections in one pass
- **Serialization** - System.Text.Json (automatic), Newtonsoft.Json, XML
- **ASP.NET binding** - `TypeConverter` for route params, query strings, form fields
- **Zero dependencies** - Newtonsoft and STJ support internalized via PrivateAssets
- **Broad compatibility** - .NET Standard 2.0, .NET 5-10, Framework 4.6.1+, Xamarin, MAUI, Unity

---

## FAQ

**What .NET versions does NepDate support?**
.NET Standard 2.0 covers .NET Framework 4.6.1+, .NET Core 2.0+, .NET 5/6/7/8/9, Xamarin, Unity, MAUI. A net8.0 target adds `IParsable` and `ISpanFormattable`.

**What is the supported date range?**
1901/01/01 BS through 2199/12/last-day BS. Calendar metadata only for 2001-2089 BS.

**Does NepDate have external dependencies?**
No. Newtonsoft.Json and System.Text.Json are internalized with `PrivateAssets=all`.

**Is NepDate thread-safe?**
Yes. `NepaliDate` is an immutable readonly struct. `BulkConvert` is safe to call from several threads.

**How does NepDate handle invalid dates?**
Constructors and `Parse` throw typed exceptions. Use `TryParse` for non-throwing validation.

**Can I use NepDate with ASP.NET model binding?**
Yes. The registered `TypeConverter` enables automatic binding from route params, query strings, and form fields.

**How is calendar data sourced?**
Compiled from authoritative BS calendar references, covering 2001-2089 BS. Outside this range properties return empty string without throwing.

**What is the advantage of the struct design?**
Stack allocation eliminates GC pressure, making it ideal for bulk date processing, large LINQ pipelines, and server-side batch jobs.

---

## Links

- [API Reference](https://nepdate.rajuprasai.com.np/docs.html) - every method, property, enum, exception
- [Changelog](https://nepdate.rajuprasai.com.np/changelog.html) - version history
- [NuGet](https://www.nuget.org/packages/NepDate/)
- [GitHub](https://github.com/RajuPrasai/NepDate)
- [NepDate Widget](https://nepdatewidget.rajuprasai.com.np/) - Windows desktop app using this library
