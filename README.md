# Horae

Horae is a modern, minimalistic .NET library for elegant and precise time utilities.
It provides syntactic sugar such as `1.January(2026)`, robust conversions between `DateTimeOffset`, `DateOnly`, and `DateTime`, DST-safe interval operations, and ISO 8601 serialization with `System.Text.Json`.

License: Apache-2.0 · Target frameworks: net8.0 · net9.0 · net10.0

---

## Table of Contents

- [Configuration](#configuration)
- [Date Construction](#date-construction)
- [TimeSpan Construction](#timespan-construction)
- [DateTimeOffset Operations](#datetimeoffset-operations)
- [Conversions](#conversions)
- [Regional Time Zones](#regional-time-zones)
- [ISO 8601 Formatting & Parsing](#iso-8601-formatting--parsing)
- [Duration](#resolution)
- [TimeInterval](#timeinterval)
- [JSON Converters](#json-converters)
- [Time Zone Constants](#time-zone-constants)

---

## Configuration

`HoareSettings.CurrentTimeZone` sets the default time zone used by all methods with an optional `TimeZoneInfo?` parameter when `null` is passed. The default value is `TimeZoneInfo.Local`.

```csharp
// Set once during application startup (not thread-safe)
HoareSettings.CurrentTimeZone = TimeZones.CentralEuropean;
```

---

## Date Construction

Fluent methods on `int` for readable date values:

```csharp
DateTimeOffset d1 = 1.January(2026);         // 2026-01-01T00:00:00+00:00
DateTimeOffset d2 = 15.June(2026);           // 2026-06-15T00:00:00+00:00

// Add a time of day (preserves the base UTC offset)
DateTimeOffset d3 = 1.January(2026).At(14, 30);       // 2026-01-01T14:30:00+00:00
DateTimeOffset d4 = 1.January(2026).At(TimeSpan.FromHours(9));

// Relative arithmetic
DateTimeOffset d5 = 2.Hours().After(1.January(2026));
DateTimeOffset d6 = 30.Minutes().Before(1.January(2026));

// Nanosecond support (100 ns resolution)
DateTimeOffset d7 = 1.January(2026).AddNanoseconds(500);
```

---

## TimeSpan Construction

Fluent methods on `int`, `long`, and `double`:

```csharp
TimeSpan t1 = 15.Minutes();
TimeSpan t2 = 1.Hours();
TimeSpan t3 = 2.Days();
TimeSpan t4 = 500.Milliseconds();
TimeSpan t5 = 100.Microseconds();
TimeSpan t6 = 1000.Nanoseconds();   // precision below 100 ns is lost

// Chain with .And()
TimeSpan t7 = 1.Hours().And(30.Minutes());   // 1:30:00

// Offset overloads
TimeSpan t8 = 2.Hours(30.Minutes());         // 2:30:00
```

---

## DateTimeOffset Operations

### Day boundaries

All methods accept an optional `TimeZoneInfo?` (default: `HoareSettings.CurrentTimeZone`).

```csharp
var now = DateTimeOffset.UtcNow;

DateTimeOffset start  = now.StartOfDay(TimeZones.CentralEuropean);    // midnight in CET/CEST
DateTimeOffset startU = now.StartOfDayUtc(TimeZones.CentralEuropean); // same instant, UTC offset

DateTimeOffset end    = now.EndOfDay(TimeZones.CentralEuropean);      // next midnight (exclusive)
DateTimeOffset endU   = now.EndOfDayUtc(TimeZones.CentralEuropean);

// Intraday session (= current day)
DateTimeOffset is = now.StartOfIntraday();
DateTimeOffset ie = now.EndOfIntraday();

// Day-ahead session (= next calendar day)
DateTimeOffset das = now.StartOfDayAhead();
DateTimeOffset dae = now.EndOfDayAhead();
```

### Rounding

```csharp
DateTimeOffset r1 = now.RoundDown(Duration.QuarterHour);              // round down to 15 minutes
DateTimeOffset r2 = now.RoundUp(TimeSpan.FromHours(1));                 // round up to the hour
DateTimeOffset r3 = now.RoundToNearest(Duration.HalfHour, TimeZones.CentralEuropean);
```

### Change time zone

```csharp
DateTimeOffset cest = now.ToTimeZone(TimeZones.CentralEuropean);        // same instant, different offset
```

---

## Conversions

```csharp
var dto  = new DateTimeOffset(2026, 6, 15, 12, 0, 0, TimeSpan.Zero);
var date = new DateOnly(2026, 6, 15);

// DateTimeOffset → DateOnly
DateOnly d1 = dto.AsDateOnly(TimeZones.Japan);    // calendar day in JST
DateOnly d2 = dto.AsUtcDateOnly();                // calendar day in UTC

// DateTimeOffset → DateTime
DateTime t1 = dto.AsDateTime(TimeZones.CentralEuropean); // DateTimeKind.Unspecified
DateTime t2 = dto.AsUtcDateTime();                        // DateTimeKind.Utc

// DateOnly → DateTimeOffset
DateTimeOffset m1 = date.AsDateTimeOffset(TimeZones.CentralEuropean); // midnight in CET/CEST
DateTimeOffset m2 = date.AsUtcDateTimeOffset();                        // midnight UTC
```

---

## Regional Time Zones

Converts a `DateTimeOffset` value into a specific time zone. The returned value represents the same instant with the correct regional offset, including DST.

```csharp
var utc = new DateTimeOffset(2026, 6, 15, 12, 0, 0, TimeSpan.Zero);

DateTimeOffset cet  = utc.ToCentralEuropeanTime();  // 14:00 +02:00 (CEST)
DateTimeOffset lon  = utc.ToLondonTime();           // 13:00 +01:00 (BST)
DateTimeOffset lis  = utc.ToLisbonTime();           // 13:00 +01:00 (WEST)
DateTimeOffset eet  = utc.ToEasternEuropeTime();    // 15:00 +03:00 (EEST)
DateTimeOffset msk  = utc.ToMoscowTime();           // 15:00 +03:00 (MSK)
DateTimeOffset ist  = utc.ToIstanbulTime();         // 15:00 +03:00 (TRT)
DateTimeOffset est  = utc.ToEasternUsTime();        //  8:00 -04:00 (EDT)
DateTimeOffset pst  = utc.ToPacificUsTime();        //  5:00 -07:00 (PDT)
DateTimeOffset gst  = utc.ToDubaiTime();            // 16:00 +04:00 (GST)
DateTimeOffset ind  = utc.ToIndiaTime();            // 17:30 +05:30 (IST)
DateTimeOffset jst  = utc.ToJapanTime();            // 21:00 +09:00 (JST)
DateTimeOffset aest = utc.ToAustraliaEasternTime(); // 22:00 +10:00 (AEST)

// Back to UTC
DateTimeOffset back = cet.ToUtc();                  // 12:00 +00:00
```

---

## ISO 8601 Formatting & Parsing

### Formatting

```csharp
var dto  = new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero);
var dto2 = new DateTimeOffset(2026, 1, 1, 23, 0, 0, 500, TimeSpan.Zero); // 500 ms
var date = new DateOnly(2026, 6, 15);

dto.ToIsoString();    // "2026-01-01T23:00:00Z"   (UTC, without sub-seconds when not needed)
dto2.ToIsoString();   // "2026-01-01T23:00:00.5Z" (sub-seconds only when ≠ 0)
date.ToIsoString();   // "2026-06-15"

// Nullable overloads
DateTimeOffset? nullDto  = null;
DateOnly?       nullDate = null;
nullDto.ToIsoString();   // null
nullDate.ToIsoString();  // null
```

### Parsing

```csharp
// DateTimeOffset – accepts "Z" and "+HH:mm" offsets
DateTimeOffset dto1 = "2026-01-01T23:00:00Z".ParseIsoDateTime();
DateTimeOffset dto2 = "2026-01-01T23:00:00+01:00".ParseIsoDateTime();

bool ok = "2026-01-01T23:00:00Z".TryParseIsoDateTime(out DateTimeOffset result);

// DateOnly – strict "yyyy-MM-dd"
DateOnly date = "2026-06-15".ParseIsoDate();
bool ok2 = "2026-06-15".TryParseIsoDate(out DateOnly dateResult);
```

---

## Duration

`Duration` represents an ISO 8601 duration (positive `TimeSpan`) with named presets and automatic JSON serialization.

```csharp
// Predefined instances
Duration r = Duration.Minute;      // PT1M
Duration r = Duration.FiveMinutes; // PT5M
Duration r = Duration.QuarterHour; // PT15M
Duration r = Duration.HalfHour;    // PT30M
Duration r = Duration.Hour;        // PT1H
Duration r = Duration.Day;         // P1D

// Custom values
var r2 = new Duration(TimeSpan.FromMinutes(10));
var r3 = Duration.Parse("PT10M");
bool ok = Duration.TryParse("P1D", out Duration daily);

// To/from TimeSpan (implicit)
TimeSpan ts = Duration.QuarterHour;           // 00:15:00
Duration res = TimeSpan.FromHours(1);         // equivalent to Duration.Hour

// ToString → ISO 8601
Duration.QuarterHour.ToString(); // "PT15M"
Duration.Day.ToString();         // "P1D"
```

### JSON Serialization

`Duration` carries a `[JsonConverter]` attribute and serializes automatically:

```json
{ "resolution": "PT15M" }
```

---

## TimeInterval

`TimeInterval` represents a half-open interval **[Start, End)** with UTC-normalized boundaries.

```csharp
// Construction
var interval = new TimeInterval(
    new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero),
    new DateTimeOffset(2026, 6, 16, 0, 0, 0, TimeSpan.Zero));

// Factory methods
TimeInterval day      = TimeInterval.Day(new DateOnly(2026, 6, 15), TimeZones.CentralEuropean);
TimeInterval intraday = TimeInterval.Intraday(DateTimeOffset.UtcNow, TimeZones.CentralEuropean);
TimeInterval ahead    = TimeInterval.DayAhead(DateTimeOffset.UtcNow, TimeZones.CentralEuropean);

// Contains an instant?
bool contains = interval.Contains(DateTimeOffset.UtcNow);

// Deconstruction
var (start, end) = interval;

// Split by resolution
foreach (DateTimeOffset slot in interval.SplitBy(Duration.QuarterHour))
    Console.WriteLine(slot); // 96 × 15-minute slots

// Split by hours
foreach (DateTimeOffset slot in interval.SplitBy(TimeSpan.FromHours(1)))
    Console.WriteLine(slot); // 24 × 1-hour slots

// Split by calendar days (DST-safe)
foreach (DateTimeOffset midnight in interval.SplitByDays(TimeZones.CentralEuropean))
    Console.WriteLine(midnight); // UTC equivalent of each midnight in CET/CEST
```

> **DST note:** `SplitBy(Duration.Day)` and `SplitByDays()` iterate over `DateOnly` calendar days in the target time zone. This correctly handles days with 23 hours (DST start) and 25 hours (DST end).

---

## JSON Converters

All converters live in the `Horae.Converters` namespace and can be used via `[JsonConverter]` attributes or through `JsonSerializerOptions.Converters`.

| Converter | Type | JSON representation |
|-----------|------|---------------------|
| `IsoDateTimeOffsetConverter` | `DateTimeOffset` | `"2026-01-01T23:00:00Z"` |
| `NullableIsoDateTimeOffsetConverter` | `DateTimeOffset?` | `"2026-01-01T23:00:00Z"` / `null` |
| `IsoDateConverter` | `DateOnly` | `"2026-01-01"` |
| `NullableIsoDateConverter` | `DateOnly?` | `"2026-01-01"` / `null` |
| `DurationConverter` | `Duration` | `"PT15M"` |
| `NullableDurationConverter` | `Duration?` | `"PT15M"` / `null` |

```csharp
record DeliveryPeriod(
    [property: JsonConverter(typeof(IsoDateTimeOffsetConverter))]
    DateTimeOffset Start,

    [property: JsonConverter(typeof(IsoDateTimeOffsetConverter))]
    DateTimeOffset End,

    [property: JsonConverter(typeof(IsoDateConverter))]
    DateOnly DeliveryDate,

    [property: JsonConverter(typeof(DurationConverter))]
    Duration Duration);
```

---

## Time Zone Constants

`TimeZones` provides `TimeZoneInfo` instances for common regions (IANA IDs, supported on all platforms since .NET 6).

| Property | IANA ID | Offset (standard time) |
|----------|---------|------------------------|
| `TimeZones.Utc` | UTC | +00:00 |
| `TimeZones.CentralEuropean` | Europe/Berlin | +01:00 / +02:00 |
| `TimeZones.London` | Europe/London | +00:00 / +01:00 |
| `TimeZones.Lisbon` | Europe/Lisbon | +00:00 / +01:00 |
| `TimeZones.EasternEurope` | Europe/Athens | +02:00 / +03:00 |
| `TimeZones.Moscow` | Europe/Moscow | +03:00 (no DST) |
| `TimeZones.Istanbul` | Europe/Istanbul | +03:00 (no DST) |
| `TimeZones.EasternUs` | America/New_York | -05:00 / -04:00 |
| `TimeZones.PacificUs` | America/Los_Angeles | -08:00 / -07:00 |
| `TimeZones.Dubai` | Asia/Dubai | +04:00 (no DST) |
| `TimeZones.India` | Asia/Kolkata | +05:30 (no DST) |
| `TimeZones.Japan` | Asia/Tokyo | +09:00 (no DST) |
| `TimeZones.AustraliaEastern` | Australia/Sydney | +10:00 / +11:00 |

