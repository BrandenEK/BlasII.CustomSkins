using System.Collections.Generic;
using UnityEngine;

namespace BlasII.CustomSkins.Models;

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
    /// List of SpriteInfos about every sprite
    /// </summary>
    public IEnumerable<SpriteInfo> Infos { get; set; }
}
