
namespace BlasII.CustomSkins;

/// <summary>
/// Models a <see cref="UnityEngine.Sprite"/>
/// </summary>
public class SpriteInfo
{
    /// <summary>
    /// The name of the sprite
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// The ppu of the sprite
    /// </summary>
    public int PixelsPerUnit { get; init; }

    /// <summary>
    /// The location of the sprite in pixels
    /// </summary>
    public Vector Position { get; init; }

    /// <summary>
    /// The size of the sprite in pixels
    /// </summary>
    public Vector Size { get; init; }

    /// <summary>
    /// The normalized pivot of the sprite
    /// </summary>
    public Vector Pivot { get; init; }
}
