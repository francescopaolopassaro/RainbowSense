using System.Collections.Generic;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using RainbowSense.VisualStudio.Completion.Catalogs;
using RainbowSense.VisualStudio.Configuration;

namespace RainbowSense.VisualStudio.Completion.Providers;

[Export(typeof(CompletionProvider))]
[Shared]
public sealed class DevExpressCompletionProvider : CompletionProvider
{
    public override async Task ProvideCompletionsAsync(CompletionContext context)
    {
        if (context.CancellationToken.IsCancellationRequested)
            return;

        RainbowSenseOptions options = RainbowSenseOptionsStore.CloneCurrent();

        if (!options.EnableDevExpressComponents || !options.EnableDevExpressNamespaces)
            return;

        SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

        if (semanticModel == null || !IsDevExpressProject(semanticModel))
            return;

        List<CompletionItem> items = new List<CompletionItem>();

        foreach (string ns in DevExpressNamespacesCatalog.Items)
        {
            items.Add(RainbowCompletionItems.CreateNamespace(ns, "DevExpress Blazor " + DevExpressComponentsCatalog.Version + " namespace."));
        }

        context.AddItems(items);
    }

    private static bool IsDevExpressProject(SemanticModel semanticModel)
    {
        foreach (IAssemblySymbol assembly in semanticModel.Compilation.SourceModule.ReferencedAssemblySymbols)
        {
            if (assembly.Name.StartsWith("DevExpress", System.StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}