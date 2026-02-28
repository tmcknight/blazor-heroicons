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
}
