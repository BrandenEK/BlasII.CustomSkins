using BlasII.CustomSkins.Extensions;
using BlasII.ModdingAPI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BlasII.CustomSkins;

/// <summary>
/// Handles exporting all found sprites
/// </summary>
internal class Exporter(string path)
{
    private readonly string _exportFolder = path;

    public void ExportAll(Dictionary<string, Sprite> export)
    {
        ModLog.Warn("Starting Export...");

        // Group sprites by name
        var groups = export.GroupBy(x => x.Key[0..x.Key.LastIndexOf('_')]);

        // Export each individual spritesheet
        foreach (var group in groups)
        {
            var sprites = group
                .OrderBy(x => int.Parse(x.Key[(x.Key.LastIndexOf('_') + 1)..]))
                .Select(x => x.Value);

            Export(group.Key, sprites);
        }
    }

    private void Export(string animation, IEnumerable<Sprite> sprites)
    {
        // Create entire animation texture
        int width = (int)sprites.Sum(x => x.rect.width);
        int height = (int)sprites.Max(x => x.rect.height);
        Texture2D tex = new Texture2D(width, height);

        // Fill transparent
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                tex.SetPixel(i, j, new Color32(0, 0, 0, 0));

        // Create empty info list
        SpriteInfo[] infos = new SpriteInfo[sprites.Count()];

        // Copy each sprite to the texture and save info
        int x = 0, idx = 0;
        foreach (var sprite in sprites)
        {
            ModLog.Info($"Exporting {sprite.name}");
            int w = (int)sprite.rect.width;
            int h = (int)sprite.rect.height;
            Graphics.CopyTexture(sprite.GetSlicedTexture(), 0, 0, 0, 0, w, h, tex, 0, 0, x, 0);

            infos[idx] = new SpriteInfo()
            {
                Name = sprite.name,
                PixelsPerUnit = (int)sprite.pixelsPerUnit,
                Position = new Vector(x, 0),
                Size = new Vector(w, h),
                Pivot = new Vector(sprite.pivot.x / w, sprite.pivot.y / h),
            };

            x += w;
            idx++;
        }

        // Save texture to file
        string texturePath = Path.Combine(_exportFolder, $"{animation}.png");
        File.WriteAllBytes(texturePath, tex.EncodeToPNG());

        // Save info list to file
        string infoPath = Path.Combine(_exportFolder, $"{animation}.json");
        File.WriteAllText(infoPath, JsonConvert.SerializeObject(infos, Formatting.Indented));
    }
}
