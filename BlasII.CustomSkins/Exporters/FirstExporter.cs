using BlasII.CustomSkins.Extensions;
using BlasII.ModdingAPI;
using MelonLoader;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BlasII.CustomSkins.Exporters;

/// <summary>
/// Handles exporting all found sprites
/// </summary>
public class FirstExporter : IExporter
{
    /// <summary>
    /// Exports all spritesheets into the directory
    /// </summary>
    public void ExportAll(Dictionary<string, Sprite> export, string directory)
    {
        ModLog.Warn("Starting Export...");
        OnStartExport?.Invoke();

        MelonCoroutines.Start(ExportCoroutine(export, directory));
    }

    private IEnumerator ExportCoroutine(Dictionary<string, Sprite> export, string directory)
    {
        // Group sprites by name
        var groups = export.GroupBy(x => x.Key[0..x.Key.LastIndexOf('_')]);

        // Export each individual spritesheet
        foreach (var group in groups)
        {
            var sprites = group
                .OrderBy(x => int.Parse(x.Key[(x.Key.LastIndexOf('_') + 1)..]))
                .Select(x => x.Value);

            Export(group.Key, directory, sprites);
            yield return null;
        }

        OnFinishExport?.Invoke();
        ModLog.Warn("Finished export");
    }

    private void Export(string animation, string directory, IEnumerable<Sprite> sprites)
    {
        ModLog.Info($"Exporting {animation}");

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

    internal delegate void ExportDelegate();
    internal ExportDelegate OnStartExport;
    internal ExportDelegate OnFinishExport;
}
