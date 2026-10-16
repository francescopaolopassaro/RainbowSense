using System.Reflection;
using RainbowSense.VisualStudio.Completion.Catalogs;
using Xunit;

namespace RainbowSense.Tests;

public class DevExpressCatalogTests
{
    private static Type FindType(string fullName)
    {
        LoadDevExpressAssemblies();

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (!assembly.GetName().Name.StartsWith("DevExpress.Blazor", StringComparison.OrdinalIgnoreCase))
                continue;

            Type type = assembly.GetType(fullName);

            if (type != null)
                return type;

            string name = fullName.Substring(fullName.LastIndexOf('.') + 1);

            foreach (Type candidate in assembly.GetExportedTypes())
            {
                string candidateName = candidate.Name;
                int tick = candidateName.IndexOf('`');

                if (tick > 0)
                    candidateName = candidateName.Substring(0, tick);

                if (candidateName == name)
                    return candidate;
            }
        }

        return null;
    }

    private static void LoadDevExpressAssemblies()
    {
        string directory = System.IO.Path.GetDirectoryName(typeof(DevExpress.Blazor.DxGrid).Assembly.Location);

        foreach (string file in System.IO.Directory.GetFiles(directory, "DevExpress.Blazor.*.dll"))
        {
            try { System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(file); }
            catch { }
        }
    }

    [Fact]
    public void VersionMatchesReferencedPackage()
    {
        string version = typeof(DevExpress.Blazor.DxGrid).Assembly.GetName().Version.ToString();

        Assert.StartsWith(DevExpressComponentsCatalog.Version, version);
    }

    [Fact]
    public void CatalogIsComplete()
    {
        Assert.True(DevExpressComponentsCatalog.Items.Count >= 250, "expected a complete catalog, found " + DevExpressComponentsCatalog.Items.Count);
    }

    [Fact]
    public void EveryTagExistsInDevExpressAssembly()
    {
        List<string> missing = DevExpressComponentsCatalog.Items
            .Where(x => FindType(x.NamespaceName + "." + x.TypeName) == null)
            .Select(x => x.NamespaceName + "." + x.TypeName)
            .ToList();

        Assert.Empty(missing);
    }

    [Fact]
    public void EveryTagHasDocumentation()
    {
        List<string> missing = DevExpressComponentsCatalog.Items
            .Where(x => string.IsNullOrWhiteSpace(x.Documentation))
            .Select(x => x.TagName)
            .ToList();

        Assert.Empty(missing);
    }

    [Fact]
    public void MostComponentsHaveParameters()
    {
        List<string> missing = DevExpressComponentsCatalog.Items
            .Where(x => DevExpressMembersCatalog.GetParameterNames(x.NamespaceName + "." + x.TypeName).Count == 0)
            .Select(x => x.TagName)
            .ToList();

        int total = DevExpressComponentsCatalog.Items.Count;

        Assert.True(total - missing.Count > total * 0.95, "components without parameters: " + string.Join(", ", missing.Take(20)));
    }

    [Fact]
    public void ParametersExistOnRealType()
    {
        List<string> missing = new List<string>();

        int totalMembers = 0;

        foreach (DevExpressComponent component in DevExpressComponentsCatalog.Items)
        {
            Type type = FindType(component.NamespaceName + "." + component.TypeName);

            if (type == null)
                continue;

            string typeName = component.NamespaceName + "." + component.TypeName;

int totalMembers2 = DevExpressMembersCatalog.GetParameterNames(typeName).Count;

            totalMembers += totalMembers2;

            foreach (string name in DevExpressMembersCatalog.GetParameterNames(typeName))
            {
                bool found = false;

                for (Type current = type; current != null; current = current.BaseType)
                {
                    if (current.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly) != null)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    missing.Add(typeName + "." + name);
            }
        }

        Assert.True(missing.Count <= totalMembers * 0.05, "missing members: " + string.Join(", ", missing.Take(20)) + " (total " + missing.Count + ")");
    }

    [Fact]
    public void KnownParametersOfDxGrid()
    {
        string typeName = "DevExpress.Blazor.DxGrid";
        IReadOnlyList<string> parameters = DevExpressMembersCatalog.GetParameterNames(typeName);

        Assert.Contains("Data", parameters);
        Assert.Contains("Columns", parameters);
        Assert.Contains("PageSize", parameters);
        Assert.Contains("ShowGroupPanel", parameters);
    }

    [Fact]
    public void ParameterTypeIsResolved()
    {
        string typeName = "DevExpress.Blazor.DxGrid";

        Assert.Equal("Object", DevExpressMembersCatalog.GetTypeName(typeName, "Data"));
        Assert.Null(DevExpressMembersCatalog.GetTypeName(typeName, "NotAMember"));
    }

    [Theory]
    [InlineData("DxGrid")]
    [InlineData("DxFormLayout")]
    [InlineData("DxComboBox")]
    [InlineData("DxTextBox")]
    [InlineData("DxChart")]
    [InlineData("DxTreeList")]
    [InlineData("DxScheduler")]
    [InlineData("DxPieChart")]
    [InlineData("DxTagBox")]
    [InlineData("DxPivotGrid")]
    [InlineData("DxContextMenu")]
    [InlineData("DxPopup")]
    [InlineData("DxDropDown")]
    [InlineData("DxAccordion")]
    [InlineData("DxCarousel")]
    [InlineData("DxRibbon")]
    [InlineData("DxHtmlEditor")]
    [InlineData("DxGridDataColumn")]
    [InlineData("DxFormLayoutItem")]
        public void CommonComponentsArePresent(string tag)
    {
        Assert.Contains(DevExpressComponentsCatalog.Items, x => x.TagName == tag);
    }

    [Fact]
    public void TagsAreUnique()
    {
        List<string> duplicates = DevExpressComponentsCatalog.Items
            .GroupBy(x => x.TagName, StringComparer.Ordinal)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        Assert.Empty(duplicates);
    }

    [Fact]
    public void NamespacesAreKnown()
    {
        Assert.Contains("DevExpress.Blazor", DevExpressNamespacesCatalog.Items);
    }
}