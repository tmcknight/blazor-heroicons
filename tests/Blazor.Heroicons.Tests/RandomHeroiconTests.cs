namespace Blazor.Heroicons.Tests;

[TestClass]
public class RandomHeroiconTests : BunitTestContext
{
    [TestMethod]
    public void RendersWithDefaultAttributes()
    {
        // Arrange 
        //Act
        var cut = RenderComponent<RandomHeroicon>();
        // Assert
        Assert.AreEqual("h-6 w-6", cut.Find("svg").GetAttribute("class"));
        Assert.AreEqual(HeroiconType.Outline, cut.Instance.Type);
    }

    [TestMethod]
    public void ChangingIconTypeGetsNewRandom()
    {
        // Arrange 
        var cut = RenderComponent<RandomHeroicon>();
        var originalType = cut.Instance.Type;
        // Assert
        Assert.AreEqual("h-6 w-6", cut.Find("svg").GetAttribute("class"));
        Assert.AreEqual(HeroiconType.Outline, cut.Instance.Type);
        //Act
        cut.SetParametersAndRender(parameters => parameters
            .Add(p => p.Type, HeroiconType.Solid));
        //Assert
        Assert.AreNotEqual(originalType, cut.Instance.Type);
    }

    [TestMethod]
    public void ChangingUnmatchedAttributeRetainsIcon()
    {
        // Arrange 
        var cut = RenderComponent<RandomHeroicon>(parameters => parameters
            .Add(p => p.Type, HeroiconType.Solid));
        var originalType = cut.Instance.Type;

        //Act
        cut.SetParametersAndRender(parameters => parameters
            .AddUnmatched("class", "h-10 w-10"));

        // Assert
        Assert.AreEqual("h-10 w-10", cut.Find("svg").GetAttribute("class"));
        Assert.AreEqual(HeroiconType.Solid, cut.Instance.Type);
        Assert.AreEqual(originalType, cut.Instance.Type);
    }
}