using BlasII.ModdingAPI;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace BlasII.CustomSkins;

/// <summary>
/// Handles importing all custom sprites
/// </summary>
public class Importer
{
    /// <summary>
    /// Imports all spritesheets in the directory
    /// </summary>
    public Dictionary<string, Sprite> ImportAll(string directory)
    {
        ModLog.Warn("Starting Import...");

        // Create output dictionary
        var output = new Dictionary<string, Sprite>();

        // Create import folder
        Directory.CreateDirectory(directory);

        // Get all json files in the import folder
        foreach (var file in Directory.GetFiles(directory, "*.json", SearchOption.TopDirectoryOnly))
        {
            Import(Path.GetFileNameWithoutExtension(file), directory, output);
        }

        ModLog.Warn("Finished import");
        return output;
    }

    private void Import(string animation, string directory, Dictionary<string, Sprite> output)
    {
        ModLog.Info($"Importing {animation}");

        // Import info list from import folder
        var json = File.ReadAllText(Path.Combine(directory, $"{animation}.json"));
        var infos = JsonConvert.DeserializeObject<SpriteInfo[]>(json);

        // Import texture from import folder
        var bytes = File.ReadAllBytes(Path.Combine(directory, $"{animation}.png"));
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        texture.LoadImage(bytes, false);
        texture.filterMode = FilterMode.Point;

        // Load each sprite from the texture based on its info
        foreach (var info in infos)
        {
            var rect = new Rect(info.Position, info.Size);
            var sprite = Sprite.Create(texture, rect, info.Pivot, info.PixelsPerUnit);
            sprite.hideFlags = HideFlags.DontUnloadUnusedAsset;

            output.Add(info.Name, sprite);
        }
    }
}
