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
/// Exports animations and groups, in a two step process to save ram
/// </summary>
public class BetterExporterTwoStep : IExporter
{
    private readonly int _animationWidthPixels;
    private readonly int _groupHeightPixels;

    private SpriteSheetWithoutTexture _exportResult;
    private SpriteSheet _creationResult;

    private int _totalTextures;
    private int _currentTextureIndex;

    /// <summary>
    /// Creates a new IExporter
    /// </summary>
    public BetterExporterTwoStep(int animationWidthPixels, int groupHeightPixels)
    {
        _animationWidthPixels = animationWidthPixels;
        _groupHeightPixels = groupHeightPixels;
    }

    /// <inheritdoc/>
    public IEnumerator ExportAll(IEnumerable<Sprite> sprites, string type, string directory)
    {
        // Load specified groups from data folder
        IEnumerable<AnimationGroup> groups = LoadAllAnimationGroups(type);

        _totalTextures = groups.SelectMany(x => x.Animations).Count();
        _currentTextureIndex = 0;

        // Split sprites by group and export them
        foreach (var spritesByGroup in sprites.GroupBy(x => GetGroupName(x, groups)).OrderBy(x => x.Key))
        {
            // This will probably go away with the new group update (TPO)
            // Unfortunately it did not
            if (spritesByGroup.Key == "unknown")
            {
                //ModLog.Warn("Unknown sprites:");
                //foreach (Sprite s in spritesByGroup.OrderBy(x => x.name))
                //    ModLog.Info(s.GetUniqueName());
                continue;
            }

            string group = spritesByGroup.Key;
            var groupAnimations = spritesByGroup.OrderBy(x => x.name);

            ModLog.Info($"Exporting group {group}");
            yield return ExportGroup(groupAnimations, group, Path.Combine(directory, group));

            // Save group texture
            yield return CreateTextureFromSheet(_exportResult);
            SaveSpriteSheet(directory, _creationResult);
            Object.Destroy(_creationResult.Texture);
        }

        if (_totalTextures != _currentTextureIndex)
            ModLog.Error("Not all textures were exported!");

        _exportResult = null;
        _creationResult = null;
    }

    private IEnumerator ExportGroup(IEnumerable<Sprite> sprites, string groupName, string directory)
    {
        var sheets = new List<SpriteSheetWithoutTexture>();

        // Split sprites by animation and export them
        foreach (var spritesByAnimation in sprites.GroupBy(GetAnimationName).OrderBy(x => x.Key))
        {
            string animation = spritesByAnimation.Key;
            var animationFrames = spritesByAnimation.OrderBy(GetFrameOrder);

            ModLog.Info($"[{++_currentTextureIndex}/{_totalTextures}] Exporting animation {animation}");
            ExportAnimation(animationFrames, animation, Path.Combine(directory, animation));
            sheets.Add(_exportResult);

            // Save animation texture
            yield return CreateTextureFromSheet(_exportResult);
            SaveSpriteSheet(directory, _creationResult);
            Object.Destroy(_creationResult.Texture);
        }

        // Return sheet for group
        _exportResult = CombineSpriteSheets(groupName, true, _groupHeightPixels, sheets);
    }

    private void ExportAnimation(IEnumerable<Sprite> sprites, string animationName, string directory)
    {
        var sheets = new List<SpriteSheetWithoutTexture>();

        // Split sprites by frame and export them
        foreach (var sprite in sprites)
        {
            string frame = sprite.name;

            //ModLog.Info($"Exporting frame {frame}");
            ExportFrame(sprite);
            sheets.Add(_exportResult);
        }

        // Return sheet for animation
        _exportResult = CombineSpriteSheets(animationName, false, _animationWidthPixels, sheets);
    }

    private void ExportFrame(Sprite sprite)
    {
        int w = (int)sprite.rect.width;
        int h = (int)sprite.rect.height;

        var info = new SpriteInfoWithTexture()
        {
            Name = sprite.GetUniqueName(),
            PixelsPerUnit = (int)sprite.pixelsPerUnit,
            Position = new Vector(0, 0),
            Size = new Vector(w, h),
            Pivot = new Vector(sprite.pivot.x / w, sprite.pivot.y / h),
            Texture = sprite,
        };

        // Return sheet for frame
        _exportResult = new SpriteSheetWithoutTexture()
        {
            Name = info.Name,
            Size = info.Size,
            Infos = [info],
        };
    }

    private IEnumerator CreateTextureFromSheet(SpriteSheetWithoutTexture sheet)
    {
        // Create new texture
        int width = (int)sheet.Size.X;
        int height = (int)sheet.Size.Y;
        Texture2D tex = CreateEmptyTexture(width, height);

        // Copy each sprite's texture onto the combined one
        int idx = 0;
        foreach (var info in sheet.Infos)
        {
            Texture2D texture = info.Texture.GetSlicedTexture();
            Object.Destroy(texture);

            Graphics.CopyTexture(texture, 0, 0, 0, 0, texture.width, texture.height, tex, 0, 0, (int)info.Position.X, (int)info.Position.Y);

            if (idx++ % EXPORTS_PER_FRAME == 0)
                yield return null;
        }

        if (idx <= EXPORTS_PER_FRAME)
            yield return null;

        // Return new spritesheet
        _creationResult = new SpriteSheet()
        {
            Name = sheet.Name,
            Texture = tex,
            Infos = sheet.Infos,
        };
    }

    private Texture2D CreateEmptyTexture(int w, int h)
    {
        Color32[] colors = new Color32[w * h];
        Texture2D tex = new Texture2D(w, h);
        tex.SetPixels32(colors, 0);

        return tex;
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
        return sprite.texture.name;
    }

    private string GetGroupName(Sprite sprite, IEnumerable<AnimationGroup> groups)
    {
        string name = GetAnimationName(sprite);
        AnimationGroup group = groups.FirstOrDefault(x => x.Animations.Contains(name));

        return group?.GroupName ?? "unknown";
    }

    private int GetFrameOrder(Sprite sprite)
    {
        if (sprite.name.IndexOf('_') <= 0)
            return 0;

        string last = sprite.name[(sprite.name.LastIndexOf('_') + 1)..];
        return int.TryParse(last, out int value) ? value : 0;
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

    private IEnumerable<AnimationGroup> LoadAllAnimationGroups(string type)
    {
        var groups = new List<AnimationGroup>();

        IEnumerable<string> searchTypes = type.Split('+').Where(x => !string.IsNullOrEmpty(x));
        string folder = Path.Combine(Main.CustomSkins.FileHandler.ModdingFolder, "data", "Custom Skins");
        
        foreach (string groupFile in Directory.GetFiles(folder, "*.txt"))
        {
            string name = Path.GetFileNameWithoutExtension(groupFile);

            if (type != "all" && !searchTypes.Any(name.StartsWith))
                continue;

            Main.CustomSkins.FileHandler.LoadDataAsArray(Path.GetFileName(groupFile), out string[] animations);
            groups.Add(new AnimationGroup()
            {
                GroupName = name,
                Animations = animations
            });
        }

        ModLog.Info($"Loaded {groups.Count} animation groups");
        return groups;
    }

    // No time/ram difference when changing this, so Im not adding it to the config
    private const int EXPORTS_PER_FRAME = 30;
}
