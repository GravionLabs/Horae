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
- [Day Offset](#day-offset)
- [Duration](#duration)
- [TimeInterval](#timeinterval)
- [ZonedTimeInterval](#zonedtimeinterval)
- [JSON Converters](#json-converters)
- [Time Zone Constants](#time-zone-constants)

---

## Configuration

`HoraeSettings.CurrentTimeZone` sets the default time zone used by all methods with an optional `TimeZoneInfo?` parameter when `null` is passed. The default value is `TimeZoneInfo.Local`.

The value is stored per execution context using `AsyncLocal<T>`, making it safe for concurrent use in asynchronous applications and unit tests.

```csharp
// Set once during application startup
HoraeSettings.CurrentTimeZone = TimeZones.CentralEuropean;

// Or use a scoped override
using (HoraeSettings.Use(TimeZones.Japan))
{
    // CurrentTimeZone is Japan here
}
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

All methods accept an optional `TimeZoneInfo?` (default: `HoraeSettings.CurrentTimeZone`).

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

// Default: IsoDateTimePrecision.Seconds — sub-second digits always omitted
dto.ToIsoString();   // "2026-01-01T23:00:00Z"
dto2.ToIsoString();  // "2026-01-01T23:00:00Z"  (500 ms dropped)

// Explicit precision control
dto2.ToIsoString(IsoDateTimePrecision.Auto);        // "2026-01-01T23:00:00.5Z" (only when ≠ 0)
dto2.ToIsoString(IsoDateTimePrecision.Fractional);  // "2026-01-01T23:00:00.5000000Z" (always 7 digits)
dto.ToIsoString(IsoDateTimePrecision.Fractional);   // "2026-01-01T23:00:00.0000000Z"

date.ToIsoString();  // "2026-06-15"

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

## Day Offset

`DayOffset` describes a calendar day relative to the current instant in a specific time zone.
It resolves to a `DateOnly` on demand, staying valid indefinitely (unlike a fixed date).

```csharp
// Named presets (no embedded timezone — pass it at resolve time)
DateOnly today     = DayOffset.Today.Resolve(TimeZones.CentralEuropean);
DateOnly dayAhead  = DayOffset.DayAhead.Resolve(TimeZones.CentralEuropean);
DateOnly yesterday = DayOffset.Yesterday.Resolve(TimeZones.CentralEuropean);

// Arbitrary offset via implicit int conversion
DateOnly twoDaysOut = ((DayOffset)2).Resolve(TimeZones.CentralEuropean);

// Self-contained with embedded timezone — resolves without any argument
var dto = new DayOffset(1, TimeZones.CentralEuropean);
DateOnly date = dto.Resolve();
// Serializes as: { "days": 1, "timezone": "Europe/Berlin" }

// Inject a reference instant (useful in tests or scheduled jobs)
DateOnly pinned = new DayOffset(1).Resolve(
    new DateTimeOffset(2026, 5, 9, 0, 0, 0, TimeSpan.Zero),
    TimeZones.CentralEuropean);
// → DateOnly(2026, 5, 10)

// Bridge to TimeInterval
TimeInterval interval = TimeInterval.Day(DayOffset.DayAhead.Resolve(TimeZones.CentralEuropean),
                                         TimeZones.CentralEuropean);
```

### Timezone resolution priority

1. Explicit `tz` argument passed to `Resolve`
2. `TimeZone` embedded in the `DayOffset` instance
3. `HoareSettings.CurrentTimeZone` (ambient fallback)

### JSON serialization

`DayOffset` carries a `[JsonConverter]` attribute and serializes automatically.
The format depends on whether a time zone is embedded:

```json
// No embedded timezone → plain integer
1

// With embedded timezone → object
{ "days": 1, "timezone": "Europe/Berlin" }
```

Both formats are accepted when reading, enabling a smooth migration from the compact integer form.

---

## Duration

`Duration` represents an ISO 8601 duration (positive `TimeSpan`) with named presets and automatic JSON serialization.

```csharp
// Predefined instances
Duration r1 = Duration.Minute;      // PT1M
Duration r2 = Duration.FiveMinutes; // PT5M
Duration r3 = Duration.QuarterHour; // PT15M
Duration r4 = Duration.HalfHour;    // PT30M
Duration r5 = Duration.Hour;        // PT1H
Duration r6 = Duration.Day;         // P1D
Duration r7 = Duration.Week;        // P7D

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
{ "duration": "PT15M" }
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

## ZonedTimeInterval

`ZonedTimeInterval` represents a half-open interval **[Start, End)** whose boundaries are expressed in a specific time zone, **preserving the local UTC offset** rather than normalising to UTC. Use this type when the local time context matters — for example, a trading day in CET or a session window such as "15:00–18:00 Europe/Berlin".

The constructor converts `start` and `end` to the supplied time zone, so `Start.Offset` always reflects the correct DST offset for that instant (`+01:00` CET, `+02:00` CEST). `TimeZoneInfo` is required because a raw UTC offset alone is insufficient for DST-safe operations.

```csharp
// Trading day in CET — boundaries are stored as local CET/CEST offsets
ZonedTimeInterval day = ZonedTimeInterval.Day(new DateOnly(2026, 1, 15), TimeZones.CentralEuropean);
// day.Start = 2026-01-15T00:00:00+01:00
// day.End   = 2026-01-16T00:00:00+01:00

// Summer day — offset is automatically +02:00 (CEST)
ZonedTimeInterval summerDay = ZonedTimeInterval.Day(new DateOnly(2026, 6, 15), TimeZones.CentralEuropean);
// summerDay.Start = 2026-06-15T00:00:00+02:00
// summerDay.End   = 2026-06-16T00:00:00+02:00

// Session window: 15:00–18:00 CET
ZonedTimeInterval session = ZonedTimeInterval.Create(
    new DateTimeOffset(2026, 1, 1, 15, 0, 0, TimeSpan.FromHours(1)),
    new DateTimeOffset(2026, 1, 1, 18, 0, 0, TimeSpan.FromHours(1)),
    TimeZones.CentralEuropean);

// Contains check
bool active = day.Contains(DateTimeOffset.UtcNow);

// Deconstruct
var (start, end) = day;

// Convert to UTC-normalized TimeInterval
TimeInterval utc = day.ToUtc();

// Split — no TimeZoneInfo parameter needed (stored in the interval)
foreach (DateTimeOffset slot in day.SplitBy(Duration.QuarterHour))
    Console.WriteLine(slot); // 96 × 15-minute slots in CET offset

// Split by calendar days (DST-safe)
foreach (DateTimeOffset midnight in day.SplitByDays())
    Console.WriteLine(midnight); // midnights in the interval's time zone
```

> **DST note:** `SplitBy(Duration.Day)` and `SplitByDays()` use `DateOnly.AddDays()` in the stored time zone, correctly handling 23-hour and 25-hour DST transition days.

---

## JSON Converters

All converters live in the `Horae.Converters` namespace and can be used via `[JsonConverter]` attributes or through `JsonSerializerOptions.Converters`.

| Converter                            | Type              | JSON representation               |
|--------------------------------------|-------------------|-----------------------------------|
| `IsoDateTimeOffsetConverter`         | `DateTimeOffset`  | `"2026-01-01T23:00:00Z"`          |
| `NullableIsoDateTimeOffsetConverter` | `DateTimeOffset?` | `"2026-01-01T23:00:00Z"` / `null` |
| `IsoDateConverter`                   | `DateOnly`        | `"2026-01-01"`                    |
| `NullableIsoDateConverter`           | `DateOnly?`       | `"2026-01-01"` / `null`           |
| `DurationConverter`                  | `Duration`        | `"PT15M"`                         |
| `NullableDurationConverter`          | `Duration?`       | `"PT15M"` / `null`                |

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

### `IsoDateTimeFormatAttribute`

Use `[IsoDateTimeFormat]` instead of `[JsonConverter(typeof(...))]` on `DateTimeOffset` and `DateTimeOffset?` properties to control sub-second precision directly on the attribute:

```csharp
record Event(
    // Always emit 7 fractional digits: "2026-01-01T23:00:00.0000000Z"
    [property: IsoDateTimeFormat(IsoDateTimePrecision.Fractional)]
    DateTimeOffset Timestamp,

    // Always omit sub-seconds (default): "2026-01-01T23:00:00Z"
    [property: IsoDateTimeFormat]
    DateTimeOffset Start,

    // Include sub-seconds only when non-zero
    [property: IsoDateTimeFormat(IsoDateTimePrecision.Auto)]
    DateTimeOffset? End);
```

`IsoDateTimePrecision` values:

| Value                 | Behavior                                                        |
|-----------------------|-----------------------------------------------------------------|
| `Seconds` *(default)* | Always omit sub-second digits — `"...T23:00:00Z"`               |
| `Auto`                | Include sub-second digits only when non-zero                    |
| `Fractional`          | Always include 7 sub-second digits — `"...T23:00:00.0000000Z"`  |

---

## Time Zone Constants

`TimeZones` provides `TimeZoneInfo` instances for common regions (IANA IDs, supported on all platforms since .NET 6).

| Property                     | IANA ID             | Offset (standard time) |
|------------------------------|---------------------|------------------------|
| `TimeZones.Utc`              | UTC                 | +00:00                 |
| `TimeZones.CentralEuropean`  | Europe/Berlin       | +01:00 / +02:00        |
| `TimeZones.London`           | Europe/London       | +00:00 / +01:00        |
| `TimeZones.Lisbon`           | Europe/Lisbon       | +00:00 / +01:00        |
| `TimeZones.EasternEurope`    | Europe/Athens       | +02:00 / +03:00        |
| `TimeZones.Moscow`           | Europe/Moscow       | +03:00 (no DST)        |
| `TimeZones.Istanbul`         | Europe/Istanbul     | +03:00 (no DST)        |
| `TimeZones.EasternUs`        | America/New_York    | -05:00 / -04:00        |
| `TimeZones.PacificUs`        | America/Los_Angeles | -08:00 / -07:00        |
| `TimeZones.Dubai`            | Asia/Dubai          | +04:00 (no DST)        |
| `TimeZones.India`            | Asia/Kolkata        | +05:30 (no DST)        |
| `TimeZones.Japan`            | Asia/Tokyo          | +09:00 (no DST)        |
| `TimeZones.AustraliaEastern` | Australia/Sydney    | +10:00 / +11:00        |

