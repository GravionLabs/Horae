#!/usr/bin/env bash
set -e

REPO="Horae"
PROJECT="Horae"
TESTPROJECT="Horae.Tests"

mkdir -p $REPO/src/$PROJECT/Extensions
mkdir -p $REPO/tests/$TESTPROJECT/Extensions

cat > $REPO/src/$PROJECT/$PROJECT.csproj <<EOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
EOF

cat > $REPO/tests/$TESTPROJECT/$TESTPROJECT.csproj <<EOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../../src/$PROJECT/$PROJECT.csproj" />
    <PackageReference Include="NUnit" Version="4.2.2" />
    <PackageReference Include="NUnit3TestAdapter" Version="4.5.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
  </ItemGroup>
</Project>
EOF

cat > $REPO/src/$PROJECT/Extensions/DateExtensions.cs <<'EOF'
namespace Horae.Extensions;

public static class DateExtensions
{
    public static DateTimeOffset January(this int day, int year) =>
        new(year, 1, day, 0, 0, 0, TimeSpan.Zero);

    public static DateTimeOffset February(this int day, int year) =>
        new(year, 2, day, 0, 0, 0, TimeSpan.Zero);

    public static DateTimeOffset March(this int day, int year) =>
        new(year, 3, day, 0, 0, 0, TimeSpan.Zero);

    public static DateTimeOffset April(this int day, int year) =>
        new(year, 4, day, 0, 0, 0, TimeSpan.Zero);

    public static DateTimeOffset May(this int day, int year) =>
        new(year, 5, day, 0, 0, 0, TimeSpan.Zero);

    public static DateTimeOffset June(this int day, int year) =>
        new(year, 6, day, 0, 0, 0, TimeSpan.Zero);

    public static DateTimeOffset July(this int day, int year) =>
        new(year, 7, day, 0, 0, 0, TimeSpan.Zero);

    public static DateTimeOffset August(this int day, int year) =>
        new(year, 8, day, 0, 0, 0, TimeSpan.Zero);

    public static DateTimeOffset September(this int day, int year) =>
        new(year, 9, day, 0, 0, 0, TimeSpan.Zero);

    public static DateTimeOffset October(this int day, int year) =>
        new(year, 10, day, 0, 0, 0, TimeSpan.Zero);

    public static DateTimeOffset November(this int day, int year) =>
        new(year, 11, day, 0, 0, 0, TimeSpan.Zero);

    public static DateTimeOffset December(this int day, int year) =>
        new(year, 12, day, 0, 0, 0, TimeSpan.Zero);
}
EOF

cat > $REPO/src/$PROJECT/Extensions/ConversionExtensions.cs <<'EOF'
namespace Horae.Extensions;

public static class ConversionExtensions
{
    public static DateOnly AsDateOnly(this DateTimeOffset dto) =>
        DateOnly.FromDateTime(dto.UtcDateTime);

    public static DateTime AsDateTime(this DateTimeOffset dto) =>
        dto.UtcDateTime;

    public static DateTimeOffset AsDateTimeOffset(this DateOnly date) =>
        new(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);

    public static DateTimeOffset AsUtcDateTimeOffset(this DateOnly date) =>
        new(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
}
EOF

cat > $REPO/src/$PROJECT/Extensions/RegionExtensions.cs <<'EOF'
namespace Horae.Extensions;

public static class RegionExtensions
{
    public static DateTimeOffset CentralEuropeanToUtc(this DateTimeOffset dto)
    {
        var cet = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
        return TimeZoneInfo.ConvertTime(dto, cet, TimeZoneInfo.Utc);
    }
}
EOF

cat > $REPO/tests/$TESTPROJECT/Extensions/DateExtensionsTests.cs <<'EOF'
using Horae.Extensions;
using NUnit.Framework;

namespace Horae.Tests.Extensions;

public class DateExtensionsTests
{
    [Test]
    public void January_Should_Create_Correct_DateTimeOffset()
    {
        var dto = 1.January(2026);
        Assert.That(dto, Is.EqualTo(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)));
    }
}
EOF

cat > $REPO/tests/$TESTPROJECT/Extensions/ConversionExtensionsTests.cs <<'EOF'
using Horae.Extensions;
using NUnit.Framework;

namespace Horae.Tests.Extensions;

public class ConversionExtensionsTests
{
    [Test]
    public void AsDateOnly_Should_Return_Correct_DateOnly()
    {
        var dto = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var dateOnly = dto.AsDateOnly();
        Assert.That(dateOnly, Is.EqualTo(new DateOnly(2026, 1, 1)));
    }
}
EOF

cat > $REPO/tests/$TESTPROJECT/Extensions/RegionExtensionsTests.cs <<'EOF'
using Horae.Extensions;
using NUnit.Framework;

namespace Horae.Tests.Extensions;

public class RegionExtensionsTests
{
    [Test]
    public void CentralEuropeanToUtc_Should_Convert_Without_Exception()
    {
        var cetDate = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var utc = cetDate.CentralEuropeanToUtc();
        Assert.That(utc.Offset, Is.EqualTo(TimeSpan.Zero));
    }
}
EOF

cat > $REPO/LICENSE <<'EOF'
Apache License 2.0
EOF

cat > $REPO/.editorconfig <<'EOF'
[*.cs]
indent_style = space
indent_size = 4
EOF

cat > $REPO/README.md <<'EOF'
# Horae

Horae ist eine moderne, minimalistische .NET‑Bibliothek für elegante und präzise Zeit‑Utilities. Sie bietet syntaktischen Zucker wie `1.January(2026)`, robuste Konvertierungen zwischen `DateTimeOffset`, `DateOnly` und `DateTime`, sowie regionale Zeitumrechnungen wie `CentralEuropeanToUtc()`. Entwickelt für Klarheit, Erweiterbarkeit und professionelle Codebases.

Lizenz: Apache‑2.0
EOF

echo "Repository '$REPO' wurde erfolgreich erzeugt."
