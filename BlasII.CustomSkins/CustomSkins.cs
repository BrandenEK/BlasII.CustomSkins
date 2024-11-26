using BlasII.CheatConsole;
using BlasII.CustomSkins.Exporters;
using BlasII.ModdingAPI;
using BlasII.ModdingAPI.Helpers;
using Il2CppTGK.Game;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BlasII.CustomSkins;

/// <summary>
/// Allows using custom player skins made by the community
/// </summary>
public class CustomSkins : BlasIIMod
{
    internal CustomSkins() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    private Dictionary<string, Sprite> _loadedSprites = [];
    private bool _loadedDefault = false;

    /// <summary>
    /// Imports the sprites
    /// </summary>
    public Importer Importer { get; } = new Importer();
    /// <inheritdoc cref="IExporter"/>
    public IExporter Exporter { get; } = new FakeExporter();

    /// <summary>
    /// Registers the skin command
    /// </summary>
    protected override void OnRegisterServices(ModServiceProvider provider)
    {
        provider.RegisterCommand(new SkinCommand());
    }

    /// <summary>
    /// Update skin with default when loading menu for the first time
    /// </summary>
    protected override void OnSceneLoaded(string sceneName)
    {
        if (_loadedDefault || !SceneHelper.MenuSceneLoaded)
            return;

        _loadedDefault = true;
        var spritesheets = Importer.ImportAll(Path.Combine(FileHandler.ModdingFolder, "skins"));
        UpdateSkin(spritesheets);
    }

    /// <summary>
    /// Replace TPO sprites every frame with a loaded one
    /// </summary>
    protected override void OnLateUpdate()
    {
        if (!SceneHelper.GameSceneLoaded || CoreCache.PlayerSpawn.PlayerInstance == null)
            return;

        // Replace all TPO sprites that were loaded
        foreach (var renderer in CoreCache.PlayerSpawn.PlayerInstance.GetComponentsInChildren<SpriteRenderer>())
        {
            if (renderer.sprite == null || string.IsNullOrEmpty(renderer.sprite.name))
                continue;

            if (!_loadedSprites.TryGetValue(renderer.sprite.name, out Sprite customSprite))
                continue;

            renderer.sprite = customSprite;
        }
    }

    private void OnStartExport()
    {

    }

    private void OnFinishExport()
    {

    }

    /// <summary>
    /// Updates the skin based on the new sprites
    /// </summary>
    public void UpdateSkin(Dictionary<string, Sprite> sprites)
    {
        _loadedSprites = sprites;
    }
}
