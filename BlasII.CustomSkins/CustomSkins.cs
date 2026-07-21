using BlasII.CustomSkins.Models;
using BlasII.ModdingAPI;
using BlasII.ModdingAPI.Persistence;
using System.Collections.Generic;
using System.IO;

namespace BlasII.CustomSkins;

/// <summary>
/// Allows using custom player skins made by the community
/// </summary>
public class CustomSkins : BlasIIMod, IGlobalPersistentMod<SkinGlobalSaveData>
{
    internal CustomSkins() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    private List<SkinData> _loadedSkins;
    private string _selectedSkin = string.Empty;

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
        return new SkinGlobalSaveData()
        {
            SelectedSkin = _selectedSkin
        };
    }

    /// <summary>
    /// Loads the current selected custom skin
    /// </summary>
    public void LoadGlobal(SkinGlobalSaveData data)
    {
        ModLog.Error("Loading the skin: " + data.SelectedSkin);
        _selectedSkin = data.SelectedSkin ?? string.Empty;
    }

    internal IEnumerable<SkinData> GetAllSkins() => _loadedSkins;

    internal SkinData GetSkin(int index) => _loadedSkins[index];
}
