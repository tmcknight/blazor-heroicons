using HeroiconsGenerator;

var root = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
if (!File.Exists(Path.Combine(root, "Blazor.Heroicons.sln")))
{
    Console.Error.WriteLine("Error: Blazor.Heroicons.sln not found. Run from the repo root or pass it as an argument.");
    return 1;
}

var updater = new HeroiconsUpdater(root);
await updater.RunAsync();
return 0;
