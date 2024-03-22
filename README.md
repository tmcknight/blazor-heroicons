# Blazor.Heroicons

A third-party Blazor component library for [Heroicons](https://heroicons.com).

[![Nuget package](https://img.shields.io/nuget/v/Blazor.Heroicons?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Blazor.Heroicons)
[![Heroicons version](https://img.shields.io/badge/heroicons-v2.1.2-informational?style=flat-square)](https://github.com/tailwindlabs/heroicons/releases/tag/v2.1.2)
[![License](https://img.shields.io/github/license/tmcknight/Blazor.Heroicons?style=flat-square)](LICENSE)

## Basic Usage

First, install `Blazor.Heroicons` from nuget:

```sh
dotnet add package Blazor.Heroicons
```

Now each icon can be used as a Blazor component:

```razor
@using Blazor.Heroicons.Solid

<div>
  <BeakerIcon class="h-6 w-6 text-blue-500" />
  <p>...</p>
</div>
```

The icons are preconfigured to be stylable by setting the `color` CSS property, either manually or using utility classes like `text-gray-500` in a framework like [Tailwind CSS](https://tailwindcss.com/).

### `<Heroicon />`

You can also reference an icon by name, using the `Heroicon` component:

```razor
@using Blazor.Heroicons

<Heroicon Name="@HeroiconName.Sparkles" Type="HeroiconType.Outline" class="h-6 w-6 text-yellow-600" />
```

[Browse the full list of icons on Heroicons &rarr;](https://heroicons.com)

### `<RandomHeroicon />`

If you want to get crazy, use the `RandomHeroicon` component to render a random icon:

```razor
@using Blazor.Heroicons

<RandomHeroicon Type="HeroiconType.Mini" class="h-6 w-6 text-green-700" />
```

## Links

- [Release notes](https://github.com/tmcknight/Blazor.Heroicons/releases)
- [NuGet package](https://www.nuget.org/packages/Blazor.Heroicons)
- [License](https://github.com/tmcknight/Blazor.Heroicons/blob/main/LICENSE)

## License

This library is MIT licensed.
