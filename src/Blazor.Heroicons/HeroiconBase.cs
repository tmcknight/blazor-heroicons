using Microsoft.AspNetCore.Components;

namespace Blazor.Heroicons;

public abstract class HeroiconBase : ComponentBase
{
    private static string s_defaultCssClass = "h-8 w-8";

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalAttributes { get; set; } = new();

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!AdditionalAttributes.ContainsKey("class"))
            AdditionalAttributes.Add("class", s_defaultCssClass);
    }
}