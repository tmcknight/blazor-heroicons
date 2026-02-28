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
            var iconSets = IconSetDefinitions.Resolve(optimizedDir);

            foreach (var (iconType, svgDir) in iconSets)
                componentGenerator.CreateBlazorComponents(iconType, svgDir);

            // Generate static registry
            registryGenerator.GenerateHeroiconRegistry(iconSets);

            // Update HeroiconName.cs — use Mini (20/solid) as the canonical icon name source
            Console.WriteLine("Updating HeroiconName class");
            var miniDef = IconSetDefinitions.All.First(d => d.IconType == "Mini");
            registryGenerator.UpdateHeroiconNameClass(Path.Combine(optimizedDir, miniDef.RelativePath));

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
