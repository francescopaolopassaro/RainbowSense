using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using RainbowSense.VisualStudio.Configuration;
using Task = System.Threading.Tasks.Task;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[Guid("2b6f0c4e-51d9-4a3f-9f52-9a0c3d1e77b4")]
[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string, PackageAutoLoadFlags.BackgroundLoad)]
[ComVisible(true)]
public sealed class VSPackage : AsyncPackage
{
    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        await base.InitializeAsync(cancellationToken, progress);

        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        Instance = this;

        RainbowSenseOptionsStore.Load();
    }

    internal static VSPackage Instance { get; private set; }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            RainbowSenseOptionsStore.Save(RainbowSenseOptionsStore.CloneCurrent());

            Instance = null;
        }

        base.Dispose(disposing);
    }
}