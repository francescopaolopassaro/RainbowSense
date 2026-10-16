using System.Collections.Generic;

namespace RainbowSense.VisualStudio.Completion.Catalogs;

public static class DevExpressNamespacesCatalog
{
    private static readonly IReadOnlyList<string> _namespaces = new List<string>
    {
        "DevExpress.Blazor",
        "DevExpress.Blazor.Base",
        "DevExpress.Blazor.Configuration",
        "DevExpress.Blazor.Grid",
        "DevExpress.Blazor.Localization",
        "DevExpress.Blazor.Office",
        "DevExpress.Blazor.Ribbon",
        "DevExpress.Blazor.RichEdit",
    };

    public static IReadOnlyList<string> Items => _namespaces;
}
