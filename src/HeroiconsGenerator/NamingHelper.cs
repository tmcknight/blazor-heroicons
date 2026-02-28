namespace HeroiconsGenerator;

internal static class NamingHelper
{
    public static string ToPascalCase(string text)
    {
        return string.Concat(
            text.Replace("-", " ").Replace("_", " ")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(w => char.ToUpperInvariant(w[0]) + w[1..]));
    }
}
