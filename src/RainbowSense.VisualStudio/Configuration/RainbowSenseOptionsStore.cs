using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace RainbowSense.VisualStudio.Configuration;

public sealed class RainbowSenseOptions
{
    public bool EnableBlazorDirectives { get; set; } = true;

    public bool EnableBlazorInjectables { get; set; } = true;

    public bool EnableBlazorMarkupComponents { get; set; } = true;

    public bool EnableRazorComponentTags { get; set; } = true;

    public bool EnableDevExpressComponents { get; set; } = true;

    public bool EnableDevExpressNamespaces { get; set; } = true;

    public bool IncludeDevExpressNamespacesInRazor { get; set; } = true;

    public bool SortBlazorItemsFirst { get; set; } = true;

    public int MaxComponentResults { get; set; } = 500;

    public RainbowSenseOptions Clone() => (RainbowSenseOptions)MemberwiseClone();

    public void CopyFrom(RainbowSenseOptions other)
    {
        if (other == null)
            return;

        EnableBlazorDirectives = other.EnableBlazorDirectives;
        EnableBlazorInjectables = other.EnableBlazorInjectables;
        EnableBlazorMarkupComponents = other.EnableBlazorMarkupComponents;
        EnableRazorComponentTags = other.EnableRazorComponentTags;
        EnableDevExpressComponents = other.EnableDevExpressComponents;
        EnableDevExpressNamespaces = other.EnableDevExpressNamespaces;
        IncludeDevExpressNamespacesInRazor = other.IncludeDevExpressNamespacesInRazor;
        SortBlazorItemsFirst = other.SortBlazorItemsFirst;
        MaxComponentResults = other.MaxComponentResults;
    }
}

public static class RainbowSenseOptionsStore
{
    private const int MinResults = 10;
    private const int MaxResults = 2000;

    private static readonly object _gate = new object();

    private static RainbowSenseOptions _current = new RainbowSenseOptions();

    public static RainbowSenseOptions Current => _current;

    public static string FilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "RainbowSense",
        "settings.txt");

    public static RainbowSenseOptions CloneCurrent()
    {
        lock (_gate)
        {
            return _current.Clone();
        }
    }

    public static void Load()
    {
        lock (_gate)
        {
            RainbowSenseOptions options = new RainbowSenseOptions();

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

            options.MaxComponentResults = Clamp(options.MaxComponentResults, MinResults, MaxResults);

            _current = options;
        }
    }

    public static void Save(RainbowSenseOptions options)
    {
        if (options == null)
            return;

        lock (_gate)
        {
            RainbowSenseOptions normalized = options.Clone();
            normalized.MaxComponentResults = Clamp(normalized.MaxComponentResults, MinResults, MaxResults);

            _current = normalized;

            string directory = Path.GetDirectoryName(FilePath);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string content = Serialize(normalized);
            string temporary = FilePath + ".tmp";

            File.WriteAllText(temporary, content, Encoding.UTF8);

            if (File.Exists(FilePath))
                File.Delete(FilePath);

            File.Move(temporary, FilePath);
        }
    }

    private static string Serialize(RainbowSenseOptions options)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("# RainbowSense settings");
        sb.AppendLine("EnableBlazorDirectives=" + options.EnableBlazorDirectives.ToString(CultureInfo.InvariantCulture));
        sb.AppendLine("EnableBlazorInjectables=" + options.EnableBlazorInjectables.ToString(CultureInfo.InvariantCulture));
        sb.AppendLine("EnableBlazorMarkupComponents=" + options.EnableBlazorMarkupComponents.ToString(CultureInfo.InvariantCulture));
        sb.AppendLine("EnableRazorComponentTags=" + options.EnableRazorComponentTags.ToString(CultureInfo.InvariantCulture));
        sb.AppendLine("EnableDevExpressComponents=" + options.EnableDevExpressComponents.ToString(CultureInfo.InvariantCulture));
        sb.AppendLine("EnableDevExpressNamespaces=" + options.EnableDevExpressNamespaces.ToString(CultureInfo.InvariantCulture));
        sb.AppendLine("IncludeDevExpressNamespacesInRazor=" + options.IncludeDevExpressNamespacesInRazor.ToString(CultureInfo.InvariantCulture));
        sb.AppendLine("SortBlazorItemsFirst=" + options.SortBlazorItemsFirst.ToString(CultureInfo.InvariantCulture));
        sb.AppendLine("MaxComponentResults=" + options.MaxComponentResults.ToString(CultureInfo.InvariantCulture));

        return sb.ToString();
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
            case "sortblazoritemsfirst":
                options.SortBlazorItemsFirst = ParseBool(value, options.SortBlazorItemsFirst);
                break;
            case "maxcomponentresults":
                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int max))
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