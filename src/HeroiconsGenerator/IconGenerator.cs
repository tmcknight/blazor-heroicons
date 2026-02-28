using System.Text;

namespace HeroiconsGenerator;

internal class IconGenerator(string rootPath)
{
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
            var content = "@inherits HeroiconBase\n" + svgContent.Replace(
                "aria-hidden=\"true\"",
                "aria-hidden=\"true\" @attributes=\"AdditionalAttributes\"");

            var name = ToPascalCase(Path.GetFileNameWithoutExtension(file));
            File.WriteAllText(Path.Combine(componentDir, $"{name}Icon.razor"), content);
        }

        Console.WriteLine($"Created {svgFiles.Length} {iconType} razor components");
    }

    public void UpdateHeroiconNameClass(string svgDir)
    {
        var files = Directory.GetFiles(svgDir, "*.svg")
            .Select(f => Path.GetFileName(f))
            .Order()
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .ToList();

        using var writer = new StreamWriter(Path.Combine(rootPath, "src", "Blazor.Heroicons", "HeroiconName.cs"));
        writer.NewLine = "\n";
        writer.WriteLine("namespace Blazor.Heroicons;");
        writer.WriteLine();
        writer.WriteLine("public static class HeroiconName");
        writer.WriteLine("{");
        foreach (var file in files)
        {
            var iconName = ToTitleCase(file);
            var newModifier = iconName == "Equals" ? "new " : "";
            writer.WriteLine($"\tpublic {newModifier}const string {iconName} = \"{file}\";");
        }
        writer.WriteLine("}");
    }

    public void UpdateReadme(string version)
    {
        Console.WriteLine("Updating heroicons version in README...");
        var readmePath = Path.Combine(rootPath, "README.md");
        var lines = File.ReadAllLines(readmePath);
        for (var i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith("[![Heroicons version]"))
            {
                lines[i] = $"[![Heroicons version](https://img.shields.io/badge/heroicons-{version}-informational?style=flat-square)](https://github.com/tailwindlabs/heroicons/releases/tag/{version})";
            }
        }
        File.WriteAllLines(readmePath, lines);
        File.WriteAllText(Path.Combine(rootPath, ".heroicons-version"), version);
    }

    public void GenerateHeroiconRegistry(IReadOnlyList<(string iconType, string svgDir)> iconSets)
    {
        Console.WriteLine("Generating HeroiconRegistry...");

        // Collect all (iconType, pascalName) pairs
        var entries = new List<(string iconType, string pascalName)>();
        foreach (var (iconType, svgDir) in iconSets)
        {
            var svgFiles = Directory.GetFiles(svgDir, "*.svg");
            foreach (var file in svgFiles)
            {
                var pascalName = ToPascalCase(Path.GetFileNameWithoutExtension(file));
                entries.Add((iconType, pascalName));
            }
        }

        entries.Sort((a, b) =>
        {
            var cmp = string.Compare(a.iconType, b.iconType, StringComparison.Ordinal);
            return cmp != 0 ? cmp : string.Compare(a.pascalName, b.pascalName, StringComparison.Ordinal);
        });

        var outputPath = Path.Combine(rootPath, "src", "Blazor.Heroicons", "HeroiconRegistry.cs");
        using var writer = new StreamWriter(outputPath);
        writer.NewLine = "\n";
        writer.WriteLine("using System.Collections.Frozen;");
        writer.WriteLine();
        writer.WriteLine("namespace Blazor.Heroicons;");
        writer.WriteLine();
        writer.WriteLine("internal static class HeroiconRegistry");
        writer.WriteLine("{");
        writer.WriteLine("\tprivate static readonly FrozenDictionary<string, Type> _byKey = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)");
        writer.WriteLine("\t{");
        foreach (var (iconType, pascalName) in entries)
        {
            writer.WriteLine($"\t\t{{ \"Blazor.Heroicons.{iconType}.{pascalName}Icon\", typeof({iconType}.{pascalName}Icon) }},");
        }
        writer.WriteLine("\t}.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);");
        writer.WriteLine();

        // Group by icon type
        var iconTypes = entries.Select(e => e.iconType).Distinct().ToList();
        writer.WriteLine("\tprivate static readonly FrozenDictionary<HeroiconType, Type[]> _byType = new Dictionary<HeroiconType, Type[]>");
        writer.WriteLine("\t{");
        foreach (var iconType in iconTypes)
        {
            var typeEntries = entries.Where(e => e.iconType == iconType).ToList();
            writer.Write($"\t\t{{ HeroiconType.{iconType}, [");
            writer.Write(string.Join(", ", typeEntries.Select(e => $"typeof({iconType}.{e.pascalName}Icon)")));
            writer.WriteLine("] },");
        }
        writer.WriteLine("\t}.ToFrozenDictionary();");
        writer.WriteLine();

        writer.WriteLine("\tpublic static Type? Resolve(string key) => _byKey.GetValueOrDefault(key);");
        writer.WriteLine();
        writer.WriteLine("\tpublic static Type[] GetAll(HeroiconType type) => _byType.GetValueOrDefault(type) ?? [];");
        writer.WriteLine("}");

        Console.WriteLine($"Generated HeroiconRegistry with {entries.Count} entries");
    }

    internal static string ToPascalCase(string text)
    {
        return string.Concat(
            text.Replace("-", " ").Replace("_", " ")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(w => char.ToUpperInvariant(w[0]) + w[1..]));
    }

    internal static string ToTitleCase(string text)
    {
        var parts = text.Split('-');
        var result = new StringBuilder();
        foreach (var part in parts)
        {
            var capitalizeNext = true;
            foreach (var ch in part)
            {
                if (char.IsLetter(ch))
                {
                    result.Append(capitalizeNext ? char.ToUpperInvariant(ch) : ch);
                    capitalizeNext = false;
                }
                else
                {
                    result.Append(ch);
                    capitalizeNext = true;
                }
            }
        }
        return result.ToString();
    }
}
