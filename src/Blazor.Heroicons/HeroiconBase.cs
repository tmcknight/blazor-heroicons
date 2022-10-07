using Microsoft.AspNetCore.Components;

namespace Blazor.Heroicons;

public abstract class HeroiconBase : ComponentBase
{
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalAttributes { get; set; } = new();
}

public enum HeroiconType
{
    Solid,
    Outline,
    Mini
}