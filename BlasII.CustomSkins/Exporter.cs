using BlasII.CustomSkins.Extensions;
using BlasII.ModdingAPI;
using MelonLoader;
using Newtonsoft.Json;
using System.Collections;
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
        MelonCoroutines.Start(ExportCoroutine(export));
    }

    private IEnumerator ExportCoroutine(Dictionary<string, Sprite> export)
    {
        // Group sprites by name
        var groups = export.Take(100).GroupBy(x => x.Key[0..x.Key.LastIndexOf('_')]);

        // Export each individual spritesheet
        foreach (var group in groups)
        {
            var sprites = group
                .OrderBy(x => int.Parse(x.Key[(x.Key.LastIndexOf('_') + 1)..]))
                .Select(x => x.Value);

            Export(group.Key, sprites);
            yield return null;
        }
    }

    private void Export(string animation, IEnumerable<Sprite> sprites)
    {
        ModLog.Info($"Exporting {animation}");

        // Create entire animation texture
        int width = (int)sprites.Sum(x => x.rect.width);
        int height = (int)sprites.Max(x => x.rect.height);
        if (width > 16384 || height > 16384)
        {
            ModLog.Error("Size too big, failed to export texture");
            return;
        }
        Texture2D tex = new Texture2D(width, height);
        Object.Destroy(tex);

        // Fill transparent
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                tex.SetPixel(i, j, new Color32(0, 0, 0, 0));

        // Create empty info list
        SpriteInfo[] infos = new SpriteInfo[sprites.Count()];

        // Copy each sprite to the texture and save info
        int x = 0, y = 0, idx = 0;
        foreach (var sprite in sprites)
        {
            //ModLog.Info($"Exporting {sprite.name}");
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



    private Vector GetMaximumSize(IEnumerable<Sprite> sprites)
    {
        int totalWidth = 0, totalHeight = 0, rowHeight = 0, x = 0, y = 0;
        foreach (var sprite in sprites)
        {
            int w = (int)sprite.rect.width;
            int h = (int)sprite.rect.height;
            x += w;

            // Move to next row
            if (x > MAX_SIZE)
            {
                x = w;
                y += rowHeight;
                totalHeight += rowHeight;
                rowHeight = 0;
            }

            // Update max row height
            if (h > rowHeight)
                rowHeight = h;

            // Update max spritesheet width
            if (x > totalWidth)
                totalWidth = x;
        }

        totalHeight += rowHeight;
        return new Vector(totalWidth, totalHeight);
    }

    //private Vector GetMaximumSize(IEnumerable<Sprite> sprites)
    //{
    //    int totalWidth = 0, totalHeight = 0, rowHeight = 0, x = 0, y = 0;
    //    foreach (var sprite in sprites)
    //    {
    //        int w = (int)sprite.rect.width;
    //        int h = (int)sprite.rect.height;

    //        // Pretend to copy sprite onto global texture

    //        x += w;

    //        // Validate next position

    //        if (x > MAX_SIZE)
    //        {
    //            x = 0;
    //            y += rowHeight;
    //            totalHeight += rowHeight;
    //            rowHeight = 0;
    //        }

    //        // Update max row height
    //        if (h > rowHeight)
    //            rowHeight = h;

    //        x += w;

    //        // Update max spritesheet width
    //        if (x + w > totalWidth)
    //            totalWidth = x + w;
    //    }

    //    totalHeight += rowHeight;
    //    return new Vector(totalWidth, totalHeight);
    //}

    private IEnumerable<SpriteWithLocation> FindLocations(IEnumerable<Sprite> sprites)
    {
        var locations = new List<SpriteWithLocation>();

        int x = 0, y = 0, maxRowHeight = 0;
        foreach (var sprite in sprites)
        {
            int w = (int)sprite.rect.width;
            int h = (int)sprite.rect.height;

            if (h > maxRowHeight)
                maxRowHeight = h;

            if (x + w > MAX_SIZE)
            {
                x = 0;
                y += maxRowHeight;
                maxRowHeight = 0;
            }

            locations.Add(new SpriteWithLocation()
            {
                Sprite = sprite,
                Location = new Vector(x, y)
            });

            x += w;
        }

        return locations;
    }

    private const int MAX_SIZE = 1024;

    class SpriteWithLocation()
    {
        public Sprite Sprite { get; init; }
        public Vector Location { get; init; }
    }
}
