using Blazor.Heroicons.Outline;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Blazor.Heroicons.Tests;
[TestClass]
public class HeroiconTests : BunitContext
{
    [TestMethod]
    public void RendersWithDefaultAttributes()
    {
        // Arrange 
        //Act
        var cut = Render<Heroicon>();
        // Assert
        var sparklesIcon = Render<SparklesIcon>();
        Assert.IsFalse(cut.Find("svg").HasAttribute("class"));
        Assert.AreEqual(HeroiconName.Sparkles, cut.Instance.Name);
        Assert.AreEqual(HeroiconType.Outline, cut.Instance.Type);
        Assert.AreEqual(sparklesIcon.Markup, cut.Markup);
    }

    [TestMethod]
    public void RendersWithAdditionalAttribute()
    {
        // Arrange
        // Act
        var cut = Render<Heroicon>(parameters => parameters
            .AddUnmatched("added-attribute", "wahoo"));

        // Assert
        Assert.AreEqual("wahoo", cut.Find("svg").GetAttribute("added-attribute"));
    }

    [TestMethod]
    public void UnknownNameRendersQuestionMark()
    {
        // Arrange
        var iconName = "MythicalIcon";
        // Act
        var cut = Render<Heroicon>(parameters => parameters
            .Add(p => p.Name, iconName));
        var questionMark = Render<QuestionMarkCircleIcon>();
        //Assert
        Assert.AreEqual(questionMark.Markup, cut.Markup);
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
        var cut = Render<Heroicon>(parameters => parameters
            .Add(p => p.Name, iconName));
        //Assert
        cut.MarkupMatches("<svg diff:ignore></svg>");
    }

    [TestMethod]
    [DataRow(HeroiconType.Solid)]
    [DataRow(HeroiconType.Outline)]
    [DataRow(HeroiconType.Mini)]
    [DataRow(HeroiconType.Micro)]
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
            var cut = Render<DynamicComponent>(parameters => parameters
                .Add(p => p.Type, icon));
            //Assert
            cut.MarkupMatches("<svg diff:ignore></svg>");
        }
    }

    [TestMethod]
    public void HeroiconRendersNameChange()
    {
        // Arrange 
        //Act
        var cut = Render<Heroicon>();
        // Assert
        Assert.AreEqual(HeroiconName.Sparkles, cut.Instance.Name);
        var sparkles = cut.Markup;
        cut.Render(parameters => parameters
            .Add(p => p.Name, "HandThumbUpIcon"));
        Assert.AreEqual("HandThumbUpIcon", cut.Instance.Name);
        Assert.AreNotEqual(sparkles, cut.Markup);
    }

    [TestMethod]
    public void HeroiconRendersTypeChange()
    {
        // Arrange 
        //Act
        var cut = Render<Heroicon>();
        // Assert
        Assert.AreEqual(HeroiconType.Outline, cut.Instance.Type);
        var sparkles = cut.Markup;
        cut.Render(parameters => parameters
            .Add(p => p.Type, HeroiconType.Solid));
        Assert.AreEqual(HeroiconType.Solid, cut.Instance.Type);
        Assert.AreNotEqual(sparkles, cut.Markup);
    }

    [TestMethod]
    public void ChangingUnmatchedAttributeRetainsIcon()
    {
        // Arrange 
        var cut = Render<Heroicon>(parameters => parameters
            .Add(p => p.Name, "HandThumbUpIcon")
            .Add(p => p.Type, HeroiconType.Solid));

        //Act
        cut.Render(parameters => parameters
            .AddUnmatched("class", "h-10 w-10"));

        // Assert
        Assert.AreEqual("h-10 w-10", cut.Find("svg").GetAttribute("class"));
        Assert.AreEqual("HandThumbUpIcon", cut.Instance.Name);
        Assert.AreEqual(HeroiconType.Solid, cut.Instance.Type);
    }
}