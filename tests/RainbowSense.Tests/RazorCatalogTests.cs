using RainbowSense.VisualStudio.Completion.Catalogs;
using Xunit;

namespace RainbowSense.Tests;

public class RazorCatalogTests
{
    [Fact]
    public void CatalogIsNotEmpty()
    {
        Assert.True(RazorComponentsCatalog.Items.Count >= 25, "expected at least 25 components, found " + RazorComponentsCatalog.Items.Count);
    }

    [Theory]
    [InlineData("PageTitle")]
    [InlineData("Router")]
    [InlineData("InputText")]
    [InlineData("InputNumber")]
    [InlineData("InputSelect")]
    [InlineData("EditForm")]
    [InlineData("DataAnnotationsValidator")]
    [InlineData("ValidationSummary")]
    [InlineData("Virtualize")]
    [InlineData("AuthorizeView")]
    [InlineData("AuthorizeRouteView")]
    [InlineData("NavLink")]
    [InlineData("ErrorBoundary")]
    [InlineData("QuickGrid")]
    public void ComponentsArePresent(string tag)
    {
        Assert.Contains(RazorComponentsCatalog.Items, x => x.TagName == tag);
    }

    [Fact]
    public void EveryComponentHasDocumentation()
    {
        List<string> missing = RazorComponentsCatalog.Items
            .Where(x => string.IsNullOrWhiteSpace(x.Documentation))
            .Select(x => x.TagName)
            .ToList();

        Assert.Empty(missing);
    }

    [Fact]
    public void DirectivesArePresent()
    {
        List<string> names = BlazorDirectivesCatalog.Items.Select(x => x.Name).ToList();

        Assert.Contains("@page", names);
        Assert.Contains("@inject", names);
        Assert.Contains("@code", names);
        Assert.Contains("@using", names);
        Assert.Contains("@rendermode", names);
        Assert.Contains("@attribute", names);
    }

    [Fact]
    public void InjectablesArePresent()
    {
        List<string> names = BlazorInjectablesCatalog.Items.Select(x => x.TypeName).ToList();

        Assert.Contains("NavigationManager", names);
        Assert.Contains("IJSRuntime", names);
        Assert.Contains("HttpClient", names);
    }
}