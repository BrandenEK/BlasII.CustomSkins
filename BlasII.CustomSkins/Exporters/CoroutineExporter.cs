using BlasII.ModdingAPI;
using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlasII.CustomSkins.Exporters;

/// <summary>
/// Exports all spritesheets as a coroutine with callbacks
/// </summary>
public class CoroutineExporter : IExporter
{
    /// <inheritdoc/>
    public void ExportAll(Dictionary<string, Sprite> export, string directory)
    {
        MelonCoroutines.Start(ExportProcess(export, directory));
    }

    private IEnumerator ExportProcess(Dictionary<string, Sprite> export, string directory)
    {
        OnStart();

        yield return ExportCoroutine(export, directory);

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
    protected virtual IEnumerator ExportCoroutine(Dictionary<string, Sprite> export, string directory)
    {
        yield return null;
    }
}
