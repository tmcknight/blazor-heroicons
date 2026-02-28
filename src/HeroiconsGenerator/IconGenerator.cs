using System.Text.RegularExpressions;

namespace HeroiconsGenerator;

internal partial class IconGenerator(string rootPath)
{
    // Matches the closing > of the opening <svg ...> tag
    [GeneratedRegex(@"(<svg\b[^>]*)(>)")]
    private static partial Regex SvgOpeningTagRegex();

    public void CreateBlazorComponents(string iconType, string svgDir)
    {
        Console.WriteLine($"Creating {iconType} razor components...");

        var componentDir = Path.Combine(rootPath, "src", "Blazor.Heroicons", iconType);
        if (Directory.Exists(componentDir))
            Directory.Delete(componentDir, true);
        Directory.CreateDirectory(componentDir);

        var svgFiles = Directory.GetFiles(svgDir, "*.svg");
        foreach (var file in svgFiles)
        {
            var svgContent = File.ReadAllText(file);
            var content = "@inherits HeroiconBase\n" +
                SvgOpeningTagRegex().Replace(svgContent, "$1 @attributes=\"AdditionalAttributes\"$2", count: 1);

            var name = NamingHelper.ToPascalCase(Path.GetFileNameWithoutExtension(file));
            File.WriteAllText(Path.Combine(componentDir, $"{name}Icon.razor"), content);
        }

        Console.WriteLine($"Created {svgFiles.Length} {iconType} razor components");
    }

    // Keep static helper accessible for tests that reference IconGenerator.ToPascalCase
    internal static string ToPascalCase(string text) => NamingHelper.ToPascalCase(text);
}
