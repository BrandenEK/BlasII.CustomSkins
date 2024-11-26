using BlasII.ModdingAPI;
using System.Collections;
using System.Collections.Generic;
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

        foreach (var spritesByType in sprites.Values.GroupBy(x => GetGroupName(x, groups)))
        {
            ModLog.Warn(spritesByType.Key);
            foreach (var s in spritesByType)
            {
                ModLog.Error(s.name);
            }
        }

        yield return null;
    }

    private string GetGroupName(Sprite sprite, IEnumerable<AnimationGroup> groups)
    {
        string name = sprite.name[0..sprite.name.LastIndexOf('_')];
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
