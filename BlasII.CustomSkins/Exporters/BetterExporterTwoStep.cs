﻿using BlasII.CustomSkins.Models;
using BlasII.ModdingAPI;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace BlasII.CustomSkins.Exporters;

/// <summary>
/// Exports animations and groups, in a two step process to save ram
/// </summary>
public class BetterExporterTwoStep : IExporter
{
    private readonly int _animationWidthPixels;
    private readonly int _groupHeightPixels;
    private SpriteSheetWithoutTexture _currentSheet;

    /// <summary>
    /// Creates a new IExporter
    /// </summary>
    public BetterExporterTwoStep(int animationWidthPixels, int groupHeightPixels)
    {
        _animationWidthPixels = animationWidthPixels;
        _groupHeightPixels = groupHeightPixels;
    }

    /// <inheritdoc/>
    public IEnumerator ExportAll(SpriteCollection sprites, string directory)
    {
        // Load groups from data folder
        if (!Main.CustomSkins.FileHandler.LoadDataAsJson("groups.json", out AnimationGroup[] groups))
        {
            ModLog.Error("Failed to read data: groups.json");
            yield break;
        }

        // Split sprites by group and export them
        foreach (var spritesByGroup in sprites.Values.GroupBy(x => GetGroupName(x, groups)).OrderBy(x => x.Key))
        {
            string group = spritesByGroup.Key;
            var groupAnimations = spritesByGroup.OrderBy(x => x.name);

            ModLog.Info($"Exporting group {group}");
            yield return ExportGroup(groupAnimations, group, Path.Combine(directory, group));

            // Save group texture
            SaveSpriteSheet(directory, CreateTextureFromSheet(_currentSheet));
        }

        _currentSheet = null;
    }

    private IEnumerator ExportGroup(IEnumerable<Sprite> sprites, string groupName, string directory)
    {
        var sheets = new List<SpriteSheetWithoutTexture>();

        // Split sprites by animation and export them
        foreach (var spritesByAnimation in sprites.GroupBy(GetAnimationName).OrderBy(x => x.Key))
        {
            string animation = spritesByAnimation.Key;
            var animationFrames = spritesByAnimation.OrderBy(x => int.Parse(x.name[(x.name.LastIndexOf('_') + 1)..]));

            ModLog.Info($"Exporting animation {animation}");
            yield return ExportAnimation(animationFrames, animation, Path.Combine(directory, animation));
            sheets.Add(_currentSheet);

            // Save animation texture
            SaveSpriteSheet(directory, CreateTextureFromSheet(_currentSheet));
        }

        // Return sheet for group
        _currentSheet = CombineSpriteSheets(groupName, true, _groupHeightPixels, sheets);
    }

    private IEnumerator ExportAnimation(IEnumerable<Sprite> sprites, string animationName, string directory)
    {
        var sheets = new List<SpriteSheetWithoutTexture>();

        // Split sprites by frame and export them
        foreach (var sprite in sprites)
        {
            string frame = sprite.name;

            //ModLog.Info($"Exporting frame {frame}");
            ExportFrame(sprite);
            sheets.Add(_currentSheet);
        }

        // Return sheet for animation
        _currentSheet = CombineSpriteSheets(animationName, false, _animationWidthPixels, sheets);
        yield return null;
    }

    private void ExportFrame(Sprite sprite)
    {
        int w = (int)sprite.rect.width;
        int h = (int)sprite.rect.height;

        var info = new SpriteInfo()
        {
            Name = sprite.name,
            PixelsPerUnit = (int)sprite.pixelsPerUnit,
            Position = new Vector(0, 0),
            Size = new Vector(w, h),
            Pivot = new Vector(sprite.pivot.x / w, sprite.pivot.y / h),
        };

        // Return sheet for frame
        _currentSheet = new SpriteSheetWithoutTexture()
        {
            Name = info.Name,
            Size = info.Size,
            Infos = [info],
        };
    }

    private SpriteSheet CreateTextureFromSheet(SpriteSheetWithoutTexture sheet)
    {
        int width = (int)sheet.Size.X;
        int height = (int)sheet.Size.Y;

        // Create new texture
        Texture2D tex = new Texture2D(width, height);
        Object.Destroy(tex);

        // Fill transparent pixels
        Color32[] colors = new Color32[width * height];
        Color32 transparent = new Color32(0, 0, 0, 0);
        for (int i = 0; i < colors.Length; i++)
            colors[i] = transparent;
        tex.SetPixels32(colors, 0);

        // Copy each sprite's texture onto the combined one
        foreach (var info in sheet.Infos)
        {
            Texture texture = info.Texture;
            Object.Destroy(texture);

            Graphics.CopyTexture(texture, 0, 0, 0, 0, texture.width, texture.height, tex, 0, 0, (int)info.Position.X, (int)info.Position.Y);
        }

        // Return new spritesheet
        return new SpriteSheet()
        {
            Name = sheet.Name,
            Texture = tex,
            Infos = sheet.Infos,
        };
    }

    private SpriteSheetWithoutTexture CombineSpriteSheets(string name, bool vertical, int maxSize, IEnumerable<SpriteSheetWithoutTexture> sheets)
    {
        // Modify the position of all SpriteInfos in each sheet
        if (vertical)
            ModifySpriteInfosVertical(sheets, maxSize);
        else
            ModifySpriteInfosHorizontal(sheets, maxSize);

        // Calculate new max size
        var allInfos = sheets.SelectMany(x => x.Infos);
        float width = allInfos.Max(info => info.Position.X + info.Size.X);
        float height = allInfos.Max(info => info.Position.Y + info.Size.Y);

        // Return sheet for combination
        return new SpriteSheetWithoutTexture()
        {
            Name = name,
            Size = new Vector(width, height),
            Infos = allInfos,
        };
    }

    private void ModifySpriteInfosHorizontal(IEnumerable<SpriteSheetWithoutTexture> sheets, int maxWidth)
    {
        int x = 0, y = 0, maxRowHeight = 0;
        foreach (var sheet in sheets)
        {
            int w = (int)sheet.Size.X;
            int h = (int)sheet.Size.Y;

            if (x + w > maxWidth)
            {
                x = 0;
                y += maxRowHeight;
                maxRowHeight = 0;
            }

            if (h > maxRowHeight)
                maxRowHeight = h;

            foreach (var info in sheet.Infos)
                info.Position = new Vector(info.Position.X + x, info.Position.Y + y);

            x += w;
        }
    }

    private void ModifySpriteInfosVertical(IEnumerable<SpriteSheetWithoutTexture> sheets, int maxHeight)
    {
        int x = 0, y = 0, maxColumnWidth = 0;
        foreach (var sheet in sheets)
        {
            int w = (int)sheet.Size.X;
            int h = (int)sheet.Size.Y;

            if (y + h > maxHeight)
            {
                y = 0;
                x += maxColumnWidth;
                maxColumnWidth = 0;
            }

            if (w > maxColumnWidth)
                maxColumnWidth = w;

            foreach (var info in sheet.Infos)
                info.Position = new Vector(info.Position.X + x, info.Position.Y + y);

            y += h;
        }
    }

    private string GetAnimationName(Sprite sprite)
    {
        return sprite.name[0..sprite.name.LastIndexOf('_')];
    }

    private string GetGroupName(Sprite sprite, IEnumerable<AnimationGroup> groups)
    {
        string name = GetAnimationName(sprite);
        AnimationGroup group = groups.FirstOrDefault(x => x.Animations.Contains(name));

        return group?.GroupName ?? "unknown";
    }

    private void SaveSpriteSheet(string directory, SpriteSheet sheet)
    {
        // Ensure directory exists
        Directory.CreateDirectory(directory);

        // Save texture to file
        string texturePath = Path.Combine(directory, $"{sheet.Name}.png");
        File.WriteAllBytes(texturePath, sheet.Texture.EncodeToPNG());

        // Save info list to file
        string infoPath = Path.Combine(directory, $"{sheet.Name}.json");
        File.WriteAllText(infoPath, JsonConvert.SerializeObject(sheet.Infos, Formatting.Indented));
    }
}
