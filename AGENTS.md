# AGENTS.md

## Project Snapshot
- `Horae` is a small extension-method library for date/time ergonomics (`src/Horae/*.cs`) with NUnit tests in `test/Horae.Tests`.
- Primary API style is fluent date construction and UTC-first conversion, e.g. `1.January(2026)` and `DateOnly -> DateTimeOffset (+00:00)`.
- Solution file is `Horae.slnx`; `README.md` is the main usage guide and is English-language.

## Repo Map (Where to Look First)
- `src/Horae/DateExtensions.cs`: month-name extension methods on `int`, each returns `DateTimeOffset` at midnight UTC.
- `src/Horae/ConversionExtensions.cs`: conversions among `DateTimeOffset`, `DateTime`, and `DateOnly`; uses `UtcDateTime` consistently.
- `src/Horae/RegionExtensions.cs`: region conversion entry point (`CentralEuropeanToUtc`) via `TimeZoneInfo`.
- `test/Horae.Tests/*Tests.cs`: current behavioral contract and naming style for tests.
- `Directory.Build.props` + `test/Directory.Build.props`: shared build/test defaults and central package management behavior.
- `.editorconfig`: code style, analyzer severities, and test-folder analyzer overrides.

## Architecture and Behavior Notes
- There are no service boundaries or persistence layers; this is a pure library surface implemented as static extension classes in namespace `Horae`.
- Data flow is direct and synchronous: input value type -> extension method -> returned value type (no side effects, no DI).
- UTC normalization is intentional in conversions (`dto.UtcDateTime`, `TimeSpan.Zero`), so preserve this unless changing API semantics.
- Region conversion currently depends on system tz database ID `Europe/Berlin`; behavior can vary by platform tz data availability.

## Build/Test Workflows (Observed)
- Typical commands:
  - `dotnet restore Horae.slnx`
  - `dotnet build Horae.slnx -c Debug`
  - `dotnet test Horae.slnx -c Debug`
- Current repository state (observed on 2026-03-11): restore fails with NU1010 because `test/Directory.Build.props` references `Ardalis.GuardClauses` but `Directory.Packages.props` has no matching `PackageVersion`.
- Test project also duplicates package declarations (`Horae.Tests.csproj` plus central test props), producing NU1504 warnings.

## Project-Specific Conventions
- Prefer file-scoped namespaces and concise expression-bodied members (see `src/Horae/*.cs`, `.editorconfig`).
- Use modern C# features where they improve readability (targeting current language level in repo instructions).
- Place `using` directives outside namespaces; avoid unnecessary `this.` qualification.
- For public APIs, keep XML docs up to date; prefer `nameof(...)` over string literals for member names.
- Null checks should follow `is null` / `is not null` style.
- Test methods use `Method_Should_DoThing` naming and Shouldly-style assertions (`ShouldBe`, `ShouldThrow`, etc.) with NUnit as the test framework.
- Every test fixture/class should carry a `[Category]`; default is `[Category("UnitTest")]`.
- `ManualTest` cases must be marked `[Explicit]`.
- Test files rely on implicit NUnit global using from `test/Directory.Build.props` (`<Using Include="NUnit.Framework"/>`).
- Keep line length practical (`.editorconfig` sets `max_line_length = 120` for general files).
- Use US English spelling in code, tests, docs, XML comments, and identifiers where practical (for example, `Deserialize`, `Serialize`, `behavior`, `normalized`).
- Do not add decorative separator comments such as `// ---`, `// ===`, or `// ──`; use whitespace or normal explanatory comments instead.
- Do not add "Act/Arrange/Assert" section comments in tests.

## Integration Points and Dependencies
- Timezone integration: `TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin")` in `RegionExtensions`.
- Package versions are intended to be centrally managed in `Directory.Packages.props`; project-level explicit versions should be treated as drift.
- Root `Directory.Build.props` sets `TargetFramework` to `net10.0`, but both csproj files currently pin `net8.0`; treat this mismatch as intentional-until-confirmed and avoid silent retargeting.

## Safe Change Checklist for Agents
- Update/add tests in `test/Horae.Tests` whenever extension behavior changes.
- Prefer Shouldly in tests and follow nearby naming/capitalization conventions.
- If adding NuGet packages, add matching entries in `Directory.Packages.props` (CPM is enabled).
- Re-run restore/build/test after dependency or framework edits to catch NU1010/NU1504 regressions early.
- For timezone features, consider DST and Linux/Windows timezone ID portability in tests.

