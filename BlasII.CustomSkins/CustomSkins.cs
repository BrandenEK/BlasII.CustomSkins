using BlasII.ModdingAPI;
using Newtonsoft.Json;
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

    private readonly Dictionary<string, Sprite> _loadedSprites = [];

    /// <summary>
    /// Load all spritesheets
    /// </summary>
    protected override void OnInitialize()
    {
        LoadAllSpritesheets();
    }

    private void LoadAllSpritesheets()
    {
        ModLog.Warn("Starting Import...");

        // Get all json files in the skins folder
        string dir = Path.Combine(FileHandler.ModdingFolder, "skins");
        foreach (var file in Directory.GetFiles(dir, "*.json", SearchOption.TopDirectoryOnly))
        {
            LoadSpritesheet(Path.GetFileNameWithoutExtension(file));
        }
    }

    private void LoadSpritesheet(string animation)
    {
        // Import info list from skins folder
        var json = File.ReadAllText(Path.Combine(FileHandler.ModdingFolder, "skins", $"{animation}.json"));
        var infos = JsonConvert.DeserializeObject<SpriteInfo[]>(json);

        // Import texture from skins folder
        var bytes = File.ReadAllBytes(Path.Combine(FileHandler.ModdingFolder, "skins", $"{animation}.png"));
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        texture.LoadImage(bytes, false);
        texture.filterMode = FilterMode.Point;

        // Load each sprite from the texture based on its info
        foreach (var info in infos)
        {
            ModLog.Info($"Importing {info.Name}");

            var rect = new Rect(info.Position, 0, info.Width, info.Height);
            var sprite = Sprite.Create(texture, rect, new Vector2(info.Pivot, 0), info.PixelsPerUnit);
            sprite.hideFlags = HideFlags.DontUnloadUnusedAsset;

            _loadedSprites.Add(info.Name, sprite);
        }
    }
}
