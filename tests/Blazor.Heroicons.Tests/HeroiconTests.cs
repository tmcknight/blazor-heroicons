namespace Blazor.Heroicons.Tests;
using Bunit;
using Blazor.Heroicons;

[TestClass]
public class HeroiconTests : BunitTestContext
{
    [TestMethod]
    public void HeroiconRendersWithDefaultCssClass()
    {
        // Act
        var cut = RenderComponent<Heroicon>();

        // Assert
        cut.Find("svg").HasAttribute("class");
        cut.MarkupMatches(@"<svg class=""h-8 w-6"" diff:ignore></svg>");
    }
}