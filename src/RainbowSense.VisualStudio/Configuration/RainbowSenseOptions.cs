namespace RainbowSense.VisualStudio.Configuration;

public sealed class RainbowSenseOptions
{
    public bool EnableBlazorDirectives { get; set; } = true;

    public bool EnableBlazorInjectables { get; set; } = true;

    public bool EnableBlazorMarkupComponents { get; set; } = true;

    public bool EnableRazorComponentTags { get; set; } = true;

    public bool EnableDevExpressComponents { get; set; } = true;

    public bool EnableDevExpressNamespaces { get; set; } = true;

    public bool SortBlazorItemsFirst { get; set; } = true;

    public int MaxComponentResults { get; set; } = 500;

    public bool IncludeDevExpressNamespacesInRazor { get; set; } = true;

    public RainbowSenseOptions Clone() => (RainbowSenseOptions)MemberwiseClone();
}