using BlasII.ModdingAPI;
using BlasII.ModdingAPI.Helpers;
using Il2CppTGK.Game;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BlasII.CustomSkins;

/// <summary>
/// Allows using custom player skins made by the community
/// </summary>
public class CustomSkins : BlasIIMod
{
    internal CustomSkins() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    private Dictionary<string, Sprite> _loadedSprites = [];

    /// <summary>
    /// Load all spritesheets
    /// </summary>
    protected override void OnInitialize()
    {
        var importer = new Importer(Path.Combine(FileHandler.ModdingFolder, "skins"));
        _loadedSprites = importer.ImportAll();
    }

    /// <summary>
    /// Replace TPO sprites every frame with a loaded one
    /// </summary>
    protected override void OnLateUpdate()
    {
        // temp
        if (Input.GetKeyDown(KeyCode.F9))
        {
            //var exporter = new Exporter(FileHandler.ContentFolder);
            //exporter.ExportAll([]);
        }
        // temp
        if (Input.GetKeyDown(KeyCode.F10))
        {
            //foreach (var s in Resources.FindObjectsOfTypeAll<Sprite>().Where(x => x.name.StartsWith("TPO")).OrderBy(x => x.name))
            //{
            //    ModLog.Info(s.name);
            //}

            var sprites = Resources.FindObjectsOfTypeAll<Sprite>()
                .Where(x => x.name.StartsWith("TPO"))
                .DistinctBy(x => x.name)
                .OrderBy(x => x.name)
                .ToDictionary(x => x.name, x => x);

            var exporter = new Exporter(FileHandler.ContentFolder);
            exporter.ExportAll(sprites);
        }

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
}
