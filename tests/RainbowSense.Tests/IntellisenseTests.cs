using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Completion;
using RainbowSense.VisualStudio.Completion;
using RainbowSense.VisualStudio.Configuration;
using Xunit;

namespace RainbowSense.Tests;

public class IntellisenseTests
{
    private static IReadOnlyList<CompletionItem> Complete(string markup, int position)
    {
        RainbowCompletionContext context = RainbowCompletionContext.Create("Index.razor", markup, position);

        Assert.NotNull(context);

        return RainbowCompletionBuilder.GetItems(context, new RainbowSenseOptions());
    }

    private static IReadOnlyList<CompletionItem> Complete(string markup)
    {
        return Complete(markup, markup.Length);
    }

    [Fact]
    public void FiltersByTypedTagBeforeTruncating()
    {
        RainbowSenseOptions options = new RainbowSenseOptions { MaxComponentResults = 5 };

        RainbowCompletionContext context = RainbowCompletionContext.Create("Index.razor", "<DxPieC", 6);

        IReadOnlyList<CompletionItem> items = RainbowCompletionBuilder.GetItems(context, options);

        Assert.Contains(items, x => x.DisplayText == "<DxPieChart>");
        Assert.True(items.Count <= 5);
    }

    [Fact]
    public void DirectivesAreFilteredByTypedPrefix()
    {
        RainbowCompletionContext context = RainbowCompletionContext.Create("Index.razor", "@imp", 4);

        IReadOnlyList<CompletionItem> items = RainbowCompletionBuilder.GetItems(context, new RainbowSenseOptions());

        Assert.NotEmpty(items);
        Assert.All(items, x => Assert.StartsWith("@imp", x.DisplayText, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void NoDuplicateItems()
    {
        IReadOnlyList<CompletionItem> items = Complete("<");

        List<string> duplicates = items
            .GroupBy(x => x.DisplayText, StringComparer.Ordinal)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        Assert.Empty(duplicates);
    }

    [Fact]
    public void MarkupItemsStayWithinLimitWithAllCategories()
    {
        RainbowSenseOptions options = new RainbowSenseOptions { MaxComponentResults = 100 };

        RainbowCompletionContext context = RainbowCompletionContext.Create("Index.razor", "<", 1);

        IReadOnlyList<CompletionItem> items = RainbowCompletionBuilder.GetItems(context, options);

        Assert.True(items.Count <= 100, "items=" + items.Count);
    }

    [Fact]
    public void DescriptionsAreAvailableForEveryItem()
    {
        IReadOnlyList<CompletionItem> items = Complete("<Dx");

        Assert.NotEmpty(items);

        foreach (CompletionItem item in items)
            Assert.False(string.IsNullOrEmpty(RainbowCompletionItems.GetDescription(item)));
    }

    [Fact]
    public void CompletesDevExpressTagsInMarkup()
    {
        IReadOnlyList<CompletionItem> items = Complete("<");

        Assert.Contains(items, x => x.DisplayText == "<DxComboBox>");
        Assert.Contains(items, x => x.DisplayText == "<DxFormLayout>");
        Assert.Contains(items, x => x.DisplayText == "<DxGrid>");
    }

    [Fact]
    public void CompletesBlazorTagsInMarkup()
    {
        IReadOnlyList<CompletionItem> items = Complete("<");

        Assert.Contains(items, x => x.DisplayText == "<InputText>");
        Assert.Contains(items, x => x.DisplayText == "<InputTextArea>");
        Assert.Contains(items, x => x.DisplayText == "<EditForm>");
    }

    [Fact]
    public void CompletesRazorDirectives()
    {
        IReadOnlyList<CompletionItem> items = Complete("@pag");

        Assert.Contains(items, x => x.DisplayText.StartsWith("@page", StringComparison.Ordinal));
    }

    [Fact]
    public void CompletesDevExpressNamespacesInUsingDirective()
    {
        IReadOnlyList<CompletionItem> items = Complete("@using DevExpr");

        Assert.Contains(items, x => x.DisplayText == "DevExpress.Blazor.Grid");
    }

    [Fact]
    public void CompletesDevExpressParametersInsideComponentTag()
    {
        IReadOnlyList<CompletionItem> items = Complete("<DxGrid PageSi");

        Assert.Contains(items, x => x.DisplayText == "PageSize");
                    }

    [Fact]
    public void CompletesBlazorParametersInsideComponentTag()
    {
        IReadOnlyList<CompletionItem> items = Complete("<EditForm Mo");

        Assert.Contains(items, x => x.DisplayText == "Model");
        Assert.Single(items.Where(x => x.DisplayText == "Model"));
    }

    [Fact]
    public void CompletesDirectiveAttributes()
    {
        IReadOnlyList<CompletionItem> items = Complete("<InputText @bind-Val");

        Assert.Contains(items, x => x.DisplayText == "@bind-Value");
    }

    [Fact]
    public void CompletesHtmlAttributes()
    {
        IReadOnlyList<CompletionItem> items = Complete("<InputText cl");

        Assert.Contains(items, x => x.DisplayText == "class");
    }

    [Fact]
    public void OffersSnippetsThatWriteCode()
    {
        IReadOnlyList<CompletionItem> items = Complete("<");

        CompletionItem snippet = items.FirstOrDefault(x => x.Tags.Contains(RainbowCompletionItems.SnippetTag) && x.FilterText == "DxFormLayout");

        Assert.NotNull(snippet);
        Assert.Contains("DxFormLayoutItem", snippet.DisplayText);
        Assert.Contains("$0", snippet.DisplayText);
    }

    [Fact]
    public void TagItemsCarryFilterTextAndTags()
    {
        IReadOnlyList<CompletionItem> items = Complete("<");

        CompletionItem item = items.First(x => x.DisplayText == "<DxGrid>");

        Assert.Equal("DxGrid", item.FilterText);
        Assert.Contains(RainbowCompletionItems.BlazorTag, item.Tags);
        Assert.Contains(RainbowCompletionItems.DevExpressTag, item.Tags);
    }

    [Fact]
    public void TagItemsHaveDescription()
    {
        IReadOnlyList<CompletionItem> items = Complete("<");

        CompletionItem item = items.First(x => x.DisplayText == "<DxGrid>");

        string description = RainbowCompletionItems.GetDescription(item);

        Assert.NotNull(description);
        Assert.Contains("tabular", description);
    }

    [Fact]
    public void FiltersByTypedTag()
    {
        IReadOnlyList<CompletionItem> items = Complete("<DxForm");

        Assert.Contains(items, x => x.DisplayText == "<DxFormLayout>");
        Assert.DoesNotContain(items, x => x.DisplayText == "<DxGrid>");
    }

    [Fact]
    public void NoItemsOutsideMarkup()
    {
        const string markup = "@code {\n    ";

        RainbowCompletionContext context = RainbowCompletionContext.Create("Index.razor", markup, markup.Length);

        Assert.NotNull(context);

        IReadOnlyList<CompletionItem> items = RainbowCompletionBuilder.GetItems(context, new RainbowSenseOptions());

        Assert.Empty(items);
    }

    [Fact]
    public void NoItemsInCSharpFile()
    {
        RainbowCompletionContext context = RainbowCompletionContext.Create("Program.cs", "<Dx", 3);

        IReadOnlyList<CompletionItem> items = RainbowCompletionBuilder.GetItems(context, new RainbowSenseOptions());

        Assert.Empty(items);
    }

    [Fact]
    public void RespectsDisabledOptions()
    {
        RainbowSenseOptions options = new RainbowSenseOptions
        {
            EnableDevExpressComponents = false,
            EnableRazorComponentTags = false,
        };

        RainbowCompletionContext context = RainbowCompletionContext.Create("Index.razor", "<", 1);

        IReadOnlyList<CompletionItem> items = RainbowCompletionBuilder.GetMarkupItems(context, options);

        Assert.DoesNotContain(items, x => x.DisplayText == "<DxGrid>");
        Assert.DoesNotContain(items, x => x.DisplayText == "<InputText>");
    }

    [Fact]
    public void RespectsResultLimit()
    {
        RainbowSenseOptions options = new RainbowSenseOptions { MaxComponentResults = 5 };

        RainbowCompletionContext context = RainbowCompletionContext.Create("Index.razor", "<", 1);

        IReadOnlyList<CompletionItem> items = RainbowCompletionBuilder.GetMarkupItems(context, options);

        Assert.True(items.Count <= 5, "expected at most 5 items, found " + items.Count);
    }

    [Fact]
    public void UsingDirectiveIsNotLimited()
    {
        RainbowSenseOptions options = new RainbowSenseOptions { MaxComponentResults = 3 };

        RainbowCompletionContext context = RainbowCompletionContext.Create("_Imports.razor", "@using ", 7);

        IReadOnlyList<CompletionItem> items = RainbowCompletionBuilder.GetUsingItems(context, options);

        Assert.True(items.Count > 3, "items=" + items.Count + " using=" + context.IsUsingContext + " razor=" + context.IsRazorFile);
    }

    [Fact]
    public void BlazorItemsAreSortedFirst()
    {
        RainbowSenseOptions options = new RainbowSenseOptions { SortBlazorItemsFirst = true };

        RainbowCompletionContext context = RainbowCompletionContext.Create("Index.razor", "<", 1);

        IReadOnlyList<CompletionItem> items = RainbowCompletionBuilder.GetMarkupItems(context, options);

        Assert.NotEmpty(items);
        Assert.Contains(RainbowCompletionItems.BlazorTag, items[0].Tags);
    }
}