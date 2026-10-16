using System.Collections.Generic;
using Microsoft.CodeAnalysis.Completion;
using RainbowSense.VisualStudio.Completion.Catalogs;
using RainbowSense.VisualStudio.Configuration;

namespace RainbowSense.VisualStudio.Completion;

public static class RainbowCompletionBuilder
{
    public static IReadOnlyList<CompletionItem> GetMarkupItems(RainbowCompletionContext context, RainbowSenseOptions options)
    {
        List<CompletionItem> items = new List<CompletionItem>();

        if (context == null || !context.IsRazorFile || !context.IsMarkupContext)
            return items;

        if (options.EnableRazorComponentTags)
        {
            foreach (RazorComponent component in RazorComponentsCatalog.Items)
            {
                items.Add(RainbowCompletionItems.CreateTag(
                    component.TagName,
                    component.Documentation + " " + component.NamespaceName,
                    "A"));
            }
        }

        if (options.EnableDevExpressComponents)
        {
            foreach (DevExpressComponent component in DevExpressComponentsCatalog.Items)
            {
                if (context.TagName.Length > 0 && !IsRelated(context.TagName, component.TagName))
                    continue;

                items.Add(RainbowCompletionItems.CreateTag(
                    component.TagName,
                    component.Documentation + " " + component.NamespaceName,
                    component.SortPrefix));
            }

            if (options.IncludeDevExpressNamespacesInRazor)
            {
                foreach (string ns in DevExpressNamespacesCatalog.Items)
                {
                    items.Add(RainbowCompletionItems.CreateDirective("@using " + ns, "Imports the " + ns + " namespace."));
                }
            }
        }

        foreach (ComponentSnippet snippet in ComponentSnippetsCatalog.Items)
        {
            items.Add(RainbowCompletionItems.CreateTagSnippet(snippet.TagName, snippet.Skeleton, snippet.Documentation));
        }

        if (options.EnableBlazorDirectives && !context.IsTagContext)
        {
            foreach (BlazorDirective directive in BlazorDirectivesCatalog.Items)
            {
                items.Add(RainbowCompletionItems.CreateDirectiveSnippet(directive.Name, directive.InsertionText, directive.Documentation));
            }
        }

        if (options.EnableBlazorInjectables)
        {
            foreach (BlazorInjectable injectable in BlazorInjectablesCatalog.Items)
            {
                items.Add(RainbowCompletionItems.CreateParameter(
                    "@inject " + injectable.TypeName,
                    injectable.DefaultName,
                    injectable.Documentation));
            }
        }

        return RainbowCompletionItems.Order(items, context, options);
    }

    public static IReadOnlyList<CompletionItem> GetAttributeItems(RainbowCompletionContext context, RainbowSenseOptions options)
    {
        List<CompletionItem> items = new List<CompletionItem>();

        if (context == null || !context.IsRazorFile)
            return items;

        foreach (KeyValuePair<string, string> pair in CommonAttributes)
        {
            if (!pair.Key.StartsWith(context.AttributeName, System.StringComparison.Ordinal))
                continue;

            items.Add(RainbowCompletionItems.CreateDirectiveAttribute(pair.Key, pair.Value));
        }

        DevExpressComponent devexpress = DevExpressComponentsCatalog.Find(context.TagName);

        if (devexpress != null && options.EnableDevExpressComponents)
        {
            string typeName = devexpress.NamespaceName + "." + devexpress.TypeName;

            foreach (string name in DevExpressMembersCatalog.GetParameterNames(typeName))
            {
                if (!name.StartsWith(context.AttributeName, System.StringComparison.Ordinal))
                    continue;

                string memberType = DevExpressMembersCatalog.GetTypeName(typeName, name);

                items.Add(RainbowCompletionItems.CreateParameter(name, memberType, devexpress.DisplayName + " " + name + " : " + memberType));
            }
        }

        RazorComponent razor = RazorComponentsCatalog.Find(context.TagName);

        if (razor != null && options.EnableRazorComponentTags)
        {
            string typeName = razor.NamespaceName + "." + razor.TypeName;

            foreach (string name in RazorMembersCatalog.GetParameterNames(typeName))
            {
                if (!name.StartsWith(context.AttributeName, System.StringComparison.Ordinal))
                    continue;

                string memberType = RazorMembersCatalog.GetTypeName(typeName, name);

                items.Add(RainbowCompletionItems.CreateParameter(name, memberType, razor.DisplayName + " " + name + " : " + memberType));
            }
        }

        return RainbowCompletionItems.Order(items, context, options);
    }

    public static IReadOnlyList<CompletionItem> GetDirectiveItems(RainbowCompletionContext context, RainbowSenseOptions options)
    {
        List<CompletionItem> items = new List<CompletionItem>();

        if (context == null || !context.IsRazorFile || !context.IsDirectiveContext)
            return items;

        if (!options.EnableBlazorDirectives)
            return items;

        foreach (BlazorDirective directive in BlazorDirectivesCatalog.Items)
        {
            items.Add(RainbowCompletionItems.CreateDirectiveSnippet(directive.Name, directive.InsertionText, directive.Documentation));
        }

        if (options.EnableBlazorInjectables)
        {
            foreach (BlazorInjectable injectable in BlazorInjectablesCatalog.Items)
            {
                items.Add(RainbowCompletionItems.CreateDirectiveSnippet("@inject", injectable.InsertionText, injectable.Documentation));
            }
        }

        if (options.IncludeDevExpressNamespacesInRazor)
        {
            foreach (string ns in DevExpressNamespacesCatalog.Items)
            {
                items.Add(RainbowCompletionItems.CreateDirectiveSnippet("@using " + ns, "@using " + ns, "Imports the " + ns + " namespace."));
            }
        }

        return RainbowCompletionItems.Order(items, context, options);
    }

    public static IReadOnlyList<CompletionItem> GetUsingItems(RainbowCompletionContext context, RainbowSenseOptions options)
    {
        List<CompletionItem> items = new List<CompletionItem>();

        if (context == null || !context.IsRazorFile || !context.IsUsingContext)
            return items;

        if (options.IncludeDevExpressNamespacesInRazor)
        {
            foreach (string ns in DevExpressNamespacesCatalog.Items)
            {
                items.Add(RainbowCompletionItems.CreateNamespace(ns, "DevExpress Blazor " + DevExpressComponentsCatalog.Version + " namespace."));
            }
        }

        items.Add(RainbowCompletionItems.CreateDirective("@using", "Imports a namespace in the generated code."));

        return RainbowCompletionItems.Order(items, context, options);
    }

    public static IReadOnlyList<CompletionItem> GetItems(RainbowCompletionContext context, RainbowSenseOptions options)
    {
        if (context == null || !context.IsRazorFile)
            return new List<CompletionItem>();

        if (context.IsUsingContext)
            return GetUsingItems(context, options);

        if (context.IsDirectiveContext)
            return GetDirectiveItems(context, options);

        if (context.AttributeName.Length > 0)
            return GetAttributeItems(context, options);

        return GetMarkupItems(context, options);
    }

    private static bool IsRelated(string tagName, string componentTag)
    {
        return componentTag.StartsWith(tagName, System.StringComparison.Ordinal)
            || tagName.StartsWith(componentTag, System.StringComparison.Ordinal);
    }

    private static readonly KeyValuePair<string, string>[] CommonAttributes =
    {
        new KeyValuePair<string, string>("@bind-Value", "Two-way binding of the component value."),
        new KeyValuePair<string, string>("@bind", "Two-way binding expression."),
        new KeyValuePair<string, string>("@bind:after", "Callback invoked after the value changes."),
        new KeyValuePair<string, string>("@ref", "Captures a reference to the rendered element."),
        new KeyValuePair<string, string>("@key", "Key used to identify the element."),
        new KeyValuePair<string, string>("@attributes", "Additional attributes splatted onto the element."),
        new KeyValuePair<string, string>("@onclick", "Event handler for the click event."),
        new KeyValuePair<string, string>("class", "CSS classes of the element."),
        new KeyValuePair<string, string>("style", "Inline style of the element."),
        new KeyValuePair<string, string>("id", "Identifier of the element."),
    };
}