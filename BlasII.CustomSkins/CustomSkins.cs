using BlasII.CheatConsole;
using BlasII.CustomSkins.Exporters;
using BlasII.CustomSkins.Finders;
using BlasII.CustomSkins.Importers;
using BlasII.CustomSkins.Models;
using BlasII.ModdingAPI;
using BlasII.ModdingAPI.Helpers;
using Il2CppTGK.Game;
using MelonLoader;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace BlasII.CustomSkins;

/// <summary>
/// Allows using custom player skins made by the community
/// </summary>
public class CustomSkins : BlasIIMod
{
    internal CustomSkins() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    private SkinConfig _config;
    private SpriteCollection _loadedSprites = [];
    private bool _loadedDefault = false;

    /// <summary>
    /// Registers the skin command
    /// </summary>
    protected override void OnRegisterServices(ModServiceProvider provider)
    {
        provider.RegisterCommand(new SkinCommand());
    }

    /// <summary>
    /// Loads the config properties
    /// </summary>
    protected override void OnInitialize()
    {
        _config = ConfigHandler.Load<SkinConfig>();
    }

    /// <summary>
    /// Update skin with default when loading menu for the first time
    /// </summary>
    protected override void OnSceneLoaded(string sceneName)
    {
        if (_loadedDefault || !SceneHelper.MenuSceneLoaded)
            return;

        _loadedDefault = true;
        StartImport(Path.Combine(FileHandler.ModdingFolder, "skins"), ReplaceSkin);
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

    // Import methods

    /// <summary>
    /// Starts the import process
    /// </summary>
    public void StartImport(string directory, Action<SpriteCollection> callback)
    {
        if (!Directory.Exists(directory))
        {
            ModLog.Error($"{directory} does not exist. Failed to import spritesheets");
            return;
        }

        MelonCoroutines.Start(ImportCoroutine(directory, callback));
    }

    private IEnumerator ImportCoroutine(string directory, Action<SpriteCollection> callback)
    {
        IImporter importer = new SimpleImporter(_config.ImportsPerFrame);

        ModLog.Warn("Starting import...");
        yield return importer.ImportAll(directory);
        ModLog.Warn("Finished import");

        callback(importer.Result);
    }

    // Export methods

    /// <summary>
    /// Starts the export process
    /// </summary>
    public void StartExport(string directory)
    {
        if (!Directory.Exists(directory))
        {
            ModLog.Error($"{directory} does not exist. Failed to export spritesheets");
            return;
        }

        MelonCoroutines.Start(ExportCoroutine(directory));
    }

    private IEnumerator ExportCoroutine(string directory)
    {
        IFinder finder = new FinderWithCrisanta(new ResourcesFinder());
        IExporter exporter = new BetterExporterLessRam(_config.ExportAnimationWidth, _config.ExportGroupHeight);

        ModLog.Warn("Starting export...");
        yield return finder.FindAll();
        yield return exporter.ExportAll(finder.Result, directory);
        ModLog.Warn("Finished export");
    }

    // Update methods

    /// <summary>
    /// Merges the skin with the new one
    /// </summary>
    public void MergeSkin(SpriteCollection sprites)
    {
        foreach (var kvp in sprites)
        {
            _loadedSprites[kvp.Key] = kvp.Value;
        }
    }

    /// <summary>
    /// Replaces skin with the new one
    /// </summary>
    public void ReplaceSkin(SpriteCollection sprites)
    {
        _loadedSprites = sprites;
    }

    /// <summary>
    /// Resets skin to default
    /// </summary>
    public void ResetSkin()
    {
        _loadedSprites = [];
    }
}
