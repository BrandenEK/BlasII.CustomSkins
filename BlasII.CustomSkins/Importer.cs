using BlasII.ModdingAPI;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace BlasII.CustomSkins;

/// <summary>
/// Handles importing all custom sprites
/// </summary>
internal class Importer(string path)
{
    private readonly string _importFolder = path;

    public Dictionary<string, Sprite> ImportAll()
    {
        ModLog.Warn("Starting Import...");

        // Create output dictionary
        var output = new Dictionary<string, Sprite>();

        // Create import folder
        Directory.CreateDirectory(_importFolder);

        // Get all json files in the import folder
        foreach (var file in Directory.GetFiles(_importFolder, "*.json", SearchOption.TopDirectoryOnly))
        {
            Import(Path.GetFileNameWithoutExtension(file), output);
        }

        return output;
    }

    private void Import(string animation, Dictionary<string, Sprite> output)
    {
        // Import info list from import folder
        var json = File.ReadAllText(Path.Combine(_importFolder, $"{animation}.json"));
        var infos = JsonConvert.DeserializeObject<SpriteInfo[]>(json);

        // Import texture from import folder
        var bytes = File.ReadAllBytes(Path.Combine(_importFolder, $"{animation}.png"));
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        texture.LoadImage(bytes, false);
        texture.filterMode = FilterMode.Point;

        // Load each sprite from the texture based on its info
        foreach (var info in infos)
        {
            ModLog.Info($"Importing {info.Name}");

            var rect = new Rect(info.Position, info.Size);
            var sprite = Sprite.Create(texture, rect, info.Pivot, info.PixelsPerUnit);
            sprite.hideFlags = HideFlags.DontUnloadUnusedAsset;

            output.Add(info.Name, sprite);
        }
    }
}
