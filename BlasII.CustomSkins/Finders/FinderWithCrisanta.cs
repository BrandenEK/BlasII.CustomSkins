using BlasII.ModdingAPI.Assets;
using BlasII.ModdingAPI;
using System.Collections;

namespace BlasII.CustomSkins.Finders;

/// <summary>
/// Finds the sprites the specified way, but with the Crisanta figure
/// </summary>
public class FinderWithCrisanta(IFinder finder) : IFinder
{
    bool _alreadyHasCrisanta = false;

    /// <inheritdoc/>
    public SpriteCollection Result => finder.Result;

    /// <inheritdoc/>
    public IEnumerator FindAll()
    {
        GiveCrisanta();

        yield return null;
        yield return finder.FindAll();

        RemoveCrisanta();
    }

    private void GiveCrisanta()
    {
        var crisanta = AssetStorage.Figures["FG44"];

        _alreadyHasCrisanta = AssetStorage.PlayerInventory.HasItem(crisanta);
        if (_alreadyHasCrisanta)
            return;

        ModLog.Info("Temporarily adding FG44");
        AssetStorage.PlayerInventory.AddItemAsync(crisanta);
    }

    private void RemoveCrisanta()
    {
        if (_alreadyHasCrisanta)
            return;

        ModLog.Info("Removing FG44");
        var crisanta = AssetStorage.Figures["FG44"];
        AssetStorage.PlayerInventory.RemoveItem(crisanta);
    }
}
