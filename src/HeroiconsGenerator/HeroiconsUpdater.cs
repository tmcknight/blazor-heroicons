using System.Formats.Tar;
using System.IO.Compression;
using System.Text.Json;

namespace HeroiconsGenerator;

internal class HeroiconsUpdater(string rootPath)
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
        using var http = new HttpClient();
        http.DefaultRequestHeaders.UserAgent.ParseAdd("HeroiconsGenerator/1.0");

        var json = await http.GetStringAsync("https://api.github.com/repos/tailwindlabs/heroicons/releases/latest");
        using var doc = JsonDocument.Parse(json);
        var version = doc.RootElement.GetProperty("tag_name").GetString()!;
        var tarballUrl = doc.RootElement.GetProperty("tarball_url").GetString()!;

        Console.WriteLine($"Downloading {version}...");

        var generator = new IconGenerator(rootPath);

        // Update readme badge and version file
        generator.UpdateReadme(version);

        // Download and extract tarball
        Directory.CreateDirectory(tmpDir);
        await using var tarballStream = await http.GetStreamAsync(tarballUrl);
        await using var gzipStream = new GZipStream(tarballStream, CompressionMode.Decompress);
        await TarFile.ExtractToDirectoryAsync(gzipStream, tmpDir, overwriteFiles: true);

        // Find the optimized directory inside the extracted content
        var optimizedDir = Directory.GetDirectories(tmpDir)
            .Select(d => Path.Combine(d, "optimized"))
            .FirstOrDefault(Directory.Exists)
            ?? throw new DirectoryNotFoundException("Could not find optimized/ directory in extracted tarball.");

        Console.WriteLine("Looping through svg files and creating razor components...");
        generator.CreateBlazorComponents("Micro", Path.Combine(optimizedDir, "16", "solid"));
        generator.CreateBlazorComponents("Mini", Path.Combine(optimizedDir, "20", "solid"));
        generator.CreateBlazorComponents("Solid", Path.Combine(optimizedDir, "24", "solid"));
        generator.CreateBlazorComponents("Outline", Path.Combine(optimizedDir, "24", "outline"));

        // Update HeroiconName.cs
        Console.WriteLine("Updating HeroiconName class");
        generator.UpdateHeroiconNameClass(Path.Combine(optimizedDir, "20", "solid"));

        // Clean up
        Console.WriteLine("Cleaning up temp files...");
        Directory.Delete(tmpDir, true);

        Console.WriteLine("Done!");
    }
}
