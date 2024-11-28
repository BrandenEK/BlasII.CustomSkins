namespace BlasII.CustomSkins.Models;

/// <summary>
/// Models a <see cref="UnityEngine.Sprite"/>
/// </summary>
public class SpriteInfo
{
    /// <summary>
    /// The name of the sprite
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The ppu of the sprite
    /// </summary>
    public int PixelsPerUnit { get; set; }

    /// <summary>
    /// The location of the sprite in pixels
    /// </summary>
    public Vector Position { get; set; }

    /// <summary>
    /// The size of the sprite in pixels
    /// </summary>
    public Vector Size { get; set; }

    /// <summary>
    /// The normalized pivot of the sprite
    /// </summary>
    public Vector Pivot { get; set; }
}
