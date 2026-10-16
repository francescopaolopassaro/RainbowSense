using System;
using System.Collections.Generic;

namespace RainbowSense.VisualStudio.Completion.Catalogs;

public sealed class RazorMember
{
    public RazorMember(string name, string typeName, bool isEvent)
    {
        Name = name;
        TypeName = typeName;
        IsEvent = isEvent;
    }

    public string Name { get; }

    public string TypeName { get; }

    public bool IsEvent { get; }
}

public static class RazorMembersCatalog
{
    private static readonly Dictionary<string, string> _members = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["Microsoft.AspNetCore.Components.Authorization.AuthorizeView"] = "Policy:String;Roles:String;ChildContent:RenderFragment`1<AuthenticationState>;NotAuthorized:RenderFragment`1<AuthenticationState>;Authorized:RenderFragment`1<AuthenticationState>;Authorizing:RenderFragment;Resource:Object",
        ["Microsoft.AspNetCore.Components.Authorization.CascadingAuthenticationState"] = "ChildContent:RenderFragment",
        ["Microsoft.AspNetCore.Components.Forms.EditForm"] = "AdditionalAttributes:IReadOnlyDictionary`2<String,Object>;EditContext:EditContext;Enhance:Boolean;Model:Object;ChildContent:RenderFragment`1<EditContext>;OnSubmit:EventCallback`1<EditContext>;OnValidSubmit:EventCallback`1<EditContext>;OnInvalidSubmit:EventCallback`1<EditContext>;FormName:String",
        ["Microsoft.AspNetCore.Components.Forms.InputCheckbox"] = "Element:Nullable`1<ElementReference>;AdditionalAttributes:IReadOnlyDictionary`2<String,Object>;Value:!0;ValueChanged:EventCallback`1<!0>;ValueExpression:Expression`1<Func`1<!0>>;DisplayName:String",
        ["Microsoft.AspNetCore.Components.Forms.InputDate"] = "Type:InputDateType;ParsingErrorMessage:String;Element:Nullable`1<ElementReference>;AdditionalAttributes:IReadOnlyDictionary`2<String,Object>;Value:!0;ValueChanged:EventCallback`1<!0>;ValueExpression:Expression`1<Func`1<!0>>;DisplayName:String",
        ["Microsoft.AspNetCore.Components.Forms.InputFile"] = "OnChange:EventCallback`1<InputFileChangeEventArgs>;AdditionalAttributes:IDictionary`2<String,Object>;Element:Nullable`1<ElementReference>",
        ["Microsoft.AspNetCore.Components.Forms.InputNumber"] = "ParsingErrorMessage:String;Element:Nullable`1<ElementReference>;AdditionalAttributes:IReadOnlyDictionary`2<String,Object>;Value:!0;ValueChanged:EventCallback`1<!0>;ValueExpression:Expression`1<Func`1<!0>>;DisplayName:String",
        ["Microsoft.AspNetCore.Components.Forms.InputRadio"] = "AdditionalAttributes:IReadOnlyDictionary`2<String,Object>;Value:!0;Name:String;Element:Nullable`1<ElementReference>",
        ["Microsoft.AspNetCore.Components.Forms.InputRadioGroup"] = "ChildContent:RenderFragment;Name:String;AdditionalAttributes:IReadOnlyDictionary`2<String,Object>;Value:!0;ValueChanged:EventCallback`1<!0>;ValueExpression:Expression`1<Func`1<!0>>;DisplayName:String",
        ["Microsoft.AspNetCore.Components.Forms.InputSelect"] = "ChildContent:RenderFragment;Element:Nullable`1<ElementReference>;AdditionalAttributes:IReadOnlyDictionary`2<String,Object>;Value:!0;ValueChanged:EventCallback`1<!0>;ValueExpression:Expression`1<Func`1<!0>>;DisplayName:String",
        ["Microsoft.AspNetCore.Components.Forms.InputText"] = "Element:Nullable`1<ElementReference>;AdditionalAttributes:IReadOnlyDictionary`2<String,Object>;Value:!0;ValueChanged:EventCallback`1<!0>;ValueExpression:Expression`1<Func`1<!0>>;DisplayName:String",
        ["Microsoft.AspNetCore.Components.Forms.InputTextArea"] = "Element:Nullable`1<ElementReference>;AdditionalAttributes:IReadOnlyDictionary`2<String,Object>;Value:!0;ValueChanged:EventCallback`1<!0>;ValueExpression:Expression`1<Func`1<!0>>;DisplayName:String",
        ["Microsoft.AspNetCore.Components.Forms.ValidationMessage"] = "AdditionalAttributes:IReadOnlyDictionary`2<String,Object>;For:Expression`1<Func`1<!0>>",
        ["Microsoft.AspNetCore.Components.Forms.ValidationSummary"] = "Model:Object;AdditionalAttributes:IReadOnlyDictionary`2<String,Object>",
        ["Microsoft.AspNetCore.Components.Routing.FocusOnNavigate"] = "RouteData:RouteData;Selector:String",
        ["Microsoft.AspNetCore.Components.Routing.NavLink"] = "ActiveClass:String;AdditionalAttributes:IReadOnlyDictionary`2<String,Object>;ChildContent:RenderFragment;Match:NavLinkMatch",
        ["Microsoft.AspNetCore.Components.Web.ErrorBoundary"] = "ChildContent:RenderFragment;ErrorContent:RenderFragment`1<Exception>;MaximumErrorCount:Int32",
        ["Microsoft.AspNetCore.Components.Web.HeadContent"] = "ChildContent:RenderFragment",
        ["Microsoft.AspNetCore.Components.Web.PageTitle"] = "ChildContent:RenderFragment",
        ["Microsoft.AspNetCore.Components.Web.Virtualization.Virtualize"] = "ChildContent:RenderFragment`1<!0>;Placeholder:RenderFragment`1<PlaceholderContext>;EmptyContent:RenderFragment;OverscanCount:Int32;SpacerElement:String;MaxItemCount:Int32",
    };

    public static IReadOnlyList<string> GetParameterNames(string typeName)
    {
        List<string> list = new List<string>();

        foreach (string name in Get(typeName, false))
            list.Add(name);

        return list;
    }

    public static IReadOnlyList<string> GetEventNames(string typeName)
    {
        List<string> list = new List<string>();

        foreach (string name in Get(typeName, true))
            list.Add(name);

        return list;
    }

    public static string GetTypeName(string typeName, string memberName)
    {
        if (!_members.TryGetValue(typeName ?? string.Empty, out string value))
            return null;

        foreach (string item in value.Split(';'))
        {
            int index = item.IndexOf(':');

            if (index < 0)
                continue;

            string name = item[0] == '@' ? item.Substring(1, index - 1) : item.Substring(0, index);

            if (string.Equals(name, memberName, System.StringComparison.Ordinal))
                return item.Substring(index + 1);
        }

        return null;
    }

    private static IEnumerable<string> Get(string typeName, bool events)
    {
        if (!_members.TryGetValue(typeName ?? string.Empty, out string value))
            yield break;

        foreach (string item in value.Split(';'))
        {
            int index = item.IndexOf(':');

            if (index < 0)
                continue;

            bool isEvent = item[0] == '@';

            if (isEvent != events)
                continue;

            yield return isEvent ? item.Substring(1, index - 1) : item.Substring(0, index);
        }
    }
}
