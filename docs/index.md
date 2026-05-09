# Horae

Horae is a modern, minimalistic .NET library for elegant and precise time utilities.

It provides syntactic sugar such as `1.January(2026)`, robust conversions between
`DateTimeOffset`, `DateOnly`, and `DateTime`, DST-safe interval operations, and
ISO 8601 serialization with `System.Text.Json`.

**License:** Apache-2.0 &nbsp;·&nbsp; **Target frameworks:** net8.0 · net9.0 · net10.0 &nbsp;·&nbsp;
[GitHub](https://github.com/GravionLabs/Horae)

---

## Install

```
dotnet add package Horae
```

---

## Quick start

```csharp
using Horae;

// Fluent date construction
DateTimeOffset release  = 1.January(2026);          // midnight UTC
DateTimeOffset meeting  = 15.March(2026).At(9, 30); // 09:30 UTC

// DST-safe interval for "today" in Europe/Berlin
TimeInterval today = TimeInterval.Day(DayOffset.Today.Resolve(TimeZones.CentralEuropean),
                                      TimeZones.CentralEuropean);

// ISO 8601 round-trip
string iso    = DateTimeOffset.UtcNow.ToIsoString();
DateTimeOffset back = iso.ToDateTimeOffset();
```

For the full API, see the **[API Reference](../api/index.md)**.

For usage examples and all features, see the
**[README on GitHub](https://github.com/GravionLabs/Horae#readme)**.
