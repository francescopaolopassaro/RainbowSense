using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using RainbowSense.VisualStudio.Configuration;

namespace RainbowSense.VisualStudio.Completion.Providers;

[Export(typeof(CompletionProvider))]
[Shared]
public sealed class RainbowSenseCompletionProvider : CompletionProvider
{
    public override async Task ProvideCompletionsAsync(CompletionContext context)
    {
        if (context.CancellationToken.IsCancellationRequested)
            return;

        RainbowSenseOptions options = RainbowSenseOptionsStore.CloneCurrent();

        RainbowCompletionContext razorContext = await RainbowCompletionContext
            .CreateAsync(context.Document, context.Position)
            .ConfigureAwait(false);

        if (razorContext == null || !razorContext.IsRazorFile)
            return;

        var items = RainbowCompletionBuilder.GetItems(razorContext, options);

        if (items.Count == 0)
            return;

        context.AddItems(items);
    }

    public override Task<CompletionDescription> GetDescriptionAsync(Document document, CompletionItem item, System.Threading.CancellationToken cancellationToken)
    {
        string description = RainbowCompletionItems.GetDescription(item);

        if (string.IsNullOrEmpty(description))
            return base.GetDescriptionAsync(document, item, cancellationToken);

        return Task.FromResult(CompletionDescription.FromText(description));
    }
}