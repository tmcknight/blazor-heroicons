using System.Formats.Tar;
using System.IO.Compression;
using System.Text.Json;

namespace HeroiconsGenerator;

internal class GitHubReleaseProvider : IReleaseProvider, IDisposable
{
    private readonly HttpClient _http = new();

    public GitHubReleaseProvider()
    {
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("HeroiconsGenerator/1.0");
    }

    public async Task<ReleaseInfo> GetLatestReleaseAsync()
    {
        var json = await _http.GetStringAsync(
            "https://api.github.com/repos/tailwindlabs/heroicons/releases/latest");
        using var doc = JsonDocument.Parse(json);
        var version = doc.RootElement.GetProperty("tag_name").GetString()!;
        var tarballUrl = doc.RootElement.GetProperty("tarball_url").GetString()!;
        return new ReleaseInfo(version, tarballUrl);
    }

    public async Task<string> DownloadAndExtractAsync(string tarballUrl, string targetDir)
    {
        Directory.CreateDirectory(targetDir);
        await using var tarballStream = await _http.GetStreamAsync(tarballUrl);
        await using var gzipStream = new GZipStream(tarballStream, CompressionMode.Decompress);
        await TarFile.ExtractToDirectoryAsync(gzipStream, targetDir, overwriteFiles: true);

        var optimizedDir = Directory.GetDirectories(targetDir)
            .Select(d => Path.Combine(d, "optimized"))
            .FirstOrDefault(Directory.Exists)
            ?? throw new DirectoryNotFoundException(
                "Could not find optimized/ directory in extracted tarball.");

        return optimizedDir;
    }

    public void Dispose() => _http.Dispose();
}
