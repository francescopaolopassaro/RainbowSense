using System.Collections.Generic;

namespace RainbowSense.VisualStudio.Completion.Catalogs;

public sealed class BlazorInjectable
{
    public BlazorInjectable(string typeName, string defaultName, string documentation)
    {
        TypeName = typeName;
        DefaultName = defaultName;
        Documentation = documentation;
    }

    public string TypeName { get; }

    public string DefaultName { get; }

    public string Documentation { get; }

    public string InsertionText => "@inject " + TypeName + " " + DefaultName;
}

public static class BlazorInjectablesCatalog
{
    private static readonly IReadOnlyList<BlazorInjectable> _items = new List<BlazorInjectable>
    {
        new BlazorInjectable("NavigationManager", "Navigation", "Provides access to the current URI and navigation operations."),
        new BlazorInjectable("IJSRuntime", "JSRuntime", "Invokes JavaScript functions from .NET code."),
        new BlazorInjectable("HttpClient", "Http", "Sends HTTP requests and receives HTTP responses."),
        new BlazorInjectable("ILogger<T>", "Logger", "Writes log messages."),
        new BlazorInjectable("IStringLocalizer<T>", "Localizer", "Provides localized strings."),
        new BlazorInjectable("IStringLocalizerFactory", "LocalizerFactory", "Creates localizer instances."),
        new BlazorInjectable("AuthenticationStateProvider", "AuthenticationStateProvider", "Provides the current authentication state."),
        new BlazorInjectable("IDialogService", "DialogService", "Opens dialogs and modal windows."),
        new BlazorInjectable("ISnackbar", "Snackbar", "Displays snackbar notifications."),
        new BlazorInjectable("MessageService", "MessageService", "Displays confirmation and prompt dialogs."),
        new BlazorInjectable("ThemeService", "ThemeService", "Applies application-wide themes."),
        new BlazorInjectable("PermissionService", "PermissionService", "Evaluates user permissions."),
    };

    public static IReadOnlyList<BlazorInjectable> Items => _items;
}