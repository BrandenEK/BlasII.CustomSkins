using BlasII.ModdingAPI;
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

            ModLog.Info($"Exporting group {group}");
            yield return ExportGroup(spritesByGroup, group, directory);
        }
    }

    private IEnumerator ExportGroup(IEnumerable<Sprite> sprites, string groupName, string directory)
    {
        // Split sprites by animation and export them
        foreach (var spritesByAnimation in sprites.GroupBy(GetAnimationName))
        {
            string animation = spritesByAnimation.Key;

            ModLog.Info($"Exporting animation {animation}");
            yield return ExportAnimation(spritesByAnimation, animation, Path.Combine(directory, groupName));
        }

        // Combine all animations
    }

    private IEnumerator ExportAnimation(IEnumerable<Sprite> sprites, string animationName, string directory)
    {
        yield return null;
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
}

/// <summary>
/// A texture, along with info about each sprite
/// </summary>
public class SpriteSheet
{
    /// <summary>
    /// The texture
    /// </summary>
    public Texture2D Texture { get; init; }

    /// <summary>
    /// List of SpriteInfos about every sprite
    /// </summary>
    public List<SpriteInfo> Infos { get; init; }
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
