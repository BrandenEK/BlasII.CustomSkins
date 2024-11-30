using BlasII.CustomSkins.Extensions;
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
/// Not sure yet
/// </summary>
public class BetterExporterLessRam : IExporter
{
    private readonly int _animationWidthPixels;
    private readonly int _groupHeightPixels;
    private SpriteSheetWithPosition _currentSheet;

    /// <summary>
    /// Creates a new IExporter
    /// </summary>
    public BetterExporterLessRam(int animationWidthPixels, int groupHeightPixels)
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

        var sheets = new List<SpriteSheetWithPosition>();

        // Split sprites by group and export them
        foreach (var spritesByGroup in sprites.Values.GroupBy(x => GetGroupName(x, groups)).OrderBy(x => x.Key))
        {
            string group = spritesByGroup.Key;
            var groupAnimations = spritesByGroup.OrderBy(x => x.name);

            ModLog.Info($"Exporting group {group}");
            yield return ExportGroup(groupAnimations, group, Path.Combine(directory, group));
            sheets.Add(_currentSheet);

            // Save and destroy group texture
            SaveSpriteSheet(directory, _currentSheet);
            Object.Destroy(_currentSheet.Texture);
        }

        _currentSheet = null;
    }

    private IEnumerator ExportGroup(IEnumerable<Sprite> sprites, string groupName, string directory)
    {
        var sheets = new List<SpriteSheetWithPosition>();
        SpriteSheetWithPosition groupSheet = null;

        // Split sprites by animation and export them
        foreach (var spritesByAnimation in sprites.GroupBy(GetAnimationName).OrderBy(x => x.Key))
        {
            string animation = spritesByAnimation.Key;
            var animationFrames = spritesByAnimation.OrderBy(x => int.Parse(x.name[(x.name.LastIndexOf('_') + 1)..]));

            ModLog.Info($"Exporting animation {animation}");
            yield return ExportAnimation(animationFrames, animation, Path.Combine(directory, animation));
            groupSheet = AddSheet(groupSheet, _currentSheet);

            // Save animation texture
            SaveSpriteSheet(directory, _currentSheet);
            Object.Destroy(_currentSheet.Texture);
        }

        // Destroy animation textures
        //foreach (var sheet in sheets)
        //    Object.Destroy(sheet.Texture);

        // Combine all animations
        _currentSheet = groupSheet;// CombineSpriteSheets(groupName, true, _groupHeightPixels, sheets);
    }

    private SpriteSheetWithPosition AddSheet(SpriteSheetWithPosition source, SpriteSheetWithPosition addition)
    {
        if (source == null)
            return addition;

        int w = System.Math.Max(source.Texture.width, addition.Texture.width);
        int h = source.Texture.height + addition.Texture.height;
        int x =

        if (h > 6500)
        {
            w = source.Texture.width + addition.Texture.width;
            h = source.Texture.height;
        }



        //ADd addition onto the source and change their sprite infos
        return source;
    }

    private IEnumerator ExportAnimation(IEnumerable<Sprite> sprites, string animationName, string directory)
    {
        SpriteSheetWithPosition animationSheet = null;

        // Split sprites by frame and export them
        foreach (var sprite in sprites)
        {
            string frame = sprite.name;

            //ModLog.Info($"Exporting frame {frame}");
            SpriteSheetWithPosition frameSheet = ExportFrame(sprite);

            animationSheet = AddFrameToAnimation(animationName, animationSheet, frameSheet);
            // Dont save frame textures
        }

        _currentSheet = animationSheet;
        yield return null;
    }

    private SpriteSheetWithPosition AddFrameToAnimation(string name, SpriteSheetWithPosition anim, SpriteSheetWithPosition frame)
    {
        if (anim == null)
        {
            return new SpriteSheetWithPosition()
            {
                Name = name,
                Texture = frame.Texture,
                Infos = frame.Infos,
                NextPosition = frame.NextPosition,
            };

        }

        int x = (int)anim.NextPosition.X;
        int y = (int)anim.NextPosition.Y;

        // Possibly wrap around
        if (x + frame.Texture.width > _animationWidthPixels)
        {
            x = 0;
            y = ?;
        }

        // Add new spriteinfo
        foreach (SpriteInfo info in frame.Infos)
            info.Position = new Vector(info.Position.X + x, info.Position.Y + y);

        // Create new texture
        int w = System.Math.Max(anim.Texture.width, x + frame.Texture.width);
        int h = System.Math.Max(anim.Texture.height, y + frame.Texture.height);
        Texture2D texture = new Texture2D(w, h);

        // Fill transparent pixels
        Color32[] colors = new Color32[w * h];
        Color32 transparent = new Color32(0, 0, 0, 0);
        for (int i = 0; i < colors.Length; i++)
            colors[i] = transparent;
        texture.SetPixels32(colors, 0);

        // Add old and new textures onto this one
        Graphics.CopyTexture(anim.Texture, 0, 0, 0, 0, anim.Texture.width, anim.Texture.height, texture, 0, 0, 0, 0);
        Graphics.CopyTexture(frame.Texture, 0, 0, 0, 0, frame.Texture.width, frame.Texture.height, texture, 0, 0, x, y);

        // Destroy old textures
        Object.Destroy(anim.Texture);
        Object.Destroy(frame.Texture);

        return new SpriteSheetWithPosition()
        {
            Name = name,
            Texture = texture,
            Infos = anim.Infos.Concat(frame.Infos),
            NextPosition = new Vector(x, y),
        };
    }

    private SpriteSheetWithPosition ExportFrame(Sprite sprite)
    {
        int w = (int)sprite.rect.width;
        int h = (int)sprite.rect.height;

        var texture = sprite.GetSlicedTexture();
        var info = new SpriteInfo()
        {
            Name = sprite.name,
            PixelsPerUnit = (int)sprite.pixelsPerUnit,
            Position = new Vector(0, 0),
            Size = new Vector(w, h),
            Pivot = new Vector(sprite.pivot.x / w, sprite.pivot.y / h),
        };

        return new SpriteSheetWithPosition()
        {
            Name = sprite.name,
            Texture = texture,
            Infos = [info],
            NextPosition = new Vector(w, 0)
        };
    }

    private SpriteSheet CombineSpriteSheets(string name, bool vertical, int maxSize, IEnumerable<SpriteSheet> sheets)
    {
        // Modify the position of all SpriteInfos in each sheet
        if (vertical)
            ModifySpriteInfosVertical(sheets, maxSize);
        else
            ModifySpriteInfosHorizontal(sheets, maxSize);

        // Create new bigger texture
        var allInfos = sheets.SelectMany(x => x.Infos);
        int width = (int)allInfos.Max(info => info.Position.X + info.Size.X);
        int height = (int)allInfos.Max(info => info.Position.Y + info.Size.Y);
        Texture2D tex = new Texture2D(width, height);

        // Fill transparent pixels
        Color32[] colors = new Color32[width * height];
        Color32 transparent = new Color32(0, 0, 0, 0);
        for (int i = 0; i < colors.Length; i++)
            colors[i] = transparent;
        tex.SetPixels32(colors, 0);

        // Copy each sheet's texture onto the bigger one
        foreach (var sheet in sheets)
        {
            Texture texture = sheet.Texture;
            SpriteInfo info = sheet.Infos.First();
            Graphics.CopyTexture(texture, 0, 0, 0, 0, texture.width, texture.height, tex, 0, 0, (int)info.Position.X, (int)info.Position.Y);
        }

        // Return new single spritesheet
        return new SpriteSheet()
        {
            Name = name,
            Texture = tex,
            Infos = allInfos,
        };
    }

    private void ModifySpriteInfosHorizontal(IEnumerable<SpriteSheet> sheets, int maxWidth)
    {
        int x = 0, y = 0, maxRowHeight = 0;
        foreach (var sheet in sheets)
        {
            int w = sheet.Texture.width;
            int h = sheet.Texture.height;

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

    private void ModifySpriteInfosVertical(IEnumerable<SpriteSheet> sheets, int maxHeight)
    {
        int x = 0, y = 0, maxColumnWidth = 0;
        foreach (var sheet in sheets)
        {
            int w = sheet.Texture.width;
            int h = sheet.Texture.height;

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
