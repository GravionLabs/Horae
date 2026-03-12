# Horae

Horae ist eine moderne, minimalistische .NET‑Bibliothek für elegante und präzise Zeit‑Utilities.
Sie bietet syntaktischen Zucker wie `1.January(2026)`, robuste Konvertierungen zwischen `DateTimeOffset`, `DateOnly` und `DateTime`, DST‑sichere Intervall‑Operationen sowie ISO 8601‑Serialisierung mit `System.Text.Json`.

Lizenz: Apache‑2.0 · Zielframeworks: net8.0 · net9.0 · net10.0

---

## Inhaltsverzeichnis

- [Konfiguration](#konfiguration)
- [Datumskonstruktion](#datumskonstruktion)
- [TimeSpan-Konstruktion](#timespan-konstruktion)
- [DateTimeOffset-Operationen](#datetimeoffset-operationen)
- [Konvertierungen](#konvertierungen)
- [Regionale Zeitzonen](#regionale-zeitzonen)
- [ISO 8601 Formatierung & Parsing](#iso-8601-formatierung--parsing)
- [Resolution](#resolution)
- [TimeInterval](#timeinterval)
- [JSON Converter](#json-converter)
- [Zeitzonen-Konstanten](#zeitzonen-konstanten)

---

## Konfiguration

`HoareSettings.CurrentTimeZone` legt die Standard‑Zeitzone fest, die alle Methoden mit optionalem `TimeZoneInfo?`‑Parameter verwenden, wenn `null` übergeben wird. Standardwert ist `TimeZoneInfo.Local`.

```csharp
// Einmalig beim Applikationsstart setzen (nicht thread-sicher)
HoareSettings.CurrentTimeZone = TimeZones.CentralEuropean;
```

---

## Datumskonstruktion

Fluente Methoden auf `int` für lesbare Datumswerte:

```csharp
DateTimeOffset d1 = 1.January(2026);         // 2026-01-01T00:00:00+00:00
DateTimeOffset d2 = 15.June(2026);           // 2026-06-15T00:00:00+00:00

// Uhrzeit hinzufügen (erhält den UTC-Offset der Basis)
DateTimeOffset d3 = 1.January(2026).At(14, 30);       // 2026-01-01T14:30:00+00:00
DateTimeOffset d4 = 1.January(2026).At(TimeSpan.FromHours(9));

// Relative Arithmetik
DateTimeOffset d5 = 2.Hours().After(1.January(2026));
DateTimeOffset d6 = 30.Minutes().Before(1.January(2026));

// Nanosekunden-Unterstützung (100 ns Auflösung)
DateTimeOffset d7 = 1.January(2026).AddNanoseconds(500);
```

---

## TimeSpan‑Konstruktion

Fluente Methoden auf `int`, `long` und `double`:

```csharp
TimeSpan t1 = 15.Minutes();
TimeSpan t2 = 1.Hours();
TimeSpan t3 = 2.Days();
TimeSpan t4 = 500.Milliseconds();
TimeSpan t5 = 100.Microseconds();
TimeSpan t6 = 1000.Nanoseconds();   // sub-100 ns Präzision geht verloren

// Verkettung mit .And()
TimeSpan t7 = 1.Hours().And(30.Minutes());   // 1:30:00

// Offset-Varianten
TimeSpan t8 = 2.Hours(30.Minutes());         // 2:30:00
```

---

## DateTimeOffset‑Operationen

### Tagesgrenzen

Alle Methoden akzeptieren ein optionales `TimeZoneInfo?` (Standard: `HoareSettings.CurrentTimeZone`).

```csharp
var now = DateTimeOffset.UtcNow;

DateTimeOffset start  = now.StartOfDay(TimeZones.CentralEuropean);    // Mitternacht in CET/CEST
DateTimeOffset startU = now.StartOfDayUtc(TimeZones.CentralEuropean); // selbes Instant, UTC-Offset

DateTimeOffset end    = now.EndOfDay(TimeZones.CentralEuropean);      // nächste Mitternacht (exklusiv)
DateTimeOffset endU   = now.EndOfDayUtc(TimeZones.CentralEuropean);

// Intraday-Session (= aktueller Tag)
DateTimeOffset is = now.StartOfIntraday();
DateTimeOffset ie = now.EndOfIntraday();

// Day-Ahead-Session (= nächster Kalendertag)
DateTimeOffset das = now.StartOfDayAhead();
DateTimeOffset dae = now.EndOfDayAhead();
```

### Runden

```csharp
DateTimeOffset r1 = now.RoundDown(Resolution.QuarterHour);              // auf 15-Minuten abrunden
DateTimeOffset r2 = now.RoundUp(TimeSpan.FromHours(1));                 // auf Stunde aufrunden
DateTimeOffset r3 = now.RoundToNearest(Resolution.HalfHour, TimeZones.CentralEuropean);
```

### Zeitzone wechseln

```csharp
DateTimeOffset cest = now.ToTimeZone(TimeZones.CentralEuropean);        // selber Instant, anderer Offset
```

---

## Konvertierungen

```csharp
var dto  = new DateTimeOffset(2026, 6, 15, 12, 0, 0, TimeSpan.Zero);
var date = new DateOnly(2026, 6, 15);

// DateTimeOffset → DateOnly
DateOnly d1 = dto.AsDateOnly(TimeZones.Japan);    // Kalendertag in JST
DateOnly d2 = dto.AsUtcDateOnly();                // Kalendertag in UTC

// DateTimeOffset → DateTime
DateTime t1 = dto.AsDateTime(TimeZones.CentralEuropean); // DateTimeKind.Unspecified
DateTime t2 = dto.AsUtcDateTime();                        // DateTimeKind.Utc

// DateOnly → DateTimeOffset
DateTimeOffset m1 = date.AsDateTimeOffset(TimeZones.CentralEuropean); // Mitternacht in CET/CEST
DateTimeOffset m2 = date.AsUtcDateTimeOffset();                        // Mitternacht UTC
```

---

## Regionale Zeitzonen

Konvertierung eines `DateTimeOffset`‑Werts in eine bestimmte Zeitzone. Der zurückgegebene Wert repräsentiert denselben Instant mit dem korrekten regionalen Offset (inkl. DST).

```csharp
var utc = new DateTimeOffset(2026, 6, 15, 12, 0, 0, TimeSpan.Zero);

DateTimeOffset cet  = utc.ToCentralEuropeanTime();  // 14:00 +02:00 (CEST)
DateTimeOffset lon  = utc.ToLondonTime();            // 13:00 +01:00 (BST)
DateTimeOffset lis  = utc.ToLisbonTime();            // 13:00 +01:00 (WEST)
DateTimeOffset eet  = utc.ToEasternEuropeTime();     // 15:00 +03:00 (EEST)
DateTimeOffset msk  = utc.ToMoscowTime();            // 15:00 +03:00 (MSK)
DateTimeOffset ist  = utc.ToIstanbulTime();          // 15:00 +03:00 (TRT)
DateTimeOffset est  = utc.ToEasternUsTime();         //  8:00 -04:00 (EDT)
DateTimeOffset pst  = utc.ToPacificUsTime();         //  5:00 -07:00 (PDT)
DateTimeOffset gst  = utc.ToDubaiTime();             // 16:00 +04:00 (GST)
DateTimeOffset ind  = utc.ToIndiaTime();             // 17:30 +05:30 (IST)
DateTimeOffset jst  = utc.ToJapanTime();             // 21:00 +09:00 (JST)
DateTimeOffset aest = utc.ToAustraliaEasternTime();  // 22:00 +10:00 (AEST)

// Zurück nach UTC
DateTimeOffset back = cet.ToUtc();                   // 12:00 +00:00
```

---

## ISO 8601 Formatierung & Parsing

### Formatierung

```csharp
var dto  = new DateTimeOffset(2026, 1, 1, 23, 0, 0, TimeSpan.Zero);
var dto2 = new DateTimeOffset(2026, 1, 1, 23, 0, 0, 500, TimeSpan.Zero); // 500 ms
var date = new DateOnly(2026, 6, 15);

dto.ToIsoString();    // "2026-01-01T23:00:00Z"   (UTC, ohne Sub-Sekunden wenn nicht nötig)
dto2.ToIsoString();   // "2026-01-01T23:00:00.5Z" (Sub-Sekunden nur wenn ≠ 0)
date.ToIsoString();   // "2026-06-15"

// Nullable-Varianten
DateTimeOffset? nullDto  = null;
DateOnly?       nullDate = null;
nullDto.ToIsoString();   // null
nullDate.ToIsoString();  // null
```

### Parsing

```csharp
// DateTimeOffset – akzeptiert "Z" und "+HH:mm"-Offsets
DateTimeOffset dto1 = "2026-01-01T23:00:00Z".ParseIsoDateTime();
DateTimeOffset dto2 = "2026-01-01T23:00:00+01:00".ParseIsoDateTime();

bool ok = "2026-01-01T23:00:00Z".TryParseIsoDateTime(out DateTimeOffset result);

// DateOnly – strikt "yyyy-MM-dd"
DateOnly date = "2026-06-15".ParseIsoDate();
bool ok2 = "2026-06-15".TryParseIsoDate(out DateOnly dateResult);
```

---

## Resolution

`Resolution` repräsentiert eine feste Zeitauflösung und serialisiert sich als ISO 8601 Duration.

```csharp
// Vordefinierte Instanzen
Resolution r = Resolution.Minute;       // PT1M
Resolution r = Resolution.FiveMinutes; // PT5M
Resolution r = Resolution.QuarterHour; // PT15M
Resolution r = Resolution.HalfHour;    // PT30M
Resolution r = Resolution.Hour;        // PT1H
Resolution r = Resolution.Day;         // P1D

// Eigene Werte
var r2 = new Resolution(TimeSpan.FromMinutes(10));
var r3 = Resolution.Parse("PT10M");
bool ok = Resolution.TryParse("P1D", out Resolution daily);

// Zu/von TimeSpan (implizit)
TimeSpan ts = Resolution.QuarterHour;            // 00:15:00
Resolution res = TimeSpan.FromHours(1);          // Resolution.Hour äquivalent

// ToString → ISO 8601
Resolution.QuarterHour.ToString(); // "PT15M"
Resolution.Day.ToString();         // "P1D"
```

### JSON-Serialisierung

`Resolution` trägt `[JsonConverter]`-Attribut und serialisiert automatisch:

```json
{ "resolution": "PT15M" }
```

---

## TimeInterval

`TimeInterval` repräsentiert ein halboffenes Intervall **[Start, End)** mit UTC‑normalisierten Grenzen.

```csharp
// Konstruktion
var interval = new TimeInterval(
    new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero),
    new DateTimeOffset(2026, 6, 16, 0, 0, 0, TimeSpan.Zero));

// Factory-Methoden
TimeInterval day      = TimeInterval.Day(new DateOnly(2026, 6, 15), TimeZones.CentralEuropean);
TimeInterval intraday = TimeInterval.Intraday(DateTimeOffset.UtcNow, TimeZones.CentralEuropean);
TimeInterval ahead    = TimeInterval.DayAhead(DateTimeOffset.UtcNow, TimeZones.CentralEuropean);

// Enthält ein Instant?
bool contains = interval.Contains(DateTimeOffset.UtcNow);

// Dekonstruktion
var (start, end) = interval;

// Aufteilen nach Auflösung
foreach (DateTimeOffset slot in interval.SplitBy(Resolution.QuarterHour))
    Console.WriteLine(slot); // 96 × 15-Minuten-Slots

// Aufteilen nach Stunden
foreach (DateTimeOffset slot in interval.SplitBy(TimeSpan.FromHours(1)))
    Console.WriteLine(slot); // 24 × 1-Stunden-Slots

// Aufteilen nach Kalendertagen (DST-sicher)
foreach (DateTimeOffset midnight in interval.SplitByDays(TimeZones.CentralEuropean))
    Console.WriteLine(midnight); // UTC-Äquivalent jeder Mitternacht in CET/CEST
```

> **DST-Hinweis:** `SplitBy(Resolution.Day)` und `SplitByDays()` iterieren über `DateOnly`-Kalender­tage in der Ziel­zeitzone. Damit werden Tage mit 23h (Sommer­zeitbeginn) und 25h (Sommer­zeitende) korrekt behandelt.

---

## JSON Converter

Alle Converter liegen im Namespace `Horae.Converters` und können per `[JsonConverter]`-Attribut oder via `JsonSerializerOptions.Converters` verwendet werden.

| Converter | Typ | JSON-Darstellung |
|-----------|-----|-----------------|
| `IsoDateTimeOffsetConverter` | `DateTimeOffset` | `"2026-01-01T23:00:00Z"` |
| `NullableIsoDateTimeOffsetConverter` | `DateTimeOffset?` | `"2026-01-01T23:00:00Z"` / `null` |
| `IsoDateConverter` | `DateOnly` | `"2026-01-01"` |
| `NullableIsoDateConverter` | `DateOnly?` | `"2026-01-01"` / `null` |
| `ResolutionJsonConverter` | `Resolution` | `"PT15M"` |
| `NullableResolutionJsonConverter` | `Resolution?` | `"PT15M"` / `null` |

```csharp
record DeliveryPeriod(
    [property: JsonConverter(typeof(IsoDateTimeOffsetConverter))]
    DateTimeOffset Start,

    [property: JsonConverter(typeof(IsoDateTimeOffsetConverter))]
    DateTimeOffset End,

    [property: JsonConverter(typeof(IsoDateConverter))]
    DateOnly DeliveryDate,

    [property: JsonConverter(typeof(ResolutionJsonConverter))]
    Resolution Resolution);
```

---

## Zeitzonen‑Konstanten

`TimeZones` stellt `TimeZoneInfo`-Instanzen für gängige Regionen bereit (IANA-IDs, alle Plattformen ab .NET 6).

| Eigenschaft | IANA-ID | Offset (Normalzeit) |
|-------------|---------|---------------------|
| `TimeZones.Utc` | UTC | +00:00 |
| `TimeZones.CentralEuropean` | Europe/Berlin | +01:00 / +02:00 |
| `TimeZones.London` | Europe/London | +00:00 / +01:00 |
| `TimeZones.Lisbon` | Europe/Lisbon | +00:00 / +01:00 |
| `TimeZones.EasternEurope` | Europe/Athens | +02:00 / +03:00 |
| `TimeZones.Moscow` | Europe/Moscow | +03:00 (kein DST) |
| `TimeZones.Istanbul` | Europe/Istanbul | +03:00 (kein DST) |
| `TimeZones.EasternUs` | America/New_York | −05:00 / −04:00 |
| `TimeZones.PacificUs` | America/Los_Angeles | −08:00 / −07:00 |
| `TimeZones.Dubai` | Asia/Dubai | +04:00 (kein DST) |
| `TimeZones.India` | Asia/Kolkata | +05:30 (kein DST) |
| `TimeZones.Japan` | Asia/Tokyo | +09:00 (kein DST) |
| `TimeZones.AustraliaEastern` | Australia/Sydney | +10:00 / +11:00 |

