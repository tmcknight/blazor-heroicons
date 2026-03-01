# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Blazor.Heroicons is a Blazor component library wrapping the [Heroicons](https://heroicons.com/) icon set (v2.2.0). It provides four icon styles (Outline, Solid, Mini, Micro) as Razor components, published as a NuGet package targeting .NET 8.0, 9.0, and 10.0.

## Build Commands

```bash
dotnet build                                    # Build entire solution
dotnet test                                     # Run all tests
dotnet test tests/Blazor.Heroicons.Tests        # Component tests only (bunit)
dotnet test tests/HeroiconsGenerator.Tests      # Generator tests only
dotnet run --project src/HeroiconsGenerator     # Regenerate icons from latest Heroicons release
dotnet run --project src/DemoWebApp             # Run demo app locally
```

## Architecture

### Code Generation Pipeline

Most icon-related code is **auto-generated** — do not edit generated files directly.

`src/HeroiconsGenerator/` is a console app that:
1. Fetches the latest Heroicons release tarball from GitHub (`GitHubReleaseProvider`)
2. Converts each SVG into a Razor component (`IconGenerator`) — injects `@attributes="AdditionalAttributes"` into the `<svg>` tag
3. Generates `HeroiconRegistry.cs` (FrozenDictionary for O(1) lookup) and `HeroiconName.cs` (string constants for IntelliSense) via `RegistryGenerator`
4. Updates the version badge in README via `ReadmeUpdater`
5. `HeroiconsUpdater` orchestrates the full pipeline

**Naming**: SVG filenames like `academic-cap.svg` become `AcademicCapIcon.razor` (see `NamingHelper`).

### Library (`src/Blazor.Heroicons/`)

- `HeroiconBase.cs` — Base class capturing unmatched HTML attributes via `[Parameter(CaptureUnmatchedValues=true)]`
- `Heroicon.razor` — Dynamic component that resolves icons by name+type from the registry; accepts multiple name formats (`"HandThumbUp"`, `"hand-thumb-up"`, case-insensitive)
- `RandomHeroicon.razor` — Renders a random icon
- `HeroiconRegistry.cs` — Generated frozen dictionary mapping `type.name` → component type
- `HeroiconName.cs` — Generated static string constants for all icon names
- `Outline/`, `Solid/`, `Mini/`, `Micro/` — Generated icon component directories

### Key Design Decisions

- **Zero runtime reflection**: Static registry with explicit type references prevents trimming issues
- **AOT/trimming safe**: `LinkerConfig.xml` preserves icon component constructors for `DynamicComponent`
- **FrozenDictionary** with `OrdinalIgnoreCase` comparer for efficient, flexible name resolution

## Testing

Uses **MSTest** + **bunit** (v2.6.2). `Blazor.Heroicons.Tests` multi-targets net8.0/net9.0/net10.0. Generator tests use `TempDirectory` for isolated file I/O and `FakeReleaseProvider` to mock GitHub API calls.

## Code Style

Per `.editorconfig`: tabs for C#/Razor indentation, LF line endings, UTF-8. YAML/JSON/Markdown use 2-space indentation.
