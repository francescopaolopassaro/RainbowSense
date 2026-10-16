using System.IO;
using RainbowSense.VisualStudio.Configuration;
using Xunit;

namespace RainbowSense.Tests;

public class OptionsStoreTests
{
    [Fact]
    public void DefaultsAreEnabled()
    {
        RainbowSenseOptions options = RainbowSenseOptionsStore.CloneCurrent();

        Assert.True(options.EnableBlazorDirectives);
        Assert.True(options.EnableDevExpressComponents);
        Assert.True(options.SortBlazorItemsFirst);
    }

    [Fact]
    public void SaveAndLoadRoundTrip()
    {
        RainbowSenseOptions options = RainbowSenseOptionsStore.CloneCurrent();

        options.EnableDevExpressComponents = false;
        options.MaxComponentResults = 42;

        RainbowSenseOptionsStore.Save(options);

        RainbowSenseOptionsStore.Load();

        RainbowSenseOptions reloaded = RainbowSenseOptionsStore.CloneCurrent();

        Assert.False(reloaded.EnableDevExpressComponents);
        Assert.Equal(42, reloaded.MaxComponentResults);

        RainbowSenseOptions restored = RainbowSenseOptionsStore.CloneCurrent();
        restored.EnableDevExpressComponents = true;
        restored.MaxComponentResults = 500;

        RainbowSenseOptionsStore.Save(restored);
    }

    [Fact]
    public void MaxResultsIsClamped()
    {
        RainbowSenseOptions options = RainbowSenseOptionsStore.CloneCurrent();
        options.MaxComponentResults = 100000;

        RainbowSenseOptionsStore.Save(options);
        RainbowSenseOptionsStore.Load();

        int value = RainbowSenseOptionsStore.CloneCurrent().MaxComponentResults;

        Assert.InRange(value, 10, 2000);

        RainbowSenseOptionsStore.FilePath.ToString();
        Assert.True(Directory.Exists(Path.GetDirectoryName(RainbowSenseOptionsStore.FilePath)));
    }

    [Fact]
    public void CloneIsIndependent()
    {
        RainbowSenseOptions options = RainbowSenseOptionsStore.CloneCurrent();
        RainbowSenseOptions clone = options.Clone();

        clone.EnableBlazorDirectives = !options.EnableBlazorDirectives;

        Assert.NotEqual(options.EnableBlazorDirectives, clone.EnableBlazorDirectives);
    }
}