namespace HeroiconsGenerator;

internal record IconSetDefinition(string IconType, string RelativePath);

internal static class IconSetDefinitions
{
    public static readonly IReadOnlyList<IconSetDefinition> All =
    [
        new("Micro", Path.Combine("16", "solid")),
        new("Mini", Path.Combine("20", "solid")),
        new("Solid", Path.Combine("24", "solid")),
        new("Outline", Path.Combine("24", "outline")),
    ];

    public static (string iconType, string svgDir)[] Resolve(string optimizedDir)
    {
        return All
            .Select(d => (d.IconType, Path.Combine(optimizedDir, d.RelativePath)))
            .ToArray();
    }
}
