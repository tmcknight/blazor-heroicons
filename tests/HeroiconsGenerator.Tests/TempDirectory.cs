namespace HeroiconsGenerator.Tests;

internal sealed class TempDirectory : IDisposable
{
	public string Path { get; }

	public TempDirectory(string prefix = "heroicons-test")
	{
		Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{prefix}-{Guid.NewGuid():N}");
		Directory.CreateDirectory(Path);
	}

	public string CreateSubdirectory(params string[] segments)
	{
		var dir = System.IO.Path.Combine([Path, .. segments]);
		Directory.CreateDirectory(dir);
		return dir;
	}

	public void Dispose()
	{
		if (Directory.Exists(Path))
			Directory.Delete(Path, true);
	}
}
