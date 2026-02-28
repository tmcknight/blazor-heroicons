namespace HeroiconsGenerator;

internal class HeroiconsUpdater(string rootPath, IReleaseProvider releaseProvider)
{
    public async Task RunAsync()
    {
        var tmpDir = Path.Combine(rootPath, "tmp");

        // Clean up prior run
        if (Directory.Exists(tmpDir))
        {
            Console.WriteLine("Cleaning up existing tmp files...");
            Directory.Delete(tmpDir, true);
        }

        // Get latest release info
        Console.WriteLine("Getting latest release info...");
        var release = await releaseProvider.GetLatestReleaseAsync();
        Console.WriteLine($"Downloading {release.Version}...");

        var generator = new IconGenerator(rootPath);

        // Update readme badge and version file
        generator.UpdateReadme(release.Version);

        // Download and extract tarball
        var optimizedDir = await releaseProvider.DownloadAndExtractAsync(release.TarballUrl, tmpDir);

        Console.WriteLine("Looping through svg files and creating razor components...");
        var iconSets = new (string iconType, string svgDir)[]
        {
            ("Micro", Path.Combine(optimizedDir, "16", "solid")),
            ("Mini", Path.Combine(optimizedDir, "20", "solid")),
            ("Solid", Path.Combine(optimizedDir, "24", "solid")),
            ("Outline", Path.Combine(optimizedDir, "24", "outline")),
        };

        foreach (var (iconType, svgDir) in iconSets)
            generator.CreateBlazorComponents(iconType, svgDir);

        // Generate static registry
        generator.GenerateHeroiconRegistry(iconSets);

        // Update HeroiconName.cs
        Console.WriteLine("Updating HeroiconName class");
        generator.UpdateHeroiconNameClass(Path.Combine(optimizedDir, "20", "solid"));

        // Clean up
        Console.WriteLine("Cleaning up temp files...");
        Directory.Delete(tmpDir, true);

        Console.WriteLine("Done!");
    }
}
