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
                    "TPO_AlhambraMirrorTransition_MirroredToNormal"
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

        // Split sprites by group and export them
        foreach (var spritesByGroup in sprites.Values.GroupBy(x => GetGroupName(x, groups)))
        {
            string group = spritesByGroup.Key;

            if (group == "unknown")
                continue;
            
            ModLog.Info($"Exporting group {group}");
            SpriteSheet sheet = ExportGroup(spritesByGroup, group, directory);
            SaveSpriteSheet(directory, sheet);

            yield return null;
        }
    }

    private SpriteSheet ExportGroup(IEnumerable<Sprite> sprites, string groupName, string directory)
    {
        var sheets = new List<SpriteSheet>();

        // Split sprites by animation and export them
        foreach (var spritesByAnimation in sprites.GroupBy(GetAnimationName))
        {
            string animation = spritesByAnimation.Key;

            ModLog.Info($"Exporting animation {animation}");
            SpriteSheet sheet = ExportAnimation(spritesByAnimation, animation, Path.Combine(directory, groupName));
            // Save to file
            sheets.Add(sheet);
        }

        // Combine all animations
        return CombineAnimationsToGroup(groupName, sheets);
    }

    private SpriteSheet CombineAnimationsToGroup(string name, IEnumerable<SpriteSheet> sheets)
    {
        // Just returns first right now
        return new SpriteSheet()
        {
            Name = name,
            Texture = sheets.First().Texture,
            Infos = sheets.First().Infos
        };
    }

    private SpriteSheet ExportAnimation(IEnumerable<Sprite> sprites, string animationName, string directory)
    {
        var sheets = sprites.Select(ExtractSprite);
        return CombineSpritesToAnimation(animationName, sheets);
    }

    private SpriteSheet CombineSpritesToAnimation(string name, IEnumerable<SpriteSheet> sheets)
    {
        // Just returns first right now
        return new SpriteSheet()
        {
            Name = name,
            Texture = sheets.First().Texture,
            Infos = sheets.First().Infos
        };
    }

    private SpriteSheet ExtractSprite(Sprite sprite)
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

        // Fill transparent
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                tex.SetPixel(i, j, new Color32(0, 0, 0, 0));

        return tex;
    }

    private const int MAX_SIZE = 2048;
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
    public List<SpriteInfo> Infos { get; set; }
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
