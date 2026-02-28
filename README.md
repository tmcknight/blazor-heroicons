<p align="center">
  <img src=".github/icon.png" alt="Blazor.Heroicons" width="128" />
</p>

<h1 align="center">Blazor.Heroicons</h1>

<p align="center">
  A third-party Blazor component library for <a href="https://heroicons.com">Heroicons</a> — beautiful hand-crafted SVG icons by the makers of Tailwind CSS.
</p>

[![NuGet package](https://img.shields.io/nuget/v/Blazor.Heroicons?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Blazor.Heroicons)
[![Heroicons version](https://img.shields.io/badge/heroicons-v2.2.0-informational?style=flat-square)](https://github.com/tailwindlabs/heroicons/releases/tag/v2.2.0)
[![License](https://img.shields.io/github/license/tmcknight/Blazor.Heroicons?style=flat-square)](LICENSE)
[![Build and test](https://img.shields.io/github/actions/workflow/status/tmcknight/Blazor.Heroicons/dotnet.yml?branch=main&style=flat-square&logo=github&label=tests)](https://github.com/tmcknight/Blazor.Heroicons/actions/workflows/dotnet.yml)

## 📦 Installation

```sh
dotnet add package Blazor.Heroicons
```

Supports .NET 8.0, .NET 9.0, and .NET 10.0.

## 🎨 Icon Styles

Each icon is available in four styles:

| Style | Namespace | Size | Best for |
|-------|-----------|------|----------|
| **Outline** | `Blazor.Heroicons.Outline` | 24x24, 1.5px stroke | Primary navigation and marketing sections |
| **Solid** | `Blazor.Heroicons.Solid` | 24x24, filled | Primary navigation and marketing sections |
| **Mini** | `Blazor.Heroicons.Mini` | 20x20, filled | Smaller elements like buttons and form inputs |
| **Micro** | `Blazor.Heroicons.Micro` | 16x16, filled | Tight, high-density UIs |

[Browse the full list of icons on heroicons.com &rarr;](https://heroicons.com)

## 🚀 Usage

### Individual icon components

Each icon is available as a standalone Blazor component. Import the namespace for the style you want and use the icon directly:

```razor
@using Blazor.Heroicons.Outline

<BeakerIcon class="size-6 text-blue-500" />
```

All standard HTML attributes (`class`, `id`, `style`, etc.) are passed through to the underlying SVG element. Icons use `currentColor` for fill/stroke, so they can be styled with CSS or utility classes like [Tailwind CSS](https://tailwindcss.com/).

### Dynamic icon by name (`<Heroicon />`)

Reference any icon by name and style using the `Heroicon` component:

```razor
@using Blazor.Heroicons

<Heroicon Name="@HeroiconName.Sparkles" Type="HeroiconType.Outline" class="size-6 text-yellow-600" />
```

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Name` | `string` | `HeroiconName.Sparkles` | Icon name — supports multiple formats (see below) |
| `Type` | `HeroiconType` | `HeroiconType.Outline` | Icon style to render |

The `Name` parameter is flexible and accepts several formats:

- `"HandThumbUp"` or `"HandThumbUpIcon"`
- `"hand-thumb-up"` (hyphenated)
- `"handthumbup"` (case-insensitive)

Use the `HeroiconName` static class for IntelliSense support (e.g. `HeroiconName.AcademicCap`).

### Random icon (`<RandomHeroicon />`)

Render a random icon from a given style:

```razor
@using Blazor.Heroicons

<RandomHeroicon Type="HeroiconType.Mini" class="size-6 text-green-700" />
```

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Type` | `HeroiconType` | `HeroiconType.Outline` | Icon style to choose from |

## 🔗 Links

- [Heroicons](https://heroicons.com)
- [Release notes](https://github.com/tmcknight/Blazor.Heroicons/releases)
- [NuGet package](https://www.nuget.org/packages/Blazor.Heroicons)
- [License](https://github.com/tmcknight/Blazor.Heroicons/blob/main/LICENSE)

## 📄 License

This library is MIT licensed.
