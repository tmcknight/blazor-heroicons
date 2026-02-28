using Microsoft.AspNetCore.Components;

namespace Blazor.Heroicons;

public abstract class HeroiconBase : ComponentBase
{
  /// <summary>
  /// Gets or sets a collection of additional attributes that will be applied to the created element.
  /// </summary>
  [Parameter(CaptureUnmatchedValues = true)]
  public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; } = new Dictionary<string, object>();
}

/// <summary>
/// The type/size of the icon.
/// </summary>
public enum HeroiconType
{
	/// <summary>
	/// For primary navigation and marketing sections, with a filled appearance.
	/// </summary>
	Solid,
	/// <summary>
	/// For primary navigation and marketing sections, with an outlined appearance.
	/// </summary>
	Outline,
	/// <summary>
	/// For smaller elements like buttons, form elements, and to support text.
	/// </summary>
	Mini,
	/// <summary>
	/// For tighter, higher density UIs.
	/// </summary>
	Micro
}
