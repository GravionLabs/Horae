# Changelog

All notable changes to Horae are documented here.
Versioning follows [Semantic Versioning](https://semver.org/).

---

## [1.0.4] — 2026-05-10

### Documentation
- Add missing Calendar Queries, ISO Week, DST Queries, Unix Time sections


## [1.0.3] — 2026-05-10

### Bug Fixes
- Serve HTML instead of JSON from GitHub Pages


### Documentation
- Update CHANGELOG.md for v1.0.3 [skip ci]


## [1.0.2] — 2026-05-10

### Documentation
- Update CHANGELOG.md for v1.0.1 [skip ci]
- Update CHANGELOG.md for v1.0.2 [skip ci]


## [1.0.1] — 2026-05-10

### Bug Fixes
- Add dotnet restore to docfx-build action so Pages job succeeds


### Documentation
- Update CHANGELOG.md for v1.0.1 [skip ci]


## [1.0.0] — 2026-05-10

### Bug Fixes
- Add checks and pull-requests write permissions for test reporter


### Documentation
- Update CHANGELOG.md for v1.0.0 [skip ci]


### Features
- Add git-cliff CHANGELOG auto-generation
- Prepare release 1.0 (pre-existing work + calendar boundary extensions #2) (#10)
- Add calendar query extensions (IsLeapYear, DaysInMonth, DaysInYear, Quarter, DayOfYear, IsWeekend, IsWeekday) (#11)
- Add ISO week calendar extensions (IsoWeekNumber, IsoWeekYear, IsoWeekday) (#12)
- Add DST query extensions (IsInDst, IsDstAmbiguous) (#13)
- Add Unix timestamp helpers (FromUnixSeconds, FromUnixMilliseconds, ToUnixSeconds, ToUnixMilliseconds) (#14)
- Add TimeInterval set operations (Duration, Overlaps, Engulfs, Intersection, Union) (#15)
- Add Week/Month/Year factories and SplitByWeeks/SplitByMonths to TimeInterval (#8) (#16)
- Add TimeOnly ISO string support to IsoStringExtensions (#9) (#17)


### Refactoring
- Rename Hoare to Horae and update AsyncLocal settings for context isolation
- Cleanup formatting and remove redundant code in tests and workflow configuration


## [0.0.1] — 2026-03-15

### Bug Fixes
- DST-safe SplitBy for day-granularity resolutions
- Harden Duration ISO 8601 parser against overflow and duplicate designators
- Resolve all build warnings in source library


### Documentation
- Vollständige README-Dokumentation der gesamten API
- Update README for ZonedTimeInterval, IsoDateTimePrecision, default Seconds
- Add CHANGELOG.md and PackageReleaseNotes for v1.0.0
- Update project conventions and README for consistency in spelling and formatting


### Features
- Add TimeInterval readonly record struct
- Add Resolution type with ISO 8601 JSON serialisation
- ISO 8601 JSON converters and formatting extensions
- Add CI workflow and NuGet packaging metadata
- Add IsoDateTimePrecision and IsoDateTimeFormatAttribute
- Add ZonedTimeInterval
- Seconds/Minutes/Hours/Days return Duration instead of TimeSpan


### Refactoring
- Rename SplitByDeliveryDates to SplitByDays with optional TimeZoneInfo
- RegionExtensions To*Time + DST-safe SplitByDays
- Rename Resolution to Duration
- Change default IsoDateTimePrecision to Seconds
- Switch HoareSettings to AsyncLocal for per-context isolation



