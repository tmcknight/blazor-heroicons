namespace HeroiconsGenerator;

internal class ReadmeUpdater(string rootPath)
{
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
}
