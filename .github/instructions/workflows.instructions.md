---
applyTo: "**/*.yml"
---

# GravionLabs/ci Workflow Instructions

When creating or editing GitHub Actions workflow files in this repository, use the `GravionLabs/ci` reusable workflows and composite actions.

## NuGet Package Workflow

For .NET NuGet library CI/CD, use the reusable workflow:

```yaml
jobs:
  package:
    name: Package
    uses: GravionLabs/ci/.github/workflows/nuget-package.yml@main
    with:
      dotnet-project: src/Horae/Horae.csproj
      gitversion-config-file: GitVersion.yml
      publish-feed-url: https://api.nuget.org/v3/index.json
      verify-package-files: |
        lib/net8.0/Horae.dll
        lib/net9.0/Horae.dll
        lib/net10.0/Horae.dll
        lib/net8.0/Horae.xml
        lib/net9.0/Horae.xml
        lib/net10.0/Horae.xml
        README.md
        CHANGELOG.md
    secrets:
      publish-api-key: ${{ secrets.NUGET_API_KEY }}
```

## Key Rules

- **Do NOT add a `version:` input** — versioning runs internally via GitVersion.
- **Do NOT add a `determine-version` job** — it is part of the reusable workflow.
- The reusable workflow handles: version determination → build → test → pack → verify → publish.
- Publishing only runs on `refs/heads/main` unless `force-publish: true`.
- The publish job requires a `nuget-publish` environment configured in GitHub repository settings.

## verify-package-files Format

Each line is a path that must exist inside the packed `.nupkg`:

```
lib/net8.0/Horae.dll
lib/net9.0/Horae.dll
lib/net10.0/Horae.dll
lib/net8.0/Horae.xml
lib/net9.0/Horae.xml
lib/net10.0/Horae.xml
README.md
CHANGELOG.md
```

Include one entry per TFM × file combination. The check runs after `pack` and before `publish`.
