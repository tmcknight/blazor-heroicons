namespace HeroiconsGenerator;

internal record ReleaseInfo(string Version, string TarballUrl);

internal interface IReleaseProvider
{
    Task<ReleaseInfo> GetLatestReleaseAsync();
    Task<string> DownloadAndExtractAsync(string tarballUrl, string targetDir);
}
