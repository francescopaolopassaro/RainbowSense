using System.Collections.Generic;

namespace RainbowSense.VisualStudio.Completion.Catalogs;

public sealed class BlazorDirective
{
    public BlazorDirective(string name, string insertionText, string documentation)
    {
        Name = name;
        InsertionText = insertionText;
        Documentation = documentation;
    }

    public string Name { get; }

    public string InsertionText { get; }

    public string Documentation { get; }
}

public static class BlazorDirectivesCatalog
{
    private static readonly IReadOnlyList<BlazorDirective> _items = new List<BlazorDirective>
    {
        new BlazorDirective("@page", "@page \"/route\"", "Sets the routable URL of the component."),
        new BlazorDirective("@layout", "@layout MainLayout", "Sets the layout used by the component."),
        new BlazorDirective("@inherits", "@inherits ComponentBase", "Sets the base class of the component."),
        new BlazorDirective("@implements", "@implements IDisposable", "Adds interfaces implemented by the component."),
        new BlazorDirective("@inject", "@inject IService Service", "Injects a service into the component."),
        new BlazorDirective("@using", "@using Namespace", "Imports a namespace in the generated code."),
        new BlazorDirective("@namespace", "@namespace MyApp.Pages", "Sets the namespace of the generated code."),
        new BlazorDirective("@typeparam", "@typeparam TItem", "Declares a generic type parameter."),
        new BlazorDirective("@attribute", "@attribute [Authorize]", "Adds an attribute to the component."),
        new BlazorDirective("@rendermode", "@rendermode InteractiveServer", "Sets the render mode of the component."),
        new BlazorDirective("@code", "@code { }", "Adds a code block to the component."),
        new BlazorDirective("@functions", "@functions { }", "Adds a functions block to the component."),
        new BlazorDirective("@preservewhitespace", "@preservewhitespace true", "Preserves whitespace between markup elements."),
        new BlazorDirective("@bind", "@bind-Value", "Creates a two-way binding."),
        new BlazorDirective("@onclick", "@onclick=\"Handler\"", "Attaches an event handler."),
        new BlazorDirective("@ref", "@ref Element", "Captures a reference to the rendered element."),
    };

    public static IReadOnlyList<BlazorDirective> Items => _items;
}