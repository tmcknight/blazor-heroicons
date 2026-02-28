namespace Blazor.Heroicons.Tests;

[TestClass]
public class RandomHeroiconTests : BunitContext
{
    [TestMethod]
    public void RendersWithDefaultAttributes()
    {
        // Arrange 
        //Act
        var cut = Render<RandomHeroicon>();
        // Assert
        Assert.IsFalse(cut.Find("svg").HasAttribute("class"));
        Assert.AreEqual(HeroiconType.Outline, cut.Instance.Type);
    }

    [TestMethod]
    public void ChangingIconTypeGetsNewRandom()
    {
        // Arrange 
        var cut = Render<RandomHeroicon>();
        var originalType = cut.Instance.Type;
        // Assert
        Assert.AreEqual(HeroiconType.Outline, cut.Instance.Type);
        //Act
        cut.Render(parameters => parameters
            .Add(p => p.Type, HeroiconType.Solid));
        //Assert
        Assert.AreNotEqual(originalType, cut.Instance.Type);
    }

    [TestMethod]
    public void ChangingUnmatchedAttributeRetainsIcon()
    {
        // Arrange
        var cut = Render<RandomHeroicon>(parameters => parameters
            .Add(p => p.Type, HeroiconType.Solid));
        var originalType = cut.Instance.Type;

        //Act
        cut.Render(parameters => parameters
            .AddUnmatched("class", "h-10 w-10"));

        // Assert
        Assert.AreEqual("h-10 w-10", cut.Find("svg").GetAttribute("class"));
        Assert.AreEqual(HeroiconType.Solid, cut.Instance.Type);
        Assert.AreEqual(originalType, cut.Instance.Type);
    }

    [TestMethod]
    public void SameSeedProducesSameIcon()
    {
        // Arrange & Act
        var cut1 = Render<RandomHeroicon>(parameters => parameters
            .Add(p => p.Seed, 42));
        var cut2 = Render<RandomHeroicon>(parameters => parameters
            .Add(p => p.Seed, 42));

        // Assert
        Assert.AreEqual(cut1.Markup, cut2.Markup);
    }

    [TestMethod]
    public void DifferentSeedsProduceDifferentIcons()
    {
        // Arrange & Act - use seeds known to produce different Random.Next results
        var cut1 = Render<RandomHeroicon>(parameters => parameters
            .Add(p => p.Seed, 1));
        var cut2 = Render<RandomHeroicon>(parameters => parameters
            .Add(p => p.Seed, 2));

        // Assert
        Assert.AreNotEqual(cut1.Markup, cut2.Markup);
    }

    [TestMethod]
    public void ChangingSeedPicksNewIcon()
    {
        // Arrange
        var cut = Render<RandomHeroicon>(parameters => parameters
            .Add(p => p.Seed, 1));
        var originalMarkup = cut.Markup;

        // Act
        cut.Render(parameters => parameters
            .Add(p => p.Seed, 2));

        // Assert
        Assert.AreNotEqual(originalMarkup, cut.Markup);
    }

    [TestMethod]
    public void SameSeedRetainsIconOnRerender()
    {
        // Arrange
        var cut = Render<RandomHeroicon>(parameters => parameters
            .Add(p => p.Seed, 42));
        var originalMarkup = cut.Markup;

        // Act - re-render with same seed
        cut.Render(parameters => parameters
            .Add(p => p.Seed, 42));

        // Assert
        Assert.AreEqual(originalMarkup, cut.Markup);
    }

    [TestMethod]
    public void NullSeedIsDefault()
    {
        // Arrange & Act
        var cut = Render<RandomHeroicon>();

        // Assert
        Assert.IsNull(cut.Instance.Seed);
    }
}