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

        try
        {
            // Get latest release info
            Console.WriteLine("Getting latest release info...");
            var release = await releaseProvider.GetLatestReleaseAsync();
            Console.WriteLine($"Downloading {release.Version}...");

            var readmeUpdater = new ReadmeUpdater(rootPath);
            var componentGenerator = new IconGenerator(rootPath);
            var registryGenerator = new RegistryGenerator(rootPath);

            // Update readme badge and version file
            readmeUpdater.UpdateReadme(release.Version);

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
                componentGenerator.CreateBlazorComponents(iconType, svgDir);

            // Generate static registry
            registryGenerator.GenerateHeroiconRegistry(iconSets);

            // Update HeroiconName.cs
            Console.WriteLine("Updating HeroiconName class");
            registryGenerator.UpdateHeroiconNameClass(Path.Combine(optimizedDir, "20", "solid"));

            Console.WriteLine("Done!");
        }
        finally
        {
            // Clean up temp directory even if an error occurs
            if (Directory.Exists(tmpDir))
            {
                Console.WriteLine("Cleaning up temp files...");
                Directory.Delete(tmpDir, true);
            }
        }
    }
}
