using BlasII.ModdingAPI.Persistence;

namespace BlasII.CustomSkins;

/// <summary>
/// Stores global data for the Custom Skins mod
/// </summary>
public class SkinGlobalSaveData : GlobalSaveData
{
    /// <summary>
    /// The id of the currently selected skin
    /// </summary>
    public string SelectedSkin { get; set; } = string.Empty;
}
