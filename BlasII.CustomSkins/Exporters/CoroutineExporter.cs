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
    public void ExportAll(SpriteCollection sprites, string directory)
    {
        MelonCoroutines.Start(ExportProcess(sprites, directory));
    }

    private IEnumerator ExportProcess(SpriteCollection sprites, string directory)
    {
        OnStart();

        yield return ExportCoroutine(sprites, directory);

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
