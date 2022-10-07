# Blazor.Heroicons

A third-party Blazor component library for [Heroicons](https://heroicons.com).

[![Nuget package](https://img.shields.io/nuget/v/Blazor.Heroicons?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Blazor.Heroicons)
[![Heroicons version](https://img.shields.io/badge/heroicons-v2.0.12-informational?style=flat-square)](https://github.com/tailwindlabs/heroicons/releases/tag/v2.0.12)
[![License](https://img.shields.io/github/license/tmcknight/Blazor.Heroicons?style=flat-square)](LICENSE)

## Basic Usage

First, install `Blazor.Heroicons` from nuget:

```sh
dotnet add package Blazor.Heroicons
```

Now each icon can be used as a Blazor component:

```razor
@using Blazor.Heroicons.Solid

<SparklesIcon class="h-6 w-6 text-blue-500" />
<HandThumbUpIcon class="h-10 w-10 text-green-800" />
```

You can also reference an icon by name, using the `Heroicon` component:

```razor
@using Blazor.Heroicons

<Heroicon
  Name="sparkles"
  Type="Heroicon.IconType.Outline"
  class="h-10 w-10 text-yellow-600"
/>
```

[Browse the full list of icons on Heroicons &rarr;](https://heroicons.com)

## Links

- [Release notes](https://github.com/tmcknight/Blazor.Heroicons/releases)
- [NuGet package](https://www.nuget.org/packages/Blazor.Heroicons)
- [License](https://github.com/tmcknight/Blazor.Heroicons/blob/main/LICENSE)

## License

This library is MIT licensed.
