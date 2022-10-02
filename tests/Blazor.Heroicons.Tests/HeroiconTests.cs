namespace Blazor.Heroicons.Tests;
using Bunit;
using Blazor.Heroicons;
using System.Reflection;
using Microsoft.AspNetCore.Components;

[TestClass]
public class HeroiconTests : BunitTestContext
{
    [TestMethod]
    public void RendersWithDefaultAttributes()
    {
        // Arrange 
        //Act
        var cut = RenderComponent<Heroicon>();
        // Assert
        Assert.AreEqual("h-6 w-6", cut.Find("svg").GetAttribute("class"));
        Assert.AreEqual("SparklesIcon", cut.Instance.Name);
        Assert.AreEqual(HeroiconType.Outline, cut.Instance.Type);
    }

    [TestMethod]
    public void RendersWithAdditionalAttribute()
    {
        // Arrange
        var additionalAttributes = new Dictionary<string, object>() {
            {"added-attribute", "wahoo"}
        };

        // Act
        var cut = RenderComponent<Heroicon>(parameters => parameters
            .Add(p => p.AdditionalAttributes, additionalAttributes));

        // Assert
        Assert.AreEqual("wahoo", cut.Find("svg").GetAttribute("added-attribute"));
    }

    [TestMethod]
    public void UnknownNameThrowsException()
    {
        // Arrange
        // Act
        try
        {
            var cut = RenderComponent<Heroicon>(parameters => parameters
                .Add(p => p.Name, "MythicalIcon"));
            Assert.Fail("An exception should have been thrown.");
        }
        catch (Exception ex)
        {
            //Assert
            Assert.AreEqual("Heroicon 'Outline.MythicalIcon' not found", ex.Message);
        }
    }

    [TestMethod]
    [DataRow("HandThumbUpIcon", DisplayName = "Fully qualified name")]
    [DataRow("HandThumbUp", DisplayName = "Name without Icon suffix")]
    [DataRow("handthumbup", DisplayName = "Lowercase name")]
    [DataRow("hand-thumb-up", DisplayName = "Name with hyphens")]
    public void NameRendersCorrectly(string iconName)
    {
        // Arrange
        // Act
        var cut = RenderComponent<Heroicon>(parameters => parameters
            .Add(p => p.Name, iconName));
        //Assert
        cut.MarkupMatches("<svg diff:ignore></svg>");
    }

    [TestMethod]
    [DataRow(HeroiconType.Solid)]
    [DataRow(HeroiconType.Outline)]
    [DataRow(HeroiconType.Mini)]
    public void AllIconsRenderCorrectly(HeroiconType heroiconType)
    {
        // Arrange
        var icons = Assembly.GetExecutingAssembly().GetTypes()
                            .Where(t => t.Namespace == $"Blazor.Heroicons.{heroiconType}")
                            .ToList();

        // Act
        foreach (var icon in icons)
        {
            // Act
            var cut = RenderComponent<DynamicComponent>(parameters => parameters
                .Add(p => p.Type, icon));
            //Assert
            cut.MarkupMatches("<svg diff:ignore></svg>");
        }
    }
}