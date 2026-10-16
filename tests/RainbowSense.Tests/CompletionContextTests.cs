using RainbowSense.VisualStudio.Completion;
using Xunit;

namespace RainbowSense.Tests;

public class CompletionContextTests
{
    [Fact]
    public void NotRazorFileIsIgnored()
    {
        RainbowCompletionContext context = RainbowCompletionContext.Create("Program.cs", "var x = 1;", 5);

        Assert.NotNull(context);
        Assert.False(context.IsRazorFile);
    }

    [Fact]
    public void DetectsDirective()
    {
        const string text = "@pag";

        RainbowCompletionContext context = RainbowCompletionContext.Create("Index.razor", text, text.Length);

        Assert.True(context.IsRazorFile);
        Assert.True(context.IsDirectiveContext);
    }

    [Fact]
    public void DetectsUsingDirective()
    {
        const string text = "@using DevExpr";

        RainbowCompletionContext context = RainbowCompletionContext.Create("_Imports.razor", text, text.Length);

        Assert.True(context.IsUsingContext);
    }

    [Fact]
    public void DetectsTagName()
    {
        const string text = "<DxG";

        RainbowCompletionContext context = RainbowCompletionContext.Create("Index.razor", text, text.Length);

        Assert.True(context.IsTagContext);
        Assert.Equal("DxG", context.TagName);
        Assert.True(context.IsMarkupContext);
    }

    [Fact]
    public void DetectsEnclosingTag()
    {
        const string text = "<DxGrid>\n    ";

        RainbowCompletionContext context = RainbowCompletionContext.Create("Index.razor", text, text.Length);

        Assert.True(context.IsInsideComponentTag);
        Assert.Equal("DxGrid", context.TagName);
    }

    [Fact]
    public void DetectsAttributeName()
    {
        const string text = "<DxGrid Data";

        RainbowCompletionContext context = RainbowCompletionContext.Create("Index.razor", text, text.Length);

        Assert.Equal("Data", context.AttributeName);
    }

    [Fact]
    public void DetectsCodeBlock()
    {
        const string text = "@code {\n    var x = 1;\n";

        RainbowCompletionContext context = RainbowCompletionContext.Create("Index.razor", text, text.Length);

        Assert.False(context.IsMarkupContext);
    }

    [Fact]
    public void ClosedTagIsNotTagContext()
    {
        const string text = "<DxGrid>";

        RainbowCompletionContext context = RainbowCompletionContext.Create("Index.razor", text, text.Length);

        Assert.False(context.IsTagContext);
        Assert.Equal("DxGrid", context.TagName);
    }
}