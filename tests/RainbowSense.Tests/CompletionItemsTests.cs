using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Completion;
using RainbowSense.VisualStudio.Completion;
using RainbowSense.VisualStudio.Configuration;
using Xunit;

namespace RainbowSense.Tests;

public class CompletionItemsTests
{
    [Fact]
    public void TagItemInsertsElement()
    {
        CompletionItem item = RainbowCompletionItems.CreateTag("DxGrid", "grid");

        Assert.Equal("<DxGrid>", item.DisplayText);
        Assert.Equal("DxGrid", item.FilterText);
        Assert.Contains(RainbowCompletionItems.BlazorTag, item.Tags);
    }

    [Fact]
    public void TagItemUsesSortPrefix()
    {
        CompletionItem item = RainbowCompletionItems.CreateTag("DxGrid", "grid", "C");

        Assert.Equal("CDxGrid", item.SortText);
    }

    [Fact]
    public void DirectiveItemSortsFirst()
    {
        CompletionItem item = RainbowCompletionItems.CreateDirective("@page", "route");

        Assert.Equal("0@page", item.SortText);
        Assert.Contains(RainbowCompletionItems.RazorDirective, item.Tags);
    }

    [Fact]
    public void OrderPutsBlazorItemsFirst()
    {
        List<CompletionItem> items = new List<CompletionItem>
        {
            RainbowCompletionItems.CreateNamespace("DevExpress.Blazor", "ns"),
            RainbowCompletionItems.CreateTag("DxGrid", "grid"),
        };

        RainbowSenseOptions options = new RainbowSenseOptions { MaxComponentResults = 100 };

        List<CompletionItem> ordered = RainbowCompletionItems.Order(items, null, options);

        Assert.Equal("<DxGrid>", ordered[0].DisplayText);
    }

    [Fact]
    public void OrderRespectsLimit()
    {
        List<CompletionItem> items = new List<CompletionItem>();

        for (int i = 0; i < 50; i++)
            items.Add(RainbowCompletionItems.CreateTag("Dx" + i, "doc"));

        RainbowSenseOptions options = new RainbowSenseOptions { MaxComponentResults = 10 };

        List<CompletionItem> ordered = RainbowCompletionItems.Order(items, null, options);

        Assert.Equal(10, ordered.Count);
    }

    [Fact]
    public void TagsAreImmutableArray()
    {
        CompletionItem item = RainbowCompletionItems.CreateTag("DxGrid", "grid");

        Assert.False(item.Tags.IsDefault);
        Assert.Contains(RainbowCompletionItems.DevExpressTag, item.Tags);
    }
}