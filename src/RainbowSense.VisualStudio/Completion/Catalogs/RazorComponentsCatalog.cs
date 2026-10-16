using System.Collections.Generic;

namespace RainbowSense.VisualStudio.Completion.Catalogs;

public sealed class RazorComponent
{
    public RazorComponent(string tagName, string namespaceName, string typeName, string arity, string documentation)
    {
        TagName = tagName;
        NamespaceName = namespaceName;
        TypeName = typeName;
        Arity = arity;
        Documentation = documentation;
    }

    public string TagName { get; }

    public string NamespaceName { get; }

    public string TypeName { get; }

    public string Arity { get; }

    public string Documentation { get; }

    public string DisplayName => Arity.Length > 0 ? TypeName + "<" + Arity + ">" : TypeName;
}

public static class RazorComponentsCatalog
{
    private static readonly IReadOnlyList<RazorComponent> _items = new List<RazorComponent>
    {
        new RazorComponent("AuthorizeView", "Microsoft.AspNetCore.Components.Authorization", "AuthorizeView", "", "AuthorizeView component."),
        new RazorComponent("CascadingAuthenticationState", "Microsoft.AspNetCore.Components.Authorization", "CascadingAuthenticationState", "", "CascadingAuthenticationState component."),
        new RazorComponent("DataAnnotationsValidator", "Microsoft.AspNetCore.Components.Forms", "DataAnnotationsValidator", "", "DataAnnotationsValidator component."),
        new RazorComponent("EditForm", "Microsoft.AspNetCore.Components.Forms", "EditForm", "", "EditForm component."),
        new RazorComponent("ErrorBoundary", "Microsoft.AspNetCore.Components.Web", "ErrorBoundary", "", "ErrorBoundary component."),
        new RazorComponent("FocusOnNavigate", "Microsoft.AspNetCore.Components.Routing", "FocusOnNavigate", "", "FocusOnNavigate component."),
        new RazorComponent("HeadContent", "Microsoft.AspNetCore.Components.Web", "HeadContent", "", "HeadContent component."),
        new RazorComponent("HeadOutlet", "Microsoft.AspNetCore.Components.Web", "HeadOutlet", "", "HeadOutlet component."),
        new RazorComponent("InputCheckbox", "Microsoft.AspNetCore.Components.Forms", "InputCheckbox", "", "InputCheckbox component."),
        new RazorComponent("InputDate", "Microsoft.AspNetCore.Components.Forms", "InputDate", "TValue", "InputDate component."),
        new RazorComponent("InputFile", "Microsoft.AspNetCore.Components.Forms", "InputFile", "", "InputFile component."),
        new RazorComponent("InputNumber", "Microsoft.AspNetCore.Components.Forms", "InputNumber", "TValue", "InputNumber component."),
        new RazorComponent("InputRadio", "Microsoft.AspNetCore.Components.Forms", "InputRadio", "TValue", "InputRadio component."),
        new RazorComponent("InputRadioGroup", "Microsoft.AspNetCore.Components.Forms", "InputRadioGroup", "TValue", "InputRadioGroup component."),
        new RazorComponent("InputSelect", "Microsoft.AspNetCore.Components.Forms", "InputSelect", "TValue", "InputSelect component."),
        new RazorComponent("InputText", "Microsoft.AspNetCore.Components.Forms", "InputText", "", "InputText component."),
        new RazorComponent("InputTextArea", "Microsoft.AspNetCore.Components.Forms", "InputTextArea", "", "InputTextArea component."),
        new RazorComponent("NavLink", "Microsoft.AspNetCore.Components.Routing", "NavLink", "", "NavLink component."),
        new RazorComponent("PageTitle", "Microsoft.AspNetCore.Components.Web", "PageTitle", "", "PageTitle component."),
        new RazorComponent("ValidationMessage", "Microsoft.AspNetCore.Components.Forms", "ValidationMessage", "TValue", "ValidationMessage component."),
        new RazorComponent("ValidationSummary", "Microsoft.AspNetCore.Components.Forms", "ValidationSummary", "", "ValidationSummary component."),
        new RazorComponent("Virtualize", "Microsoft.AspNetCore.Components.Web.Virtualization", "Virtualize", "TItem", "Virtualize component."),
        new RazorComponent("Router", "Microsoft.AspNetCore.Components.Routing", "Router", "", "Provides routing functionality for components that have routable URLs."),
        new RazorComponent("RouteView", "Microsoft.AspNetCore.Components", "RouteView", "", "Renders the component associated with the current route."),
        new RazorComponent("AuthorizeRouteView", "Microsoft.AspNetCore.Components.Authorization", "AuthorizeRouteView", "", "Restricts access to the component matching the current route."),
        new RazorComponent("NotAuthorized", "Microsoft.AspNetCore.Components.Authorization", "NotAuthorized", "", "Displays content when the user is not authorized."),
        new RazorComponent("AuthorizeRouteViewCore", "Microsoft.AspNetCore.Components.Authorization", "AuthorizeRouteViewCore", "", "Restricts access to a component and provides additional content when access is denied."),
        new RazorComponent("QuickGrid", "Microsoft.AspNetCore.Components.QuickGrid", "QuickGrid", "TItem", "Displays data in a tabular format with sorting, filtering and grouping."),
        new RazorComponent("QuickGridColumn", "Microsoft.AspNetCore.Components.QuickGrid", "QuickGridColumn", "TItem", "Represents a column of a QuickGrid."),    };

    public static IReadOnlyList<RazorComponent> Items => _items;

    public static RazorComponent Find(string tagName)
    {
        foreach (RazorComponent item in _items)
        {
            if (string.Equals(item.TagName, tagName, System.StringComparison.Ordinal))
                return item;
        }

        return null;
    }
}
