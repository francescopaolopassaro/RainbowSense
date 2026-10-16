using System.Collections.Generic;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using RainbowSense.VisualStudio.Configuration;

namespace RainbowSense.VisualStudio.Completion.Providers;

[Export(typeof(CompletionProvider))]
[Shared]
public sealed class RazorDirectiveCompletionProvider : CompletionProvider
{
    public override async Task ProvideCompletionsAsync(CompletionContext context)
    {
        if (context.CancellationToken.IsCancellationRequested)
            return;

        RainbowSenseOptions options = RainbowSenseOptionsStore.CloneCurrent();

        if (!options.EnableBlazorDirectives)
            return;

        RainbowCompletionContext razorContext = await RainbowCompletionContext.CreateAsync(context.Document, context.Position).ConfigureAwait(false);

        if (razorContext == null || !razorContext.IsRazorFile)
            return;

        IReadOnlyList<CompletionItem> items = RainbowCompletionBuilder.GetItems(razorContext, options);

        if (items.Count == 0)
            return;

        context.AddItems(items);
    }
}