using System.Collections.Generic;
using UnityEngine;

namespace BlasII.CustomSkins.Models;

/// <summary>
/// A !(texture), along with info about each sprite
/// </summary>
public class SpriteSheetWithoutTexture
{
    /// <summary>
    /// The name the spritesheet should be saved as
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The size of the texture
    /// </summary>
    public Vector Size { get; set; }

    /// <summary>
    /// List of SpriteInfoWithTextures about every sprite
    /// </summary>
    public IEnumerable<SpriteInfoWithTexture> Infos { get; set; }
}
