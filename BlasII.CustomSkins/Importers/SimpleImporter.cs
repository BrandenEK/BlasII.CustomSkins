using BlasII.ModdingAPI;
using Newtonsoft.Json;
using System.Collections;
using System.IO;
using UnityEngine;

namespace BlasII.CustomSkins.Importers;

/// <summary>
/// Handles importing all custom sprites
/// </summary>
public class SimpleImporter : IImporter
{
    /// <inheritdoc/>
    public SpriteCollection Result { get; private set; }

    /// <inheritdoc/>
    public IEnumerator ImportAll(string directory)
    {
        ModLog.Warn("Starting Import...");

        // Create output dictionary
        Result = [];

        // Create import folder
        Directory.CreateDirectory(directory);

        // Get all json files in the import folder
        foreach (var file in Directory.GetFiles(directory, "*.json", SearchOption.TopDirectoryOnly))
        {
            Import(Path.GetFileNameWithoutExtension(file), directory);
            yield return null;
        }

        ModLog.Warn("Finished import");
    }

    private void Import(string animation, string directory)
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

            Result.Add(info.Name, sprite);
        }
    }
}
