using BlasII.ModdingAPI;
using BlasII.ModdingAPI.Assets;
using System.Collections;

namespace BlasII.CustomSkins.Exporters;

/// <summary>
/// Performs the specified export process while ensuring Crisanta is owned
/// </summary>
public class ExporterWithCrisanta(IExporter exporter) : CoroutineExporter
{
    bool _alreadyHasCrisanta = false;

    /// <inheritdoc/>
    protected override IEnumerator ExportCoroutine(SpriteCollection sprites, string directory)
    {
        // Does not work, since the figure needs to be given before the sprites are loaded in

        yield return null;
        exporter.ExportAll(sprites, directory);
    }

    /// <summary>
    /// Gives the crisanta figure if unowned
    /// </summary>
    protected override void OnStart()
    {
        var crisanta = AssetStorage.Figures["FG44"];

        _alreadyHasCrisanta = AssetStorage.PlayerInventory.HasItem(crisanta);
        if (!_alreadyHasCrisanta)
        {
            ModLog.Info("Temporarily adding FG44");
            AssetStorage.PlayerInventory.AddItemAsync(crisanta);
        }
    }

    /// <summary>
    /// Removes the crisanta figure if it was given
    /// </summary>
    protected override void OnFinish()
    {
        if (!_alreadyHasCrisanta)
        {
            ModLog.Info("Removing FG44");

            var crisanta = AssetStorage.Figures["FG44"];
            AssetStorage.PlayerInventory.RemoveItem(crisanta);
        }
    }
}
