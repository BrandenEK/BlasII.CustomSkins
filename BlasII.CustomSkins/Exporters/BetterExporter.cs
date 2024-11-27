using BlasII.CustomSkins.Extensions;
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
public class BetterExporter : IExporter
{
    /// <inheritdoc/>
    public IEnumerator ExportAll(SpriteCollection sprites, string directory)
    {
        AnimationGroup[] groups = [
            new AnimationGroup()
            {
                GroupName = "player",
                Animations = [
                    "TPO_airDashSequence_HeavyWeapon_armor00",
                    "TPO_airDashSequence_HeavyWeapon_flurryVFX",
                    "TPO_AlhambraMirrorTransition_MirroredToNormal",
                    "TPO_cherubsRespawn_armor00"
                ]
            },
            new AnimationGroup()
            {
                GroupName = "censer",
                Animations = [
                    "TPO_airDashSequence_HeavyWeapon_censer",
                    "TPO_chargingCenser_toIdle_weapon"
                ]
            }
        ];
        // Load groups from data folder json

        var sheets = new List<SpriteSheet>();

        // Split sprites by group and export them
        foreach (var spritesByGroup in sprites.Values.GroupBy(x => GetGroupName(x, groups)))
        {
            string group = spritesByGroup.Key;
            var groupAnimations = spritesByGroup.OrderBy(x => x.name);

            if (group == "unknown")
                continue;
            
            ModLog.Info($"Exporting group {group}");
            SpriteSheet sheet = ExportGroup(groupAnimations, group, directory);
            sheets.Add(sheet);
            SaveSpriteSheet(directory, sheet);

            yield return null;
        }

        // Combine and save total sheet ?
    }

    private SpriteSheet ExportGroup(IEnumerable<Sprite> sprites, string groupName, string directory)
    {
        var sheets = new List<SpriteSheet>();

        // Split sprites by animation and export them
        foreach (var spritesByAnimation in sprites.GroupBy(GetAnimationName))
        {
            string animation = spritesByAnimation.Key;
            var animationFrames = spritesByAnimation.OrderBy(x => int.Parse(x.name[(x.name.LastIndexOf('_') + 1)..]));

            ModLog.Info($"Exporting animation {animation}");
            SpriteSheet sheet = ExportAnimation(animationFrames, animation, Path.Combine(directory, groupName));
            sheets.Add(sheet);
            SaveSpriteSheet(Path.Combine(directory, groupName), sheet);
        }

        // Combine all animations
        return CombineSpriteSheets(groupName, true, 8192, sheets);
    }

    private SpriteSheet ExportAnimation(IEnumerable<Sprite> sprites, string animationName, string directory)
    {
        var sheets = new List<SpriteSheet>();

        // Split sprites by frame and export them
        foreach (var sprite in sprites)
        {
            string frame = sprite.name;

            ModLog.Info($"Exporting frame {frame}");
            SpriteSheet sheet = ExportFrame(sprite);
            sheets.Add(sheet);
            // Save to file
        }

        // Combine all frames
        return CombineSpriteSheets(animationName, false, 2048, sheets);
    }

    private SpriteSheet ExportFrame(Sprite sprite)
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

        return new SpriteSheet()
        {
            Name = sprite.name,
            Texture = texture,
            Infos = [info]
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
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                tex.SetPixel(i, j, new Color32(0, 0, 0, 0));

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
        Object.Destroy(sheet.Texture);

        // Save info list to file
        string infoPath = Path.Combine(directory, $"{sheet.Name}.json");
        File.WriteAllText(infoPath, JsonConvert.SerializeObject(sheet.Infos, Formatting.Indented));
    }
}

/// <summary>
/// A texture, along with info about each sprite
/// </summary>
public class SpriteSheet
{
    /// <summary>
    /// The name the spritesheet should be saved as
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The texture
    /// </summary>
    public Texture2D Texture { get; set; }

    /// <summary>
    /// List of SpriteInfos about every sprite
    /// </summary>
    public IEnumerable<SpriteInfo> Infos { get; set; }
}

/// <summary>
/// A group of related animations
/// </summary>
public class AnimationGroup
{
    /// <summary>
    /// The name of the group
    /// </summary>
    public string GroupName { get; set; }

    /// <summary>
    /// The names of the animations in the group
    /// </summary>
    public string[] Animations { get; set; }
}
