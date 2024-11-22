using BlasII.ModdingAPI;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace BlasII.CustomSkins;

/// <summary>
/// Handles loading all custom sprites
/// </summary>
internal class Importer(string skinsFolder)
{
    private readonly string _skinsFolder = skinsFolder;

    public Dictionary<string, Sprite> LoadAllSpritesheets()
    {
        ModLog.Warn("Starting Import...");

        // Create output dictionary
        var output = new Dictionary<string, Sprite>();

        // Create skins folder
        //string dir = Path.Combine(Main.CustomSkins.FileHandler.ModdingFolder, "skins");
        Directory.CreateDirectory(_skinsFolder);

        // Get all json files in the skins folder
        foreach (var file in Directory.GetFiles(_skinsFolder, "*.json", SearchOption.TopDirectoryOnly))
        {
            LoadSpritesheet(Path.GetFileNameWithoutExtension(file), output);
        }

        return output;
    }

    private void LoadSpritesheet(string animation, Dictionary<string, Sprite> output)
    {
        // Import info list from skins folder
        var json = File.ReadAllText(Path.Combine(_skinsFolder, $"{animation}.json"));
        var infos = JsonConvert.DeserializeObject<SpriteInfo[]>(json);

        // Import texture from skins folder
        var bytes = File.ReadAllBytes(Path.Combine(_skinsFolder, $"{animation}.png"));
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

            output.Add(info.Name, sprite);
        }
    }
}
