using Newtonsoft.Json;
using UnityEngine;

namespace BlasII.CustomSkins.Models;

/// <summary>
/// Models a <see cref="Sprite"/> with its actual texture/>
/// </summary>
public class SpriteInfoWithTexture : SpriteInfo
{
    /// <summary>
    /// The texture
    /// </summary>
    [JsonIgnore]
    public Sprite Texture { get; set; }
}
