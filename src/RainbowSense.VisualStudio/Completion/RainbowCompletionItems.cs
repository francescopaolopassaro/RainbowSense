using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
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

    private static readonly ConditionalWeakTable<CompletionItem, DescriptionHolder> _descriptions = new ConditionalWeakTable<CompletionItem, DescriptionHolder>();

    public static CompletionItem CreateTag(string tagName, string documentation, string sortPrefix = null)
    {
        return Create(
            "<" + tagName + ">",
            tagName,
            (sortPrefix ?? string.Empty) + tagName,
            ImmutableArray.Create(BlazorTag, DevExpressTag),
            documentation);
    }

    public static CompletionItem CreateTagSnippet(string tagName, string skeleton, string documentation)
    {
        return Create(
            skeleton,
            tagName,
            "0" + tagName,
            ImmutableArray.Create(BlazorTag, DevExpressTag, SnippetTag),
            documentation);
    }

    public static CompletionItem CreateDirective(string name, string documentation)
    {
        return Create(
            name,
            name,
            "0" + name,
            ImmutableArray.Create(RazorDirective),
            documentation);
    }

    public static CompletionItem CreateDirectiveSnippet(string name, string skeleton, string documentation)
    {
        return Create(
            skeleton,
            name,
            "0" + name,
            ImmutableArray.Create(RazorDirective, SnippetTag),
            documentation);
    }

    public static CompletionItem CreateNamespace(string value, string documentation)
    {
        return Create(
            value,
            value,
            "1" + value,
            ImmutableArray.Create(DevExpressTag),
            documentation);
    }

    public static CompletionItem CreateParameter(string name, string typeName, string documentation)
    {
        return Create(
            name,
            name,
            "2" + name,
            ImmutableArray.Create(ParameterTag),
            string.IsNullOrEmpty(documentation) ? typeName + " " + name : documentation);
    }

    public static CompletionItem CreateDirectiveAttribute(string name, string documentation)
    {
        return Create(
            name,
            name,
            "3" + name,
            ImmutableArray.Create(ParameterTag, EventTag),
            documentation);
    }

    public static string GetDescription(CompletionItem item)
    {
        if (item != null && _descriptions.TryGetValue(item, out DescriptionHolder holder))
            return holder.Description;

        return null;
    }

    public static List<CompletionItem> Order(IEnumerable<CompletionItem> items, RainbowCompletionContext context, RainbowSenseOptions options)
    {
        List<CompletionItem> list = new List<CompletionItem>();

        if (options.SortBlazorItemsFirst)
        {
            foreach (CompletionItem item in items)
            {
                if (HasTag(item, BlazorTag))
                    list.Add(item);
            }

            foreach (CompletionItem item in items)
            {
                if (!HasTag(item, BlazorTag))
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

    public static bool Matches(CompletionItem item, string prefix)
    {
        if (string.IsNullOrEmpty(prefix))
            return true;

        if (HasTag(item, SnippetTag))
            return FilterOf(item).StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase);

        return FilterOf(item).StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase);
    }

    private static string FilterOf(CompletionItem item)
    {
        return string.IsNullOrEmpty(item.FilterText) ? item.DisplayText : item.FilterText;
    }

    private static bool HasTag(CompletionItem item, string tag)
    {
        return !item.Tags.IsDefaultOrEmpty && item.Tags.Contains(tag);
    }

    private static CompletionItem Create(
        string displayText,
        string filterText,
        string sortText,
        ImmutableArray<string> tags,
        string description)
    {
        CompletionItem item = CompletionItem.Create(
            displayText,
            null,
            sortText,
            ImmutableDictionary<string, string>.Empty,
            tags,
            default)
            .WithFilterText(filterText);

        if (!string.IsNullOrEmpty(description))
            _descriptions.Remove(item);

        if (!string.IsNullOrEmpty(description))
            _descriptions.Add(item, new DescriptionHolder(description));

        return item;
    }

    private sealed class DescriptionHolder
    {
        public DescriptionHolder(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}