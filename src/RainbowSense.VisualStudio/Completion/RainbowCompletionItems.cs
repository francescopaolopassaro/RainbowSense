using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using RainbowSense.VisualStudio.Configuration;

namespace RainbowSense.VisualStudio.Completion;

public static class RainbowCompletionItems
{
    public const string BlazorTag = "blazor";
    public const string RazorDirective = "razor";
    public const string DevExpressTag = "devexpress";
    public const string ParameterTag = "parameter";
    public const string EventTag = "event";
    public const string SnippetTag = "snippet";

    private static readonly Dictionary<CompletionItem, string> _descriptions = new Dictionary<CompletionItem, string>();

    public static CompletionItem CreateTag(string tagName, string documentation, string sortPrefix = null)
    {
        string displayText = "<" + tagName + ">";

        CompletionItem item = Create(
            displayText,
            null,
            -1,
            (sortPrefix ?? string.Empty) + tagName,
            tagName,
            ImmutableArray.Create(BlazorTag, DevExpressTag));

        Register(item, documentation);

        return item;
    }

    public static CompletionItem CreateTagSnippet(string tagName, string skeleton, string documentation)
    {
        int invocationPosition = skeleton.Length;

        CompletionItem item = Create(
            skeleton,
            null,
            invocationPosition,
            "0" + tagName,
            tagName,
            ImmutableArray.Create(BlazorTag, DevExpressTag, SnippetTag));

        Register(item, documentation);

        return item;
    }

    public static CompletionItem CreateDirective(string name, string documentation)
    {
        CompletionItem item = Create(
            name,
            null,
            -1,
            "0" + name,
            name,
            ImmutableArray.Create(RazorDirective));

        Register(item, documentation);

        return item;
    }

    public static CompletionItem CreateDirectiveSnippet(string name, string skeleton, string documentation)
    {
        CompletionItem item = Create(
            skeleton,
            null,
            skeleton.Length,
            "0" + name,
            name,
            ImmutableArray.Create(RazorDirective, SnippetTag));

        Register(item, documentation);

        return item;
    }

    public static CompletionItem CreateNamespace(string value, string documentation)
    {
        CompletionItem item = Create(
            value,
            null,
            -1,
            "1" + value,
            value,
            ImmutableArray.Create(DevExpressTag));

        Register(item, documentation);

        return item;
    }

    public static CompletionItem CreateParameter(string name, string typeName, string documentation)
    {
        CompletionItem item = Create(
            name,
            null,
            -1,
            "2" + name,
            name,
            ImmutableArray.Create(ParameterTag));

        Register(item, string.IsNullOrEmpty(documentation) ? typeName + " " + name : documentation);

        return item;
    }

    public static CompletionItem CreateDirectiveAttribute(string name, string documentation)
    {
        CompletionItem item = Create(
            name,
            null,
            -1,
            "3" + name,
            name,
            ImmutableArray.Create(ParameterTag, EventTag));

        Register(item, documentation);

        return item;
    }

    public static string GetDescription(CompletionItem item)
    {
        if (item != null && _descriptions.TryGetValue(item, out string description))
            return description;

        return null;
    }

    public static List<CompletionItem> Order(IEnumerable<CompletionItem> items, RainbowCompletionContext context, RainbowSenseOptions options)
    {
        List<CompletionItem> list = new List<CompletionItem>();

        if (options.SortBlazorItemsFirst)
        {
            foreach (CompletionItem item in items)
            {
                if (item.Tags.IsDefaultOrEmpty || !item.Tags.Contains(BlazorTag))
                    continue;

                list.Add(item);
            }

            foreach (CompletionItem item in items)
            {
                if (item.Tags.IsDefaultOrEmpty || item.Tags.Contains(BlazorTag))
                    continue;

                list.Add(item);
            }
        }
        else
        {
            foreach (CompletionItem item in items)
                list.Add(item);
        }

        int max = options.MaxComponentResults;

        if (max > 0 && list.Count > max && (context == null || !context.IsUsingContext))
            return list.GetRange(0, max);

        return list;
    }

    public static ImmutableArray<CompletionItem> ToImmutable(List<CompletionItem> items)
    {
        return items.ToImmutableArray();
    }

    private static CompletionItem Create(
        string displayText,
        string displayTextPrefix,
        int invocationPosition,
        string sortText,
        string filterText,
        ImmutableArray<string> tags)
    {
        CompletionItem item = CompletionItem.Create(
            displayText,
            null,
            sortText,
            ImmutableDictionary<string, string>.Empty,
            tags,
            default,
            displayTextPrefix);

        return string.IsNullOrEmpty(filterText) ? item : item.WithFilterText(filterText);
    }

    private static void Register(CompletionItem item, string description)
    {
        if (string.IsNullOrEmpty(description))
            return;

        lock (_descriptions)
        {
            _descriptions[item] = description;
        }
    }
}