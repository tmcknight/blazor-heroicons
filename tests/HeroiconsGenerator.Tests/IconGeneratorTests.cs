using HeroiconsGenerator;

namespace HeroiconsGenerator.Tests;

[TestClass]
public class IconGeneratorTests
{
	[TestMethod]
	[DataRow("academic-cap", "AcademicCap")]
	[DataRow("arrow-down", "ArrowDown")]
	[DataRow("hand-thumb-up", "HandThumbUp")]
	[DataRow("x-mark", "XMark")]
	[DataRow("cpu-chip", "CpuChip")]
	[DataRow("h1", "H1")]
	[DataRow("bars-3", "Bars3")]
	[DataRow("building-office-2", "BuildingOffice2")]
	[DataRow("single", "Single")]
	public void ToPascalCase_ConvertsHyphenatedNames(string input, string expected)
	{
		Assert.AreEqual(expected, NamingHelper.ToPascalCase(input));
	}

	[TestMethod]
	[DataRow("underscored_name", "UnderscoredName")]
	[DataRow("mixed-hyphen_underscore", "MixedHyphenUnderscore")]
	public void ToPascalCase_ConvertsUnderscores(string input, string expected)
	{
		Assert.AreEqual(expected, NamingHelper.ToPascalCase(input));
	}

	[TestMethod]
	[DataRow("academic-cap", "AcademicCap")]
	[DataRow("hand-thumb-up", "HandThumbUp")]
	[DataRow("x-mark", "XMark")]
	[DataRow("bars-3", "Bars3")]
	[DataRow("building-office-2", "BuildingOffice2")]
	[DataRow("h1", "H1")]
	[DataRow("equals", "Equals")]
	public void ToPascalCase_ConvertsForTitleUse(string input, string expected)
	{
		Assert.AreEqual(expected, NamingHelper.ToPascalCase(input));
	}

	[TestMethod]
	public void CreateBlazorComponents_CreatesRazorFiles()
	{
		using var tmp = new TempDirectory();
		var svgDir = tmp.CreateSubdirectory("svgs");

		File.WriteAllText(Path.Combine(svgDir, "arrow-down.svg"),
			"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 16 16\" fill=\"currentColor\" aria-hidden=\"true\"><path d=\"M1 2\"/></svg>");
		File.WriteAllText(Path.Combine(svgDir, "hand-thumb-up.svg"),
			"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 16 16\" fill=\"currentColor\" aria-hidden=\"true\"><path d=\"M3 4\"/></svg>");

		var generator = new IconGenerator(tmp.Path);
		generator.CreateBlazorComponents("TestType", svgDir);

		var componentDir = Path.Combine(tmp.Path, "src", "Blazor.Heroicons", "TestType");
		Assert.IsTrue(Directory.Exists(componentDir));
		var files = Directory.GetFiles(componentDir, "*.razor");
		Assert.AreEqual(2, files.Length);

		Assert.IsTrue(File.Exists(Path.Combine(componentDir, "ArrowDownIcon.razor")));
		Assert.IsTrue(File.Exists(Path.Combine(componentDir, "HandThumbUpIcon.razor")));

		var content = File.ReadAllText(Path.Combine(componentDir, "ArrowDownIcon.razor"));
		Assert.IsTrue(content.StartsWith("@inherits HeroiconBase\n"));
		Assert.IsTrue(content.Contains("@attributes=\"AdditionalAttributes\""));
		Assert.IsFalse(content.Contains("aria-hidden=\"true\" aria-hidden"));
	}

	[TestMethod]
	public void CreateBlazorComponents_CleansExistingDirectory()
	{
		using var tmp = new TempDirectory();
		var componentDir = tmp.CreateSubdirectory("src", "Blazor.Heroicons", "TestType");
		var svgDir = tmp.CreateSubdirectory("svgs");

		File.WriteAllText(Path.Combine(componentDir, "StaleIcon.razor"), "old content");
		File.WriteAllText(Path.Combine(svgDir, "arrow-down.svg"),
			"<svg xmlns=\"http://www.w3.org/2000/svg\" aria-hidden=\"true\"><path d=\"M1 2\"/></svg>");

		var generator = new IconGenerator(tmp.Path);
		generator.CreateBlazorComponents("TestType", svgDir);

		Assert.IsFalse(File.Exists(Path.Combine(componentDir, "StaleIcon.razor")));
		Assert.AreEqual(1, Directory.GetFiles(componentDir, "*.razor").Length);
	}

	[TestMethod]
	public void UpdateHeroiconNameClass_GeneratesCorrectFile()
	{
		using var tmp = new TempDirectory();
		var heroiconsDir = tmp.CreateSubdirectory("src", "Blazor.Heroicons");
		var svgDir = tmp.CreateSubdirectory("svgs");

		File.WriteAllText(Path.Combine(svgDir, "arrow-down.svg"), "<svg/>");
		File.WriteAllText(Path.Combine(svgDir, "hand-thumb-up.svg"), "<svg/>");
		File.WriteAllText(Path.Combine(svgDir, "equals.svg"), "<svg/>");

		var generator = new RegistryGenerator(tmp.Path);
		generator.UpdateHeroiconNameClass(svgDir);

		var outputPath = Path.Combine(heroiconsDir, "HeroiconName.cs");
		Assert.IsTrue(File.Exists(outputPath));

		var content = File.ReadAllText(outputPath);
		Assert.IsTrue(content.Contains("namespace Blazor.Heroicons;"));
		Assert.IsTrue(content.Contains("public static class HeroiconName"));
		Assert.IsTrue(content.Contains("public const string ArrowDown = \"arrow-down\";"));
		Assert.IsTrue(content.Contains("public const string HandThumbUp = \"hand-thumb-up\";"));
		Assert.IsTrue(content.Contains("public new const string Equals = \"equals\";"));
	}

	[TestMethod]
	public void UpdateHeroiconNameClass_SortsAlphabetically()
	{
		using var tmp = new TempDirectory();
		tmp.CreateSubdirectory("src", "Blazor.Heroicons");
		var svgDir = tmp.CreateSubdirectory("svgs");

		File.WriteAllText(Path.Combine(svgDir, "z-icon.svg"), "<svg/>");
		File.WriteAllText(Path.Combine(svgDir, "a-icon.svg"), "<svg/>");
		File.WriteAllText(Path.Combine(svgDir, "m-icon.svg"), "<svg/>");

		var generator = new RegistryGenerator(tmp.Path);
		generator.UpdateHeroiconNameClass(svgDir);

		var content = File.ReadAllText(Path.Combine(tmp.Path, "src", "Blazor.Heroicons", "HeroiconName.cs"));
		var aIndex = content.IndexOf("AIcon");
		var mIndex = content.IndexOf("MIcon");
		var zIndex = content.IndexOf("ZIcon");
		Assert.IsTrue(aIndex < mIndex && mIndex < zIndex, "Icons should be sorted alphabetically");
	}

	[TestMethod]
	public void GenerateHeroiconRegistry_ProducesCorrectOutput()
	{
		using var tmp = new TempDirectory();
		var heroiconsDir = tmp.CreateSubdirectory("src", "Blazor.Heroicons");
		var svgDir1 = tmp.CreateSubdirectory("svgs", "solid");
		var svgDir2 = tmp.CreateSubdirectory("svgs", "outline");

		File.WriteAllText(Path.Combine(svgDir1, "arrow-down.svg"), "<svg/>");
		File.WriteAllText(Path.Combine(svgDir1, "hand-thumb-up.svg"), "<svg/>");
		File.WriteAllText(Path.Combine(svgDir2, "arrow-down.svg"), "<svg/>");

		var generator = new RegistryGenerator(tmp.Path);
		generator.GenerateHeroiconRegistry([
			("Solid", svgDir1),
			("Outline", svgDir2),
		]);

		var outputPath = Path.Combine(heroiconsDir, "HeroiconRegistry.cs");
		Assert.IsTrue(File.Exists(outputPath));

		var content = File.ReadAllText(outputPath);
		Assert.IsTrue(content.Contains("using System.Collections.Frozen;"));
		Assert.IsTrue(content.Contains("internal static class HeroiconRegistry"));
		Assert.IsTrue(content.Contains("StringComparer.OrdinalIgnoreCase"),
			"Registry should use case-insensitive keys");
		Assert.IsTrue(content.Contains("\"Blazor.Heroicons.Solid.ArrowDownIcon\", typeof(Solid.ArrowDownIcon)"));
		Assert.IsTrue(content.Contains("\"Blazor.Heroicons.Solid.HandThumbUpIcon\", typeof(Solid.HandThumbUpIcon)"));
		Assert.IsTrue(content.Contains("\"Blazor.Heroicons.Outline.ArrowDownIcon\", typeof(Outline.ArrowDownIcon)"));
		Assert.IsTrue(content.Contains("HeroiconType.Solid"));
		Assert.IsTrue(content.Contains("HeroiconType.Outline"));
		Assert.IsTrue(content.Contains("Resolve(string key)"));
		Assert.IsTrue(content.Contains("GetAll(HeroiconType type)"));
	}

	[TestMethod]
	public void UpdateReadme_UpdatesBadgeAndVersionFile()
	{
		using var tmp = new TempDirectory();

		File.WriteAllLines(Path.Combine(tmp.Path, "README.md"),
		[
			"# Blazor Heroicons",
			"[![Heroicons version](https://img.shields.io/badge/heroicons-v1.0.0-informational?style=flat-square)](https://github.com/tailwindlabs/heroicons/releases/tag/v1.0.0)",
			"Some other content"
		]);

		var updater = new ReadmeUpdater(tmp.Path);
		updater.UpdateReadme("v2.5.0");

		var lines = File.ReadAllLines(Path.Combine(tmp.Path, "README.md"));
		Assert.AreEqual("# Blazor Heroicons", lines[0]);
		Assert.IsTrue(lines[1].Contains("v2.5.0"));
		Assert.IsTrue(lines[1].Contains("heroicons-v2.5.0-informational"));
		Assert.AreEqual("Some other content", lines[2]);

		var versionFile = File.ReadAllText(Path.Combine(tmp.Path, ".heroicons-version"));
		Assert.AreEqual("v2.5.0", versionFile);
	}

	[TestMethod]
	public void UpdateReadme_PreservesNonBadgeLines()
	{
		using var tmp = new TempDirectory();

		File.WriteAllLines(Path.Combine(tmp.Path, "README.md"),
		[
			"Line 1",
			"Line 2",
			"Line 3"
		]);

		var updater = new ReadmeUpdater(tmp.Path);
		updater.UpdateReadme("v2.0.0");

		var lines = File.ReadAllLines(Path.Combine(tmp.Path, "README.md"));
		Assert.AreEqual(3, lines.Length);
		Assert.AreEqual("Line 1", lines[0]);
		Assert.AreEqual("Line 2", lines[1]);
		Assert.AreEqual("Line 3", lines[2]);
	}

	[TestMethod]
	public void CreateBlazorComponents_ThrowsForMissingSvgDirectory()
	{
		using var tmp = new TempDirectory();
		var generator = new IconGenerator(tmp.Path);
		Assert.ThrowsExactly<DirectoryNotFoundException>(() =>
			generator.CreateBlazorComponents("TestType", Path.Combine(tmp.Path, "nonexistent")));
	}

	[TestMethod]
	public void CreateBlazorComponents_EmptySvgDirectory_CreatesEmptyComponentDir()
	{
		using var tmp = new TempDirectory();
		var svgDir = tmp.CreateSubdirectory("svgs");

		var generator = new IconGenerator(tmp.Path);
		generator.CreateBlazorComponents("TestType", svgDir);

		var componentDir = Path.Combine(tmp.Path, "src", "Blazor.Heroicons", "TestType");
		Assert.IsTrue(Directory.Exists(componentDir));
		Assert.AreEqual(0, Directory.GetFiles(componentDir, "*.razor").Length);
	}

	[TestMethod]
	public void GenerateHeroiconRegistry_EmptyIconSets_ProducesValidFile()
	{
		using var tmp = new TempDirectory();
		tmp.CreateSubdirectory("src", "Blazor.Heroicons");

		var generator = new RegistryGenerator(tmp.Path);
		generator.GenerateHeroiconRegistry([]);

		var content = File.ReadAllText(Path.Combine(tmp.Path, "src", "Blazor.Heroicons", "HeroiconRegistry.cs"));
		Assert.IsTrue(content.Contains("internal static class HeroiconRegistry"));
		Assert.IsTrue(content.Contains("Resolve(string key)"));
	}

	[TestMethod]
	public void UpdateReadme_MissingReadmeFile_Throws()
	{
		using var tmp = new TempDirectory();
		var updater = new ReadmeUpdater(tmp.Path);
		Assert.ThrowsExactly<FileNotFoundException>(() => updater.UpdateReadme("v1.0.0"));
	}
}
