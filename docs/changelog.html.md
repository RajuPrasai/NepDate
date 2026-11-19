# NepDate Changelog

https://nepdate.rajuprasai.com.np/changelog.html

---

## v2.1.0 - November 2026 (Latest)

Faster core, a fix for .NET 8 apps, and corrected docs. No public API was added, removed or changed.

- Conversion, date arithmetic and parsing are 3.6 to 10 times faster, and `ToString()` is about 15% faster (BenchmarkDotNet on .NET 10; full table in the README). A `NepaliDate` now takes 8 bytes instead of 32, and the assembly is about half the size.
- Fix: on .NET 8, serializing a `NepaliDate` with System.Text.Json could fail with `FileNotFoundException` for System.Text.Json 10.0.0.0. The net8.0 build now uses the runtime's System.Text.Json. The netstandard2.0 build needs System.Text.Json 8.0.5 or later when you call the JSON helpers.
- Fix: the event on 1 Baisakh 2084 is now "New Year 2084" (it said 2083).
- Fix: range operations at the end of the supported dates, such as the fiscal quarters of 2199, no longer throw.
- Fix: `GetFiscalYearQuarterStartDate`, `GetFiscalYearQuarterEndDate` and `GetFiscalYearQuarterStartAndEndDate` now treat months 1-3 as part of the given fiscal year. `GetFiscalYearQuarterStartAndEndDate(2080, 1)` returns 2081/01/01 to 2081/03/31 (it returned the fourth quarter of fiscal year 2079). Calls with months 4-12 are unchanged.
- Behaviour change: `EnglishDate` always has `DateTimeKind.Unspecified`. It used to keep the kind of the `DateTime` the date was built from.
- Behaviour change: `BulkConvert` converts the whole collection when you call it, on the calling thread. An invalid date now throws from the call instead of while you enumerate the result. `useParallel` and `batchSize` are kept for source compatibility and have no effect.
- The package now ships XML documentation and a symbol package with Source Link, and no longer adds `nuget-icon.ico` to your project.
- Docs: calendar data coverage corrected to the years the data really has (tithi for 2078-2083 BS, events and holidays for 2077-2083 BS, plus a few days of 2084), and members that never existed were removed from the API reference. The docs now say that automatic System.Text.Json support, `ISpanFormattable` and `IParsable` come with the net8.0 build (.NET 8 and later); .NET 5 to 7 apps use the netstandard2.0 build.

---

## v2.0.7 - May 2026

Corrected English transliterations of Tithi names and calendar events. Improved accuracy and consistency of Nepali calendar metadata for 2001-2089 BS.

---

## v2.0.6 - November 2025

Resolved a NuGet publish conflict from v2.0.5 and stabilized the CI release workflow. No API changes.

---

## v2.0.5 - October 2025

Major feature release:
- Calendar metadata: Tithi (lunar day), Nepal government gazetted public holidays, and festival events for 2001-2089 BS
- `NepaliDateRange` struct with `Contains`, `Overlaps`, `IsContiguousWith`, `Union`, `Intersect`, `Except`
- Fiscal year helpers: `FiscalYearStartDate`, `FiscalYearEndDate`, `FiscalYearQuarter`
- Smart parsing: 100+ month-name spellings, Nepali Unicode digits, flexible separators
- `BulkConvert` static class with auto-parallelized `ToNepaliDates` and `ToEnglishDates`
- `CalendarInfo.GetForMonth` for efficient batch calendar data retrieval
- `System.Text.Json` serialization via `[JsonConverter]` attribute (automatic)
- Newtonsoft.Json serialization via `NepDateNewtonsoftConverter`
- XML serialization via `IXmlSerializable`
- `IParsable<NepaliDate>` and `ISpanFormattable` on net8.0 target
- Significant performance overhaul: BS-to-AD at 4.55 ns/op, zero allocations

---

## v2.0.0 - 2024

Breaking release:
- Restructured `NepaliDate` into a `readonly partial struct`
- Removed mutable state
- Changed namespace from `NepDate.Core` to `NepDate`
- Dropped legacy `DateConverter` class (replaced by constructor and `NepaliDate.Now`)
- All arithmetic now returns new instances (immutable)

---

## v1.x - 2023

Initial public releases. Core BS/AD conversion, basic formatting, NuGet publish infrastructure.

---

## Links

- [Homepage](https://nepdate.rajuprasai.com.np/)
- [API Reference](https://nepdate.rajuprasai.com.np/docs.html)
- [NuGet](https://www.nuget.org/packages/NepDate/)
- [GitHub Releases](https://github.com/RajuPrasai/NepDate/releases)
