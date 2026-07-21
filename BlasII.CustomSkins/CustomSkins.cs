using BlasII.CustomSkins.Models;
using BlasII.ModdingAPI;
using BlasII.ModdingAPI.Persistence;
using Il2CppTGK.Game;
using Il2CppTGK.Game.Managers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BlasII.CustomSkins;

/// <summary>
/// Allows using custom player skins made by the community
/// </summary>
public class CustomSkins : BlasIIMod, IGlobalPersistentMod<SkinGlobalSaveData>
{
    internal CustomSkins() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    private List<SkinData> _loadedSkins;

    /// <summary>
    /// Loads all skins from the modding folder
    /// </summary>
    protected override void OnAllInitialized()
    {
        var importer = new SkinImporter();
        _loadedSkins = importer.LoadAllSkins(Path.Combine(FileHandler.ModdingFolder, "custom_skins"));
    }

    /// <summary>
    /// Saves the current selected custom skin
    /// </summary>
    public SkinGlobalSaveData SaveGlobal()
    {
        int current = CoreCache.PlayerRecolorManager.currentPalette;
        var pal = Resources.FindObjectsOfTypeAll<PaletteID>().FirstOrDefault(x => x.id == current);

        //ModLog.Warn("Saving custom skin: " + (pal == null || pal.name.StartsWith("PLT") ? string.Empty : pal.name));

        return new SkinGlobalSaveData()
        {
            SelectedSkin = pal == null || pal.name.StartsWith("PLT") ? string.Empty : pal.name
        };
    }

    /// <summary>
    /// Loads the current selected custom skin
    /// </summary>
    public void LoadGlobal(SkinGlobalSaveData data)
    {
        //ModLog.Warn("Loading custom skin: " + data.SelectedSkin);

        if (string.IsNullOrEmpty(data.SelectedSkin))
            return;

        var skin = _loadedSkins.FirstOrDefault(x => x.Info.Id == data.SelectedSkin);

        if (skin == null)
            return;

        CoreCache.PlayerRecolorManager.SetPalette(skin.Palette);
    }

    internal IEnumerable<SkinData> GetAllSkins() => _loadedSkins;

    internal SkinData GetSkin(int index) => _loadedSkins[index];
}
