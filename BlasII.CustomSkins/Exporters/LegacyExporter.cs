﻿using BlasII.CustomSkins.Extensions;
using BlasII.CustomSkins.Models;
using BlasII.ModdingAPI;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BlasII.CustomSkins.Exporters;

/// <summary>
/// Exports all spritesheets individually 
/// </summary>
public class LegacyExporter : IExporter
{
    /// <inheritdoc/>
    public IEnumerator ExportAll(IEnumerable<Sprite> sprites, string directory)
    {
        // Group sprites by name
        var groups = sprites.GroupBy(x => x.name[0..x.name.LastIndexOf('_')]);
        int idx = 0;

        // Export each individual spritesheet
        foreach (var group in groups)
        {
            var spriteGroup = group
                .OrderBy(x => int.Parse(x.name[(x.name.LastIndexOf('_') + 1)..]));

            Export(group.Key, ++idx, directory, spriteGroup);
            yield return null;
        }

        // Ensure all animations were exported
        if (idx != NUM_ANIMATIONS)
        {
            ModLog.Error("Failed to find all animations!");
        }
    }

    private void Export(string animation, int idx, string directory, IEnumerable<Sprite> sprites)
    {
        ModLog.Info($"[{idx}/{NUM_ANIMATIONS}] Exporting {animation}");

        // Create sprite infos and texture
        Dictionary<Sprite, SpriteInfo> infos = CreateSpriteInfos(sprites);
        Texture2D texture = CreateTexture(infos.Values);

        // Copy each sprite onto the texture
        foreach (var kvp in infos)
        {
            Sprite sprite = kvp.Key;
            SpriteInfo info = kvp.Value;
            Graphics.CopyTexture(sprite.GetSlicedTexture(), 0, 0, 0, 0, (int)info.Size.X, (int)info.Size.Y, texture, 0, 0, (int)info.Position.X, (int)info.Position.Y);
        }

        // Save texture to file
        string texturePath = Path.Combine(directory, $"{animation}.png");
        File.WriteAllBytes(texturePath, texture.EncodeToPNG());

        // Save info list to file
        string infoPath = Path.Combine(directory, $"{animation}.json");
        File.WriteAllText(infoPath, JsonConvert.SerializeObject(infos.Values, Formatting.Indented));
    }

    private Dictionary<Sprite, SpriteInfo> CreateSpriteInfos(IEnumerable<Sprite> sprites)
    {
        var infos = new Dictionary<Sprite, SpriteInfo>();

        int x = 0, y = 0, maxRowHeight = 0;
        foreach (var sprite in sprites)
        {
            int w = (int)sprite.rect.width;
            int h = (int)sprite.rect.height;

            if (x + w > MAX_SIZE)
            {
                x = 0;
                y += maxRowHeight;
                maxRowHeight = 0;
            }

            if (h > maxRowHeight)
                maxRowHeight = h;

            infos.Add(sprite, new SpriteInfo()
            {
                Name = sprite.name,
                PixelsPerUnit = (int)sprite.pixelsPerUnit,
                Position = new Vector(x, y),
                Size = new Vector(w, h),
                Pivot = new Vector(sprite.pivot.x / w, sprite.pivot.y / h),
            });

            x += w;
        }

        return infos;
    }

    private Texture2D CreateTexture(IEnumerable<SpriteInfo> infos)
    {
        int width = (int)infos.Max(info => info.Position.X + info.Size.X);
        int height = (int)infos.Max(info => info.Position.Y + info.Size.Y);
        Texture2D tex = new Texture2D(width, height);
        Object.Destroy(tex);

        // Fill transparent
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                tex.SetPixel(i, j, new Color32(0, 0, 0, 0));

        return tex;
    }

    private const int MAX_SIZE = 2048;
    private const int NUM_ANIMATIONS = 540;
}
