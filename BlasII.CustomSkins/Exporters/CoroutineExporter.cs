using BlasII.CustomSkins.Finders;
using BlasII.ModdingAPI;
using MelonLoader;
using System.Collections;

namespace BlasII.CustomSkins.Exporters;

/// <summary>
/// Exports all spritesheets as a coroutine with callbacks
/// </summary>
public class CoroutineExporter : IExporter
{
    /// <inheritdoc/>
    public void ExportAll(IFinder finder, string directory)
    {
        MelonCoroutines.Start(ExportProcess(finder, directory));
    }

    private IEnumerator ExportProcess(IFinder finder, string directory)
    {
        OnStart();

        yield return finder.FindAll();

        yield return ExportCoroutine(finder.Result, directory);

        OnFinish();
    }

    /// <summary>
    /// Called before the coroutine
    /// </summary>
    protected virtual void OnStart()
    {
        ModLog.Warn("Starting Export...");
    }

    /// <summary>
    /// Called after the coroutine
    /// </summary>
    protected virtual void OnFinish()
    {
        ModLog.Warn("Finished export");
    }

    /// <summary>
    /// Performs the export process
    /// </summary>
    protected virtual IEnumerator ExportCoroutine(SpriteCollection sprites, string directory)
    {
        yield return null;
    }
}
