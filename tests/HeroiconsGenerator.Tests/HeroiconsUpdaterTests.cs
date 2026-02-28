using HeroiconsGenerator;

namespace HeroiconsGenerator.Tests;

[TestClass]
public class HeroiconsUpdaterTests
{
    private static readonly string SvgContent =
        "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 16 16\" fill=\"currentColor\" aria-hidden=\"true\"><path d=\"M1 2\"/></svg>";

    private class FakeReleaseProvider : IReleaseProvider
    {
        public string Version { get; set; } = "v2.1.0";
        public string? ExtractedOptimizedDir { get; private set; }

        public Task<ReleaseInfo> GetLatestReleaseAsync()
            => Task.FromResult(new ReleaseInfo(Version, "https://fake/tarball.tar.gz"));

        public Task<string> DownloadAndExtractAsync(string tarballUrl, string targetDir)
        {
            // Create the directory structure that the real provider would produce
            var optimizedDir = Path.Combine(targetDir, "heroicons-abc123", "optimized");

            var dirs = new[]
            {
                Path.Combine(optimizedDir, "16", "solid"),
                Path.Combine(optimizedDir, "20", "solid"),
                Path.Combine(optimizedDir, "24", "solid"),
                Path.Combine(optimizedDir, "24", "outline"),
            };

            foreach (var dir in dirs)
            {
                Directory.CreateDirectory(dir);
                File.WriteAllText(Path.Combine(dir, "arrow-down.svg"), SvgContent);
                File.WriteAllText(Path.Combine(dir, "hand-thumb-up.svg"), SvgContent);
            }

            ExtractedOptimizedDir = optimizedDir;
            return Task.FromResult(optimizedDir);
        }
    }

    private static TempDirectory CreateTestRoot()
    {
        var tmp = new TempDirectory("heroicons-updater-test");

        // Create required project structure
        tmp.CreateSubdirectory("src", "Blazor.Heroicons");

        // Create a README with a badge line
        File.WriteAllLines(Path.Combine(tmp.Path, "README.md"),
        [
            "# Blazor Heroicons",
            "[![Heroicons version](https://img.shields.io/badge/heroicons-v1.0.0-informational?style=flat-square)](https://github.com/tailwindlabs/heroicons/releases/tag/v1.0.0)",
            "Some other content"
        ]);

        return tmp;
    }

    [TestMethod]
    public async Task RunAsync_CreatesBlazorComponentsForAllIconSets()
    {
        using var tmp = CreateTestRoot();
        var provider = new FakeReleaseProvider();
        var updater = new HeroiconsUpdater(tmp.Path, provider);
        await updater.RunAsync();

        var heroiconsDir = Path.Combine(tmp.Path, "src", "Blazor.Heroicons");
        foreach (var iconType in new[] { "Micro", "Mini", "Solid", "Outline" })
        {
            var dir = Path.Combine(heroiconsDir, iconType);
            Assert.IsTrue(Directory.Exists(dir), $"{iconType} directory should exist");
            var files = Directory.GetFiles(dir, "*.razor");
            Assert.AreEqual(2, files.Length, $"{iconType} should have 2 razor files");
        }
    }

    [TestMethod]
    public async Task RunAsync_UpdatesReadmeAndVersionFile()
    {
        using var tmp = CreateTestRoot();
        var provider = new FakeReleaseProvider { Version = "v2.5.0" };
        var updater = new HeroiconsUpdater(tmp.Path, provider);
        await updater.RunAsync();

        var readme = File.ReadAllText(Path.Combine(tmp.Path, "README.md"));
        Assert.IsTrue(readme.Contains("v2.5.0"), "README should contain new version");

        var versionFile = File.ReadAllText(Path.Combine(tmp.Path, ".heroicons-version"));
        Assert.AreEqual("v2.5.0", versionFile);
    }

    [TestMethod]
    public async Task RunAsync_CleansUpTempDirectory()
    {
        using var tmp = CreateTestRoot();
        var provider = new FakeReleaseProvider();
        var updater = new HeroiconsUpdater(tmp.Path, provider);
        await updater.RunAsync();

        var tmpDir = Path.Combine(tmp.Path, "tmp");
        Assert.IsFalse(Directory.Exists(tmpDir), "tmp directory should be cleaned up after run");
    }

    [TestMethod]
    public async Task RunAsync_GeneratesRegistryAndNameClass()
    {
        using var tmp = CreateTestRoot();
        var provider = new FakeReleaseProvider();
        var updater = new HeroiconsUpdater(tmp.Path, provider);
        await updater.RunAsync();

        var heroiconsDir = Path.Combine(tmp.Path, "src", "Blazor.Heroicons");

        var registryPath = Path.Combine(heroiconsDir, "HeroiconRegistry.cs");
        Assert.IsTrue(File.Exists(registryPath), "HeroiconRegistry.cs should be generated");
        var registryContent = File.ReadAllText(registryPath);
        Assert.IsTrue(registryContent.Contains("internal static class HeroiconRegistry"));

        var namePath = Path.Combine(heroiconsDir, "HeroiconName.cs");
        Assert.IsTrue(File.Exists(namePath), "HeroiconName.cs should be generated");
        var nameContent = File.ReadAllText(namePath);
        Assert.IsTrue(nameContent.Contains("public static class HeroiconName"));
        Assert.IsTrue(nameContent.Contains("ArrowDown"));
        Assert.IsTrue(nameContent.Contains("HandThumbUp"));
    }
}
