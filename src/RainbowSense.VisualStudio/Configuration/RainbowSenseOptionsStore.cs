using System;
using System.IO;

namespace RainbowSense.VisualStudio.Configuration;

public sealed class RainbowSenseOptionsStore
{
    private static readonly object _gate = new object();

    private static RainbowSenseOptions _current = new RainbowSenseOptions();

    public static RainbowSenseOptions Current => _current;

    public static string FilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "RainbowSense",
        "settings.txt");

    public static void Load()
    {
        lock (_gate)
        {
            RainbowSenseOptions options = new RainbowSenseOptions();

            try
            {
                if (File.Exists(FilePath))
                {
                    foreach (string line in File.ReadAllLines(FilePath))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        int index = line.IndexOf('=');

                        if (index <= 0)
                            continue;

                        Apply(options, line.Substring(0, index).Trim(), line.Substring(index + 1).Trim());
                    }
                }
            }
            catch (Exception)
            {
            }

            options.MaxComponentResults = Clamp(options.MaxComponentResults, 10, 2000);

            _current = options;
        }
    }

    public static void Save(RainbowSenseOptions options)
    {
        lock (_gate)
        {
            _current = options.Clone();

            try
            {
                string directory = Path.GetDirectoryName(FilePath);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                File.WriteAllLines(
                    FilePath,
                    new[]
                    {
                        "EnableBlazorDirectives=" + options.EnableBlazorDirectives,
                        "EnableBlazorInjectables=" + options.EnableBlazorInjectables,
                        "EnableBlazorMarkupComponents=" + options.EnableBlazorMarkupComponents,
                        "EnableRazorComponentTags=" + options.EnableRazorComponentTags,
                        "EnableDevExpressComponents=" + options.EnableDevExpressComponents,
                        "EnableDevExpressNamespaces=" + options.EnableDevExpressNamespaces,
                        "IncludeDevExpressNamespacesInRazor=" + options.IncludeDevExpressNamespacesInRazor,
                        "SortBlazorItemsFirst=" + options.SortBlazorItemsFirst,
                        "MaxComponentResults=" + options.MaxComponentResults,
                    });
            }
            catch (Exception)
            {
            }
        }
    }

    public static RainbowSenseOptions CloneCurrent()
    {
        lock (_gate)
        {
            return _current.Clone();
        }
    }

    private static void Apply(RainbowSenseOptions options, string name, string value)
    {
        switch (name.ToLowerInvariant())
        {
            case "enableblazordirectives":
                options.EnableBlazorDirectives = ParseBool(value, options.EnableBlazorDirectives);
                break;
            case "enableblazorinjectables":
                options.EnableBlazorInjectables = ParseBool(value, options.EnableBlazorInjectables);
                break;
            case "enableblazormarkupcomponents":
                options.EnableBlazorMarkupComponents = ParseBool(value, options.EnableBlazorMarkupComponents);
                break;
            case "enablerazorcomponenttags":
                options.EnableRazorComponentTags = ParseBool(value, options.EnableRazorComponentTags);
                break;
            case "enabledevexpresscomponents":
                options.EnableDevExpressComponents = ParseBool(value, options.EnableDevExpressComponents);
                break;
            case "enabledevexpressnamespaces":
                options.EnableDevExpressNamespaces = ParseBool(value, options.EnableDevExpressNamespaces);
                break;
            case "includedevexpressnamespacesinrazor":
                options.IncludeDevExpressNamespacesInRazor = ParseBool(value, options.IncludeDevExpressNamespacesInRazor);
                break;
            case "sortblazoritemfirst":
                options.SortBlazorItemsFirst = ParseBool(value, options.SortBlazorItemsFirst);
                break;
            case "maxcomponentresults":
                if (int.TryParse(value, out int max))
                    options.MaxComponentResults = max;
                break;
        }
    }

    private static bool ParseBool(string value, bool fallback)
    {
        if (bool.TryParse(value, out bool result))
            return result;

        if (value == "1")
            return true;

        if (value == "0")
            return false;

        return fallback;
    }

    private static int Clamp(int value, int min, int max)
    {
        if (value < min)
            return min;

        if (value > max)
            return max;

        return value;
    }
}