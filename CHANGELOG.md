# Changelog

All notable changes to Horae are documented in this file.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/);
versioning follows [Semantic Versioning](https://semver.org/).

---

## [1.0.0] — Initial public release

### Added

- **`ZonedTimeInterval`** — half-open interval `[Start, End)` with time-zone-preserved boundaries.
  Unlike `TimeInterval`, offsets reflect the DST-correct local time (`+01:00` CET, `+02:00` CEST).
  Requires `TimeZoneInfo` (raw offset is insufficient for DST-safe operations).
  `SplitBy` and `SplitByDays` use the stored time zone automatically.

- **`IsoDateTimePrecision`** enum — controls sub-second formatting in `ToIsoString()` and JSON converters:
  - `Seconds` *(default)* — always omit sub-second digits
  - `Auto` — include sub-second digits only when non-zero
  - `Fractional` — always emit 7 sub-second digits (`fffffff`)

- **`IsoDateTimeFormatAttribute`** — `JsonConverterAttribute` subclass for per-property precision control
  on `DateTimeOffset` and `DateTimeOffset?` properties.

- **`Duration`** — ISO 8601 duration wrapper (`PT15M`, `P1D`).  
  Named presets: `Minute`, `FiveMinutes`, `QuarterHour`, `HalfHour`, `Hour`, `Day`, `Week`.  
  Implicit conversion from/to `TimeSpan`; JSON serialization via `DurationConverter`.

- **`TimeInterval`** — UTC-normalized half-open interval with `Day`, `Intraday`, `DayAhead` factories,
  `SplitBy(Duration)`, `SplitByDays(TimeZoneInfo?)`, `Contains`, and `Deconstruct`.

- **`DateTimeOffsetExtensions`** — `StartOfDay`, `EndOfDay`, `StartOfIntraday`, `EndOfIntraday`,
  `StartOfDayAhead`, `EndOfDayAhead`, `RoundDown`, `RoundUp`, `RoundToNearest`, `ToTimeZone`,
  and `AddNanoseconds`.

- **`ConversionExtensions`** — `AsDateOnly`, `AsUtcDateOnly`, `AsDateTime`, `AsUtcDateTime`,
  `AsDateTimeOffset`, `AsUtcDateTimeOffset`.

- **`IsoStringExtensions`** — `ToIsoString(IsoDateTimePrecision)` for `DateTimeOffset` and `DateOnly`
  (including nullable variants); `ParseIsoDateTime`, `TryParseIsoDateTime`, `ParseIsoDate`, `TryParseIsoDate`.

- **`RegionExtensions`** — `ToUtc()`, `ToCentralEuropeanTime()`, `ToLondonTime()`, `ToLisbonTime()`,
  `ToEasternEuropeTime()`, `ToMoscowTime()`, `ToIstanbulTime()`, `ToEasternUsTime()`, `ToPacificUsTime()`,
  `ToDubaiTime()`, `ToIndiaTime()`, `ToJapanTime()`, `ToAustraliaEasternTime()`.

- **`DateExtensions`** — fluent month constructors on `int` (`1.January(2026)`), `.At(hour, minute)`,
  `.After(TimeSpan)`, `.Before(TimeSpan)`, `.AddNanoseconds(long)`.

- **`TimeSpanExtensions`** — fluent constructors on `int`/`long`/`double` (`15.Minutes()`, `2.Hours()`,
  `1.Days()`), `.And(TimeSpan)`, offset overloads.

- **`HoareSettings`** — ambient default time zone via `CurrentTimeZone`
  (default: `TimeZoneInfo.Local`).

- **`TimeZones`** — strongly-typed constants for 13 common time zones.

- JSON converter classes: `IsoDateTimeOffsetConverter`, `NullableIsoDateTimeOffsetConverter`,
  `IsoDateConverter`, `NullableIsoDateConverter`, `DurationConverter`, `NullableDurationConverter`.

- SourceLink + `.snupkg` symbol package for debugger source-stepping support.

