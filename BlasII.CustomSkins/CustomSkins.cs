using BlasII.ModdingAPI;
using System.IO;

namespace BlasII.CustomSkins;

/// <summary>
/// Allows using custom player skins made by the community
/// </summary>
public class CustomSkins : BlasIIMod
{
    internal CustomSkins() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    /// <summary>
    /// Loads all skins from the modding folder
    /// </summary>
    protected override void OnAllInitialized()
    {
        var importer = new SkinImporter();
        importer.LoadAllSkins(Path.Combine(FileHandler.ModdingFolder, "custom_skins"));
    }
}
