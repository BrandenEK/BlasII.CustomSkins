using BlasII.CustomSkins.Models;
using BlasII.ModdingAPI;
using Il2CppTGK.Game;
using System.Collections.Generic;
using System.IO;

namespace BlasII.CustomSkins;

/// <summary>
/// Allows using custom player skins made by the community
/// </summary>
public class CustomSkins : BlasIIMod
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

    protected override void OnUpdate()
    {
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.P))
        {
            ModLog.Warn("equip");
            CoreCache.PlayerRecolorManager.SetPalette(_loadedSkins[0].Palette);
        }
    }
}
