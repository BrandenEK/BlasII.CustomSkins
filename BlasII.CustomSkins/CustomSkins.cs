using BlasII.CheatConsole;
using BlasII.CustomSkins.Exporters;
using BlasII.CustomSkins.Extensions;
using BlasII.CustomSkins.Finders;
using BlasII.CustomSkins.Importers;
using BlasII.CustomSkins.Models;
using BlasII.ModdingAPI;
using BlasII.ModdingAPI.Helpers;
using BlasII.ModdingAPI.Persistence;
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
public class CustomSkins : BlasIIMod, IGlobalPersistentMod<SkinGlobalSaveData>
{
    internal CustomSkins() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    private SkinConfig _config;
    private SpriteCollection _loadedSprites = [];
    private bool _loadedDefault = false;

    /// <summary>
    /// The currently selected skin id
    /// </summary>
    public string CurrentSkin { get; private set; } = string.Empty;

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
    protected override void OnAllInitialized()
    {
        _config = ConfigHandler.Load<SkinConfig>();

        if (!_config.UsePerformanceMode)
            ModLog.Warn("The setting 'UsePerformanceMode' is disabled. If you experience significant frame drops, enable this setting in the config at the cost of certain animations not being replaced.");
    }

    /// <summary>
    /// Update skin with default when loading menu for the first time
    /// </summary>
    protected override void OnSceneLoaded(string sceneName)
    {
        if (SceneHelper.MenuSceneLoaded)
            PerformDefaultLoad();
    }

    /// <summary>
    /// Replace TPO sprites every frame with a loaded one
    /// </summary>
    protected override void OnLateUpdate()
    {
        if (!SceneHelper.GameSceneLoaded || CoreCache.PlayerSpawn.PlayerInstance == null)
            return;

        var renderers = _config.UsePerformanceMode
            ? CoreCache.PlayerSpawn.PlayerInstance.GetComponentsInChildren<SpriteRenderer>()
            : UnityEngine.Object.FindObjectsOfType<SpriteRenderer>();

        // Replace all TPO sprites that were loaded
        foreach (var renderer in renderers)
        {
            string name = renderer.sprite?.GetUniqueName();

            if (string.IsNullOrEmpty(name) || !_loadedSprites.TryGetValue(name, out Sprite customSprite))
                continue;

            renderer.sprite = customSprite;
        }
    }

    private void PerformDefaultLoad()
    {
        if (_loadedDefault)
            return;

        _loadedDefault = true;
        string path = Path.Combine(FileHandler.ModdingFolder, "skins");

        Directory.CreateDirectory(path);
        StartImport(path, ReplaceSkin);
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
    public void StartExport(string type, string directory)
    {
        if (!Directory.Exists(directory))
        {
            ModLog.Error($"{directory} does not exist. Failed to export spritesheets");
            return;
        }

        MelonCoroutines.Start(ExportCoroutine(type, directory));
    }

    private IEnumerator ExportCoroutine(string type, string directory)
    {
        IFinder finder = new FinderWithCrisanta(new ResourcesFinder());
        IExporter exporter = new BetterExporterTwoStep(_config.ExportAnimationWidth, _config.ExportGroupHeight);

        ModLog.Warn("Starting export...");
        yield return finder.FindAll();
        yield return exporter.ExportAll(finder.Result, type, directory);
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

    // Persistence methods

    /// <summary>
    /// Save the current skin
    /// </summary>
    public SkinGlobalSaveData SaveGlobal()
    {
        return new SkinGlobalSaveData()
        {
            SelectedSkin = CurrentSkin
        };
    }

    /// <summary>
    /// Load the current skin
    /// </summary>
    public void LoadGlobal(SkinGlobalSaveData data)
    {
        CurrentSkin = data.SelectedSkin;
    }
}
